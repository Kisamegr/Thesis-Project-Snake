using UnityEngine;
using System.Collections;


public class Food : MonoBehaviour
{
	public float turnSpeed;
	private Game gameScript;

	void Start ()
	{
	
		gameScript = GameObject.FindGameObjectWithTag ("Game").GetComponent<Game> ();

	}

	void FixedUpdate() {

		GetComponent<Rigidbody>().angularVelocity = Vector3.up * turnSpeed;

	}

	void OnTriggerEnter (Collider other)
	{
		
//		Debug.Log ("fooooooood");
		if (other.tag == "PlayerHead" || other.tag == "AIHead" || other.tag == "AIHeadNeural") {
			Head head = other.GetComponent<Head> ();

			Debug.Log("TRIGGERED!!!!!");

			head.ExtendSnake ();
			gameScript.CreateFood ();
			Destroy (gameObject);
			
		}
		
	}
}
