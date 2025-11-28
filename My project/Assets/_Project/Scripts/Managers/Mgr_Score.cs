using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace BirdGame.Managers
{
    public class Mgr_Score : NetworkBehaviour
    {
        public static Mgr_Score Instance { get; private set; }

        [Header("Configuration")]
        [SerializeField] private int maxTeams = 2;
        [SerializeField] private int pointsPerEggDeposit = 100;
        [SerializeField] private int pointsPerEggSteal = 50;

        [Header("Events")]
        public UnityEvent<int, int> OnScoreChanged;
        public UnityEvent<int> OnTeamWon;

        private NetworkList<int> _teamScores;

        public int PointsPerEggDeposit => pointsPerEggDeposit;
        public int PointsPerEggSteal => pointsPerEggSteal;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            _teamScores = new NetworkList<int>();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public override void OnNetworkSpawn()
        {
            _teamScores.OnListChanged += OnTeamScoresChanged;

            if (IsServer)
            {
                InitializeScores();
            }
        }

        public override void OnNetworkDespawn()
        {
            _teamScores.OnListChanged -= OnTeamScoresChanged;
        }

        public void AddScore(int teamId, int points)
        {
            if (!IsServer) return;
            if (teamId < 0 || teamId >= _teamScores.Count) return;

            _teamScores[teamId] += points;
        }

        public void AddEggDepositScore(int teamId) => AddScore(teamId, pointsPerEggDeposit);

        public void AddEggStealScore(int teamId) => AddScore(teamId, pointsPerEggSteal);

        public int GetScore(int teamId)
        {
            if (teamId < 0 || teamId >= _teamScores.Count) return 0;
            return _teamScores[teamId];
        }

        public void ResetScores()
        {
            if (!IsServer) return;

            for (int i = 0; i < _teamScores.Count; i++)
            {
                _teamScores[i] = 0;
            }
        }

        public int GetWinningTeam()
        {
            int winningTeam = 0;
            int highestScore = 0;

            for (int i = 0; i < _teamScores.Count; i++)
            {
                if (_teamScores[i] > highestScore)
                {
                    highestScore = _teamScores[i];
                    winningTeam = i;
                }
            }

            return winningTeam;
        }

        private void InitializeScores()
        {
            _teamScores.Clear();
            for (int i = 0; i < maxTeams; i++)
            {
                _teamScores.Add(0);
            }
        }

        private void OnTeamScoresChanged(NetworkListEvent<int> changeEvent)
        {
            OnScoreChanged?.Invoke(changeEvent.Index, changeEvent.Value);
        }
    }
}
