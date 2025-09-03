using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSpawnPoint : MonoBehaviour {
    public Transform[] points;

#if UNITY_EDITOR
    private void OnValidate() {
        List<Transform> transforms = new List<Transform>(GetComponentsInChildren<Transform>());
        transforms.Remove(transform);
        points = transforms.ToArray();
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        foreach (var item in points) {
            Gizmos.DrawCube(item.transform.position, Vector3.one);
        }
    }
#endif
}
