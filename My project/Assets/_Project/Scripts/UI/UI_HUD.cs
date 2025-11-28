using BirdGame.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BirdGame.UI
{
    public class UI_HUD : MonoBehaviour
    {
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

        private MatchState _lastState;

        private void Start()
        {
            if (Mgr_Match.Instance != null)
            {
                Mgr_Match.Instance.OnStateChanged.AddListener(OnMatchStateChanged);
                Mgr_Match.Instance.OnTimerUpdated.AddListener(OnTimerUpdated);

                OnMatchStateChanged(Mgr_Match.Instance.CurrentState);
                OnTimerUpdated(Mgr_Match.Instance.PhaseTimeRemaining);
            }
        }

        private void OnDestroy()
        {
            if (Mgr_Match.Instance != null)
            {
                Mgr_Match.Instance.OnStateChanged.RemoveListener(OnMatchStateChanged);
                Mgr_Match.Instance.OnTimerUpdated.RemoveListener(OnTimerUpdated);
            }
        }

        private void Update()
        {
            UpdateEggCount();
        }

        private void OnMatchStateChanged(MatchState state)
        {
            _lastState = state;
            UpdatePhaseDisplay(state);
        }

        private void OnTimerUpdated(float timeRemaining)
        {
            UpdateTimerDisplay(timeRemaining);
        }

        private void UpdateTimerDisplay(float timeRemaining)
        {
            if (timerText == null) return;

            int minutes = Mathf.FloorToInt(timeRemaining / 60f);
            int seconds = Mathf.FloorToInt(timeRemaining % 60f);
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

        public void UpdateTeamScore(int team, int score)
        {
            if (team == 1 && team1ScoreText != null)
            {
                team1ScoreText.text = score.ToString();
            }
            else if (team == 2 && team2ScoreText != null)
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
    }
}
