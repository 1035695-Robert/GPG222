using Unity.Netcode;
using UnityEngine;

namespace Player.hands
{
    public class HandsView : NetworkBehaviour
    {
        [SerializeField] private HandsModel handModel;
        [SerializeField] private PlayerInteractModel interactState;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            handModel.Grab += HandGrab;
            handModel.Drop += HandDrop;
        }


        private void HandGrab(NetworkObject grabObject, Vector3 hitPoint)
        {
            //transform.SetParent(grabObject.gameObject.transform);
            transform.position = hitPoint + new Vector3(0, 0.5f, 0);
        }


        private void HandDrop(NetworkObject player)
        {
            //transform.setParent(player.gameObject)

            transform.localPosition = new Vector3(0, 0, 0.75f);
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            handModel.Grab -= HandGrab;
            handModel.Drop -= HandDrop;
        }
    }
}