using UnityEngine;
using System.Collections;

public class Tail : BodyPart
{

	// Use this for initialization
	protected override void Start ()
	{

		base.Start ();

		gameScript = GameObject.FindGameObjectWithTag ("Game").GetComponent<Game> ();
		mapScript = GameObject.FindGameObjectWithTag ("Game").GetComponent<Map> ();

		base.Init();
	}

	public override void Move (float x, float y)
	{
		base.Move (x, y);


	}

	public int Step (float x, float y)
	{
		oldPosition = gridPosition;

		//Debug.Log (oldPosition);
		Move (x, y);

		//mapScript.setState ((int)gridPosition.x, (int)gridPosition.y, Cell.Tail);

		if (nextPart != null) {
			((Tail)nextPart).Step (oldPosition.x, oldPosition.y);

		}

		return 0;

	}
}
