using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

	public InputField nameInput;

	// Use this for initialization
	void Start () {
		Debug.Log(Master.instance.playerName);
		nameInput.text = Master.instance.playerName;
	}
	

	public void Singleplayer() {
		Master.instance.gameMode.mode = GameMode.Mode.SINGLEPLAYER;
		Application.LoadLevel("lobby single");
	}

	public void Multiplayer() {
		Master.instance.gameMode.mode = GameMode.Mode.MULTIPLAYER;
		Application.LoadLevel("prelobby multi");
	}

	public void Options() {
		Application.LoadLevel(4);
	}

	public void NameChange(string name) {
		string newName;

		if(name=="")
			newName =  "Player " + Random.Range(1000,9999);
		else
			newName = name;

		PlayerPrefs.SetString("PlayerName", newName);

		Master.instance.playerName = newName;
		Debug.Log("KALIRIRIRIRRIRI");

	}
}
