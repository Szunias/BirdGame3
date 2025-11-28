using BirdGame.Core.Interfaces;
using Unity.Netcode;
using UnityEngine;

namespace BirdGame.Bird.Core
{
    public class Bird_Team : NetworkBehaviour, IBirdTeam
    {
        [Header("Configuration")]
        [SerializeField] private int defaultTeamId = -1;

        private NetworkVariable<int> _teamId = new NetworkVariable<int>(
            -1,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

        public int TeamId => _teamId.Value;

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                _teamId.Value = defaultTeamId;
            }
        }

        public void SetTeam(int teamId)
        {
            if (!IsServer) return;
            _teamId.Value = teamId;
        }
    }
}
