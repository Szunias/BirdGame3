using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BirdGame.Managers
{
    /// <summary>
    /// Debug utility for starting/restarting matches during development.
    /// Single Responsibility: Debug controls for match flow.
    /// </summary>
    public class Debug_MatchStarter : MonoBehaviour
    {
        #region Constants
        private const float GUI_AREA_X = 10f;
        private const float GUI_AREA_Y = 10f;
        private const float GUI_AREA_WIDTH = 300f;
        private const float GUI_AREA_HEIGHT = 100f;
        #endregion

        #region Unity Lifecycle
        private void Update()
        {
            if (!IsServerRunning()) return;

            HandleStartRestartInput();
            HandleStatusLogInput();
        }
        #endregion

        #region Input Handling
        private bool IsServerRunning()
        {
            return NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer;
        }

        private void HandleStartRestartInput()
        {
            if (!Keyboard.current.pKey.wasPressedThisFrame) return;
            if (Mgr_Match.Instance == null) return;

            if (Mgr_Match.Instance.CurrentState == MatchState.Waiting)
            {
                Mgr_Match.Instance.StartMatch();
            }
            else if (Mgr_Match.Instance.CurrentState == MatchState.End)
            {
                Mgr_Match.Instance.RestartMatch();
            }
        }

        private void HandleStatusLogInput()
        {
            if (!Keyboard.current.oKey.wasPressedThisFrame) return;
            if (Mgr_Match.Instance == null) return;

            Debug.Log($"[Debug] Match State: {Mgr_Match.Instance.CurrentState}, Time: {Mgr_Match.Instance.PhaseTimeRemaining:F1}s");
        }
        #endregion

        #region Debug GUI
        private void OnGUI()
        {
            if (Mgr_Match.Instance == null) return;

            GUILayout.BeginArea(new Rect(GUI_AREA_X, GUI_AREA_Y, GUI_AREA_WIDTH, GUI_AREA_HEIGHT));

            GUILayout.Label($"State: {Mgr_Match.Instance.CurrentState}");
            GUILayout.Label($"Time: {Mgr_Match.Instance.PhaseTimeRemaining:F1}s");

            if (Mgr_EggSpawner.Instance != null)
            {
                GUILayout.Label($"Eggs: {Mgr_EggSpawner.Instance.CurrentEggCount}/{Mgr_EggSpawner.Instance.MaxEggs}");
            }

            GUILayout.Label("P = Start/Restart | O = Log Status");

            GUILayout.EndArea();
        }
        #endregion
    }
}
