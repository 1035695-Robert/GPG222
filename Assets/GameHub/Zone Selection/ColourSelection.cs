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

public class ColourSelection : MonoBehaviour
{
    [SerializeField] public Colours colour;

    private Renderer playerBot;

    public void ColourSelected(int colourIndex)
    {
        colour = (Colours)colourIndex;

        playerBot = GetComponent<Renderer>();
        BodyColour_Rpc();
    }

    [Rpc(SendTo.ClientsAndHost, Delivery = RpcDelivery.Unreliable)]
    private void BodyColour_Rpc()
    {
        switch (colour)
        {
            case Colours.Grey:
                playerBot.material.color = Color.grey;
                break;
            case Colours.Red:
                playerBot.material.color = Color.red;
                break;
            case Colours.Green:
                playerBot.material.color = Color.green;
                break;
            case Colours.Blue:
                playerBot.material.color = Color.blue;
                break;
            case Colours.Yellow:
                playerBot.material.color = Color.yellow;
                break;
            case Colours.Orange:
                playerBot.material.color = Color.yellow;
                break;
            case Colours.Magenta:
                playerBot.material.color = Color.magenta;
                break;
            case Colours.Cyan:
                playerBot.material.color = Color.cyan;
                break;
            case Colours.White:
                playerBot.material.color = Color.white;
                break;
            case Colours.Black:
                playerBot.material.color = Color.black;
                break;
        }
    }
}