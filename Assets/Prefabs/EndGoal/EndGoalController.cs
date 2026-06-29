using Unity.Netcode;
using UnityEngine;

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

            goalModel.IsCompleted += OnCompletion_Rpc;
        }

        void OnTriggerStay(Collider other)
        { 
            goalModel.IsObjectFullyInArea(zoneCollider, moveableObjectCollider);
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void OnCompletion_Rpc()
        { 
            goalView.LevelCompleted();
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            goalModel.IsCompleted -= OnCompletion_Rpc;
        }
    }
}
