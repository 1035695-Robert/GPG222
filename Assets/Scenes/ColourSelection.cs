using Unity.Netcode;
using UnityEngine;

public enum Colours
{
    Grey,
    Red,
    Green,
    Blue,
    Yellow,
    Orange,
    Magenta,
    Cyan,
    White,
    Black
}

public class ColourSelection : NetworkBehaviour
{
    [SerializeField] public Colours colour;

    private Renderer PlayerBot;

    public void ColourSelected(int colourIndex)
    {
        if (!IsLocalPlayer) return;
        colour = (Colours)colourIndex;

        PlayerBot = GetComponent<Renderer>();
        BodyColour_Rpc();
    }

    [Rpc(SendTo.ClientsAndHost, Delivery = RpcDelivery.Unreliable)]
    private void BodyColour_Rpc()
    {
        switch (colour)
        {
            case Colours.Grey:
                PlayerBot.material.color = Color.grey;
                break;
            case Colours.Red:
                PlayerBot.material.color = Color.red;
                break;
            case Colours.Green:
                PlayerBot.material.color = Color.green;
                break;
            case Colours.Blue:
                PlayerBot.material.color = Color.blue;
                break;
            case Colours.Yellow:
                PlayerBot.material.color = Color.yellow;
                break;
            case Colours.Orange:
                PlayerBot.material.color = Color.yellow;
                break;
            case Colours.Magenta:
                PlayerBot.material.color = Color.magenta;
                break;
            case Colours.Cyan:
                PlayerBot.material.color = Color.cyan;
                break;
            case Colours.White:
                PlayerBot.material.color = Color.white;
                break;
            case Colours.Black:
                PlayerBot.material.color = Color.black;
                break;
        }
    }
}