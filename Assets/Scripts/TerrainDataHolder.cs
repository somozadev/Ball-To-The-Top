using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class TerrainDataHolder : MonoBehaviour
    {
        [SerializeField] private List<Vector3> _lastStablePositions;


        public void AddStablePosition(Vector3 position)
        {
            _lastStablePositions.Add(position);
        }

        public bool HasPositions => _lastStablePositions.Count > 0;

        public Vector3 GetLastStablePosition()
        {
            return _lastStablePositions[^1];
        }
    }
}