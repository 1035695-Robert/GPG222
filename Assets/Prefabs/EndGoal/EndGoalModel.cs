using System;
using Unity.Netcode;
using UnityEngine;

namespace Prefabs.EndGoal
{
    public class EndGoalModel : MonoBehaviour
    {
        public event Action IsCompleted;

        public void IsObjectFullyInArea(BoxCollider zone, Collider[] targetArray)
        {
            Bounds areaBound = zone.bounds;
            foreach (Collider col in targetArray)
            {
                Bounds objectBound = col.bounds;
                if (!areaBound.Contains(objectBound.min) || !areaBound.Contains(objectBound.max))
                {
                    return;
                }
            }
            zone.enabled = false;
            IsCompleted?.Invoke();
        }
    }
}