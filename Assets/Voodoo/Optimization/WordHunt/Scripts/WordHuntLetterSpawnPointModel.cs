using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Voodoo.Optimization.WordHunt.Scripts {
    public class WordHuntLetterSpawnPointModel : MonoBehaviour {
        public Vector3 Center => transform.position;
        public Vector3 Size => Abs(transform.rotation * bounds.size);

        [SerializeField] private Bounds bounds = new Bounds(Vector3.zero, new Vector3(9, 3, 9) * 2);

        private void OnDrawGizmos() {
            Gizmos.color = Color.blue;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
        
        // Mathf.Abs applied to a vector3
        private static Vector3 Abs(Vector3 v) {
            return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
        }
    }
}