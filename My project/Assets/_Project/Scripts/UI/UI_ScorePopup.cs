using TMPro;
using UnityEngine;

namespace BirdGame.UI
{
    public class UI_ScorePopup : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Animation")]
        [SerializeField] private float floatSpeed = 1f;
        [SerializeField] private float fadeDuration = 1f;
        [SerializeField] private float lifetime = 1.5f;
        [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Header("Colors")]
        [SerializeField] private Color positiveColor = new Color(0.2f, 0.9f, 0.2f);
        [SerializeField] private Color negativeColor = new Color(0.9f, 0.2f, 0.2f);

        private float _spawnTime;
        private Vector3 _startPosition;
        private Vector3 _startScale;
        private Camera _mainCamera;

        private void Awake()
        {
            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
            }
            _startScale = transform.localScale;
            _mainCamera = Camera.main;
        }

        public void Initialize(int score, Vector3 worldPosition)
        {
            _spawnTime = Time.time;
            _startPosition = worldPosition;
            transform.position = worldPosition;

            if (scoreText != null)
            {
                string prefix = score > 0 ? "+" : "";
                scoreText.text = $"{prefix}{score}";
                scoreText.color = score >= 0 ? positiveColor : negativeColor;
            }

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
            }

            transform.localScale = Vector3.zero;
        }

        private void Update()
        {
            float elapsed = Time.time - _spawnTime;
            float normalizedTime = elapsed / lifetime;

            if (normalizedTime >= 1f)
            {
                Destroy(gameObject);
                return;
            }

            // Float upward
            transform.position = _startPosition + Vector3.up * (floatSpeed * elapsed);

            // Scale animation
            float scaleValue = scaleCurve.Evaluate(Mathf.Min(normalizedTime * 2f, 1f));
            transform.localScale = _startScale * scaleValue;

            // Fade out
            if (canvasGroup != null)
            {
                float fadeStart = 1f - (fadeDuration / lifetime);
                if (normalizedTime > fadeStart)
                {
                    float fadeProgress = (normalizedTime - fadeStart) / (1f - fadeStart);
                    canvasGroup.alpha = 1f - fadeProgress;
                }
            }

            // Billboard - face camera
            if (_mainCamera != null)
            {
                transform.rotation = _mainCamera.transform.rotation;
            }
        }
    }
}
