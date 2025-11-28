using BirdGame.Data;
using BirdGame.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BirdGame.UI
{
    public class UI_CharacterSelect : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI birdNameText;
        [SerializeField] private TextMeshProUGUI birdDescriptionText;
        [SerializeField] private Image birdIconImage;

        [Header("Stats Display")]
        [SerializeField] private TextMeshProUGUI speedStatText;
        [SerializeField] private TextMeshProUGUI flightStatText;
        [SerializeField] private TextMeshProUGUI healthStatText;

        [Header("Buttons")]
        [SerializeField] private Button previousButton;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button startButton;

        [Header("Preview")]
        [SerializeField] private Transform previewSpawnPoint;

        [Header("Game Scene")]
        [SerializeField] private string gameSceneName = "BootstrapScene";

        private GameObject _currentPreview;
        private bool _initialized;

        private void Start()
        {
            if (previousButton != null)
                previousButton.onClick.AddListener(OnPreviousClicked);

            if (nextButton != null)
                nextButton.onClick.AddListener(OnNextClicked);

            if (startButton != null)
                startButton.onClick.AddListener(OnStartClicked);

            TryInitialize();
        }

        private void Update()
        {
            if (!_initialized)
            {
                TryInitialize();
            }
        }

        private void TryInitialize()
        {
            if (Mgr_CharacterSelect.Instance != null && !_initialized)
            {
                _initialized = true;
                Mgr_CharacterSelect.Instance.OnBirdSelected.AddListener(OnBirdSelected);
                UpdateDisplay(Mgr_CharacterSelect.Instance.SelectedBird);
                Debug.Log($"UI_CharacterSelect initialized with bird: {Mgr_CharacterSelect.Instance.SelectedBird?.BirdName}");
            }
        }

        private void OnDestroy()
        {
            if (previousButton != null)
                previousButton.onClick.RemoveListener(OnPreviousClicked);

            if (nextButton != null)
                nextButton.onClick.RemoveListener(OnNextClicked);

            if (startButton != null)
                startButton.onClick.RemoveListener(OnStartClicked);

            if (Mgr_CharacterSelect.Instance != null)
            {
                Mgr_CharacterSelect.Instance.OnBirdSelected.RemoveListener(OnBirdSelected);
            }
        }

        private void OnPreviousClicked()
        {
            if (Mgr_CharacterSelect.Instance != null)
            {
                Mgr_CharacterSelect.Instance.SelectPreviousBird();
            }
        }

        private void OnNextClicked()
        {
            if (Mgr_CharacterSelect.Instance != null)
            {
                Mgr_CharacterSelect.Instance.SelectNextBird();
            }
        }

        private void OnStartClicked()
        {
            SceneManager.LoadScene(gameSceneName);
        }

        private void OnBirdSelected(Data_BirdDefinition bird)
        {
            UpdateDisplay(bird);
        }

        private void UpdateDisplay(Data_BirdDefinition bird)
        {
            if (bird == null) return;

            if (birdNameText != null)
                birdNameText.text = bird.BirdName;

            if (birdDescriptionText != null)
                birdDescriptionText.text = bird.Description;

            if (birdIconImage != null && bird.Icon != null)
                birdIconImage.sprite = bird.Icon;

            if (speedStatText != null)
                speedStatText.text = $"Speed: {bird.GroundSpeed}";

            if (flightStatText != null)
                flightStatText.text = $"Flight: {bird.FlightSpeed}";

            if (healthStatText != null)
                healthStatText.text = $"Health: {bird.MaxHealth}";

            UpdatePreview(bird);
        }

        private void UpdatePreview(Data_BirdDefinition bird)
        {
            if (_currentPreview != null)
            {
                Destroy(_currentPreview);
            }

            if (bird.PreviewPrefab != null && previewSpawnPoint != null)
            {
                _currentPreview = Instantiate(bird.PreviewPrefab, previewSpawnPoint.position, Quaternion.Euler(bird.PreviewRotation));
                _currentPreview.transform.localScale = Vector3.one * bird.PreviewScale;
            }
        }
    }
}
