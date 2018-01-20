using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameUI : MonoBehaviour {

	public GameObject canvasWorld;
	public GameObject canvasOverlay;

	public GameObject scoreBoard;

	public ScoreBit[] scoreBits;

	public GameObject activeRoundPanel;

	private string winner;

	// Use this for initialization
	public void Init () {
		canvasWorld = (GameObject)GameObject.Find("CanvasWorld");
		canvasOverlay = (GameObject)GameObject.Find("CanvasOverlay");
		activeRoundPanel = canvasOverlay.transform.Find("RoundPanel").gameObject;

		scoreBoard = canvasWorld.transform.Find("ScoreBoard").gameObject;

		GameMode gmode = Master.instance.gameMode;

		scoreBits = new ScoreBit[gmode.noPlayers];
		PlayerConfig pc;
		for (int i=0 ; i <gmode.noPlayers ; i++) {

			if(i%2==0)
				scoreBits[i] = (ScoreBit) Instantiate(ResLib.instance.scoreBitLeft).GetComponent<ScoreBit>();
			else
				scoreBits[i] = (ScoreBit) Instantiate(ResLib.instance.scoreBitRight).GetComponent<ScoreBit>();

			if(i==0) 
				pc = gmode.main_player;
			else
				pc = gmode.players[i-1];

			scoreBits[i].transform.SetParent(scoreBoard.transform,false);
			scoreBits[i].Initialize(pc,gmode.noRounds);

		}

		canvasOverlay.transform.Find("GameWinPanel").Find("Play").GetComponent<Button>().onClick.AddListener(delegate {Application.LoadLevel( Application.loadedLevel);});
		canvasOverlay.transform.Find("GameWinPanel").Find("Quit").GetComponent<Button>().onClick.AddListener(delegate { Application.LoadLevel(0);});

	}

	public void ShowGameWin() {
		Transform gameWinPanel = canvasOverlay.transform.Find("GameWinPanel");

		gameWinPanel.Find("PlayerName").GetComponent<Text>().text = winner;
		gameWinPanel.gameObject.SetActive(true);

		SoundManager.instance.PlayFinishSound();

	}

	public bool ShowRoundWin(int pid, bool showPanel, string winnerName="") {
	
		ScoreBit scoreBit=null;



		//Add the win
		foreach (ScoreBit bit in scoreBits) {
			if(pid == bit.id) {
				scoreBit = bit;
				break;
			}
		}

		scoreBit.AddWin();

		if(showPanel) {

			if(winnerName == "")
				winnerName = scoreBit.playerNameText.text;

			activeRoundPanel.transform.Find("PlayerName").GetComponent<Text>().text = winnerName;
			activeRoundPanel.SetActive(true);
		}

		if(scoreBit.GetWins() == Master.instance.gameMode.noRounds) {
			winner = scoreBit.playerNameText.text;
			return true;
		}

		return false;

	}


	public void ClearRoundPanel() {
		activeRoundPanel.SetActive(false);
	}





}
