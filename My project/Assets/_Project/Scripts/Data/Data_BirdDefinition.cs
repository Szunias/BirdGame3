using UnityEngine;

namespace BirdGame.Data
{
    [CreateAssetMenu(fileName = "Bird_", menuName = "BirdGame/Bird Definition")]
    public class Data_BirdDefinition : ScriptableObject
    {
        [Header("Basic Info")]
        [SerializeField] private string birdName;
        [SerializeField] private string description;
        [SerializeField] private Sprite icon;

        [Header("Prefab")]
        [SerializeField] private GameObject birdPrefab;

        [Header("Stats")]
        [SerializeField] private float groundSpeed = 10f;
        [SerializeField] private float flightSpeed = 15f;
        [SerializeField] private float maxHealth = 100f;

        [Header("Preview")]
        [SerializeField] private GameObject previewPrefab;
        [SerializeField] private Vector3 previewRotation = Vector3.zero;
        [SerializeField] private float previewScale = 1f;

        // Public properties for read access
        public string BirdName => birdName;
        public string Description => description;
        public Sprite Icon => icon;
        public GameObject BirdPrefab => birdPrefab;
        public float GroundSpeed => groundSpeed;
        public float FlightSpeed => flightSpeed;
        public float MaxHealth => maxHealth;
        public GameObject PreviewPrefab => previewPrefab;
        public Vector3 PreviewRotation => previewRotation;
        public float PreviewScale => previewScale;
    }
}
