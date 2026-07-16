using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProjectSceneManager : NetworkBehaviour
{
    [SerializeField] private string sceneName;

    private int _playerCount;

//controller
    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            _playerCount++;
        }
    }
    
    
    private void LoadLevel()
    {
        if (IsServer && !string.IsNullOrEmpty(sceneName))
        {
            var status = NetworkManager.SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            if (status != SceneEventProgressStatus.Started)
            {
                Debug.LogWarning("Scene " + sceneName + " failed to load.");
            }
        }
    }
}