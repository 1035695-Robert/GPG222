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
        
        private Renderer _playerRenderer;
        
        [SerializeField] private PlayerColourModel colourState;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            interactModel.OnPickupEvent += Grab;
            interactModel.OnDroppedEvent += Drop;
            
            _playerRenderer = GetComponent<Renderer>();
            colourState.networkBodyColour.OnValueChanged += CheckBodyColourUpdate;
            
        }

       

        private void CheckBodyColourUpdate(Color previousValue, Color newValue)
        {
            ColourChange_Rpc(newValue);
        }

        [Rpc(SendTo.ClientsAndHost, Delivery = RpcDelivery.Unreliable)]
        private void ColourChange_Rpc(Color newValue)
        {
            _playerRenderer.material.color = newValue;
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