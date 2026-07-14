using Unity.Netcode;
using UnityEngine;

namespace Player
{
    public class PlayerView : NetworkBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip dropAudioClip;
        [SerializeField] private AudioClip grabAudioClip;
        [SerializeField] private PlayerInteractModel interactModel;
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            interactModel.OnPickupEvent += Grab;
            interactModel.OnDroppedEvent += Drop;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            interactModel.OnPickupEvent -= Grab;
            interactModel.OnDroppedEvent -= Drop;
        }

        private void Grab()
        {
            audioSource.PlayOneShot(grabAudioClip);
        }

        private void Drop()
        {
            audioSource.PlayOneShot(dropAudioClip);
        }
    }
}