using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableRandomIdle : MonoBehaviour {

	#region REFERENCES

	[SerializeField] public EditorReferences references = new EditorReferences();
	[System.Serializable] public class EditorReferences {
		public RuntimeAnimatorController[] idleAnimations;
		
		public RuntimeAnimatorController GetRandomAnimation() {
			if (idleAnimations == null || idleAnimations.Length == 0)
				return null;
			
			return idleAnimations[Random.Range(0, idleAnimations.Length)];
		}
	}

	#endregion

	private void Start() {
		RuntimeAnimatorController randomAnimation = references.GetRandomAnimation();

		if (randomAnimation) {
			MovableAnimation ma = GetComponent<MovableAnimation>();
			ma.SetAnimation(randomAnimation);
		}
	}
}
