using Unity.Netcode;
using UnityEngine;

namespace Player.hands
{
    public class HandsView : NetworkBehaviour
    {
        [SerializeField] private HandsModel handModel;

       

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            handModel.GrabClient += HandGrabServer;
            handModel.Drop += HandDropServer;
        }


        private void HandGrabServer(NetworkObject grabObject, Vector3 hitPoint)
        {
            transform.SetParent(grabObject.transform);
            HandGrabClient_Rpc(hitPoint);
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void HandGrabClient_Rpc(Vector3 hitPoint)
        {
            transform.position = hitPoint + new Vector3(0, 0.5f, 0);
        }


        private void HandDropServer(NetworkObject player)
        {
            transform.SetParent(player.transform);
            HandDropClient_Rpc();
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void HandDropClient_Rpc()
        {
            
            transform.localPosition = Vector3.zero;
           
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            handModel.GrabClient -= HandGrabServer;
            handModel.Drop -= HandDropServer;
        }
    }
}