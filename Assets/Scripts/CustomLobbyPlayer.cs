using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class CustomLobbyPlayer : NetworkLobbyPlayer {

	private NetworkLobbyPlayer lobbyPlayer;


	
	void Awake()
	{
		lobbyPlayer = GetComponent<NetworkLobbyPlayer>();
	}

	// Use this for initialization
	void Start () {
	
		if(isLocalPlayer) {
			LobbyManager.instance.clobbyPlayer = this;
		}


	}


	public void SendReadyToServer() {
		if(isLocalPlayer)
			lobbyPlayer.SendReadyToBeginMessage();

	}

	public override void OnClientReady(bool readyState) {
		if(isLocalPlayer)
			LobbyManager.instance.StartLobby();
	}

}
