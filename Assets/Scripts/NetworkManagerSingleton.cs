using UnityEngine;

public class NetworkManagerSingleton : MonoBehaviour
{
   public static NetworkManagerSingleton Instance;

   private void Awake()
   {
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
}
