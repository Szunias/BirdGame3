using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BirdGame.Managers
{
    public class Debug_MatchStarter : MonoBehaviour
    {
        private void Update()
        {
            if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer) return;

            if (Keyboard.current.pKey.wasPressedThisFrame)
            {
                if (Mgr_Match.Instance != null)
                {
                    if (Mgr_Match.Instance.CurrentState == MatchState.Waiting)
                    {
                        Debug.Log("[Debug] Starting match...");
                        Mgr_Match.Instance.StartMatch();
                    }
                    else if (Mgr_Match.Instance.CurrentState == MatchState.End)
                    {
                        Debug.Log("[Debug] Restarting match...");
                        Mgr_Match.Instance.RestartMatch();
                    }
                }
            }

            if (Keyboard.current.oKey.wasPressedThisFrame)
            {
                if (Mgr_Match.Instance != null)
                {
                    Debug.Log($"[Debug] Match State: {Mgr_Match.Instance.CurrentState}, Time: {Mgr_Match.Instance.PhaseTimeRemaining:F1}s");
                }
            }
        }

        private void OnGUI()
        {
            if (Mgr_Match.Instance == null) return;

            GUILayout.BeginArea(new Rect(10, 10, 300, 100));
            GUILayout.Label($"State: {Mgr_Match.Instance.CurrentState}");
            GUILayout.Label($"Time: {Mgr_Match.Instance.PhaseTimeRemaining:F1}s");

            if (Mgr_EggSpawner.Instance != null)
            {
                GUILayout.Label($"Eggs: {Mgr_EggSpawner.Instance.CurrentEggCount}/{Mgr_EggSpawner.Instance.MaxEggs}");
            }

            GUILayout.Label("P = Start/Restart | O = Log Status");
            GUILayout.EndArea();
        }
    }
}
