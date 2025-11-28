using System.Collections.Generic;
using UnityEngine;

namespace BirdGame.UI
{
    public class UI_WorldManager : MonoBehaviour
    {
        public static UI_WorldManager Instance { get; private set; }

        [Header("Popup Settings")]
        [SerializeField] private GameObject scorePopupPrefab;
        [SerializeField] private int popupPoolSize = 20;

        [Header("Indicator Settings")]
        [SerializeField] private GameObject nestIndicatorPrefab;
        [SerializeField] private RectTransform indicatorContainer;

        [Header("References")]
        [SerializeField] private Canvas worldCanvas;
        [SerializeField] private Transform localPlayer;

        private Queue<UI_ScorePopup> _popupPool;
        private List<UI_NestIndicator> _nestIndicators;
        private RectTransform _canvasRect;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            _popupPool = new Queue<UI_ScorePopup>();
            _nestIndicators = new List<UI_NestIndicator>();

            if (worldCanvas != null)
            {
                _canvasRect = worldCanvas.GetComponent<RectTransform>();
            }

            InitializePopupPool();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void InitializePopupPool()
        {
            if (scorePopupPrefab == null) return;

            for (int i = 0; i < popupPoolSize; i++)
            {
                var popup = Instantiate(scorePopupPrefab, worldCanvas.transform);
                var popupComponent = popup.GetComponent<UI_ScorePopup>();
                popup.SetActive(false);
                _popupPool.Enqueue(popupComponent);
            }
        }

        public void SpawnScorePopup(Vector3 worldPosition, int score)
        {
            if (scorePopupPrefab == null) return;

            UI_ScorePopup popup;

            if (_popupPool.Count > 0)
            {
                popup = _popupPool.Dequeue();
                popup.gameObject.SetActive(true);
            }
            else
            {
                var go = Instantiate(scorePopupPrefab, worldCanvas.transform);
                popup = go.GetComponent<UI_ScorePopup>();
            }

            popup.Initialize(score, worldPosition);
        }

        public void ReturnPopupToPool(UI_ScorePopup popup)
        {
            popup.gameObject.SetActive(false);
            _popupPool.Enqueue(popup);
        }

        public UI_NestIndicator CreateNestIndicator(Transform nestTransform, bool isFriendly)
        {
            if (nestIndicatorPrefab == null || indicatorContainer == null) return null;

            var indicatorGO = Instantiate(nestIndicatorPrefab, indicatorContainer);
            var indicator = indicatorGO.GetComponent<UI_NestIndicator>();

            if (indicator != null)
            {
                indicator.Initialize(nestTransform, localPlayer, _canvasRect, isFriendly);
                _nestIndicators.Add(indicator);
            }

            return indicator;
        }

        public void RemoveNestIndicator(UI_NestIndicator indicator)
        {
            if (indicator == null) return;

            _nestIndicators.Remove(indicator);
            Destroy(indicator.gameObject);
        }

        public void ClearAllIndicators()
        {
            foreach (var indicator in _nestIndicators)
            {
                if (indicator != null)
                {
                    Destroy(indicator.gameObject);
                }
            }
            _nestIndicators.Clear();
        }

        public void SetLocalPlayer(Transform player)
        {
            localPlayer = player;

            foreach (var indicator in _nestIndicators)
            {
                if (indicator != null)
                {
                    indicator.Initialize(null, player, _canvasRect, true);
                }
            }
        }
    }
}
