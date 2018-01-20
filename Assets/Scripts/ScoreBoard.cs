using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour {


	//
	// DEPRECATED
	//

	public GameObject scoreBitLeftPrefab;
	public GameObject scoreBitRightPrefab;
	public GameObject winSignPrefab;

	private Transform scoreBoard;

	// Use this for initialization
	void Start () {
		scoreBoard = transform.Find("ScoreBoard");
	}
	

	public void SetScoreBoard(GameMode gameMode) {

		GameObject scoreBit;
		for(int i=0 ; i<gameMode.noPlayers ; i++ ) {

			if(i%2==0)
				scoreBit = (GameObject) Instantiate(scoreBitLeftPrefab);
			else
				scoreBit = (GameObject) Instantiate(scoreBitRightPrefab);


			scoreBit.transform.SetParent(scoreBoard,false);

			scoreBit.transform.Find("PlayerName").GetComponent<Text>().text = gameMode.players[i].name;
		}
	}
}
