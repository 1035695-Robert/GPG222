using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;


public class NetworkSceneLoader : MonoBehaviour
    {

        public void OnLevelSelection(string sceneName)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
    }

