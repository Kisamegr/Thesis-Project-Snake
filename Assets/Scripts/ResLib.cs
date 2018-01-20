using UnityEngine;
using System.Collections;

public class ResLib : MonoBehaviour {

	public static ResLib instance;

	public GameObject foodPrefab;
	public GameObject playerPrefab;
	public GameObject[] aiPrefabs;
	public Transform[] spawnpoints;
	public GameObject scoreBitLeft;
	public GameObject scoreBitRight;
	public GameObject roundWinPanel;

	void Awake() {
		instance = this;
	}

}
