using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;


namespace Player
{
    // player controller = inputs from controls
    public class PlayerController : NetworkBehaviour
    { 
        [SerializeField] private PlayerModel playerModel;

        private ControlInputs _inputs;
        private Vector2 _moveValue;

        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;
            base.OnNetworkSpawn();
            
            if(playerModel == null)
            {
                playerModel = GetComponent<PlayerModel>();
            }


            _inputs = new ControlInputs();
            if (_inputs == null) Debug.LogError("error");
            _inputs.Player.Move.performed += PlayerMove;
            _inputs.Player.Move.canceled += PlayerMove;
            _inputs.Player.Interact.performed += Interaction;
            _inputs.Enable();
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


        // private void Interaction_Rpc()
        // {
        //   s
        // }
        
        public override void OnNetworkDespawn()
        {
            if (!IsOwner) return;
            base.OnNetworkDespawn();
            _inputs.Player.Move.performed -= PlayerMove;
            _inputs.Player.Move.canceled -= PlayerMove;
            _inputs.Player.Interact.performed -= Interaction;
            _inputs.Disable();
        }
    }
}