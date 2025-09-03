using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomFallingObjectEnabler : MonoBehaviour {

	public FallingObject[] prefabs;
	
	void Start () {
		FallingObject fo = Instantiate(prefabs[Random.Range(0, prefabs.Length)], transform);
		fo.transform.localPosition = Vector3.zero;
		fo.transform.localRotation = Quaternion.identity;
	}
	
}
