using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace BirdGame.Managers
{
    public enum MatchState
    {
        Waiting,
        Scramble,
        Heist,
        Frenzy,
        End
    }

    public class Mgr_Match : NetworkBehaviour
    {
        public static Mgr_Match Instance { get; private set; }

        [Header("Phase Durations (seconds)")]
        [SerializeField] private float waitingDuration = 10f;
        [SerializeField] private float scrambleDuration = 60f;
        [SerializeField] private float heistDuration = 120f;
        [SerializeField] private float frenzyDuration = 60f;

        [Header("Respawn Times")]
        [SerializeField] private float normalRespawnTime = 5f;
        [SerializeField] private float frenzyRespawnTime = 3f;

        [Header("Egg Spawning")]
        [SerializeField] private float scrambleEggSpawnInterval = 5f;
        [SerializeField] private float heistEggSpawnInterval = 10f;
        [SerializeField] private float frenzyEggSpawnInterval = 3f;
        [SerializeField] private int scrambleInitialEggCount = 10;

        [Header("Events")]
        public UnityEvent<MatchState> OnStateChanged;
        public UnityEvent<float> OnTimerUpdated;
        public UnityEvent OnMatchStarted;
        public UnityEvent OnMatchEnded;

        private NetworkVariable<MatchState> _currentState = new NetworkVariable<MatchState>(
            MatchState.Waiting,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

        private NetworkVariable<float> _phaseTimeRemaining = new NetworkVariable<float>(
            0f,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

        private float _eggSpawnTimer;

        public MatchState CurrentState => _currentState.Value;
        public float PhaseTimeRemaining => _phaseTimeRemaining.Value;
        public float CurrentRespawnTime => _currentState.Value == MatchState.Frenzy ? frenzyRespawnTime : normalRespawnTime;
        public bool IsMatchActive => _currentState.Value != MatchState.Waiting && _currentState.Value != MatchState.End;

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
            _currentState.OnValueChanged += OnStateValueChanged;
            _phaseTimeRemaining.OnValueChanged += OnTimerValueChanged;

            if (IsServer)
            {
                SetState(MatchState.Waiting);
            }
        }

        public override void OnNetworkDespawn()
        {
            _currentState.OnValueChanged -= OnStateValueChanged;
            _phaseTimeRemaining.OnValueChanged -= OnTimerValueChanged;
        }

        private void Update()
        {
            if (!IsServer) return;
            if (_currentState.Value == MatchState.Waiting || _currentState.Value == MatchState.End) return;

            UpdatePhaseTimer();
            UpdateEggSpawning();
        }

        public void StartMatch()
        {
            if (!IsServer) return;
            if (_currentState.Value != MatchState.Waiting) return;

            OnMatchStarted?.Invoke();
            SetState(MatchState.Scramble);
        }

        public void EndMatch()
        {
            if (!IsServer) return;

            SetState(MatchState.End);
            OnMatchEnded?.Invoke();
        }

        public void RestartMatch()
        {
            if (!IsServer) return;

            SetState(MatchState.Waiting);
        }

        private void SetState(MatchState newState)
        {
            _currentState.Value = newState;
            _phaseTimeRemaining.Value = GetPhaseDuration(newState);
            _eggSpawnTimer = 0f;

            if (newState == MatchState.Scramble)
            {
                SpawnInitialEggs();
            }
        }

        private float GetPhaseDuration(MatchState state)
        {
            return state switch
            {
                MatchState.Waiting => waitingDuration,
                MatchState.Scramble => scrambleDuration,
                MatchState.Heist => heistDuration,
                MatchState.Frenzy => frenzyDuration,
                MatchState.End => 0f,
                _ => 0f
            };
        }

        private float GetEggSpawnInterval()
        {
            return _currentState.Value switch
            {
                MatchState.Scramble => scrambleEggSpawnInterval,
                MatchState.Heist => heistEggSpawnInterval,
                MatchState.Frenzy => frenzyEggSpawnInterval,
                _ => float.MaxValue
            };
        }

        private void UpdatePhaseTimer()
        {
            _phaseTimeRemaining.Value -= Time.deltaTime;

            if (_phaseTimeRemaining.Value <= 0f)
            {
                AdvanceToNextPhase();
            }
        }

        private void UpdateEggSpawning()
        {
            _eggSpawnTimer += Time.deltaTime;

            float spawnInterval = GetEggSpawnInterval();
            if (_eggSpawnTimer >= spawnInterval)
            {
                _eggSpawnTimer = 0f;
                SpawnEgg();
            }
        }

        private void AdvanceToNextPhase()
        {
            MatchState nextState = _currentState.Value switch
            {
                MatchState.Scramble => MatchState.Heist,
                MatchState.Heist => MatchState.Frenzy,
                MatchState.Frenzy => MatchState.End,
                _ => MatchState.End
            };

            SetState(nextState);

            if (nextState == MatchState.End)
            {
                OnMatchEnded?.Invoke();
            }
        }

        private void SpawnInitialEggs()
        {
            for (int i = 0; i < scrambleInitialEggCount; i++)
            {
                SpawnEgg();
            }
        }

        private void SpawnEgg()
        {
            if (Mgr_EggSpawner.Instance != null)
            {
                Mgr_EggSpawner.Instance.SpawnEgg();
            }
        }

        private void OnStateValueChanged(MatchState previous, MatchState current)
        {
            OnStateChanged?.Invoke(current);
        }

        private void OnTimerValueChanged(float previous, float current)
        {
            OnTimerUpdated?.Invoke(current);
        }
    }
}
