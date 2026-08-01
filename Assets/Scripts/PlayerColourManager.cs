using System.Collections.Generic;
using Player;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerColourManager : NetworkBehaviour
{
    public static PlayerColourManager Instance;
    public List<ulong> connectedPlayers = new List<ulong>();
    public List<Color> playerBodyColour = new List<Color>();
    public List<Color> playerHandsColour = new List<Color>();

    private void Awake()
    {//helps to reference a singleton crossScenes when player joins.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        NetworkManager.Singleton.OnClientConnectedCallback += ConnectedClient;
    }

    private void ConnectedClient(ulong id)
    {//when new player joins creates a new lists to help reference across scene Loads.
        if (!connectedPlayers.Contains(id))
        {
            connectedPlayers.Add(id);
            playerBodyColour.Add(Color.grey);
            playerHandsColour.Add(Color.gray);
        }
    }

    public void SetColourOnSceneLoad(ulong clientId,string part)
    {
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var client))
        {
            if (client.PlayerObject != null)
            {
                Debug.Log("identify Player");
                PlayerColourModel playerColour = client.PlayerObject.gameObject.GetComponent<PlayerColourModel>();
                if (connectedPlayers != null)
                {
                    switch (part)
                    {//updates the players component that has just joined the scene 
                        // this is also called to update the ui for the waiting lobby
                        case "body": 
                            playerColour.networkBodyColour.Value = playerBodyColour[(int)clientId];
                            return;
                        // case "hand":
                        //     playerColour.networkHandsColour.Value = playerHandsColour[(int)clientId];
                        //     return;
                            
                    }
                    Debug.Log("NO player connected");
                }
            }
        }
    }


    public void BodyColour(string colourName, ulong id, string part)
    {
        string colourInput = colourName.ToLower();
        switch (colourInput)
        {//checks the Colour name variable and updates the colour. 
            default:
                SetColour(Color.gray, id, part); break;
            case "red":
                SetColour(Color.red, id, part); break;
            case "lime":
                SetColour(Color.green, id, part); break;
            case "blue":
                SetColour(Color.blue, id, part); break;
            case "yellow":
                SetColour(Color.yellow, id, part); break;
            case "magenta":
                SetColour(Color.magenta, id, part); break;
            case "cyan":
                SetColour(Color.cyan, id, part); break;
            case "white":
                SetColour(Color.white, id, part); break;
            case "black":
                SetColour(Color.black, id, part); break;
        }
    }
    
    private void SetColour(Color colour, ulong playerID, string part)
    {
        if (connectedPlayers.Contains(playerID))
        {
            switch (part)
            {// //updates the colour in the list allowing it to be called on during when scenes are loaded.
                case "body":
                    playerBodyColour[(int)playerID] = colour;
                    break;
                // //originally had hands and body but hands were called at wrong timing
                // case "hand":
                //     playerHandsColour[(int)playerID] = colour;
                //     break;
            }
        }
    }

    public override void OnNetworkDespawn()
    {
        NetworkManager.Singleton.OnClientConnectedCallback -= ConnectedClient;
    }
}