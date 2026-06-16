using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;


namespace Player
{
    // player controller = inputs from controls
    public class PlayerController : NetworkBehaviour
    {
        private PlayerModel _playerModel;
        private PlayerView _playerView;
        
        private ControlInputs _inputs;
        private Vector2 _moveValue;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            _playerModel = GetComponent<PlayerModel>();
            _playerView = GetComponent<PlayerView>();
            
            _inputs = new ControlInputs();
            if (_inputs == null) Debug.LogError("error");
            _inputs.Player.Move.performed += PlayerMove;
            _inputs.Player.Move.canceled += PlayerMove;
            _inputs.Player.Interact.performed += Interaction;
            _inputs.Enable();
        }


        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            _inputs.Player.Move.performed -= PlayerMove;
            _inputs.Player.Move.canceled -= PlayerMove;
            _inputs.Player.Interact.performed -= Interaction;
            _inputs.Disable();
        }

        void PlayerMove(InputAction.CallbackContext ctx)
        {
            _playerModel.moveValue =  ctx.ReadValue<Vector2>();
        }
        
      

        private void Interaction(InputAction.CallbackContext ctx)
        {
            if (!IsOwner) return;
            _playerModel.Interact();
        }
    }
}