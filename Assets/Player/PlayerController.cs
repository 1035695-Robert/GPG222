using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;


namespace Player
{
    // player controller = inputs from controls
    public class PlayerController : NetworkBehaviour
    {
        [SerializeField] private PlayerModel playerModel;

        private ControlInputs _playerInputs;
        private Vector2 _moveValue;

        [SerializeField] private GameObject playerHands;

        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;
            base.OnNetworkSpawn();

            if (playerModel == null)
            {
                playerModel = GetComponent<PlayerModel>();
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
            networkHands.TrySetParent(transform,false);
        }

        private void PlayerMove(InputAction.CallbackContext ctx)
        {
            if (!IsOwner) return;
            _moveValue = ctx.ReadValue<Vector2>();
            MovementToRpc(_moveValue);
        }

        // [Rpc(SendTo.Server)]
        private void MovementToRpc(Vector2 moveValue)
        {
            playerModel.moveValue = moveValue;
        }

        private void Interaction(InputAction.CallbackContext obj)
        {
            // Interaction_Rpc();
            playerModel.Interact_Rpc();
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