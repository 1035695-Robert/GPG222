using Unity.Netcode;
using UnityEngine;

namespace Player
{
    public class PlayerColourModel : NetworkBehaviour
    {
         public NetworkVariable<Color> networkBodyColour = new NetworkVariable<Color>(Color.gray,
            NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        public NetworkVariable<Color> networkHandsColour = new NetworkVariable<Color>(Color.gray,
            NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        [SerializeField] private Color BodyColour;
        [SerializeField] private Color HandsColour;

        // public override void OnNetworkSpawn()
        // {
        //     networkBodyColour.OnValueChanged += Changed;
        // }
        //
        // private void Changed(Color previousValue, Color newValue)
        // {
        //     BodyColour = newValue;
        // }
        
    }
}