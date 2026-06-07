using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerInputs : NetworkBehaviour
    {
        [SerializeField] private InputActionReference _move;
        private Vector2 _moveDirectionInput;

        private CharacterController _controller;

        [Header("movement speed values")] [SerializeField]
        private float walkSpeed = 2f;

        [SerializeField] private float rotationSpeed = 5f;

        private void Start()
        {
            _controller = GetComponent<CharacterController>();
        }

        private void OnEnable()
        {
            _move.action.Enable();
        }

        private void OnDisable()
        {
            _move.action.Disable();
        }

        private void Update()
        {
            _moveDirectionInput = _move.action.ReadValue<Vector2>();

            Vector3 moveDirection = new Vector3(_moveDirectionInput.x, 0, _moveDirectionInput.y);
            moveDirection.Normalize();

            _controller.Move(moveDirection * (walkSpeed * Time.deltaTime));

            if (moveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }
        }
    }
}