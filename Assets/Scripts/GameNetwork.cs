using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using System;
using System.Collections.Generic;



public class GameNetwork : Game {

	public int playerObjectsReady;
	public int clientsReady;

	public List<Head> playerList; 

	void Awake() {

		playerList = new List<Head>();
		
		CustomNetManager.instance.client.RegisterHandler(NetMessages.START_GAME, OnClientStartGameReceived);
		CustomNetManager.instance.client.RegisterHandler(NetMessages.CREATE_FOOD, OnClientCreateFoodReceived);
		CustomNetManager.instance.client.RegisterHandler(NetMessages.PLAYER_LOST,OnClientPlayerLostReceived);
		CustomNetManager.instance.client.RegisterHandler(NetMessages.GAME_STEP,OnGameStepReceived);

		NetworkServer.RegisterHandler(NetMessages.START_GAME, OnServerStartGameReceived);
		NetworkServer.RegisterHandler(NetMessages.CREATE_FOOD,OnServerCreateFoodReceived);
		NetworkServer.RegisterHandler(NetMessages.PLAYER_LOST,OnServerPlayerLostReceived);


	}

	// Use this for initialization
	public void Init () {
		base.Init();

		clientsReady = 0;
		playerObjectsReady = 0;
	
		if(!Master.instance.gameMode.isHost)
			iterativeSteps = false;

	}
	
	public override void GameOver(Head loser) {
		NetMessages.PlayerLost msg = new NetMessages.PlayerLost();
		msg.loserId = loser.id;
		CustomNetManager.instance.client.Send(NetMessages.PLAYER_LOST,msg);

	}

	public void NetworkGameOver(int id) {
		gameFinished=false;
		Head loser=null;


		for (int i=0 ; i< currentPlayers.Count ; i++) {
			if(currentPlayers[i].id == id) {
				loser = currentPlayers[i];
				break;
			}
		}
		
		if(loser.tag == "PlayerHead") {
			
			if(currentPlayers.Count > 2) {
				
				loser.StopAllCoroutines();
				markedPlayerID = loser.id;
				loser.logicRunning = false;

				return;
			}
			else {

				UnityEngine.Debug.Log("Player Won");

				int winnerId=-1;

				for (int i=0 ; i< currentPlayers.Count ; i++) {
					if(currentPlayers[i].id != id) {
						winnerId = currentPlayers[i].id ;
						break;
					}
				}
				
				gameFinished = GetComponent<GameUI>().ShowRoundWin(winnerId,true);
				
			}
		}
		/*	else {
			
			UnityEngine.Debug.Log("AI Won");
			
			bool show = true;
			foreach(AIHead ai in ai_players) {
				gameFinished = GetComponent<GameUI>().ShowRoundWin(ai.id,show,"Computer AI");
				show = false;
			}
			
		}
		
		*/
		gameRunning = false;
		StopAllCoroutines();
		
		
		currentPlayers.Clear();
		
		foreach(Head player in playerList) {
			currentPlayers.Add(player);
		}
		
		//currentPlayers.Add(playerHead);
		
		StartCoroutine(Reset(2));
	}

	public void OnGameStepReceived(NetworkMessage netMsg) {
		if(!Master.instance.gameMode.isHost) {
			NetMessages.GameStep stepMsg = netMsg.ReadMessage<NetMessages.GameStep>();

			foreach(Head player in currentPlayers) {
				player.userInput = stepMsg.userInputs[player.id];
			}

			StartCoroutine(executeStep());
		}



		Debug.Log("AUTO EINAI TO GAME STEP NAIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII");
	}


	public void OnServerStartGameReceived(NetworkMessage netMsg)  {
		NetworkServer.SendToAll(NetMessages.START_GAME,new NetMessages.InfoMessage());
	}
	public void OnClientStartGameReceived(NetworkMessage netMsg)  {
		StartGame();
	}
	
	public void OnServerCreateFoodReceived(NetworkMessage netMsg) {
		NetMessages.FoodMessage msg = netMsg.ReadMessage<NetMessages.FoodMessage>();
		NetworkServer.SendToAll(NetMessages.CREATE_FOOD, msg);
	}

	public void OnClientCreateFoodReceived(NetworkMessage netMsg) {
		NetMessages.FoodMessage msg = netMsg.ReadMessage<NetMessages.FoodMessage>();

		CreateFood(msg.foodPosition);
	}

	public void OnServerPlayerLostReceived(NetworkMessage netMsg) {
		
		NetMessages.PlayerLost msg = netMsg.ReadMessage<NetMessages.PlayerLost>();
		
		NetworkServer.SendToAll(NetMessages.PLAYER_LOST,msg);
		
	}


	public void OnClientPlayerLostReceived(NetworkMessage netMsg) {

		NetMessages.PlayerLost msg = netMsg.ReadMessage<NetMessages.PlayerLost>();

		NetworkGameOver(msg.loserId);

	}




}
