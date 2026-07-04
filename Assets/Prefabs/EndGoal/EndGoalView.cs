using Unity.Netcode;
using UnityEngine;

namespace Prefabs.EndGoal
{
    public class EndGoalView : NetworkBehaviour
    {
        [SerializeField] private GameObject completionUI;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip audioClip;

        public void LevelCompleted()
        {
            audioSource.PlayOneShot(audioClip);
            Debug.Log("Level Completed");
            completionUI.SetActive(true);
            GetComponent<Renderer>().material.color = Color.green;
        }
    }
}