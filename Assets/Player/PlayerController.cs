using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;


namespace Player
{
    // player controller = inputs from controls
    public class PlayerController : NetworkBehaviour
    {
        private PlayerModel _playerModel;


        private ControlInputs _inputs;


        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;
            base.OnNetworkSpawn();
            _playerModel = GetComponent<PlayerModel>();


            _inputs = new ControlInputs();
            if (_inputs == null) Debug.LogError("error");
            _inputs.Player.Move.performed += PlayerMove;
            _inputs.Player.Move.canceled += PlayerMove;
            _inputs.Player.Interact.performed += Interaction;
            _inputs.Enable();
        }

       

        void PlayerMove(InputAction.CallbackContext ctx)
        {
            Vector2 moveValue = ctx.ReadValue<Vector2>();
            MovementHandler_Rpc(moveValue);
        }

        [Rpc(SendTo.Server)]
        void MovementHandler_Rpc(Vector2 moveValue)
        {
            _playerModel.UpdateMoveValue(moveValue);
        }

        private void Interaction(InputAction.CallbackContext obj)
        {
            Interaction_Rpc();
        }

        [Rpc(SendTo.Server)]
        private void Interaction_Rpc()
        {
            _playerModel.Interact();
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            _inputs.Player.Move.performed -= PlayerMove;
            _inputs.Player.Move.canceled -= PlayerMove;
            _inputs.Player.Interact.performed -= Interaction;
            _inputs.Disable();
        }
    }
}