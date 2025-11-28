using UnityEngine;
using UnityEngine.UI;

namespace BirdGame.UI
{
    public class UI_NestIndicator : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private RectTransform indicatorRect;
        [SerializeField] private Image arrowImage;
        [SerializeField] private Image nestIcon;

        [Header("Settings")]
        [SerializeField] private float edgePadding = 50f;
        [SerializeField] private float minDistance = 10f;
        [SerializeField] private float maxDistance = 100f;

        [Header("Colors")]
        [SerializeField] private Color friendlyColor = new Color(0.2f, 0.7f, 1f);
        [SerializeField] private Color enemyColor = new Color(1f, 0.3f, 0.3f);

        private Transform _target;
        private Transform _player;
        private Camera _mainCamera;
        private RectTransform _canvasRect;
        private bool _isFriendly;

        public void Initialize(Transform target, Transform player, RectTransform canvasRect, bool isFriendly)
        {
            _target = target;
            _player = player;
            _canvasRect = canvasRect;
            _isFriendly = isFriendly;
            _mainCamera = Camera.main;

            Color indicatorColor = isFriendly ? friendlyColor : enemyColor;
            if (arrowImage != null) arrowImage.color = indicatorColor;
            if (nestIcon != null) nestIcon.color = indicatorColor;
        }

        private void Update()
        {
            if (_target == null || _player == null || _mainCamera == null)
            {
                gameObject.SetActive(false);
                return;
            }

            UpdateIndicator();
        }

        private void UpdateIndicator()
        {
            Vector3 targetScreenPos = _mainCamera.WorldToScreenPoint(_target.position);

            // Check if target is behind camera
            bool isBehind = targetScreenPos.z < 0;
            if (isBehind)
            {
                targetScreenPos.x = Screen.width - targetScreenPos.x;
                targetScreenPos.y = Screen.height - targetScreenPos.y;
            }

            // Check if on screen
            bool isOnScreen = !isBehind &&
                              targetScreenPos.x > edgePadding &&
                              targetScreenPos.x < Screen.width - edgePadding &&
                              targetScreenPos.y > edgePadding &&
                              targetScreenPos.y < Screen.height - edgePadding;

            if (isOnScreen)
            {
                // Hide indicator when nest is visible
                if (arrowImage != null) arrowImage.enabled = false;
                if (nestIcon != null) nestIcon.enabled = true;

                indicatorRect.position = targetScreenPos;
                indicatorRect.rotation = Quaternion.identity;
            }
            else
            {
                // Show arrow pointing to nest
                if (arrowImage != null) arrowImage.enabled = true;
                if (nestIcon != null) nestIcon.enabled = false;

                // Clamp to screen edge
                Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
                Vector3 direction = (targetScreenPos - screenCenter).normalized;

                // Calculate edge position
                float angle = Mathf.Atan2(direction.y, direction.x);
                float cos = Mathf.Cos(angle);
                float sin = Mathf.Sin(angle);

                float halfWidth = (Screen.width / 2f) - edgePadding;
                float halfHeight = (Screen.height / 2f) - edgePadding;

                float edgeX = halfWidth / Mathf.Abs(cos);
                float edgeY = halfHeight / Mathf.Abs(sin);
                float edgeDistance = Mathf.Min(edgeX, edgeY);

                Vector3 edgePos = screenCenter + direction * edgeDistance;
                indicatorRect.position = edgePos;

                // Rotate arrow to point toward target
                float rotationAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                indicatorRect.rotation = Quaternion.Euler(0f, 0f, rotationAngle - 90f);
            }

            // Update scale based on distance
            float distance = Vector3.Distance(_player.position, _target.position);
            float normalizedDistance = Mathf.InverseLerp(maxDistance, minDistance, distance);
            float scale = Mathf.Lerp(0.5f, 1f, normalizedDistance);
            indicatorRect.localScale = Vector3.one * scale;
        }

        public void SetTarget(Transform target)
        {
            _target = target;
        }
    }
}
