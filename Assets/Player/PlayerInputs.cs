using Unity.Netcode;
using UnityEngine;


namespace Player
{
    public class PlayerInputs : NetworkBehaviour
    {
        private ControlInputs _inputs;
        private Rigidbody _rigidbody;


        public delegate void Pickup();

        public static event Pickup OnPickup;


        [Header("movement speed values")] [SerializeField]
        private float walkSpeed = 2f;

        [SerializeField] private float rotationSpeed = 5f;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _inputs = new ControlInputs();
            if (_inputs == null) Debug.LogError("error");
        }

        private void OnEnable()
        {
            _inputs.Enable();
        }

        private void OnDisable()
        {
            _inputs.Disable();
        }

        private void Update()
        {
            if (!IsOwner) return;
            Vector2 moveDirectionInput = _inputs.Player.Move.ReadValue<Vector2>();
            Vector3 moveDirection = new Vector3(moveDirectionInput.x, 0, moveDirectionInput.y);
            Move_Rpc(moveDirection);
            if (moveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                Rotate_Rpc(targetRotation);
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            if (collision.gameObject.CompareTag("Interactable"))
            { 
                Debug.Log("touched");
                if (_inputs.Player.Interact.WasPerformedThisFrame())
                    if (OnPickup != null)
                        OnPickup();
            }
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void Move_Rpc(Vector3 directionInput)
        {
            _rigidbody.MovePosition(_rigidbody.position + directionInput * (walkSpeed * Time.deltaTime));
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void Rotate_Rpc(Quaternion rotateDirection)
        {
            _rigidbody.rotation = Quaternion.Slerp(
                transform.rotation, rotateDirection,
                rotationSpeed * Time.deltaTime * 5f);
        }
    }
}