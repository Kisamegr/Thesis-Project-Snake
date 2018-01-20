using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class NetMessages {

	public const short CHAT_USER = 1000;
	public const short CHAT_INFO = 1001;
	public const short LOBBY_POSITION = 1002;
	public const short START_GAME = 1003;
	public const short CREATE_FOOD = 1004;
	public const short PLAYER_LOST = 1005;
	public const short GAME_STEP = 1006;

	public class PositionMessage : MessageBase {
		public string label;
		public int oldID;
		public byte clientID;
		public string name;
	}

	public class InfoMessage : MessageBase {

	}

	public class FoodMessage : MessageBase {
		public Vector3 foodPosition;
	}

	public class PlayerLost : MessageBase {
		public int loserId;
	}

	public class GameStep : MessageBase {
		public int[] userInputs;

	}
}
