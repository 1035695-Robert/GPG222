using Unity.Netcode;
using UnityEngine;

namespace Prefabs.EndGoal
{
    public class EndGoalView : NetworkBehaviour
    {
        [SerializeField] private GameObject completionUI;

        public void LevelCompleted()
        {
            Debug.Log("Level Completed");
            completionUI.SetActive(true);
            GetComponent<Renderer>().material.color = Color.green;
        }
    }
}