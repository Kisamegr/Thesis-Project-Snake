using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using UnityEngine.Networking.Match;
using System.Collections.Generic;
using UnityEngine.UI;

public class LobiesFetcher : NetworkMatch {

	public bool matchCreated;
	public GameObject gameList;
	public GameObject matchPrefab;
	public GameObject hostPanelInputs;
	public GameObject joinPassPanel;

	//List<MatchDesc> matchList;

	private Match selectedMatch;
	private Button joinButton;
	private Button refreshButton;
  /*
	// Use this for initialization
	void Start () {
		matchCreated = false;
		matchList = new List<MatchDesc>();

		joinButton = transform.FindChild("Join Game").GetComponent<Button>();
		joinButton.interactable = false;

		refreshButton = transform.FindChild("Refresh").GetComponent<Button>();

		 
		SetProgramAppID((AppID)56752);

		RefreshMatches();
		
	}
	
	public void OnMatchCreate(CreateMatchResponse matchResponse)
	{
		if (matchResponse.success)
		{
			Debug.Log("Create match succeeded");
			matchCreated = true;
			Utility.SetAccessTokenForNetwork(matchResponse.networkId, new NetworkAccessToken(matchResponse.accessTokenString));
			Master.instance.matchInfo = new MatchInfo(matchResponse);
			Master.instance.gameMode.isHost = true;

			Application.LoadLevel(2);
			//NetworkServer.Listen(new MatchInfo(matchResponse), 9000);
		}
		else
		{
			Debug.LogError ("Create match failed");
		}
	}
	
	public void OnMatchList(ListMatchResponse matchListResponse)
	{
		if (matchListResponse.success && matchListResponse.matches != null)
		{
			//JoinMatch(matchListResponse.matches[0].networkId, "", OnMatchJoined);

			matchList = matchListResponse.matches;

			for (int i=0 ; i<matchListResponse.matches.Count ; i++){
				MatchDesc match = matchListResponse.matches[i];
				GameObject m = (GameObject) Instantiate(matchPrefab);

				m.GetComponent<Match>().SetMatch(i,match,this);
				m.transform.SetParent(gameList.transform,false);

				//DestroyMatch(match.networkId,null);
			}
		}

		refreshButton.interactable = true;
	}
	
	public void OnMatchJoined(JoinMatchResponse matchJoin)
	{
		if (matchJoin.success)
		{
			Debug.Log("Join match succeeded");
			if (matchCreated)
			{
				Debug.LogWarning("Match already set up, aborting...");
				return;
			}
			Utility.SetAccessTokenForNetwork(matchJoin.networkId, new NetworkAccessToken(matchJoin.accessTokenString));

			Master.instance.gameMode.isHost = false;
			
			Application.LoadLevel(2);

			//NetworkClient myClient = new NetworkClient();
			//myClient.RegisterHandler(MsgType.Connect, OnConnected);
			//myClient.Connect(new MatchInfo(matchJoin));
		}
		else
		{
			Debug.LogError("Join match failed");
		}
	}
	
	public void OnConnected(NetworkMessage msg)
	{
		Debug.Log("Connected!");
	}

	public void CreateMatch() {

		Master.instance.matchName = hostPanelInputs.transform.FindChild("Game Name").FindChild("Text").GetComponent<Text>().text;
		Master.instance.matchHost = Master.instance.playerName;


		CreateMatchRequest create = new CreateMatchRequest();
		create.name = Master.instance.matchName + "#" + Master.instance.matchHost;
		create.size = (uint) hostPanelInputs.transform.FindChild("No Players").GetComponent<Slider>().value;
		create.password = hostPanelInputs.transform.FindChild("Password").FindChild("Text").GetComponent<Text>().text;

		Dictionary<string,long> attributes = new Dictionary<string,long>();

		if(create.password == "")
			attributes.Add("pass",0);
		else
			attributes.Add("pass",1);

		create.advertise = true;
		create.matchAttributes = attributes;
		
		CreateMatch(create, OnMatchCreate);
	}

	public void JoinGame() {
		MatchDesc match = matchList[selectedMatch.id];

		Match.SplitName(match.name, out Master.instance.matchName, out Master.instance.matchHost);

		long hasPassword=0;
		//Dictionary<string,long> attributes = match.matchAttributes;

		//if(attributes == null) 
		//	Debug.Log("EINAI NAIENAENIANIEAIENI");

		//if(attributes.TryGetValue("pass", out hasPassword)) {

			if(hasPassword == 0)
			   JoinMatch(match.networkId,"",OnMatchJoined);
			else {
				joinPassPanel.SetActive(true);
			}

		//}

	}

	public void RefreshMatches() {

		refreshButton.interactable = false;

		ListMatches(0, 100, "", OnMatchList);
		for(int i=0 ; i< gameList.transform.childCount ; i++) {
			Destroy(gameList.transform.GetChild(i).gameObject);
		}

		ResetMatchSelection();
	}

	public void SelectMatch(Match match) {
		Debug.Log(match.id);

		ResetMatchSelection();

		selectedMatch = match;
		joinButton.interactable = true;

		match.SetSelected();
	}

	public void ResetMatchSelection() {
		if(selectedMatch != null)
			selectedMatch.ClearSelected();
		selectedMatch = null;
		joinButton.interactable = false;

		Debug.Log(selectedMatch);

	}

	public void PasswordJoin() {
		MatchDesc match = matchList[selectedMatch.id];
		
		string pass = joinPassPanel.transform.FindChild("Password").FindChild("Text").GetComponent<Text>().text;

		JoinMatch(match.networkId,pass,OnMatchJoined);

	}

	public void BackButton() {
		Application.LoadLevel(0);
	}

  */
}
