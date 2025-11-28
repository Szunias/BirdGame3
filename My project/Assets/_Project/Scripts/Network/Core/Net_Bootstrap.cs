using Unity.Netcode;
using UnityEngine;

namespace BirdGame.Network.Core
{
    public class Net_Bootstrap : MonoBehaviour
    {
        [SerializeField] private bool autoStartHost = false;

        private void Start()
        {
            if (NetworkManager.Singleton == null) return;

            // Simple logic: If autoStartHost is checked, start host immediately.
            // Ignoring complex ParrelSync logic for now to simplify testing.
            if (autoStartHost)
            {
                Debug.Log("Net_Bootstrap: Auto-Starting Host...");
                NetworkManager.Singleton.StartHost();
            }
        }

        private void OnGUI()
        {
            if (NetworkManager.Singleton == null) return;

            // If we are already running (Host or Client), show status
            if (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsServer)
            {
                GUILayout.BeginArea(new Rect(10, 10, 200, 50));
                if (NetworkManager.Singleton.IsHost) GUILayout.Label("Status: HOST");
                else if (NetworkManager.Singleton.IsServer) GUILayout.Label("Status: SERVER");
                else GUILayout.Label("Status: CLIENT");
                GUILayout.EndArea();
                return;
            }

            // If not running, show buttons
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));

            if (GUILayout.Button("Start Host", GUILayout.Height(40))) NetworkManager.Singleton.StartHost();
            if (GUILayout.Button("Start Client", GUILayout.Height(40))) NetworkManager.Singleton.StartClient();
            if (GUILayout.Button("Start Server", GUILayout.Height(40))) NetworkManager.Singleton.StartServer();

            GUILayout.EndArea();
        }
    }
}
