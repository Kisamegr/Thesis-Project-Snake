using UnityEngine;
using System.Collections;

public class Disable : MonoBehaviour {

	void Awake() {
		gameObject.SetActive(false);
	}
}
