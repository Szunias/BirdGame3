using UnityEngine;
using UnityEngine.UI;

namespace BirdGame.UI.FX
{
    public class FX_EggSplat : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image splatImage;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Settings")]
        [SerializeField] private float fadeInDuration = 0.1f;
        [SerializeField] private float fadeOutDuration = 0.5f;

        private float _blindDuration;
        private float _blindTimer;
        private SplatState _state;

        private enum SplatState
        {
            Inactive,
            FadingIn,
            Active,
            FadingOut
        }

        private void Awake()
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
            }
            _state = SplatState.Inactive;
        }

        private void Update()
        {
            if (_state == SplatState.Inactive) return;

            switch (_state)
            {
                case SplatState.FadingIn:
                    UpdateFadeIn();
                    break;
                case SplatState.Active:
                    UpdateActive();
                    break;
                case SplatState.FadingOut:
                    UpdateFadeOut();
                    break;
            }
        }

        public void ShowSplat(float duration)
        {
            _blindDuration = duration;
            _blindTimer = 0f;
            _state = SplatState.FadingIn;
        }

        public void HideSplat()
        {
            if (_state != SplatState.Inactive)
            {
                _state = SplatState.FadingOut;
                _blindTimer = 0f;
            }
        }

        private void UpdateFadeIn()
        {
            _blindTimer += Time.deltaTime;
            float t = Mathf.Clamp01(_blindTimer / fadeInDuration);

            if (canvasGroup != null)
            {
                canvasGroup.alpha = t;
            }

            if (t >= 1f)
            {
                _state = SplatState.Active;
                _blindTimer = 0f;
            }
        }

        private void UpdateActive()
        {
            _blindTimer += Time.deltaTime;

            if (_blindTimer >= _blindDuration)
            {
                _state = SplatState.FadingOut;
                _blindTimer = 0f;
            }
        }

        private void UpdateFadeOut()
        {
            _blindTimer += Time.deltaTime;
            float t = Mathf.Clamp01(_blindTimer / fadeOutDuration);

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f - t;
            }

            if (t >= 1f)
            {
                _state = SplatState.Inactive;
            }
        }
    }
}
