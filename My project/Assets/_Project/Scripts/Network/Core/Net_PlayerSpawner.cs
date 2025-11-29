using BirdGame.Data;
using BirdGame.Managers;
using Unity.Netcode;
using UnityEngine;

namespace BirdGame.Network.Core
{
    public class Net_PlayerSpawner : MonoBehaviour
    {
        [Header("Spawn Points")]
        [SerializeField] private Transform[] spawnPoints;

        [Header("Fallback")]
        [SerializeField] private GameObject defaultBirdPrefab;

        private bool _hasSpawnedLocalPlayer;

        private void Awake()
        {
            if (NetworkManager.Singleton == null) return;

            // Disable automatic player spawning BEFORE network starts
            NetworkManager.Singleton.NetworkConfig.PlayerPrefab = null;

            // Subscribe to events in Awake to ensure we catch them before Net_Bootstrap starts host
            NetworkManager.Singleton.OnServerStarted += OnServerStarted;
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

            Debug.Log("Net_PlayerSpawner: Initialized in Awake, subscribed to network events");
        }

        private void Start()
        {
            if (NetworkManager.Singleton == null) return;

            // Double-check PlayerPrefab is still disabled
            NetworkManager.Singleton.NetworkConfig.PlayerPrefab = null;

            // If host already started (e.g., Net_Bootstrap ran first), spawn the local player now
            // But only if we haven't already spawned via OnClientConnected
            if (NetworkManager.Singleton.IsServer && NetworkManager.Singleton.IsClient && !_hasSpawnedLocalPlayer)
            {
                Debug.Log("Net_PlayerSpawner: Host already running, spawning local player now");
                SpawnPlayerForClient(NetworkManager.Singleton.LocalClientId);
            }
        }

        private void OnDestroy()
        {
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            }
        }

        private void OnServerStarted()
        {
            // Disable default player prefab spawning - we handle it manually
            NetworkManager.Singleton.NetworkConfig.PlayerPrefab = null;
            Debug.Log($"Net_PlayerSpawner: Server started. SpawnPoints count: {(spawnPoints != null ? spawnPoints.Length : 0)}");
        }

        private void OnClientConnected(ulong clientId)
        {
            Debug.Log($"Net_PlayerSpawner: OnClientConnected called for client {clientId}. IsServer: {NetworkManager.Singleton.IsServer}");

            if (!NetworkManager.Singleton.IsServer) return;

            SpawnPlayerForClient(clientId);
        }

        private void SpawnPlayerForClient(ulong clientId)
        {
            Debug.Log($"Net_PlayerSpawner: SpawnPlayerForClient called for client {clientId}");

            GameObject prefabToSpawn = GetBirdPrefab();
            if (prefabToSpawn == null)
            {
                Debug.LogError("Net_PlayerSpawner: No bird prefab to spawn!");
                return;
            }

            Vector3 spawnPos = GetSpawnPosition(clientId);
            Quaternion spawnRot = Quaternion.identity;

            Debug.Log($"Net_PlayerSpawner: Instantiating {prefabToSpawn.name} at position {spawnPos}");
            GameObject playerInstance = Instantiate(prefabToSpawn, spawnPos, spawnRot);
            NetworkObject networkObject = playerInstance.GetComponent<NetworkObject>();

            if (networkObject != null)
            {
                networkObject.SpawnAsPlayerObject(clientId);
                Debug.Log($"Net_PlayerSpawner: Successfully spawned {prefabToSpawn.name} for client {clientId} at {spawnPos}");

                // Track that we've spawned the local player to prevent double-spawn
                if (clientId == NetworkManager.Singleton.LocalClientId)
                {
                    _hasSpawnedLocalPlayer = true;
                }
            }
            else
            {
                Debug.LogError($"Net_PlayerSpawner: Bird prefab {prefabToSpawn.name} missing NetworkObject component!");
                Destroy(playerInstance);
            }
        }

        private GameObject GetBirdPrefab()
        {
            // Try to get selected bird from character select
            if (Mgr_CharacterSelect.Instance != null)
            {
                GameObject selectedPrefab = Mgr_CharacterSelect.Instance.GetSelectedBirdPrefab();
                if (selectedPrefab != null)
                {
                    return selectedPrefab;
                }
            }

            // Fallback to default
            return defaultBirdPrefab;
        }

        private Vector3 GetSpawnPosition(ulong clientId)
        {
            if (spawnPoints == null || spawnPoints.Length == 0)
            {
                Debug.LogWarning("Net_PlayerSpawner: No spawn points set! Spawning at Vector3.zero");
                return Vector3.zero;
            }

            int index = (int)(clientId % (ulong)spawnPoints.Length);
            Vector3 pos = spawnPoints[index].position;
            Debug.Log($"Net_PlayerSpawner: Spawning client {clientId} at spawn point {index} -> {pos}");
            return pos;
        }
    }
}
