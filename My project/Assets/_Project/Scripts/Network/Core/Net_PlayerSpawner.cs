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

        private void Start()
        {
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnServerStarted += OnServerStarted;
                NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
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
        }

        private void OnClientConnected(ulong clientId)
        {
            if (!NetworkManager.Singleton.IsServer) return;

            SpawnPlayerForClient(clientId);
        }

        private void SpawnPlayerForClient(ulong clientId)
        {
            GameObject prefabToSpawn = GetBirdPrefab();
            if (prefabToSpawn == null)
            {
                Debug.LogError("Net_PlayerSpawner: No bird prefab to spawn!");
                return;
            }

            Vector3 spawnPos = GetSpawnPosition(clientId);
            Quaternion spawnRot = Quaternion.identity;

            GameObject playerInstance = Instantiate(prefabToSpawn, spawnPos, spawnRot);
            NetworkObject networkObject = playerInstance.GetComponent<NetworkObject>();

            if (networkObject != null)
            {
                networkObject.SpawnAsPlayerObject(clientId);
                Debug.Log($"Net_PlayerSpawner: Spawned bird for client {clientId}");
            }
            else
            {
                Debug.LogError("Net_PlayerSpawner: Bird prefab missing NetworkObject component!");
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
                return Vector3.zero;
            }

            int index = (int)(clientId % (ulong)spawnPoints.Length);
            return spawnPoints[index].position;
        }
    }
}
