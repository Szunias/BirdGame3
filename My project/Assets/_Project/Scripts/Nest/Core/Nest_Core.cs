using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace BirdGame.Nest.Core
{
    public class Nest_Core : NetworkBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private int teamId;
        [SerializeField] private int maxEggCapacity = 10;

        [Header("Egg Placement Area")]
        [SerializeField] private Vector3 eggAreaCenter = Vector3.zero;
        [SerializeField] private Vector3 eggAreaSize = new Vector3(0.6f, 0.1f, 0.6f);
        [SerializeField] private Color eggAreaGizmoColor = new Color(1f, 0.8f, 0f, 0.3f);

        [Header("Events")]
        public UnityEvent<int> OnEggCountChanged;
        public UnityEvent<int> OnEggStolen;
        public UnityEvent<int> OnEggDeposited;

        private NetworkVariable<int> _eggCount = new NetworkVariable<int>(
            0,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

        public int TeamId => teamId;
        public int EggCount => _eggCount.Value;
        public int MaxCapacity => maxEggCapacity;
        public bool IsFull => _eggCount.Value >= maxEggCapacity;
        public bool IsEmpty => _eggCount.Value <= 0;

        public override void OnNetworkSpawn()
        {
            _eggCount.OnValueChanged += OnEggCountValueChanged;
        }

        public override void OnNetworkDespawn()
        {
            _eggCount.OnValueChanged -= OnEggCountValueChanged;
        }

        private void OnEggCountValueChanged(int previous, int current)
        {
            OnEggCountChanged?.Invoke(current);
        }

        public bool TryDepositEgg()
        {
            if (!IsServer) return false;
            if (IsFull) return false;

            _eggCount.Value++;
            OnEggDeposited?.Invoke(_eggCount.Value);
            return true;
        }

        public bool TryStealEgg()
        {
            if (!IsServer) return false;
            if (IsEmpty) return false;

            _eggCount.Value--;
            OnEggStolen?.Invoke(_eggCount.Value);
            return true;
        }

        public void SetEggCount(int count)
        {
            if (!IsServer) return;
            _eggCount.Value = Mathf.Clamp(count, 0, maxEggCapacity);
        }

        public Vector3 GetRandomEggPosition()
        {
            var halfSize = eggAreaSize * 0.5f;
            return eggAreaCenter + new Vector3(
                Random.Range(-halfSize.x, halfSize.x),
                Random.Range(-halfSize.y, halfSize.y),
                Random.Range(-halfSize.z, halfSize.z)
            );
        }

        public Vector3 GetRandomEggPositionWorld()
        {
            return transform.TransformPoint(GetRandomEggPosition());
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = eggAreaGizmoColor;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(eggAreaCenter, eggAreaSize);

            Gizmos.color = new Color(eggAreaGizmoColor.r, eggAreaGizmoColor.g, eggAreaGizmoColor.b, 1f);
            Gizmos.DrawWireCube(eggAreaCenter, eggAreaSize);
        }
    }
}
