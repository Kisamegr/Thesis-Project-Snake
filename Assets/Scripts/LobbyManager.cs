using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;


public class LobbyManager : NetworkBehaviour {

	public static LobbyManager instance;

	public Text title;
	public GameObject[] positions;
	public Slider speedSlider;
	public Slider roundsSlider;
	public Toggle readyToggle;
	public Button startButton;

	public PlayerConfig[] all_players;
	PlayerConfig player;

	int noPlayers;

	public CustomLobbyPlayer clobbyPlayer;

	void Awake() {
		instance = this;
	}

	// Use this for initialization
	void Start () {
		//PlayerPrefs.SetString("PlayerName","Kisamegr");

		all_players = new PlayerConfig[4];
		InitPlayers();

		if(Master.instance.gameMode.mode == GameMode.Mode.SINGLEPLAYER) {

			gameObject.SetActive(true);
			Destroy(readyToggle.gameObject);
		}
		else if(Master.instance.gameMode.mode == GameMode.Mode.MULTIPLAYER) {
		
			title.text = Master.instance.matchName;

			//title.text = 

			if(isServer) {
				NetworkServer.SpawnObjects();
			}
			else {
				startButton.interactable = false;
			}
		}

		noPlayers = 0;

		if(isServer) {
			NetworkServer.RegisterHandler(NetMessages.LOBBY_POSITION, OnPositionReceived);
		}
	}



	public void InitPlayers() {
		player = new PlayerConfig();
		
		player.name = Master.instance.playerName;		
		player.isAI = false;
		
		for(int i=0 ; i<all_players.Length; i++) {
			all_players[i] = new PlayerConfig();
		}
	}

	public void StartLobby() {

		Debug.Log("START CAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAALLED");
		GameMode gameMode = Master.instance.gameMode;
		
		
		gameMode.noPlayers = noPlayers;
		gameMode.stepSpeed = 2f / speedSlider.value;
		gameMode.noRounds = (int)roundsSlider.value;
		
		gameMode.main_player = player;
		gameMode.players = new PlayerConfig[noPlayers-1];
		
		int c=0;
		for(int i=0 ; i<4 ; i++) {
			if(all_players[i].id == -1)
				continue;

			Debug.Log("MPIKE MIA FORA VRE AMAN!!");
			gameMode.players[c] = all_players[i];
			c++;


		}

		if(Master.instance.gameMode.mode == GameMode.Mode.SINGLEPLAYER) {

			Application.LoadLevel(3);

		}

	

	}



	public void ChangeReady(bool ready) { 
		Debug.Log(ready);
		clobbyPlayer.readyToBegin = ready;
		clobbyPlayer.SendReadyToServer();

		readyToggle.interactable = false;

		Chat.instance.ChatInfoMsg(Master.instance.playerName + " is ready.");

	}


	public void SetPosition(string label) {
		if(Master.instance.gameMode.mode == GameMode.Mode.SINGLEPLAYER)
			SetLocalPosition(label,player.id,0);
		else if(Master.instance.gameMode.mode == GameMode.Mode.MULTIPLAYER) {
			NetMessages.PositionMessage msg = new NetMessages.PositionMessage();
			msg.label = label;
			msg.oldID = player.id;
			msg.clientID = clobbyPlayer.slot;
			msg.name = Master.instance.playerName;
			CustomNetManager.instance.client.Send(NetMessages.LOBBY_POSITION,msg);
		}


	}

	public void SetLocalPosition(string label,int oldID, byte clientID, string name=null) {
		int id = (int) char.GetNumericValue(label[0]);
		string what = label.Substring(1);

		 
		if(what == "player") {
			if(oldID!=-1)	
				SetPosition (oldID + "close");

			if(Master.instance.gameMode.mode == GameMode.Mode.SINGLEPLAYER || clobbyPlayer.slot == clientID) {
				player.id = id;
				positions[id].GetComponent<LobbyPosition>().settingsButtons[2].SetActive(true);

				all_players[id].id = -1;
			}
			else
				all_players[id].id = id;


			positions[id].GetComponent<LobbyPosition>().settingsButtons[0].SetActive(false);

			if(name == null) {
				all_players[id].name = player.name;
				positions[id].GetComponent<LobbyPosition>().SetName(player.name);
			}
			else {
				all_players[id].name = name;
				positions[id].GetComponent<LobbyPosition>().SetName(name);
			}
			
			noPlayers++;

			
		}
		else if(what == "close") {
			
			positions[id].GetComponent<LobbyPosition>().Reset();
			all_players[id].id = -1;
			all_players[id].name = "";
			if(player.id == id)
				player.id = -1;

			positions[id].GetComponent<LobbyPosition>().settingsButtons[2].SetActive(false);
			positions[id].GetComponent<LobbyPosition>().settingsButtons[0].SetActive(true);

			
			noPlayers--;
			
		}
		else {
			all_players[id].name = what + " " + (id+1);
			positions[id].GetComponent<LobbyPosition>().SetName(all_players[id].name);
			
			all_players[id].id = id;
			all_players[id].isAI = true;
			all_players[id].aiLevel = what;

			positions[id].GetComponent<LobbyPosition>().settingsButtons[2].SetActive(true);
			positions[id].GetComponent<LobbyPosition>().settingsButtons[0].SetActive(true);

			
			noPlayers++;
			
			Chat.instance.ChatInfoMsg(what + " AI Added.");
		}


		if(noPlayers >= 2 && player.id != -1) 
			startButton.interactable = true;
		else
			startButton.interactable = false;

		
	}

	[ClientRpc]
	public void RpcSetPosition(string label, int oldID, byte clientID, string name) {
		SetLocalPosition(label,oldID,clientID, name);
	}

	public void OnPositionReceived(NetworkMessage netMsg) {		

		NetMessages.PositionMessage msg = netMsg.ReadMessage<NetMessages.PositionMessage>();

		RpcSetPosition(msg.label,msg.oldID,msg.clientID,msg.name);
		SetLocalPosition(msg.label,msg.oldID,msg.clientID, msg.name);
	}

	public void BackButton() {
		Application.LoadLevel(0);
	}

}
