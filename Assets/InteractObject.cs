using System;
using UnityEngine;

namespace Player
{
    public class InteractObject : MonoBehaviour
    {
        private void OnEnable()
        {
            PlayerInputs.OnPickup += HandlePickups;
        }

        private void OnDisable()
        {
            PlayerInputs.OnPickup -= HandlePickups;
        }

        private void HandlePickups()
        {
            Debug.Log("pickup box");
        }
    }
}
