using BirdGame.Core.Interfaces;
using BirdGame.UI.FX;
using Unity.Netcode;
using UnityEngine;

namespace BirdGame.Bird.Core
{
    public class Bird_Blindable : NetworkBehaviour, IBlindable
    {
        [Header("References")]
        [SerializeField] private FX_EggSplat splatEffect;

        public void ApplyBlind(float duration)
        {
            if (IsServer)
            {
                ApplyBlindClientRpc(duration);
            }
        }

        [ClientRpc]
        private void ApplyBlindClientRpc(float duration)
        {
            // Only show effect on owner's screen
            if (!IsOwner) return;

            if (splatEffect != null)
            {
                splatEffect.ShowSplat(duration);
            }
        }
    }
}
