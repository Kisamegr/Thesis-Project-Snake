using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreBit : MonoBehaviour {


	public int id;
	public Image playerColorImage;
	public Text playerNameText;
	public Transform scoreSignHolder;
	private GameObject[] signs;

	private int wins=0;

	public GameObject winSignPrefab;

	public void Initialize(PlayerConfig playerConfig, int noRounds) {

		playerColorImage = transform.Find("PlayerColor").GetComponent<Image>();
		playerNameText = transform.Find("PlayerName").GetComponent<Text>();
		scoreSignHolder = transform.Find("Score");

		Debug.Log(playerConfig);
		id = playerConfig.id;
		playerNameText.text = playerConfig.name;
		playerColorImage.color = playerConfig.headColor;

		signs = new GameObject[noRounds];

		for(int i=0; i<noRounds ; i++) {
			signs[i] = (GameObject) Instantiate(winSignPrefab);
			signs[i].transform.SetParent(scoreSignHolder,false);
		}
	}

	public void AddWin() {
		signs[wins].GetComponent<Image>().color = playerColorImage.color;
		wins++;
	}

	public int GetWins() {
		return wins;
	}


}
