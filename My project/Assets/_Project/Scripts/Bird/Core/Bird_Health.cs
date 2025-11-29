using UnityEngine;
using UnityEngine.Events;
using BirdGame.Core.Interfaces;
using BirdGame.Core.Data;

namespace BirdGame.Bird.Core
{
    /// <summary>
    /// Handles bird health, damage, and death.
    /// Single Responsibility: Health management and damage processing.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class Bird_Health : MonoBehaviour, IDamageable
    {
        #region Constants
        private const float DEFAULT_MAX_HEALTH = 100f;
        private const float KNOCKBACK_THRESHOLD = 0.1f;
        private const float KNOCKBACK_DECAY_SPEED = 5f;
        #endregion

        #region Serialized Fields
        [Header("Configuration")]
        [SerializeField] private Data_BirdStats stats;

        [Header("Events")]
        public UnityEvent<float, float> OnHealthChanged; // current, max
        public UnityEvent OnDamaged;
        public UnityEvent OnDeath;
        public UnityEvent OnRespawn;
        #endregion

        #region Private Fields
        private float _currentHealth;
        private bool _isDead;
        private CharacterController _controller;
        private Vector3 _knockbackVelocity;
        #endregion

        #region Properties
        public float CurrentHealth => _currentHealth;
        public float MaxHealth => stats != null ? stats.maxHealth : DEFAULT_MAX_HEALTH;
        public bool IsDead => _isDead;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
        }

        private void Start()
        {
            _currentHealth = MaxHealth;
            OnHealthChanged?.Invoke(_currentHealth, MaxHealth);
        }

        private void Update()
        {
            ApplyKnockbackDecay();
        }
        #endregion

        #region Knockback
        private void ApplyKnockbackDecay()
        {
            if (_knockbackVelocity.sqrMagnitude > KNOCKBACK_THRESHOLD)
            {
                _controller.Move(_knockbackVelocity * Time.deltaTime);
                _knockbackVelocity = Vector3.Lerp(_knockbackVelocity, Vector3.zero, KNOCKBACK_DECAY_SPEED * Time.deltaTime);
            }
        }
        #endregion

        #region IDamageable Implementation
        public void TakeDamage(float damage, Vector3 knockback)
        {
            if (_isDead) return;

            _currentHealth -= damage;
            _knockbackVelocity += knockback;

            OnHealthChanged?.Invoke(_currentHealth, MaxHealth);
            OnDamaged?.Invoke();

            if (_currentHealth <= 0)
            {
                Die();
            }
        }

        public void Heal(float amount)
        {
            if (_isDead) return;

            _currentHealth = Mathf.Min(_currentHealth + amount, MaxHealth);
            OnHealthChanged?.Invoke(_currentHealth, MaxHealth);
        }
        #endregion

        #region Death and Respawn
        private void Die()
        {
            _isDead = true;
            OnDeath?.Invoke();
        }

        public void Respawn(Vector3 position)
        {
            _isDead = false;
            _currentHealth = MaxHealth;
            _knockbackVelocity = Vector3.zero;

            _controller.enabled = false;
            transform.position = position;
            _controller.enabled = true;

            OnHealthChanged?.Invoke(_currentHealth, MaxHealth);
            OnRespawn?.Invoke();
        }
        #endregion

        #region Public Methods
        public void SetStats(Data_BirdStats newStats)
        {
            stats = newStats;
            _currentHealth = MaxHealth;
            OnHealthChanged?.Invoke(_currentHealth, MaxHealth);
        }
        #endregion
    }
}
