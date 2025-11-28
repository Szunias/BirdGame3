using UnityEngine;

namespace BirdGame.Data
{
    [CreateAssetMenu(fileName = "Bird_", menuName = "BirdGame/Bird Definition")]
    public class Data_BirdDefinition : ScriptableObject
    {
        [Header("Basic Info")]
        [SerializeField] private string _birdName;
        [SerializeField] private string _description;
        [SerializeField] private Sprite _icon;

        [Header("Prefab")]
        [SerializeField] private GameObject _birdPrefab;

        [Header("Stats")]
        [SerializeField] private float _groundSpeed = 10f;
        [SerializeField] private float _flightSpeed = 15f;
        [SerializeField] private float _maxHealth = 100f;

        [Header("Preview")]
        [SerializeField] private GameObject _previewPrefab;
        [SerializeField] private Vector3 _previewRotation = Vector3.zero;
        [SerializeField] private float _previewScale = 1f;

        // Public properties for read access
        public string BirdName => _birdName;
        public string Description => _description;
        public Sprite Icon => _icon;
        public GameObject BirdPrefab => _birdPrefab;
        public float GroundSpeed => _groundSpeed;
        public float FlightSpeed => _flightSpeed;
        public float MaxHealth => _maxHealth;
        public GameObject PreviewPrefab => _previewPrefab;
        public Vector3 PreviewRotation => _previewRotation;
        public float PreviewScale => _previewScale;
    }
}
