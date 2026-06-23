using Unity.Netcode;
using UnityEngine;

namespace Player
{
    public class HandsController : NetworkBehaviour
    {
        [SerializeField] private HandsModel model;
        [SerializeField] private GameObject player;

        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;
            Debug.Log("Spawning hands");
            FindOwner();
        }

        private void FindOwner()
        {
            //find player with same ownershipID
        }

        public override void OnNetworkDespawn()
        {
            //disconnect
        }
    }
}