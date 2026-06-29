using System;
using Unity.Netcode;
using UnityEngine;

namespace Player
{
    //this is for server gameplay side
    public class PlayerMovementModel : NetworkBehaviour
    {
        //private Vector3 _directionInput;

        [Header("movement speed values")] [SerializeField]
        private float walkSpeed = 1f;

        [SerializeField] private float rotationSpeed = 5f;
        [SerializeField] private Rigidbody playerRigidbody;
        public bool isHolding = false;
        public Vector2 moveValue;
       [SerializeField] private PlayerInteractModel _interactState;
        
        
       

        private void FixedUpdate()
        {
            if (!IsServer) return;
            isHolding = _interactState.isHolding.Value;   
            Vector3 directionInput = new Vector3(moveValue.x, 0, moveValue.y).normalized;
            Move_Rpc(directionInput);
            Rotation_Rpc(directionInput);
            
        }

        public void NetworkMoveInput(Vector2 clientMoveValue)
        {
            Debug.Log("NetworkMoveInput");
            moveValue = clientMoveValue;
        }


        private void Move_Rpc(Vector3 directionInput)
        {
            playerRigidbody.MovePosition(playerRigidbody.position + directionInput * (walkSpeed * Time.fixedDeltaTime));
        }

        private void Rotation_Rpc(Vector3 directionInput)
        {
            if (isHolding) return;
            if (directionInput != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionInput, Vector3.up);
                playerRigidbody.rotation = Quaternion.Slerp(
                    transform.rotation, targetRotation,
                    rotationSpeed * Time.deltaTime * 5f);
            }
            else
                playerRigidbody.angularVelocity = Vector3.zero;
        }
    }
}