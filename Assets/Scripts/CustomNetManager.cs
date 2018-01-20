using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using UnityEngine.Networking.Match;

public class CustomNetManager : NetworkLobbyManager {

	public static CustomNetManager instance;

	void Awake() {

		DontDestroyOnLoad(transform.gameObject);
		instance = this;
		NetworkLobbyManager.singleton = this;

	}



	public void StartGame() {
		Debug.Log("asdsa");
		CheckReadyToBegin();

	}

	public override void OnServerConnect (NetworkConnection conn)
	{
		base.OnServerConnect (conn);

		//Debug.Log("Client Entered!  " + conn.address);

	}

	public override void OnLobbyServerConnect (NetworkConnection conn)
	{
		base.OnLobbyServerConnect (conn);

		//Debug.Log("Lobby Client Entered!  " + conn.address);


	}



	public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId) {
		base.OnServerAddPlayer(conn,playerControllerId);



		if(networkSceneName == playScene) {
			Debug.Log("ADD THE FREAKING PLAYER");

			//GameNetwork gs = GameObject.Find("_GAME").GetComponent<GameNetwork>();
			GameObject p = (GameObject) Instantiate(CustomNetManager.instance.playerPrefab);
			//gs.playerHead = p.GetComponent<Head>();
			//gs.playerHead.id = Master.instance.gameMode.main_player.id;
			//gs.masterScript.gameMode.main_player.headColor = p.transform.FindChild("snakehead").FindChild("Cube").GetComponent<MeshRenderer>().material.color;
			
			NetworkServer.AddPlayerForConnection(conn,p,playerControllerId);

		}

	}

	public override void OnStartClient(NetworkClient client) {
		base.OnStartClient(client);
		Debug.Log("CLIENT STARTO");
		//Master.instance.StartMultiplayer();

	}

	public override void OnClientSceneChanged(NetworkConnection conn) {
		base.OnClientSceneChanged(conn);

		Debug.Log("SCENE CHANGEDTO");

	}

	/*

	public override void OnClientEnterLobby()
	{
		Debug.Log("Client Entered!  " + conn.address);

	}
	
	public override void OnClientExitLobby()
	{
		Debug.Log("Client Disc :(  " + conn.address);

	}
	
	public override void OnClientReady(bool readyState)
	{
	}*/
	
	
	
	

	
}
