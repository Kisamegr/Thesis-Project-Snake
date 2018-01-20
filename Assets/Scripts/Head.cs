using UnityEngine;
using System.Collections;
using CnControls;
using UnityEngine.Networking;


public class Head : BodyPart
{
	[SyncVar]
	public int id;

	public int startingDirection;

	public GameObject tailPrefab;
	public bool logicRunning;
	public bool ishuman;
	
	public int direction;
	protected int newDirection;
	protected int oldDirection;
	protected BodyPart lastTail;


	protected int size;

	//[SyncVar]
	public int userInput;

	[SyncVar]
	public Vector2 userGridPosition;

	bool useMap = false;

	private Color headColor;


	// Use this for initialization
	protected override void Start ()
	{
		base.Start ();

		headColor = Color.black;
		ishuman = true;

		if(Master.instance.gameMode.mode == GameMode.Mode.SINGLEPLAYER)
			Init();


		Debug.Log("STARTO CALLED");
		//if(Master.instance.gameMode.mode == GameMode.Mode.MULTIPLAYER) {
		//	gameScript.currentPlayers.Add(this);

		//}

		if(Master.instance.gameMode.mode == GameMode.Mode.SINGLEPLAYER || (Master.instance.gameMode.mode == GameMode.Mode.MULTIPLAYER && Master.instance.gameMode.isHost))
			useMap = true;
	}

	void Update() {
		if(gameScript!=null) {

			if(ishuman && gameScript.gameRunning) {

				if(Master.instance.gameMode.mode == GameMode.Mode.SINGLEPLAYER)
					GetUserInput ();
				else if(Master.instance.gameMode.mode == GameMode.Mode.MULTIPLAYER) {
					if(isLocalPlayer)
						GetUserInput();
				}


			} 

			if(Master.instance.gameMode.mode == GameMode.Mode.MULTIPLAYER && isLocalPlayer && ((GameNetwork)gameScript).playerObjectsReady >= Master.instance.gameMode.noPlayers) {
				((GameNetwork)gameScript).playerObjectsReady = -1;
				CmdClientReady();

			}
		}

	}

	public override void OnStartLocalPlayer() {

		Master.instance.StartMultiplayer();

		gameObject.name = "LocalPlayer";
		Debug.Log("On Start Local Playerererere");

		StartCoroutine(Tempo(Master.instance.gameMode.main_player.id));
		//CmdInit(Master.instance.gameMode.main_player.id);
	

		StartCoroutine(SendUserInput());
		Debug.Log("Aftered");

	}
		
	void GetUserInput() {


		if (Input.GetAxis ("Horizontal") < 0)
			userInput = Direction.Left;
		
		if (Input.GetAxis ("Horizontal") > 0)
			userInput = Direction.Right;
		
		if (Input.GetAxis ("Vertical") > 0)
			userInput = Direction.Up;
		
		if (Input.GetAxis ("Vertical") < 0)
			userInput = Direction.Down;

		//if(Application.platform == RuntimePlatform.Android) {
			if (CnInputManager.GetAxis ("Horizontal") < 0)
				userInput = Direction.Left;
			
			if (CnInputManager.GetAxis ("Horizontal") > 0)
				userInput = Direction.Right;
			
			if (CnInputManager.GetAxis ("Vertical") > 0)
				userInput = Direction.Up;
			
			if (CnInputManager.GetAxis ("Vertical") < 0)
				userInput = Direction.Down;
		//}

		userGridPosition = gridPosition;
	}

	public virtual IEnumerator Step ()
	{	

		Vector2 pos = new Vector2();
//		Debug.Log("STEP ID: " + id + "    " + isLocalPlayer);

		//if(transform.tag == "PlayerHead")
		if(ishuman){// && (Master.instance.gameMode.mode == GameMode.Mode.SINGLEPLAYER || (Master.instance.gameMode.mode == GameMode.Mode.MULTIPLAYER && isLocalPlayer))) {
			ChangeDirection(userInput);
		}

		oldPosition = gridPosition;
		oldDirection = direction;
		direction = newDirection;



		switch (direction) {

		case Direction.Up:
			pos.Set(gridPosition.x, gridPosition.y + 1);
			break;
		case Direction.Down:
			pos.Set(gridPosition.x, gridPosition.y - 1);
			break;
		case Direction.Right:
			pos.Set(gridPosition.x + 1, gridPosition.y);
			break;
		case Direction.Left:
			pos.Set(gridPosition.x - 1, gridPosition.y);
			break;
			

		}
		/*
		if(Master.instance.gameMode.mode == GameMode.Mode.MULTIPLAYER && !isLocalPlayer) {
			if(userGridPosition == pos || userGridPosition == oldPosition) {
				Debug.Log("OK ALL GUT");
			}
			else {
				Debug.Log("SHIT SHIT HSIT HIST HISTHISTHISTHSIT ");
				//pos = userGridPosition;
			}
		}
		*/

		if(useMap && !mapScript.ValidateMove((int)pos.x,(int)pos.y)) {
			gameScript.GameOver(this);
			yield break;
		}

		if(Master.instance.gameMode.mode == GameMode.Mode.SINGLEPLAYER || isLocalPlayer);
			Move(pos.x,pos.y);

		if(useMap) {
			mapScript.setState ((int)gridPosition.x, (int)gridPosition.y, Cell.Head);
		}

		if (useMap && lastTail != null && size > 1)
			mapScript.setState ((int)lastTail.GetGridPosition().x, (int)lastTail.GetGridPosition().y, Cell.Empty);

		if (nextPart != null) {
			if(useMap)
				mapScript.setState ((int)oldPosition.x, (int)oldPosition.y, Cell.Tail);
			yield return ((Tail)nextPart).Step (oldPosition.x, oldPosition.y);
		}
		else if(useMap)
			mapScript.setState ((int)oldPosition.x, (int)oldPosition.y, Cell.Empty);



		logicRunning = false;

	}

	public void ChangeDirection (int dir)
	{
		if (Mathf.Abs(direction - dir) != 2)
			newDirection = dir;



	}

	public void ExtendSnake ()
	{
		GameObject nt = (GameObject)GameObject.Instantiate (tailPrefab, lastTail.transform.position, lastTail.transform.rotation);
		nt.GetComponent<Tail> ().previousPart = lastTail;

		if(headColor != Color.black) {
			Color c = headColor;
			c.a = 0.5f;
			nt.transform.Find("snaketail").GetComponent<MeshRenderer>().material.color = c;
		}

		lastTail.GetComponent<BodyPart> ().nextPart = nt.GetComponent<Tail>();
		lastTail = nt.GetComponent<Tail>();


		size++;



//		Debug.Log ("Size: " + size);


	}

	private void FindStartingDirection() {
		float angle=-1,minAngle = transform.rotation.eulerAngles.y;
		Debug.Log(transform.rotation.eulerAngles);

		for(int i=1; i<4; i++) {
			angle = transform.rotation.eulerAngles.y;
			if(angle < minAngle) {
				minAngle = angle;
			}
		}
		if(minAngle > 315 || minAngle < 45)
			startingDirection = 1;
		else if(minAngle < 135)
			startingDirection = 2;
		else if(minAngle < 225)
			startingDirection = 3;
		else
			startingDirection = 0;

	}

	public BodyPart GetLastTail() {
		return lastTail;
	}

	public int GetSnakeSize() {
		return size;
	}

	public int GetDirection() {
		return direction;

	}

	void OnTriggerEnter(Collider coll) {
		//if(coll.tag == "AIHead")
			//gameSript.GameOver("Draw");
	}

	public void DeleteTail() {
		BodyPart root = nextPart;
		while (root!= null) {
			BodyPart temp = root;
			root = root.nextPart;
			if(useMap)
				mapScript.setState((int)temp.gridPosition.x,(int)temp.gridPosition.y,Cell.Empty);
			Destroy(temp.gameObject);
		}

	}

	public void Init() {

		base.Init();
		FindStartingDirection();

		direction = startingDirection;
		newDirection = direction;
		
		previousPart = null;
		lastTail = this;
		
		size = 1;
		logicRunning = false;


		userInput = startingDirection;

		if(useMap)
			mapScript.setState((int)gridPosition.x,(int)gridPosition.y, Cell.Head);

	}

	public void DisableHead() {
		if(useMap)
			mapScript.setState((int)gridPosition.x,(int)gridPosition.y, Cell.Empty);

		gameObject.SetActive(false);

	}

	public void Reset() {
		gameObject.SetActive(true);

		DeleteTail();
		base.Reset();

		Init ();

		userInput = startingDirection;
	}

	[Command]
	private void CmdInit(int id) {
		Debug.Log("COMMAND INIT YEAH");
		RpcOnInit(id);
	}

	[ClientRpc]
	private void RpcOnInit(int id) {
		Debug.Log("On Start Local RPCCCCCCCCCCCCCCCCCCCC");
		gameScript = GameObject.FindGameObjectWithTag ("Game").GetComponent<Game> ();
		mapScript = GameObject.FindGameObjectWithTag ("Game").GetComponent<Map> ();
		
		this.id = id;
		//ishuman = true;
		gameScript.currentPlayers.Add(this);
		((GameNetwork)gameScript).playerList.Add(this);
		//Master.instance.gameMode.main_player.headColor = p.transform.FindChild("snakehead").FindChild("Cube").GetComponent<MeshRenderer>().material.color;
		
		transform.position = ResLib.instance.spawnpoints[id].position;
		transform.rotation = ResLib.instance.spawnpoints[id].rotation;

		Debug.Log("ID: " + id);

		if(id==1) {
			headColor = Color.cyan;
			transform.Find("snakehead").Find("Cube").GetComponent<MeshRenderer>().material.color = Color.cyan;
		}
		if(id==2) {
			headColor = Color.red;
			transform.Find("snakehead").Find("Cube").GetComponent<MeshRenderer>().material.color = Color.red;
		}
		if(id==3) {
			headColor = Color.yellow;
			transform.Find("snakehead").Find("Cube").GetComponent<MeshRenderer>().material.color = Color.yellow;
		}

		Init();

		((GameNetwork)gameScript).playerObjectsReady++;

	}

	[Command]
	public void CmdClientReady() {
		((GameNetwork)gameScript).clientsReady++;

		Debug.Log(Master.instance.gameMode.noPlayers);

		if(((GameNetwork)gameScript).clientsReady >= Master.instance.gameMode.noPlayers) {
			Debug.Log("PREPEI NA MPEI 1 FORA RE " + ((GameNetwork)gameScript).clientsReady );
			NetworkServer.SendToAll(NetMessages.START_GAME,new NetMessages.InfoMessage());
		}
	}

	[Command(channel=1)]
	void CmdUserInput(int input, int step) {
		if(step >= gameScript.steps && !gameScript.lockInput)
			userInput = input;
		//userGridPosition = gridPos;
	}


	IEnumerator SendUserInput() {
		while(true) {

			if(gameScript!=null && gameScript.gameRunning)
				CmdUserInput(userInput,gameScript.steps);
			yield return new WaitForSeconds(0.05f);
		}
	}

	IEnumerator Tempo(int id) {
		yield return new WaitForSeconds(1);
		CmdInit(id);
	}
}



