using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class NetworkSceneLoader : NetworkBehaviour
{
    [SerializeField] private string selectedScene1, selectedScene2;
    [SerializeField] private Image[] outlineSelection;
    
    
    public void OnLevelSelection(string sceneName)
    {
        if (IsClient)
        {
            Debug.Log("client");
            ButtonPressed_Rpc(sceneName);
        }
    }

    private int count;
   [Rpc(SendTo.Server)]
    private void ButtonPressed_Rpc(string sceneName)
    {
        switch (count)
        {
            case 0:
                selectedScene1 = sceneName;
                count++;
                break;
            case 1:
                selectedScene2 = sceneName;
                if (selectedScene1 == selectedScene2)
                {
                    LoadScene(sceneName);
                }
                count = 0;
                break;
        }
    }
    
    

    private void LoadScene(string sceneName)
    {
        if (IsServer)
        {
            Debug.Log(selectedScene2);
            NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
    }
}