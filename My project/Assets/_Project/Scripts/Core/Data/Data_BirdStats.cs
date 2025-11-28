using UnityEngine;

namespace BirdGame.Core.Data
{
    /// <summary>
    /// Defines the base stats for a specific bird type.
    /// ScriptableObject architecture allows designers to tweak values without code changes.
    /// </summary>
    [CreateAssetMenu(fileName = "Data_Bird_New", menuName = "BirdGame/BirdData")]
    public class Data_BirdStats : ScriptableObject
    {
        [Header("Identity")]
        public string birdName;
        // public BirdTier tier; // Enum not defined yet, skipping for now
        public Sprite icon;

        [Header("Base Stats")]
        public int maxHealth = 100;
        [Range(0.5f, 1.5f)] public float speedMultiplier = 1f;
        [Range(0.5f, 2f)] public float stealTimeMultiplier = 1f;
        public int eggCarryCapacity = 3;

        [Header("Movement Configuration")]
        public float groundSpeed = 10f;
        public float flightSpeed = 12f;
        public float climbSpeed = 8f;     // Speed when flying UP
        public float diveSpeed = 18f;     // Speed when flying DOWN
        public float acceleration = 50f;  // How snappy the movement is
        public float gravity = 20f;       // Gravity when NOT flying
        public float flightGravity = 0f;  // Usually 0 for unlimited flight

        [Header("Jump & Flight")]
        public float jumpHeight = 1.5f;   // How high the bird jumps (meters)
        public float takeoffBoost = 2f;   // Upward velocity boost when starting flight

        [Header("Rotation")]
        public float groundRotationSpeed = 10f;  // How fast bird rotates on ground
        public float flightRotationSpeed = 5f;   // How fast bird rotates in air
        public float flightHoldThreshold = 0.3f; // Seconds to hold jump to start flying

        [Header("Capabilities")]
        public bool canFly = true;        // False for Cassowary
        public bool canHover = false;     // True for Hummingbird
    }
}
