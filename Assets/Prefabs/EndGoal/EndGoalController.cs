using Unity.Netcode;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Prefabs.EndGoal
{
    public class EndGoalController : NetworkBehaviour
    {
        [SerializeField] private GameObject movableObject;
    
        [SerializeField] private BoxCollider zoneCollider;
        [SerializeField] private Collider[] moveableObjectCollider;
   
        [SerializeField] private EndGoalModel goalModel;
        [SerializeField] private EndGoalView goalView;
        
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            
            moveableObjectCollider = movableObject.GetComponentsInChildren<Collider>(true);
            zoneCollider = GetComponent<BoxCollider>();

            goalModel.IsCompleted += OnServerCompletion;
        }

        void OnTriggerStay(Collider other)
        {
            if (!IsServer) return;
            goalModel.IsObjectFullyInArea(zoneCollider, moveableObjectCollider);
        }

        
        private void OnServerCompletion()
        {  
            OnClientCompletion_Rpc();
        }
        [Rpc(SendTo.ClientsAndHost, Delivery = RpcDelivery.Reliable)]
        private void OnClientCompletion_Rpc()
        {
            goalView.LevelCompleted();
        }
        

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            goalModel.IsCompleted -= OnServerCompletion;
        }
    }
}
