using BirdGame.Core.Interfaces;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace BirdGame.Nest.Core
{
    [RequireComponent(typeof(Nest_Core))]
    public class Nest_DetectionZone : NetworkBehaviour
    {
        [Header("Events")]
        public UnityEvent<GameObject> OnBirdEntered;
        public UnityEvent<GameObject> OnBirdExited;
        public UnityEvent<GameObject> OnEnemyEntered;
        public UnityEvent<GameObject> OnEnemyExited;

        private Nest_Core _nest;

        public bool IsEnemy(GameObject bird) => GetBirdTeamId(bird) != _nest.TeamId;
        public bool IsAlly(GameObject bird) => GetBirdTeamId(bird) == _nest.TeamId;

        private void Awake()
        {
            _nest = GetComponent<Nest_Core>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent<IBirdCarrier>(out _)) return;

            int birdTeamId = GetBirdTeamId(other.gameObject);

            if (birdTeamId == _nest.TeamId)
            {
                OnBirdEntered?.Invoke(other.gameObject);
            }
            else
            {
                OnEnemyEntered?.Invoke(other.gameObject);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent<IBirdCarrier>(out _)) return;

            int birdTeamId = GetBirdTeamId(other.gameObject);

            if (birdTeamId == _nest.TeamId)
            {
                OnBirdExited?.Invoke(other.gameObject);
            }
            else
            {
                OnEnemyExited?.Invoke(other.gameObject);
            }
        }

        private int GetBirdTeamId(GameObject bird)
        {
            if (bird.TryGetComponent<IBirdTeam>(out var teamComponent))
            {
                return teamComponent.TeamId;
            }
            return -1;
        }
    }
}
