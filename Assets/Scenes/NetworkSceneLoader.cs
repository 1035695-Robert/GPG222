using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;


public class NetworkSceneLoader : MonoBehaviour
    {

        public void OnLevelSelection()
        {
            NetworkManager.Singleton.SceneManager.LoadScene("Level1", LoadSceneMode.Single);
        }
    }

