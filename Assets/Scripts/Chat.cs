using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;


public class Chat : NetworkBehaviour {

	public GameObject chatObject;

	private Text chat;
	private InputField input;
	private Scrollbar scrollBar;


	public static Chat instance;

	// Use this for initialization
	void Start () {
		instance = this;

		Debug.Log( chatObject.transform.Find("Chat Area"));

		chat = chatObject.transform.Find("Chat Area").Find("Chat Text").GetComponent<Text>();
		input = chatObject.transform.Find("Chat Input").GetComponent<InputField>();
		scrollBar = chatObject.transform.Find("Chat Area").Find("Scrollbar").GetComponent<Scrollbar>();

		if(Master.instance.gameMode.mode == GameMode.Mode.MULTIPLAYER) {
			if(isServer) {
				NetworkServer.RegisterHandler(NetMessages.CHAT_USER,OnMessageReceived);
				NetworkServer.RegisterHandler(NetMessages.CHAT_INFO,OnMessageReceived);

			}
		}

		Chat.instance.ChatInfoMsg("Select a seat to sit and add AI bots!");



	}
	private void PrintUserMessage(string msg) {
			chat.text += "\n<color=blue>" + Master.instance.playerName + ": " + msg + "</color>";
	}

	[ClientRpc]
	private void RpcPrintUserMessage(string msg) {
		PrintUserMessage(msg);

	}

	private void PrintInfoMessage(string msg) {
		chat.text += "\n <color=grey>" + msg + "</color>";
	}

	[ClientRpc]
	public void RpcPrintInfoMessage(string msg) {
		PrintInfoMessage(msg);	
	}

	public void ChatUserInput() {

		if(input.text=="")
			return;

		if(Master.instance.gameMode.mode == GameMode.Mode.SINGLEPLAYER) {
			PrintUserMessage(input.text);
		}
		else if(Master.instance.gameMode.mode == GameMode.Mode.MULTIPLAYER) {
			//CmdSendUserMsgClients(input.text);
			CustomNetManager.instance.client.Send(NetMessages.CHAT_USER, new StringMessage(input.text));
		}


		input.text = "";

		input.Select();
		input.ActivateInputField();
	}

	public void ChatInfoMsg(string msg) {
		if(Master.instance.gameMode.mode == GameMode.Mode.SINGLEPLAYER)  {
			PrintInfoMessage(msg);
		}
		else if(Master.instance.gameMode.mode == GameMode.Mode.MULTIPLAYER) {
			CustomNetManager.instance.client.Send(NetMessages.CHAT_INFO, new StringMessage(msg));
		}
	}

	public void OnMessageReceived(NetworkMessage netMsg) {

		string msg = netMsg.ReadMessage<StringMessage>().value;

		if (netMsg.msgType == NetMessages.CHAT_USER) {
			RpcPrintUserMessage(msg);
			PrintUserMessage(msg);

		}

		if (netMsg.msgType == NetMessages.CHAT_INFO) {
			RpcPrintInfoMessage(msg);
			PrintInfoMessage(msg);
			
		}

	}

}
