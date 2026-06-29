using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using Player.hands;
using UnityEngine.Serialization;


namespace Player
{
    // player controller = inputs from controls
    public class PlayerController : NetworkBehaviour
    {
        [FormerlySerializedAs("playerModel")] [SerializeField] private PlayerMovementModel playerMovementModel;
        [SerializeField] private HandsModel handsModel;
        [SerializeField] private PlayerInteractModel playerInteractModel;
        private ControlInputs _playerInputs;
        private Vector2 _moveValue;

        [SerializeField] private GameObject playerHands;


        public override void OnNetworkSpawn()
        {
            if (!IsLocalPlayer) return;
            base.OnNetworkSpawn();

            if (playerMovementModel == null)
            {
                playerMovementModel = GetComponent<PlayerMovementModel>();
            }

            ulong playerID = OwnerClientId;
            HandSpawn_Rpc(playerID);

            _playerInputs = new ControlInputs();
            if (_playerInputs == null) Debug.LogError("error");
            _playerInputs.Player.Move.performed += PlayerMove;
            _playerInputs.Player.Move.canceled += PlayerMove;
            _playerInputs.Player.Interact.performed += Interaction;
            _playerInputs.Enable();
        }

        [Rpc(SendTo.Server)]
        void HandSpawn_Rpc(ulong playerID)
        {
            NetworkObject networkHands = Instantiate(playerHands).GetComponent<NetworkObject>();
            networkHands.SpawnWithOwnership(playerID);
            handsModel = networkHands.GetComponent<HandsModel>();
            networkHands.TrySetParent(transform, false);
            handsModel.Setup();
        }

        private void PlayerMove(InputAction.CallbackContext ctx)
        {
            if (!IsOwner) return;
            _moveValue = ctx.ReadValue<Vector2>();
            MovementInputRequest_Rpc(_moveValue);
        }

        [Rpc(SendTo.Server)]
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
            if (!IsLocalPlayer) return;
            base.OnNetworkDespawn();
            _playerInputs.Player.Move.performed -= PlayerMove;
            _playerInputs.Player.Move.canceled -= PlayerMove;
            _playerInputs.Player.Interact.performed -= Interaction;
            _playerInputs.Disable();
        }
    }
}