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

        public override void OnNetworkSpawn()
        {
            networkBodyColour.OnValueChanged += Changed;
          
        }

        private void Changed(Color previousValue, Color newValue)
        {
            BodyColour = newValue;
        }


        [Rpc(SendTo.Server)]
        public void SetColour_Rpc(Color selectedColour, string part)
        {
                Debug.Log(part + " set colour");
                switch (part)
                {
                    case "body":
                        networkBodyColour.Value = selectedColour;
                        break;
                    case "hand":
                        networkHandsColour.Value = selectedColour;
                        break;
                }
         
        }
    }
}