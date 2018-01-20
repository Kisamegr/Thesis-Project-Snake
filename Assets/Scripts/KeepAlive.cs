using UnityEngine;
using System.Collections;

public class KeepAlive : MonoBehaviour {

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(gameObject);
	}

}
