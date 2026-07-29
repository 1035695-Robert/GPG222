using System;
using Unity.Netcode;
using UnityEngine;

public struct PlayerColourSelectState : INetworkSerializable, IEquatable<PlayerColourSelectState>
{
    public ulong ClientId;
    public Color ColourBodyID;
   // public Color ColourHandsID;

    public PlayerColourSelectState(ulong clientId, Color colorBodyId =  default(Color))
    {
        ClientId = clientId;
        ColourBodyID = colorBodyId;
        
    }
    
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref ColourBodyID);
        
    }

    public bool Equals(PlayerColourSelectState other)
    {
        //to know if it has changed if both clientid and characterid match
        //if any of the values are different it needs to sync
        return ClientId == other.ClientId && ColourBodyID == other.ColourBodyID;
        
    }
}
