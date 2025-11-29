using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace BirdGame.Managers
{
    public class Mgr_EggSpawner : NetworkBehaviour
    {
        public static Mgr_EggSpawner Instance { get; private set; }

        [Header("Configuration")]
        [SerializeField] private GameObject eggPrefab;
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private int maxEggsInWorld = 50;

        [Header("Events")]
        public UnityEvent<GameObject> OnEggSpawned;

        private int _currentEggCount;

        public int CurrentEggCount => _currentEggCount;
        public int MaxEggs => maxEggsInWorld;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public override void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
            base.OnDestroy();
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer && Mgr_Match.Instance != null)
            {
                Mgr_Match.Instance.OnStateChanged.AddListener(OnMatchStateChanged);
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsServer && Mgr_Match.Instance != null)
            {
                Mgr_Match.Instance.OnStateChanged.RemoveListener(OnMatchStateChanged);
            }
        }

        public void SpawnEgg()
        {
            if (!IsServer) return;
            if (_currentEggCount >= maxEggsInWorld) return;
            if (eggPrefab == null) return;
            if (spawnPoints == null || spawnPoints.Length == 0) return;

            Transform spawnPoint = GetRandomSpawnPoint();
            if (spawnPoint == null) return;

            var egg = Instantiate(eggPrefab, spawnPoint.position, spawnPoint.rotation);
            var networkObject = egg.GetComponent<NetworkObject>();

            if (networkObject != null)
            {
                networkObject.Spawn();
                _currentEggCount++;
                OnEggSpawned?.Invoke(egg);
                NotifyEggSpawnedClientRpc(egg.GetComponent<NetworkObject>().NetworkObjectId);
            }
        }

        public void SpawnEggAt(Vector3 position)
        {
            if (!IsServer) return;
            if (_currentEggCount >= maxEggsInWorld) return;
            if (eggPrefab == null) return;

            var egg = Instantiate(eggPrefab, position, Quaternion.identity);
            var networkObject = egg.GetComponent<NetworkObject>();

            if (networkObject != null)
            {
                networkObject.Spawn();
                _currentEggCount++;
                OnEggSpawned?.Invoke(egg);
            }
        }

        public void OnEggDestroyed()
        {
            if (!IsServer) return;
            _currentEggCount = Mathf.Max(0, _currentEggCount - 1);
        }

        public void DespawnAllEggs()
        {
            if (!IsServer) return;

            var eggs = FindObjectsByType<BirdGame.Egg.Egg_Base>(FindObjectsSortMode.None);
            foreach (var egg in eggs)
            {
                if (egg.TryGetComponent<NetworkObject>(out var networkObject))
                {
                    networkObject.Despawn();
                }
            }
            _currentEggCount = 0;
        }

        private Transform GetRandomSpawnPoint()
        {
            if (spawnPoints == null || spawnPoints.Length == 0) return null;
            return spawnPoints[Random.Range(0, spawnPoints.Length)];
        }

        private void OnMatchStateChanged(MatchState state)
        {
            if (state == MatchState.Waiting || state == MatchState.End)
            {
                DespawnAllEggs();
            }
        }

        [ClientRpc]
        private void NotifyEggSpawnedClientRpc(ulong eggNetworkId)
        {
        }

        private void OnDrawGizmosSelected()
        {
            if (spawnPoints == null) return;

            Gizmos.color = new Color(1f, 1f, 0f, 0.5f);
            foreach (var point in spawnPoints)
            {
                if (point != null)
                {
                    Gizmos.DrawSphere(point.position, 0.3f);
                    Gizmos.DrawLine(point.position, point.position + Vector3.up * 0.5f);
                }
            }
        }
    }
}
