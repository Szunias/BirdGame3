using UnityEngine;

namespace BirdGame.Core.Interfaces
{
    /// <summary>
    /// Interface for objects that can take damage
    /// </summary>
    public interface IDamageable
    {
        float CurrentHealth { get; }
        float MaxHealth { get; }
        bool IsDead { get; }

        void TakeDamage(float damage, Vector3 knockback);
        void Heal(float amount);
    }
}
