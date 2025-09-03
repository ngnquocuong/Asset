using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapFallback : MonoBehaviour {
    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("FallingObject")) {
            FallingObjectBehaviour fob = other.gameObject.GetComponent<FallingObjectBehaviour>();
            fob.ForceDisableObject();
        }
    }
}
