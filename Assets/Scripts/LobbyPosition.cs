using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LobbyPosition : MonoBehaviour {

	private Text playerName;
	private Button button;
	private GameObject nameLayout;

	private Color originColor;

	public GameObject[] settingsButtons;  

	// Use this for initialization
	void Start () {
		playerName = transform.Find("Name").Find("Name Text").GetComponent<Text>();
		nameLayout = transform.Find("Name").gameObject;

		button = GetComponent<Button>();
		originColor = button.colors.normalColor;

		settingsButtons = new GameObject[3];
		Transform settings = transform.Find("Settings Panel");

		settingsButtons[0] = settings.GetChild(0).gameObject;
		settingsButtons[1] = settings.GetChild(1).gameObject;
		settingsButtons[2] = settings.GetChild(2).gameObject;

		//if(Master.instance.gameMode.mode == GameMode.Mode.MULTIPLAYER)
		//	settingsButtons[1].SetActive(false);


		settingsButtons[2].SetActive(false);

	}

	public void SetName(string name) {
		playerName.text = name;

		ColorBlock colors = button.colors;
		colors.normalColor = Color.white;
		button.colors = colors;

		button.interactable = false;

		nameLayout.SetActive(true);
	}

	public void Reset() {
		nameLayout.SetActive(false);

		ColorBlock colors = button.colors;
		colors.normalColor = originColor;
		button.colors = colors;
		button.interactable = true;

	}

}
