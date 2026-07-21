using System;
using Unity.Netcode;
using UnityEngine;

public class NetworkManagerSetup : MonoBehaviour
{
  private void Awake()
  {
    if (NetworkManager.Singleton != null && NetworkManager.Singleton != GetComponent<NetworkManager>())
    {
      Destroy(gameObject);
    }
    else
     DontDestroyOnLoad(gameObject);
  }
}
