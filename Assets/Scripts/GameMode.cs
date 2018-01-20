using UnityEngine;
using System.Collections;

public class GameMode {

	public enum Mode { SINGLEPLAYER, MULTIPLAYER, AI };

	public Mode mode;
	public int noPlayers = 3;
	public int noRounds = 1;
	public float stepSpeed = 0.25f;
	public float mapCellSize = 10;
	public Vector2 mapGridSize = new Vector2(20,20);

	public PlayerConfig main_player;
	public PlayerConfig[] players;

	public bool isHost=false;

	/*public GameMode( Mode m) {
		mode = m;

		
		playerNames = new string[noPlayers];
		playerNames[0] = "Player";
		
		for(int i=1; i<noPlayers ; i++) 
			playerNames[i] = "EnemyPlayer" + i;

	}*/

}
