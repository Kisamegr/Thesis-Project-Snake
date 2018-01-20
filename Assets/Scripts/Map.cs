using UnityEngine;
using System.Collections;

public enum Cell
{
	Empty,
	Head,
	Tail,
	Food
}
;

public class Map : MonoBehaviour
{

	public Vector2 gridSize;
	public float cellSize;
	
	public Game gameScript;
	public Master masterScript;

	private Cell[,] stateMap;
	private Hashtable emptyHash;
	private Vector2 foodPosition;
	private float maxDist;

	// Use this for initialization
	public void Init()
	{
		gridSize = masterScript.gameMode.mapGridSize;
		cellSize = masterScript.gameMode.mapCellSize;


		Reset ();

	}

	public Cell getCell (int x, int y) {
		return stateMap [x, y];
	}

	public void setState (int x, int y, Cell s)
	{
		/*if(s == Cell.Head && !ValidateMove(x,y)) {
			Debug.Log("NOT VALID MAN   X " + x + "  Y " + y);
			Debug.Break();
			gameScript.GameOver("D");
		}
		*/

		lock(stateMap) {

			//if(stateMap[x,y] == Cell.Head && s == Cell.Head)
			//	gameObject.GetComponent<Game>().GameOver("Draw");

			stateMap [x, y] = s;

			float key = x*gridSize.x + y;

			if(s != Cell.Empty && emptyHash.ContainsKey(key))
				emptyHash.Remove(x*gridSize.x + y);

			if(s == Cell.Empty && !emptyHash.ContainsKey(key))
				emptyHash.Add(key,null);
		}

	}

	public void setFood(int x, int y) {
		foodPosition = new Vector2(x,y);
		setState(x,y,Cell.Food);
	}

	public Vector2 RandomEmptyCell ()
	{
		int x, y,rand;

		if( emptyHash.Count < gridSize.x*gridSize.y/5)
		{
			float key;

			rand = Random.Range(0,emptyHash.Count);

			ICollection keys = emptyHash.Keys;

			IEnumerator id = keys.GetEnumerator();
			//Debug.Log("KEYESS: " +keys.Count);

			if(keys.Count == 0)
				return Vector2.zero * -1;

			for(int i=0 ; i<=rand ; i++) 
				id.MoveNext();

			key = (float)id.Current;



			x = (int) key / (int) gridSize.x;
			y = (int) key % (int) gridSize.x;


		}
		else {
	
			do {
				x = Random.Range (0, (int)gridSize.x);
				y = Random.Range (0, (int)gridSize.y);

			} while(stateMap[x,y] != Cell.Empty);
		}
		return new Vector2 (x, y);
	}


	public Vector3 GridToWorld (Vector2 gridPosition, float height)
	{
		return new Vector3 (gridPosition.x * cellSize + cellSize / 2, height, gridPosition.y * cellSize + cellSize / 2);
	}

	public Vector2 WorldToGrid (Vector3 worldPosition)
	{
		return new Vector2 (Mathf.Floor (worldPosition.x / cellSize), Mathf.Floor (worldPosition.z / cellSize));
	}

	public bool ValidateMove(int x, int y) {

		if(x >= gridSize.x || x < 0 || y >= gridSize.y || y < 0)
			return false;

		Cell c = getCell(x,y);

		if(c != Cell.Empty && c != Cell.Food)
			return false;

		return true;
	}

	public bool ValidateMove(int x, int y, Cell[,] stateMap) {
		
		if(x >= gridSize.x || x < 0 || y >= gridSize.y || y < 0)
			return false;
		
		Cell c = stateMap[(int)x,(int)y];
		
		if(c != Cell.Empty && c != Cell.Food)
			return false;
		
		return true;
	}

	public Cell[,] GetMap() {
		return stateMap;
	}

	public Vector2 GetFoodPosition(){
		return foodPosition;
	}

	public float mapDistance() {
		return maxDist;
	}

	public void Reset() {
		stateMap = new Cell[(int)gridSize.x, (int)gridSize.y];
		emptyHash = new Hashtable();
		
		for (int i =0; i< gridSize.x; i++)
		{
			for (int j=0; j<gridSize.y; j++)
			{
				stateMap [i, j] = Cell.Empty;
				emptyHash.Add(i*gridSize.x + j,null);
			}
		}

		maxDist = Vector2.Distance(Vector2.zero,gridSize);

	}
}
