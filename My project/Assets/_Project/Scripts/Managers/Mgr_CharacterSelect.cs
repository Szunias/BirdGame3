using BirdGame.Data;
using UnityEngine;
using UnityEngine.Events;

namespace BirdGame.Managers
{
    public class Mgr_CharacterSelect : MonoBehaviour
    {
        public static Mgr_CharacterSelect Instance { get; private set; }

        [Header("Available Birds")]
        [SerializeField] private Data_BirdDefinition[] availableBirds;

        [Header("Events")]
        public UnityEvent<Data_BirdDefinition> OnBirdSelected;

        private int _selectedIndex;
        private Data_BirdDefinition _selectedBird;

        public Data_BirdDefinition SelectedBird => _selectedBird;
        public Data_BirdDefinition[] AvailableBirds => availableBirds;
        public int SelectedIndex => _selectedIndex;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (availableBirds != null && availableBirds.Length > 0)
            {
                SelectBird(0);
            }
        }

        public void SelectBird(int index)
        {
            if (availableBirds == null || availableBirds.Length == 0) return;

            _selectedIndex = Mathf.Clamp(index, 0, availableBirds.Length - 1);
            _selectedBird = availableBirds[_selectedIndex];
            OnBirdSelected?.Invoke(_selectedBird);
        }

        public void SelectNextBird()
        {
            int nextIndex = (_selectedIndex + 1) % availableBirds.Length;
            SelectBird(nextIndex);
        }

        public void SelectPreviousBird()
        {
            int prevIndex = _selectedIndex - 1;
            if (prevIndex < 0) prevIndex = availableBirds.Length - 1;
            SelectBird(prevIndex);
        }

        public GameObject GetSelectedBirdPrefab()
        {
            return _selectedBird != null ? _selectedBird.BirdPrefab : null;
        }
    }
}
