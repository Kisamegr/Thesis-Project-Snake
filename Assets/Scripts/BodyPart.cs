using UnityEngine;
using System.Collections;
using UnityEngine.Networking;



public abstract class BodyPart : NetworkBehaviour
{

	public BodyPart nextPart;
	public BodyPart previousPart;

	public Vector2 gridPosition;

	protected Vector2 oldPosition;

	public Map mapScript;
	public Game gameScript;
	private bool moving;

	private Vector3 targetPos;
	private Quaternion targetRot;
	private Vector3 targetDir;

	private Vector3 startingPos;
	private Quaternion startingRotation;

	// Use this for initialization
	protected virtual void Start ()
	{
		if(Master.instance.gameMode.mode == GameMode.Mode.SINGLEPLAYER) {
			gameScript = GameObject.FindGameObjectWithTag ("Game").GetComponent<Game> ();
			mapScript = GameObject.FindGameObjectWithTag ("Game").GetComponent<Map> ();

		}
	
	
	
	}
	

	public virtual void Move (float x, float y)
	{
		gridPosition = new Vector2 (x, y);
		moving = true;
		targetPos = mapScript.GridToWorld (gridPosition, transform.position.y);
		targetDir = targetPos - transform.position;

		if( targetDir != Vector3.zero)
			targetRot = Quaternion.LookRotation(targetPos - transform.position);
		else
			targetRot = Quaternion.LookRotation(transform.forward);

		StartCoroutine ("moveToGrid");
		//transform.position = targetPos;
	}

	IEnumerator moveToGrid ()
	{
		while (moving) {
			transform.position = Vector3.Slerp (transform.position, targetPos, gameScript.stepSpeed * Time.smoothDeltaTime * 70f );
			transform.rotation = Quaternion.Lerp(transform.rotation,targetRot, gameScript.stepSpeed * Time.smoothDeltaTime * 70f);

			if (Vector3.Distance (transform.position, targetPos) < 0.05f) {
				transform.position = targetPos;
				moving = false;
				StopCoroutine ("moveToGrid");
			}

			yield return null;
		}

	}

	public Vector2 GetGridPosition ()
	{
		return gridPosition;
	}

	public void Init() {
		startingPos = transform.position;
		startingRotation = transform.rotation;

		nextPart = null;
		previousPart = null;
		
		gridPosition = mapScript.WorldToGrid (transform.position);
		transform.position = mapScript.GridToWorld (gridPosition, transform.position.y);
		

		moving = false;
	}

	public void Reset() {
		transform.position = startingPos;
		transform.rotation = startingRotation;
			
		Init();
	}


}
