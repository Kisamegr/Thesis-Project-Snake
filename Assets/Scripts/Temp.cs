using UnityEngine;
using System.Collections;

public class Temp : MonoBehaviour {

	public GameObject lobby;
	// Use this for initialization
	
	// Update is called once per frame
	void FixedUpdate () {

		if(Master.instance.gameMode.mode == GameMode.Mode.SINGLEPLAYER && !lobby.activeInHierarchy)
			lobby.SetActive(true);

	}
}
