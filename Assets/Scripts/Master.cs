using UnityEngine;
using System.Collections;
using UnityEngine.Networking.Match;
using UnityEngine.Networking;


public class Master : MonoBehaviour {

	public GameObject auto;
	public GameMode gameMode;
	public string playerName;

	public static Master instance;

	public MatchInfo matchInfo;
	public string matchName;
	public string matchHost;

	private CustomNetManager netManager;

	private GameObject cameraPos;
	private GameObject cameraObj;

	void Awake() {
		Debug.Log("MASTERED");
		if(instance !=null) {
			Destroy(gameObject);
			return;
		}

		Reseto();
		DontDestroyOnLoad(transform.gameObject);
		instance = this;

		//Instantiate(soundManagerPrefab);
	}

	void Start() {
		if(!PlayerPrefs.HasKey("PlayerName"))
			PlayerPrefs.SetString("PlayerName", "Player " + Random.Range(1000,9999));

		playerName = PlayerPrefs.GetString("PlayerName");


	}

	public void Reseto() {
		Debug.Log("RESETOOO");
		gameMode = new GameMode();
		gameMode.mode = GameMode.Mode.SINGLEPLAYER;
	}

	void OnLevelWasLoaded(int level) {

		// Player Lobby
		if(level == 2) {
			if(gameMode.mode == GameMode.Mode.SINGLEPLAYER) {
				GameObject lobby = (GameObject) GameObject.Find("Canvas").transform.Find("Lobby").gameObject;

				lobby.SetActive(true);
				lobby.transform.Find("ChatPanel").gameObject.SetActive(true);
			}
			if(gameMode.mode == GameMode.Mode.MULTIPLAYER) {
				netManager = gameObject.GetComponent<CustomNetManager>();
				netManager.enabled = true;
				netManager.matchInfo = matchInfo;

				if(gameMode.isHost) {
					netManager.StartHost(matchInfo);
				}
				else {
					netManager.StartClient(matchInfo);

				}

			}

		}

		// Main Game
		if(level == 3) {
			//GameObject a = (GameObject) Instantiate(auto);
			//NetworkServer.Spawn(a);
			//NetworkServer.SpawnObjects();

			cameraPos = GameObject.Find("CameraPos");
			cameraObj = GameObject.Find("Main Camera");


			if(gameMode.mode == GameMode.Mode.SINGLEPLAYER) {
				GameObject game = new GameObject("_GAME");
				game.tag = "Game";
				Map mapScript =  game.AddComponent<Map>();
				mapScript.masterScript = this;
				
				GameUI gameUI = game.AddComponent<GameUI>();
				GameAI gameScript=null;

				//if(gameMode.mode.CompareTo(GameMode.Mode.SINGLEPLAYER) == 0)
				gameScript = game.AddComponent<GameAI>();




				gameScript.masterScript = this;
				gameScript.mapScript = mapScript;
				mapScript.gameScript = gameScript;


				mapScript.Init();
				gameScript.Init();
				gameUI.Init();

				StartCoroutine("LerpCamera",gameScript);
				//gameScript.StartGame();


			}
			else if(gameMode.mode == GameMode.Mode.MULTIPLAYER) {
				cameraObj.transform.position = cameraPos.transform.position;
				cameraObj.transform.rotation = cameraPos.transform.rotation;
			}


		

		}


	}

	public void StartMultiplayer() {
		GameObject game = new GameObject("_GAME");
		game.tag = "Game";
		Map mapScript =  game.AddComponent<Map>();
		mapScript.masterScript = this;
		
		GameUI gameUI = game.AddComponent<GameUI>();




		GameNetwork gameScript=null;
		
		//if(gameMode.mode.CompareTo(GameMode.Mode.SINGLEPLAYER) == 0)
		gameScript = game.AddComponent<GameNetwork>();
		gameScript.GetComponent<NetworkIdentity>().localPlayerAuthority = true;
		
		
		
		gameScript.masterScript = this;
		gameScript.mapScript = mapScript;
		mapScript.gameScript = gameScript;
		
		
		
		mapScript.Init();
		gameScript.Init();
		gameUI.Init();
		Debug.Log("TWRA MASTER");

		//gameScript.StartGame();
	}

	IEnumerator LerpCamera(Game gameScript) {
		while(Vector3.Distance( cameraObj.transform.position,cameraPos.transform.position) > 0.01f &&
		      Quaternion.Angle(cameraObj.transform.rotation,cameraPos.transform.rotation) > 0.1f) {

			cameraObj.transform.position = Vector3.Lerp(cameraObj.transform.position,cameraPos.transform.position,0.05f);
			cameraObj.transform.rotation =  Quaternion.Lerp(cameraObj.transform.rotation,cameraPos.transform.rotation,0.05f);

			yield return new WaitForSeconds(0.02f);
		}

		SoundManager.instance.PlayStartSound();
		gameScript.StartGame();
	}
}
