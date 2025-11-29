using BirdGame.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BirdGame.UI
{
    /// <summary>
    /// Main HUD controller displaying match info, scores, and timer.
    /// Single Responsibility: HUD display and updates.
    /// </summary>
    public class UI_HUD : MonoBehaviour
    {
        #region Constants
        private const float SECONDS_PER_MINUTE = 60f;
        private const int TEAM_ONE_ID = 1;
        private const int TEAM_TWO_ID = 2;
        #endregion

        #region Serialized Fields
        [Header("Timer")]
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private TextMeshProUGUI phaseText;

        [Header("Score")]
        [SerializeField] private TextMeshProUGUI team1ScoreText;
        [SerializeField] private TextMeshProUGUI team2ScoreText;

        [Header("Egg Count")]
        [SerializeField] private TextMeshProUGUI eggCountText;
        [SerializeField] private Image eggIcon;

        [Header("Phase Colors")]
        [SerializeField] private Color scrambleColor = new Color(0.2f, 0.8f, 0.2f);
        [SerializeField] private Color heistColor = new Color(0.8f, 0.6f, 0.2f);
        [SerializeField] private Color frenzyColor = new Color(0.9f, 0.2f, 0.2f);
        [SerializeField] private Color waitingColor = new Color(0.5f, 0.5f, 0.5f);
        #endregion

        #region Private Fields
        private MatchState _lastState;
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            SubscribeToMatchEvents();
        }

        private void OnDestroy()
        {
            UnsubscribeFromMatchEvents();
        }

        private void Update()
        {
            UpdateEggCount();
        }
        #endregion

        #region Event Subscription
        private void SubscribeToMatchEvents()
        {
            if (Mgr_Match.Instance != null)
            {
                Mgr_Match.Instance.OnStateChanged.AddListener(OnMatchStateChanged);
                Mgr_Match.Instance.OnTimerUpdated.AddListener(OnTimerUpdated);

                OnMatchStateChanged(Mgr_Match.Instance.CurrentState);
                OnTimerUpdated(Mgr_Match.Instance.PhaseTimeRemaining);
            }
        }

        private void UnsubscribeFromMatchEvents()
        {
            if (Mgr_Match.Instance != null)
            {
                Mgr_Match.Instance.OnStateChanged.RemoveListener(OnMatchStateChanged);
                Mgr_Match.Instance.OnTimerUpdated.RemoveListener(OnTimerUpdated);
            }
        }
        #endregion

        #region Event Handlers
        private void OnMatchStateChanged(MatchState state)
        {
            _lastState = state;
            UpdatePhaseDisplay(state);
        }

        private void OnTimerUpdated(float timeRemaining)
        {
            UpdateTimerDisplay(timeRemaining);
        }
        #endregion

        #region Display Updates
        private void UpdateTimerDisplay(float timeRemaining)
        {
            if (timerText == null) return;

            int minutes = Mathf.FloorToInt(timeRemaining / SECONDS_PER_MINUTE);
            int seconds = Mathf.FloorToInt(timeRemaining % SECONDS_PER_MINUTE);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }

        private void UpdatePhaseDisplay(MatchState state)
        {
            if (phaseText == null) return;

            string phaseName = state switch
            {
                MatchState.Waiting => "WAITING",
                MatchState.Scramble => "SCRAMBLE",
                MatchState.Heist => "HEIST",
                MatchState.Frenzy => "FRENZY!",
                MatchState.End => "GAME OVER",
                _ => ""
            };

            Color phaseColor = state switch
            {
                MatchState.Scramble => scrambleColor,
                MatchState.Heist => heistColor,
                MatchState.Frenzy => frenzyColor,
                _ => waitingColor
            };

            phaseText.text = phaseName;
            phaseText.color = phaseColor;

            if (timerText != null)
            {
                timerText.color = phaseColor;
            }
        }

        private void UpdateEggCount()
        {
            if (eggCountText == null) return;
            if (Mgr_EggSpawner.Instance == null) return;

            int current = Mgr_EggSpawner.Instance.CurrentEggCount;
            int max = Mgr_EggSpawner.Instance.MaxEggs;
            eggCountText.text = $"{current}/{max}";
        }
        #endregion

        #region Public Methods
        public void UpdateTeamScore(int team, int score)
        {
            if (team == TEAM_ONE_ID && team1ScoreText != null)
            {
                team1ScoreText.text = score.ToString();
            }
            else if (team == TEAM_TWO_ID && team2ScoreText != null)
            {
                team2ScoreText.text = score.ToString();
            }
        }

        public void SetTeamScores(int team1Score, int team2Score)
        {
            if (team1ScoreText != null)
            {
                team1ScoreText.text = team1Score.ToString();
            }
            if (team2ScoreText != null)
            {
                team2ScoreText.text = team2Score.ToString();
            }
        }
        #endregion
    }
}
