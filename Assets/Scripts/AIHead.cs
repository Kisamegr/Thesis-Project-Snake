using UnityEngine;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;


public class AIHead : Head {



	private class GameState {
		public int currentDepth = 0;
		public Cell[,] stateMap = null;
		public bool isMin = false;
		public GameState parent = null;
		public Vector2 food;
		public bool foodEaten=false;

		public Snake minSnake;
		public Snake maxSnake;

		public float score=0;
		public int childDirection;
	}

	private class Snake {
		public int direction;
		public List<Vector2> parts;
		public int size;
		public int depthEaten = 0;

	}

	public int maxDepth;
	public float time;
	public Head otherHead;

	public float distancePercent;
	public float maxDensityPercent;
	public float minDensityPercent;
	public float eatPercent;

	private Queue stateBuffer;


	// Use this for initialization
	protected override void Start () {
		base.Start();
		stateBuffer = new Queue();
		ishuman = false;
		//Vector3 v = gameObject.GetComponent<Renderer>().bounds.size;

		if(Save.instance.GetAiDepth() < maxDepth) {
			maxDepth = Save.instance.GetAiDepth();
		}

	}
	
	public override IEnumerator Step () {

		Stopwatch watch = Stopwatch.StartNew();
	
		BodyPart temp;


		stateBuffer.Clear();

		GameState state = new GameState();

	
		state.stateMap = (Cell[,]) mapScript.GetMap().Clone();
		state.food = mapScript.GetFoodPosition();
		//UnityEngine.Debug.Log(state.food);

		Snake minSnake = new Snake();
		minSnake.parts = new List<Vector2>();
		minSnake.direction = otherHead.GetDirection();
		minSnake.size = otherHead.GetSnakeSize();

		temp = otherHead.GetComponent<BodyPart>();
		while(temp!=null) {
			minSnake.parts.Add(temp.GetGridPosition());
			temp = temp.nextPart;
		}

		Snake maxSnake = new Snake();
		maxSnake.parts = new List<Vector2>();
		maxSnake.direction = direction;
		maxSnake.size = size;

		temp = this;
		while(temp!=null) {
			maxSnake.parts.Add(temp.GetGridPosition());
			temp = temp.nextPart;
		}
		//Debug.Log(maxSnake.head.Count);

		state.minSnake = minSnake;
		state.maxSnake = maxSnake;

		stateBuffer.Enqueue(state);

		//GenerateChildren();
		yield return StartCoroutine(GenerateChildren());

		GameState child = (GameState) stateBuffer.Peek();
		//UnityEngine.Debug.Log("FIRSTOOOO LEVEL: " +child.currentDepth +  " ISMIN: " + child.isMin+ "  SCORE: " + child.score + "  PAR: " + child.parent.maxSnake.direction);

		if(stateBuffer.Count == 0) {
			//gameScript.GameOver(gameObject.tag);
			//yield break;
		}
		else {

			if(stateBuffer.Count > 1) {
				yield return StartCoroutine(FindSolutionAlphaBeta());
				//FindSolutionAlphaBeta();

				newDirection = ((GameState) stateBuffer.Peek()).childDirection;
			}
			else {
				EvaluateStateBalanced((GameState)stateBuffer.Peek());
				newDirection = ((GameState)stateBuffer.Peek()).maxSnake.direction;
			}

		}

		//UnityEngine.Debug.Log(((GameState)stateBuffer.Peek()).score);

		//UnityEngine.Debug.Log ( "MOVE SCORE: " + ((GameState) stateBuffer.Peek()).score);


		watch.Stop();


//		gameScript.SetTime(time);

		//UnityEngine.Debug.Log("TIME:  " + watch.ElapsedMilliseconds);


		//UnityEngine.Debug.Break();
	

		yield return StartCoroutine(base.Step());



		time = watch.ElapsedMilliseconds;
	}

	private IEnumerator GenerateChildren() {
		Stopwatch watch = Stopwatch.StartNew();

		GameState childState,first;
		Vector2 newPosition = new Vector2();


		int currentDepth = 0;
		bool finishedDepth = false;

		GameState firstState=null;
		Snake snake;
		
		// State Tree Expansion Phase
		while(currentDepth < maxDepth)
		{

			int c=0;
			firstState = null;

			do
			{
				finishedDepth = true;
				GameState currentState = (GameState) stateBuffer.Dequeue();

				if(currentState.stateMap == null || currentState.foodEaten == true) {

					if(firstState == null) {
						//UnityEngine.Debug.Log("CUT-OBLIBEORE-=-LEVEL: " +currentState.currentDepth +  " ISMIN: " + currentState.isMin+ "  SCORE: " + currentState.score + "  PAR: " + currentState.parent.maxSnake.direction + "  CURD: " + currentDepth);
						
						firstState = currentState;
					}
					stateBuffer.Enqueue(currentState);
					//continue;
				}
				else{

					if(currentState.isMin)
						snake = currentState.minSnake;
					else
						snake = currentState.maxSnake;

					int snakeSize = snake.parts.Count;
					
					for(int i=0 ; i<4 ; i++) {

						// Ignore the opposite direction
						if(Mathf.Abs(snake.direction - i) == 2)
							continue;

						//Debug.Log("i - " + i); 



						switch (i) {

							case Direction.Left:
								newPosition.Set(snake.parts[0].x - 1, snake.parts[0].y);
								break;
							case Direction.Up:
								newPosition.Set(snake.parts[0].x, snake.parts[0].y + 1);
								break;
							case Direction.Right:
								newPosition.Set(snake.parts[0].x + 1, snake.parts[0].y);
								break;
							case Direction.Down:
								newPosition.Set(snake.parts[0].x, snake.parts[0].y - 1);
								break;						
						}

						//UnityEngine.Debug.Log("GEN DIR: " + i );
						childState = new GameState();
						childState.parent = currentState;
						childState.currentDepth = currentDepth + 1;
						childState.isMin = !currentState.isMin;

						if(mapScript.ValidateMove((int)newPosition.x,(int)newPosition.y,currentState.stateMap)) {

							childState.stateMap = (Cell[,]) currentState.stateMap.Clone();
							childState.food = currentState.food;

							childState.stateMap[(int)snake.parts[0].x,(int)snake.parts[0].y] = Cell.Tail;
							childState.stateMap[(int)newPosition.x,(int)newPosition.y] = Cell.Head;


							Snake newSnake = new Snake();
							newSnake.parts = new List<Vector2>(snakeSize);
							newSnake.parts.Add(newPosition);
							for(int p=0; p<snakeSize-1; p++) {
								newSnake.parts.Add(snake.parts[p]);
							}
							newSnake.direction = i;

							if(currentState.foodEaten == true || newPosition != currentState.food){
								childState.stateMap[(int)snake.parts[snakeSize-1].x,(int)snake.parts[snakeSize-1].y] = Cell.Empty;
								//newSnake.parts.RemoveAt(newSnake.parts.Count -1);
								newSnake.size = snake.size;
								newSnake.depthEaten = snake.depthEaten;

							}
							else
							{
								newSnake.parts.Add(snake.parts[snakeSize-1]);
								newSnake.size = snake.size + 1;
								newSnake.depthEaten = childState.currentDepth;
								childState.foodEaten = true;
							}



							if(currentState.isMin) {
								childState.minSnake = newSnake;
								childState.maxSnake = currentState.maxSnake;
							}
							else {
								childState.minSnake = currentState.minSnake;
								childState.maxSnake = newSnake;
							}


						}
						else {

							childState.maxSnake = new Snake();

							if(currentState.isMin)
								childState.maxSnake.direction = currentState.maxSnake.direction;
							else
								childState.maxSnake.direction = i;

							if(currentState.isMin)
							{
								childState.score = 100;

							}
							else
								childState.score = -100;

						//UnityEngine.Debug.Log("DEP: " + currentState.currentDepth + "  DIR: "+ i + "  SCORE: " + childState.score);

						

						}

						if(firstState == null) {
							//UnityEngine.Debug.Log("OBLIBEORE-=-LEVEL: " +childState.currentDepth +  " ISMIN: " + childState.isMin+ "  SCORE: " + childState.score + "  PAR: " + childState.parent.maxSnake.direction + "  CURD: " + currentDepth);

							firstState = childState;
						}

						stateBuffer.Enqueue(childState);
						c++;
					}
				}

				first = (GameState) stateBuffer.Peek();

				//if(stateBuffer.Count > 0 && (first.currentDepth == currentDepth || (first.currentDepth < currentDepth && first.stateMap == null)))
				if(stateBuffer.Count > 0 && first!=firstState  && gameScript.gameRunning)
					finishedDepth = false;
				
			} while (!finishedDepth);

			//while(stateBuffer.Count > 0 && (((GameState) stateBuffer.Peek()).currentDepth == currentDepth || ((GameState) stateBuffer.Peek()).stateMap == null));

			
			//UnityEngine.Debug.Log( "TOTAL COUNT: " + stateBuffer.Count + "   ADDED: " + c+  "   DEPTH: " + currentDepth);
			
			currentDepth++;
		}

//		UnityEngine.Debug.Log(watch.ElapsedMilliseconds);

		yield break;
	}

	
	private IEnumerator FindSolutionAlphaBeta() {

		GameState parent=null,child=null,grandParent=null,firstState=null;
		bool parentScoreSet = false,alphaBetaSet = false, firstPushed=false;
		float alphabeta = 0;
		int currentDepth = maxDepth;

		int cut=0;
		//UnityEngine.Debug.Log(stateBuffer.Count);


		while (stateBuffer.Count >= 1 && ((GameState)stateBuffer.Peek()).currentDepth != 0 && gameScript.gameRunning) {
			//UnityEngine.Debug.Log("Solutiono");

			child = (GameState)stateBuffer.Dequeue();
			//UnityEngine.Debug.Log(child.maxSnake.direction);

			if(child == firstState) {
				//UnityEngine.Debug.Log("OBLIGIED-=-LEVEL: " +child.currentDepth +  " ISMIN: " + child.isMin+ "  SCORE: " + child.score + "  PAR: " + child.parent.maxSnake.direction + "  CURD: " + currentDepth);

				currentDepth--;
				alphaBetaSet = false;

				firstState = null;
			}


			if((child.stateMap == null || child.foodEaten == true) && child.currentDepth != currentDepth ) {
				//UnityEngine.Debug.Log("CUT-=-LEVEL: " +child.currentDepth +  " ISMIN: " + child.isMin+ "  SCORE: " + child.score + "  PAR: " + child.parent.maxSnake.direction + "  CURD: " + currentDepth);

				if(firstState==null) {
					firstState = child;
				}

				stateBuffer.Enqueue(child);
				continue;
			}

			
			
			if(parent != child.parent) {
				parent = child.parent;

				if(grandParent != parent.parent)
				{
					grandParent = parent.parent;
					alphaBetaSet = false;
				}
				parentScoreSet = false;
				//UnityEngine.Debug.Log("ASDS");
			}
			
			
			
			if((child.currentDepth == maxDepth && child.score == 0) || child.foodEaten == true)
				EvaluateStateBalanced(child);

			//if(child.score == 100 || child.score == -100)
			//	UnityEngine.Debug.Log("LEVEL: " +child.currentDepth +  " ISMIN: " + child.isMin+ "  SCORE: " + child.score + "  PAR: " + child.parent.maxSnake.direction);
			//else
			//	UnityEngine.Debug.Log("LEVEL: " +child.currentDepth +  " ISMIN: " + child.isMin+ "  SCORE: " + child.score + "  MAXSNAKE: " + child.maxSnake.direction + "  MINSNAKE: " + child.minSnake.direction  + "  PAR: " + child.parent.maxSnake.direction);


			if(alphaBetaSet)
			{
				if( (!child.isMin && child.score < alphabeta) || (child.isMin && child.score > alphabeta))
				{
					while(stateBuffer.Count > 0 && ((GameState)stateBuffer.Peek()).parent == parent) {
						stateBuffer.Dequeue();
						cut++;
					}

					continue;
				}

			}

			if(!parentScoreSet || (parent.isMin && child.score < parent.score) || (!parent.isMin && child.score > parent.score)) {
				parent.score = child.score;
				parent.childDirection = child.maxSnake.direction;
				parentScoreSet = true;
			}

			if( stateBuffer.Count == 0 ||  ((GameState)stateBuffer.Peek()).parent != parent)
			{

				if(!alphaBetaSet)
				{
					alphabeta = parent.score;
					alphaBetaSet = true;
				}
				else
				{

					if((parent.isMin && parent.score > alphabeta) || (!parent.isMin && parent.score < alphabeta))
						alphabeta = parent.score;

				}

				if(firstState==null) {
					firstState = parent;
				}

				stateBuffer.Enqueue(parent);

				//UnityEngine.Debug.Log("  ");

			}
			
			
			
			
		}

//		UnityEngine.Debug.Log("Cut: " + cut);
		
		yield break;
	}

	

	void EvaluateStateBalanced(GameState state) {
		float maxDist,minDist,maxDensity,minDensity,maxAteFood=0,minAteFood=0;


		Vector2 maxHead = state.maxSnake.parts[0];
		Vector2 minHead = state.minSnake.parts[0];

		if(!state.foodEaten) {
			maxDist = Vector2.Distance(maxHead,state.food)/mapScript.mapDistance(); 		
			minDist = Vector2.Distance(minHead,state.food)/mapScript.mapDistance();
			//maxDist = Mathf.Abs(state.maxSnake.parts[0].x - state.food.x) + Mathf.Abs(state.maxSnake.parts[0].y - state.food.y)/mapScript.mapDistance();
			//minDist = Mathf.Abs(state.minSnake.parts[0].x - state.food.x) + Mathf.Abs(state.minSnake.parts[0].y - state.food.y)/mapScript.mapDistance();
		}
		else {
			float dist = Vector2.Distance(maxHead,minHead)/mapScript.mapDistance();

			if(state.parent.isMin) {
				minDist = 0;
				maxDist = dist;
				minAteFood = 1;
			}
			else {
				minDist = dist;
				maxDist = 0;
				maxAteFood = 1;
			}
		}


		maxDensity = FindDensity(maxHead,state);
		minDensity = FindDensity(minHead,state);


		//UnityEngine.Debug.Log(maxDensity);

		state.score = (minDist-maxDist)*distancePercent + minDensity*minDensityPercent - maxDensity*maxDensityPercent + (maxAteFood-minAteFood)*eatPercent;


	}
		  

	private float FindDensity(Vector2 headPos, GameState state) {
		float sx,ex,sy,ey,counter=0,total=0;
		
		if(headPos.x < state.food.x) {
			sx=headPos.x;
			ex=state.food.x;
		}
		else {
			sx=state.food.x;
			ex=headPos.x;
		}
		
		if(headPos.y < state.food.y){
			sy=headPos.y;
			ey=state.food.y;
		}
		else {
			sy=state.food.y;
			ey=headPos.y;
		}
		
		
		for(int x=(int)sx ; x<=(int)ex ; x++) {
			for(int y=(int)sy; y<=(int)ey; y++) {
				if(state.stateMap[x,y]!=Cell.Empty)
					counter++;

				total++;
			}
		}

		;

		if(total==2)
			return 0;

		return (counter-2)/(total-2);
	}

	public void Reset() {
		base.Reset();

		stateBuffer.Clear();


	}


}
