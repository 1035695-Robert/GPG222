using Unity.Netcode;
using UnityEngine;

namespace Player
{
    public class HandsController : NetworkBehaviour
    {
        [SerializeField] private HandsModel model;
        [SerializeField] private NetworkObject player;
       
        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;
            Debug.Log("Spawning hands");
            player = gameObject.transform.root.GetComponent<NetworkObject>();
            model.OnSpawn(player);
        }

        public override void OnNetworkDespawn()
        {
            //disconnect
        }
    }
}