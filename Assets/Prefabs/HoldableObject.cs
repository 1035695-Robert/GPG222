using Unity.Netcode;
using UnityEngine;

namespace Prefabs
{
    public class HoldableObject : NetworkBehaviour
    {
        [SerializeField] private FixedJoint[] holdingPoints;

       private void Awake()
        {
            
        }

       
       
        public void PickUp(GameObject hands)
        {
            foreach (var t in holdingPoints)
            {
                if (t == null)
                {
                    t.connectedBody = hands.GetComponent<Rigidbody>();
                    return;
                }
            }
            
        }
        
    }
}