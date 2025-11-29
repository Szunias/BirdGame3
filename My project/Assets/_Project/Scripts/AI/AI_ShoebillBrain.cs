using UnityEngine;
using UnityEngine.Events;
using BirdGame.Core.Interfaces;
using BirdGame.Bird.Core;

namespace BirdGame.AI
{
    /// <summary>
    /// AI Brain for the Shoebill enemy. Patrols the area and attacks players who get too close.
    /// Single Responsibility: AI decision-making and state management.
    /// </summary>
    [RequireComponent(typeof(AI_BirdController))]
    public class AI_ShoebillBrain : MonoBehaviour
    {
        #region Constants
        private const float ROTATION_SPEED = 10f;
        private const float ROTATION_THRESHOLD = 0.01f;
        private const float ATTACK_RANGE_MULTIPLIER = 1.5f;
        private const float PATROL_NEAR_THRESHOLD = 2f;
        private const float HOME_NEAR_THRESHOLD = 3f;
        private const float DETECTION_INTERVAL = 0.2f; // Check every 0.2 seconds instead of every frame
        private const float PATROL_HEIGHT_MIN_RATIO = 0.5f;
        #endregion

        #region Enums
        public enum State
        {
            Patrol,
            Chase,
            Attack,
            Return
        }
        #endregion

        #region Serialized Fields
        [Header("Detection")]
        [SerializeField] private float detectionRadius = 150f;
        [SerializeField] private float attackRadius = 5f;
        [SerializeField] private float loseTargetRadius = 200f;
        [SerializeField] private LayerMask playerLayer;
        [SerializeField] private string playerTag = "Player";

        [Header("Patrol")]
        [SerializeField] private float patrolRadius = 20f;
        [SerializeField] private float patrolHeight = 10f;
        [SerializeField] private float patrolWaitTime = 2f;

        [Header("Attack")]
        [SerializeField] private float attackDamage = 25f;
        [SerializeField] private float attackCooldown = 1.5f;
        [SerializeField] private float attackKnockback = 10f;

        [Header("Events")]
        public UnityEvent<State> OnStateChanged;
        public UnityEvent<Transform> OnPlayerDetected;
        public UnityEvent<Transform> OnPlayerLost;
        public UnityEvent OnAttackPerformed;

        [Header("Debug")]
        [SerializeField] private bool showGizmos = true;
        [SerializeField] private bool showDebugLogs = false;
        #endregion

        #region Private Fields
        private AI_BirdController _controller;
        private State _currentState = State.Patrol;
        private Transform _target;
        private Vector3 _homePosition;
        private Vector3 _patrolTarget;
        private float _waitTimer;
        private float _attackTimer;
        private float _detectionTimer;

        // Cached player references - updated periodically, NOT every frame
        private Transform[] _cachedPlayers;
        private float _cacheRefreshTimer;
        private const float CACHE_REFRESH_INTERVAL = 1f;
        #endregion

        #region Properties
        public State CurrentState => _currentState;
        public Transform CurrentTarget => _target;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            _controller = GetComponent<AI_BirdController>();
        }

        private void Start()
        {
            _homePosition = transform.position;
            SetNewPatrolTarget();
            RefreshPlayerCache();

            if (showDebugLogs)
            {
                Debug.Log($"[Shoebill] Started at {transform.position}. Detection radius: {detectionRadius}m, PlayerLayer: {playerLayer.value}");
                Debug.Log($"[Shoebill] Initial state: {_currentState}. Patrol target: {_patrolTarget}");
            }
        }

        private void Update()
        {
            _attackTimer -= Time.deltaTime;
            _detectionTimer -= Time.deltaTime;
            _cacheRefreshTimer -= Time.deltaTime;

            // Refresh player cache periodically (NOT every frame)
            if (_cacheRefreshTimer <= 0f)
            {
                RefreshPlayerCache();
                _cacheRefreshTimer = CACHE_REFRESH_INTERVAL;
            }

            UpdateStateMachine();

            // Detection only runs periodically, not every frame
            if (_detectionTimer <= 0f)
            {
                CheckForPlayers();
                _detectionTimer = DETECTION_INTERVAL;
            }
        }
        #endregion

        #region State Machine
        private void UpdateStateMachine()
        {
            switch (_currentState)
            {
                case State.Patrol:
                    UpdatePatrol();
                    break;
                case State.Chase:
                    UpdateChase();
                    break;
                case State.Attack:
                    UpdateAttack();
                    break;
                case State.Return:
                    UpdateReturn();
                    break;
            }
        }

        private void UpdatePatrol()
        {
            if (_controller.IsNearPosition(_patrolTarget, PATROL_NEAR_THRESHOLD))
            {
                _controller.Stop();
                _waitTimer -= Time.deltaTime;

                if (_waitTimer <= 0f)
                {
                    SetNewPatrolTarget();
                }
            }
            else
            {
                _controller.MoveTowards(_patrolTarget, fly: true);
            }
        }

        private void UpdateChase()
        {
            if (_target == null)
            {
                if (showDebugLogs) Debug.Log("[Shoebill] Chase: Target is null, returning home");
                TransitionTo(State.Return);
                return;
            }

            float distanceToTarget = Vector3.Distance(transform.position, _target.position);

            if (showDebugLogs && Time.frameCount % 30 == 0) // Log every 30 frames to avoid spam
            {
                Debug.Log($"[Shoebill] Chasing {_target.name} - distance: {distanceToTarget:F1}m");
            }

            // Lost target - too far
            if (distanceToTarget > loseTargetRadius)
            {
                if (showDebugLogs) Debug.Log($"[Shoebill] Lost target - too far ({distanceToTarget:F1}m > {loseTargetRadius}m)");
                OnPlayerLost?.Invoke(_target);
                _target = null;
                TransitionTo(State.Return);
                return;
            }

            // Close enough to attack
            if (distanceToTarget <= attackRadius)
            {
                if (showDebugLogs) Debug.Log($"[Shoebill] Close enough to attack ({distanceToTarget:F1}m <= {attackRadius}m)");
                TransitionTo(State.Attack);
                return;
            }

            _controller.MoveTowards(_target.position, fly: true);
        }

        private void UpdateAttack()
        {
            if (_target == null)
            {
                TransitionTo(State.Return);
                return;
            }

            float distanceToTarget = Vector3.Distance(transform.position, _target.position);

            // Target escaped attack range
            if (distanceToTarget > attackRadius * ATTACK_RANGE_MULTIPLIER)
            {
                TransitionTo(State.Chase);
                return;
            }

            // Face the target
            Vector3 lookDir = (_target.position - transform.position);
            lookDir.y = 0f;
            if (lookDir.sqrMagnitude > ROTATION_THRESHOLD)
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(lookDir),
                    ROTATION_SPEED * Time.deltaTime
                );
            }

            // Attack if cooldown ready
            if (_attackTimer <= 0f)
            {
                PerformAttack();
                _attackTimer = attackCooldown;
            }

            _controller.MoveTowards(_target.position, fly: true);
        }

        private void UpdateReturn()
        {
            if (_controller.IsNearPosition(_homePosition, HOME_NEAR_THRESHOLD))
            {
                TransitionTo(State.Patrol);
                return;
            }

            _controller.MoveTowards(_homePosition, fly: true);
        }

        private void TransitionTo(State newState)
        {
            if (_currentState == newState) return;

            if (showDebugLogs)
            {
                Debug.Log($"[Shoebill] State transition: {_currentState} -> {newState}");
            }

            _currentState = newState;
            OnStateChanged?.Invoke(newState);

            switch (newState)
            {
                case State.Patrol:
                    SetNewPatrolTarget();
                    break;
                case State.Attack:
                    _attackTimer = 0f; // Attack immediately
                    break;
            }
        }
        #endregion

        #region Detection System
        /// <summary>
        /// Refreshes the cached list of players. Called periodically, NOT every frame.
        /// Always caches Bird_Movement components as a reliable fallback.
        /// </summary>
        private void RefreshPlayerCache()
        {
            // Always cache Bird_Movement components - this is the most reliable method
            var allBirds = FindObjectsByType<Bird_Movement>(FindObjectsSortMode.None);

            // Filter out self (the AI bird shouldn't chase itself)
            var playerList = new System.Collections.Generic.List<Transform>();
            foreach (var bird in allBirds)
            {
                // Skip if this is the AI's own Bird_Movement
                if (bird.transform == transform || bird.transform.IsChildOf(transform))
                    continue;

                playerList.Add(bird.transform);
            }

            _cachedPlayers = playerList.ToArray();

            if (showDebugLogs)
            {
                Debug.Log($"[Shoebill] RefreshPlayerCache: Found {_cachedPlayers.Length} players (Bird_Movement)");
                foreach (var p in _cachedPlayers)
                {
                    Debug.Log($"[Shoebill]   - Player: {p.name}");
                }
            }
        }

        private void CheckForPlayers()
        {
            // Only check when patrolling or returning
            if (_currentState == State.Chase || _currentState == State.Attack) return;

            Transform closestPlayer = null;
            float closestDist = float.MaxValue;

            if (showDebugLogs)
            {
                Debug.Log($"[Shoebill] CheckForPlayers: State={_currentState}, DetectionRadius={detectionRadius}");
            }

            // Method 1: Layer-based detection (most efficient - uses Physics)
            if (playerLayer.value != 0)
            {
                Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);

                if (showDebugLogs)
                {
                    Debug.Log($"[Shoebill] Layer detection: Found {hits.Length} colliders with layer mask {playerLayer.value}");
                }

                foreach (var hit in hits)
                {
                    // Skip self
                    if (hit.transform == transform || hit.transform.IsChildOf(transform))
                        continue;

                    float dist = Vector3.Distance(transform.position, hit.transform.position);
                    if (showDebugLogs)
                    {
                        Debug.Log($"[Shoebill]   - {hit.name} at distance {dist:F1}");
                    }
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        closestPlayer = hit.transform;
                    }
                }
            }

            // Method 2: Use cached players (always as fallback or primary if no layer set)
            if (closestPlayer == null && _cachedPlayers != null && _cachedPlayers.Length > 0)
            {
                if (showDebugLogs)
                {
                    Debug.Log($"[Shoebill] Cache detection: Checking {_cachedPlayers.Length} cached players");
                }

                foreach (var player in _cachedPlayers)
                {
                    if (player == null) continue;

                    float dist = Vector3.Distance(transform.position, player.position);

                    if (showDebugLogs)
                    {
                        Debug.Log($"[Shoebill]   - {player.name} at distance {dist:F1} (radius: {detectionRadius})");
                    }

                    if (dist <= detectionRadius && dist < closestDist)
                    {
                        closestDist = dist;
                        closestPlayer = player;
                    }
                }
            }

            if (closestPlayer != null)
            {
                if (showDebugLogs)
                {
                    Debug.Log($"[Shoebill] DETECTED: {closestPlayer.name} at {closestDist:F1}m - Starting chase!");
                }
                _target = closestPlayer;
                OnPlayerDetected?.Invoke(_target);
                TransitionTo(State.Chase);
            }
            else if (showDebugLogs && _cachedPlayers != null && _cachedPlayers.Length > 0)
            {
                Debug.Log($"[Shoebill] No players in detection range ({detectionRadius}m)");
            }
        }
        #endregion

        #region Combat
        private void PerformAttack()
        {
            if (_target == null) return;

            Vector3 knockbackDir = (_target.position - transform.position).normalized;

            // Use TryGetComponent pattern (per guidelines section 4.4)
            if (_target.TryGetComponent<IDamageable>(out var damageable))
            {
                if (showDebugLogs) Debug.Log($"[Shoebill] ATTACK! Dealing {attackDamage} damage to {_target.name}");
                damageable.TakeDamage(attackDamage, knockbackDir * attackKnockback);
                OnAttackPerformed?.Invoke();
            }
            else if (_target.parent != null && _target.parent.TryGetComponent<IDamageable>(out var parentDamageable))
            {
                if (showDebugLogs) Debug.Log($"[Shoebill] ATTACK! Dealing {attackDamage} damage to {_target.parent.name} (parent)");
                parentDamageable.TakeDamage(attackDamage, knockbackDir * attackKnockback);
                OnAttackPerformed?.Invoke();
            }
            else if (showDebugLogs)
            {
                Debug.LogWarning($"[Shoebill] Cannot attack {_target.name} - no IDamageable component found!");
            }
        }
        #endregion

        #region Patrol
        private void SetNewPatrolTarget()
        {
            Vector2 randomCircle = Random.insideUnitCircle * patrolRadius;
            float randomHeight = Random.Range(patrolHeight * PATROL_HEIGHT_MIN_RATIO, patrolHeight);

            _patrolTarget = _homePosition + new Vector3(randomCircle.x, randomHeight, randomCircle.y);
            _waitTimer = patrolWaitTime;
        }
        #endregion

        #region Debug
        private void OnDrawGizmosSelected()
        {
            if (!showGizmos) return;

            Vector3 center = Application.isPlaying ? _homePosition : transform.position;

            // Detection radius (yellow)
            Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
            Gizmos.DrawWireSphere(transform.position, detectionRadius);

            // Attack radius (red)
            Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
            Gizmos.DrawWireSphere(transform.position, attackRadius);

            // Lose target radius (orange)
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.2f);
            Gizmos.DrawWireSphere(transform.position, loseTargetRadius);

            // Patrol area (green)
            Gizmos.color = new Color(0f, 1f, 0f, 0.2f);
            Gizmos.DrawWireSphere(center, patrolRadius);

            // Patrol target (blue)
            if (Application.isPlaying)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(_patrolTarget, 0.5f);
                Gizmos.DrawLine(transform.position, _patrolTarget);
            }

            // Target line (red)
            if (Application.isPlaying && _target != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, _target.position);
            }
        }
        #endregion
    }
}
