using UnityEngine;
using UnityEngine.UI;

namespace BirdGame.Egg.UI
{
    public class Egg_ProgressUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image fillImage;
        [SerializeField] private GameObject progressBarRoot;

        [Header("Settings")]
        [SerializeField] private bool lookAtCamera = true;

        private Camera _mainCamera;

        private void Start()
        {
            _mainCamera = Camera.main;
            Hide();
        }

        private void LateUpdate()
        {
            if (lookAtCamera && _mainCamera != null && progressBarRoot.activeSelf)
            {
                transform.LookAt(transform.position + _mainCamera.transform.forward);
            }
        }

        public void Show()
        {
            progressBarRoot.SetActive(true);
        }

        public void Hide()
        {
            progressBarRoot.SetActive(false);
            SetProgress(0f);
        }

        public void SetProgress(float progress)
        {
            if (fillImage != null)
            {
                fillImage.fillAmount = Mathf.Clamp01(progress);
            }
        }
    }
}
