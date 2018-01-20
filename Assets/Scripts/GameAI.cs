using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class GameAI : Game {

	private AIHead[] ai_players;

	// Use this for initialization
	public void Init () {
		base.Init();

		GameObject p = (GameObject) Instantiate(resLib.playerPrefab,resLib.spawnpoints[masterScript.gameMode.main_player.id].position, resLib.spawnpoints[masterScript.gameMode.main_player.id].rotation);
		playerHead = p.GetComponent<Head>();
		playerHead.id = masterScript.gameMode.main_player.id;
		masterScript.gameMode.main_player.headColor = p.transform.Find("snakehead").Find("Cube").GetComponent<MeshRenderer>().material.color;

		ai_players = new AIHead[masterScript.gameMode.noPlayers-1];
		
		for (int i=0; i< ai_players.Length; i++) {
			PlayerConfig config = masterScript.gameMode.players[i];

			if(config.isAI) {

				int level;
				if(config.aiLevel == "Easy")
					level = 0;
				else if (config.aiLevel == "Medium")
					level = 1;
				else
					level = 2;

				GameObject g = (GameObject) Instantiate(resLib.aiPrefabs[level],resLib.spawnpoints[config.id].position, resLib.spawnpoints[config.id].rotation);

				masterScript.gameMode.players[i].headColor = g.transform.Find("snakehead").Find("Cube").GetComponent<MeshRenderer>().material.color;

				ai_players[i] = g.GetComponent<AIHead>();
				ai_players[i].otherHead = playerHead;
				ai_players[i].id = config.id;
				ai_players[i].ishuman = false;

				currentPlayers.Add(ai_players[i]);
			}
		}

		currentPlayers.Add(playerHead);


	}

	public override void GameOver(Head loser) {

		gameFinished=false;


		if(loser.tag == "AIHead") {

			if(currentPlayers.Count > 2) {

				loser.StopAllCoroutines();
				markedPlayerID = loser.id;
				loser.logicRunning = false;
				return;
			}
			else {

				UnityEngine.Debug.Log("Player Won");

				gameFinished = GetComponent<GameUI>().ShowRoundWin(playerHead.id,true);

			}
		}
		else {

			UnityEngine.Debug.Log("AI Won");

			bool show = true;
			foreach(AIHead ai in ai_players) {
				gameFinished = GetComponent<GameUI>().ShowRoundWin(ai.id,show,"Computer AI");
				show = false;
			}

		}


		gameRunning = false;
		StopAllCoroutines();


		currentPlayers.Clear();

		foreach(AIHead ai in ai_players) {
			currentPlayers.Add(ai);
		}

		currentPlayers.Add(playerHead);

		StartCoroutine(Reset(2));

	}
	


}
