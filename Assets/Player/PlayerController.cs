using Player.hands;
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
        
        [SerializeField] private HandsModel handsModel;
        [SerializeField] private GameObject playerHands;
        [SerializeField] private NetworkObject networkHands;


        public override void OnNetworkSpawn()
        {
            if (!IsLocalPlayer)
            {
                return;
            }

            base.OnNetworkSpawn();

            if (playerMovementModel == null)
            {
                playerMovementModel = GetComponent<PlayerMovementModel>();
            }
            HandSpawn_Rpc();

            _playerInputs = new ControlInputs();
            if (_playerInputs == null) Debug.LogError("error");
            _playerInputs.Player.Move.performed += PlayerMove;
            _playerInputs.Player.Move.canceled += PlayerMove;
            _playerInputs.Player.Interact.performed += Interaction;
            _playerInputs.Player.HandRotation.performed += RotateHands;
            _playerInputs.Player.HandRotation.canceled += RotateHands;
            _playerInputs.Enable();
        }
        
        [Rpc(SendTo.Server)]
        void HandSpawn_Rpc()
        {
                networkHands = Instantiate(playerHands).GetComponent<NetworkObject>();
                networkHands.SpawnWithOwnership(OwnerClientId);
                handsModel = networkHands.GetComponent<HandsModel>();
                networkHands.TrySetParent(transform.Find("hands"), false);
                handsModel.Setup();
            
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

        private void RotateHands(InputAction.CallbackContext obj)
        {
            if (!IsOwner) return;
            float handRotationValue = obj.ReadValue<float>();
            HandRotation_Rpc(handRotationValue);
        }

        [Rpc(SendTo.Server, Delivery = RpcDelivery.Reliable)]
        private void HandRotation_Rpc(float handRotationValue)
        {
            handsModel.HandRotationValue(handRotationValue);
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