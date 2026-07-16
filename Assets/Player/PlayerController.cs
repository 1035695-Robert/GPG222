using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;



namespace Player
{
    // player controller = inputs from controls
    public class PlayerController : NetworkBehaviour
    {
        [SerializeField] private PlayerMovementModel playerMovementModel;

        
        [SerializeField] private PlayerInteractModel playerInteractModel;
        private ControlInputs _playerInputs;
        private Vector2 _moveValue;

        

        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
            {
                Debug.Log("NO PLAYER");
                return;
            }
            base.OnNetworkSpawn();

            if (playerMovementModel == null)
            {
                playerMovementModel = GetComponent<PlayerMovementModel>();
            }
            
            

            _playerInputs = new ControlInputs();
            if (_playerInputs == null) Debug.LogError("error");
            _playerInputs.Player.Move.performed += PlayerMove;
            _playerInputs.Player.Move.canceled += PlayerMove;
            _playerInputs.Player.Interact.performed += Interaction;
            _playerInputs.Enable();
        }

       

        private void PlayerMove(InputAction.CallbackContext ctx)
        {
            if (!IsOwner) return;
            _moveValue = ctx.ReadValue<Vector2>();
            MovementInputRequest_Rpc(_moveValue);
        }

        [Rpc(SendTo.Server, Delivery = RpcDelivery.Reliable)]
        private void MovementInputRequest_Rpc(Vector2 moveValue)
        {
            playerMovementModel.NetworkMoveInput(moveValue);
        }


        private void Interaction(InputAction.CallbackContext obj)
        {
            playerInteractModel.Interact_Rpc();
        }

        public override void OnNetworkDespawn()
        {
            if (!IsOwner) return;
            base.OnNetworkDespawn();
            _playerInputs.Player.Move.performed -= PlayerMove;
            _playerInputs.Player.Move.canceled -= PlayerMove;
            _playerInputs.Player.Interact.performed -= Interaction;
            _playerInputs.Disable();
        }
    }
}