using Unity.Netcode;
using UnityEngine;

namespace Player
{
    public class HandsController : NetworkBehaviour
    {
        [SerializeField] private HandsModel model;
        [SerializeField] private NetworkObject player;
        [SerializeField] private PlayerModel playerModel;
        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;
            Debug.Log("Spawning hands");
            
        }

       

      

        public override void OnNetworkDespawn()
        {
            //disconnect
        }
    }
}