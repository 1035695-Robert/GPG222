using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

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
        private Vector2 _moveValue;
        [SerializeField] private PlayerInteractModel interactState;




        private void FixedUpdate()
        {
            if (!IsServer) return;
            isHolding = interactState.isHolding.Value;
            Vector3 directionInput = new Vector3(_moveValue.x, 0, _moveValue.y).normalized;
            Move(directionInput);
            Rotation(directionInput);

        }

        public void NetworkMoveInput(Vector2 clientMoveValue)
        {
            Debug.Log("NetworkMoveInput");
            _moveValue = clientMoveValue;
        }


        private void Move(Vector3 directionInput)
        {
            playerRigidbody.MovePosition(playerRigidbody.position + directionInput * (walkSpeed * Time.fixedDeltaTime));
        }

        private void Rotation(Vector3 directionInput)
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