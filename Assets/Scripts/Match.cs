using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking.Match;
using UnityEngine.EventSystems;



public class Match : MonoBehaviour {

	public int id;
	public Text gameName;
	public Text hostName;
	public Text players;

	private Button button;
	private Color originalNormal;
  /*
	public void SetMatch(int id, MatchDesc match, LobiesFetcher script) {

		gameName = transform.FindChild("GameName").GetComponent<Text>();
		hostName = transform.FindChild("Host").GetComponent<Text>();
		players = transform.FindChild("Players").GetComponent<Text>();
		
		button = gameObject.GetComponent<Button>();
		
		originalNormal = button.colors.normalColor;


		this.id = id;

		string mname,hname;

		SplitName(match.name,out mname, out hname);

		gameName.text = mname;
		hostName.text = hname;
		players.text = match.currentSize + "/" + match.maxSize;


		gameObject.GetComponent<Button>().onClick.AddListener(delegate { script.SelectMatch(this); });


		/*EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.Deselect;
		entry.callback.AddListener( delegate { script.ResetMatchSelection();});
		gameObject.GetComponent<EventTrigger>().triggers.Add(entry); */
/*
	}
	public void SetSelected() {
		ColorBlock colors = button.colors;
		colors.normalColor = colors.highlightedColor;
		button.colors = colors;
	}

	public void ClearSelected() {
		ColorBlock colors = button.colors;
		colors.normalColor = originalNormal;
		button.colors = colors;
	}
	
*/
	public static void SplitName(string line, out string matchName, out string hostName) {
		int cursor = line.IndexOf("#");
		matchName = line.Substring(0,cursor);
		hostName = line.Substring(cursor+1);
	}
}
