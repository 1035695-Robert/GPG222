using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelection : MonoBehaviour
{
    public void SelectLevel(string levelName)
    {
        
        NetworkSceneLoader.Instance.ButtonPressed_Rpc(levelName);
    }
}