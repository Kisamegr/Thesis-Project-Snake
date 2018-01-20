using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Diagnostics;


public abstract class Game : NetworkBehaviour
{

	public float stepSpeed;
	public bool gameRunning;
	public Map mapScript;
	public Master masterScript;

	public int steps;
	protected int lastStep;

	protected GameObject currentFood;

	protected ResLib resLib;


	public bool lockInput;
	public Head playerHead;
	public List<Head> currentPlayers;

	protected Stopwatch watch ;

	protected int markedPlayerID;
	protected bool gameFinished;

	protected bool iterativeSteps;


	// Use this for initialization
	public void Init()
	{

		steps = 0;
		markedPlayerID=-1;
		lockInput = false;
		//mapScript = gameObject.GetComponent<Map> ();

		//player = GameObject.FindGameObjectWithTag ("PlayerHead");

		//masterScript = GameObject.Find ("_MASTER").GetComponent<Master>();
		resLib = GameObject.Find("_RESOURCE").GetComponent<ResLib>();
	
		currentPlayers = new List<Head>(masterScript.gameMode.noPlayers);
/*

		if (PlayerPrefs.HasKey ("CPU_Level")) {
			ai_slider.value = PlayerPrefs.GetFloat ("CPU_Level");
			changeAI = true;
		} else
		PlayerPrefs.SetFloat ("CPU_Level", ai_slider.value);


		/*PlayerPrefs.SetFloat ("CPU_Level", 7);
		changeAI = true;
		ai_slider.value = 7;*/

		stepSpeed = masterScript.gameMode.stepSpeed;
		iterativeSteps = true;
	}
	
	// Update is called once per frame

	public abstract void GameOver (Head loser);


	protected IEnumerator executeStep ()
	{


		if(iterativeSteps)
			yield return new WaitForSeconds (stepSpeed);



		while (gameRunning) {
			
			watch = Stopwatch.StartNew();




			if (Master.instance.gameMode.mode == GameMode.Mode.MULTIPLAYER && Master.instance.gameMode.isHost) {

				lockInput = true;

				NetMessages.GameStep msg = new NetMessages.GameStep();
				msg.userInputs = new int[4];

				int q=0;
				foreach (Head player in currentPlayers) {
					msg.userInputs[player.id] = player.userInput;
				}

				NetworkServer.SendToAll(NetMessages.GAME_STEP,msg);

			}

			//UnityEngine.Debug.Log("EXECUTOR");
			
			foreach (Head player in currentPlayers) {
				player.logicRunning = true;
				StartCoroutine (player.Step ());
				
				while(player.logicRunning)
					yield return new WaitForSeconds (0.02f);
				
			}
			
			steps++;
			lockInput = false;
			//UpdateMap();
			watch.Stop();
			double executionTime = watch.ElapsedMilliseconds/1000.0;

			SoundManager.instance.PlaySnakeMoveSound();

			if(markedPlayerID != -1) {

				UnityEngine.Debug.Log("REMOVING MARKED: " + markedPlayerID );
				int markedIndex=-1;
				for (int i=0 ; i< currentPlayers.Count ; i++) {
					if(currentPlayers[i].id == markedPlayerID) {
						markedIndex = i;
						break;
					}
				}
				currentPlayers[markedIndex].StopAllCoroutines();
				currentPlayers[markedIndex].DeleteTail();
				currentPlayers[markedIndex].DisableHead();
				currentPlayers.RemoveAt(markedIndex);

				markedPlayerID = -1;


				UnityEngine.Debug.Log("REMOVED" );
			}
		
			if(!iterativeSteps)
				break;

			if(executionTime < stepSpeed)
				yield return new WaitForSeconds (stepSpeed - (float)executionTime);

		
			
			
		}

		yield break;
		
	}
	
	
	public void EndingScreen ()
	{
		gameRunning = false;

		StopAllCoroutines ();


		//if(player!=null)
		//	player.GetComponent<Head>().StopAllCoroutines();

		//foreach(AIHead head in ai_players)
		//	head.StopAllCoroutines();
		
//		canvas.transform.FindChild ("Winner").GetComponent<Text> ().enabled = true;
//		canvas.transform.FindChild ("Space").GetComponent<Text> ().enabled = true;
		
	

		StartCoroutine (Reset (1f));
	}

	public void StartGame() {
		UnityEngine.Debug.Log("Start the game FROM GAME YEAH");
		steps = 0;
		lastStep = 2;
		gameRunning = true;
		lockInput = false;

		//if(Master.instance.gameMode.mode == GameMode.Mode.SINGLEPLAYER)
			CreateFood ();


		if(Master.instance.gameMode.mode == GameMode.Mode.SINGLEPLAYER || (Master.instance.gameMode.mode == GameMode.Mode.MULTIPLAYER && Master.instance.gameMode.isHost))
			StartCoroutine (executeStep ());
	}

	protected IEnumerator Reset (float seconds)
	{

		yield return new WaitForSeconds (seconds);

		GetComponent<GameUI>().ClearRoundPanel();

		if(gameFinished) {
			GetComponent<GameUI>().ShowGameWin();
			yield break;
		}

		mapScript.Reset ();

		foreach (Head player in currentPlayers)
			player.Reset ();

		Destroy (currentFood);

		if(Master.instance.gameMode.mode == GameMode.Mode.SINGLEPLAYER)
			StartGame();
		else if(Master.instance.gameMode.mode == GameMode.Mode.MULTIPLAYER && Master.instance.gameMode.isHost) {
			yield return new WaitForSeconds(0.5f);
			CustomNetManager.instance.client.Send(NetMessages.START_GAME,new NetMessages.InfoMessage());
		}


		yield break;

	}




	public void CreateFood ()
	{
		UnityEngine.Debug.Log("SERVO: " + isServer);
		if(Master.instance.gameMode.mode == GameMode.Mode.SINGLEPLAYER || (Master.instance.gameMode.mode == GameMode.Mode.MULTIPLAYER && Master.instance.gameMode.isHost)) {

			UnityEngine.Debug.Log("CREATO CALLEDO");
			Vector2 foodCell = mapScript.RandomEmptyCell ();
			Vector3 foodPosition = mapScript.GridToWorld (foodCell, 5);

			if (foodCell != Vector2.zero * -1) {

				mapScript.setFood ((int)foodCell.x, (int)foodCell.y);

			}

			if(Master.instance.gameMode.mode == GameMode.Mode.MULTIPLAYER) {
				NetMessages.FoodMessage msg = new NetMessages.FoodMessage();
				msg.foodPosition = foodPosition;
				CustomNetManager.instance.client.Send(NetMessages.CREATE_FOOD,msg);
			}
			else
				CreateFood(foodPosition);
			



		}

		
	}
	public void CreateFood (Vector3 position)
	{
		currentFood = (GameObject)Instantiate (resLib.foodPrefab, position, Quaternion.identity);		
	}
/*
	[ClientRpc]
	public void RpcCreateFood(Vector3 position) {
		CreateFood(position);
	}

/*
	public void UpdateMap ()
	{
		string mapString = "";

		for (int j= (int)mapScript.gridSize.y - 1; j>=0; j--) {
			for (int i=0; i<mapScript.gridSize.x; i++) {
				if (mapScript.getCell (i, j) == Cell.Empty)
					mapString += "o";

				if (mapScript.getCell (i, j) == Cell.Tail)
					mapString += "+";

				if (mapScript.getCell (i, j) == Cell.Head)
					mapString += "H";

				if (mapScript.getCell (i, j) == Cell.Food)
					mapString += "F";

				mapString += " ";
			}

		}

	}

	public void SetTime (float time)
	{
		cpuTime.text = "AI Time:  " + time.ToString () + "ms";

		avgCpu += (time - avgCpu) * 0.1f;

		avgTime.text = "Avg AI Time:  " + avgCpu.ToString () + "ms";

	}
*/
}
