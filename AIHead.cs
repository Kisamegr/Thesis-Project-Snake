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
		public LinkedList<Vector2> head;
		public int size;
		public int depthEaten = 0;

	}

	public bool ai_running;

	public int id;

	public int maxDepth;

	private Queue stateBuffer;


	public bool running;

	public float time;

	private Head otherHead;

	public float distancePercent;
	public float maxDensityPercent;
	public float minDensityPercent;
	public float eatPercent;

	// Use this for initialization
	protected override void Start () {
		base.Start();
		stateBuffer = new Queue();
		running = false;

		//Vector3 v = gameObject.GetComponent<Renderer>().bounds.size;

	}
	
	public IEnumerator Step () {

		//UnityEngine.Debug.Break();
		Stopwatch watch = Stopwatch.StartNew();

		ai_running = true;

		if(otherHead == null) {
			if(gameScript.GetPlayerHead()!=null)
				otherHead = gameScript.GetPlayerHead();
			else if (gameScript.GetAIHeadNeural()!=null) {
				otherHead = gameScript.GetAIHeadNeural();
			}
			else {
				AIHead[] ais = gameScript.GetAIHeads();

				for(int i=0 ; i<ais.Length ; i++) {
					if(ais[i].id != id)
						otherHead = ais[i];
				}


			}
		}

		BodyPart temp;
		gameScript.ai_running = true;


		stateBuffer.Clear();

		GameState state = new GameState();

	
		state.stateMap = (Cell[,]) mapScript.GetMap().Clone();
		state.food = mapScript.GetFoodPosition();
		//UnityEngine.Debug.Log(state.food);

		Snake minSnake = new Snake();
		minSnake.head = new LinkedList<Vector2>();
		minSnake.direction = otherHead.GetDirection();
		minSnake.size = otherHead.GetSnakeSize();

		temp = otherHead.GetComponent<BodyPart>();
		while(temp!=null) {
			minSnake.head.AddLast(temp.GetGridPosition());
			temp = temp.nextPart;
		}

		Snake maxSnake = new Snake();
		maxSnake.head = new LinkedList<Vector2>();
		maxSnake.direction = direction;
		maxSnake.size = size;

		temp = this;
		while(temp!=null) {
			maxSnake.head.AddLast(temp.GetGridPosition());
			temp = temp.nextPart;
		}
		//Debug.Log(maxSnake.head.Count);

		state.minSnake = minSnake;
		state.maxSnake = maxSnake;

		stateBuffer.Enqueue(state);

		GenerateChildren();


		GameState child = (GameState) stateBuffer.Peek();
		//UnityEngine.Debug.Log("FIRSTOOOO LEVEL: " +child.currentDepth +  " ISMIN: " + child.isMin+ "  SCORE: " + child.score + "  PAR: " + child.parent.maxSnake.direction);

		if(stateBuffer.Count == 0) {
			//gameScript.GameOver(gameObject.tag);
			//yield break;
		}
		else {

			if(stateBuffer.Count > 1) {
				//yield return StartCoroutine(FindSolution());
				FindSolutionAlphaBeta();

				newDirection = ((GameState) stateBuffer.Peek()).childDirection;
			}
			else {
				EvaluateState((GameState)stateBuffer.Peek());
				newDirection = ((GameState)stateBuffer.Peek()).maxSnake.direction;
			}

		}

		//UnityEngine.Debug.Log(((GameState)stateBuffer.Peek()).score);

		//UnityEngine.Debug.Log ( "MOVE SCORE: " + ((GameState) stateBuffer.Peek()).score);


		watch.Stop();


		gameScript.SetTime(time);

		//UnityEngine.Debug.Log("TIME:  " + watch.ElapsedMilliseconds);


		//UnityEngine.Debug.Break();


		yield return StartCoroutine(base.Step());



		time = watch.ElapsedMilliseconds;
		ai_running = false;
	}

	private void GenerateChildren() {
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
					
					for(int i=0 ; i<4 ; i++) {

						// Ignore the opposite direction
						if(Mathf.Abs(snake.direction - i) == 2)
							continue;

						//Debug.Log("i - " + i); 



						switch (i) {

							case Direction.Left:
								newPosition.Set(snake.head.First.Value.x - 1, snake.head.First.Value.y);
								break;
							case Direction.Up:
								newPosition.Set(snake.head.First.Value.x, snake.head.First.Value.y + 1);
								break;
							case Direction.Right:
								newPosition.Set(snake.head.First.Value.x + 1, snake.head.First.Value.y);
								break;
							case Direction.Down:
								newPosition.Set(snake.head.First.Value.x, snake.head.First.Value.y - 1);
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

							childState.stateMap[(int)snake.head.First.Value.x,(int)snake.head.First.Value.y] = Cell.Tail;
							childState.stateMap[(int)newPosition.x,(int)newPosition.y] = Cell.Head;


							Snake newSnake = new Snake();
							newSnake.head = new LinkedList<Vector2>(snake.head);
							newSnake.head.AddFirst(newPosition);
							newSnake.direction = i;

							if(currentState.foodEaten == true || newPosition != currentState.food){
								childState.stateMap[(int)snake.head.Last.Value.x,(int)snake.head.Last.Value.y] = Cell.Empty;
								newSnake.head.RemoveLast();
								newSnake.size = snake.size;
								newSnake.depthEaten = snake.depthEaten;

							}
							else
							{
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

		//yield break;
	}

	private void FindSolution() {

		GameState parent=null,child=null;
		bool parentScoreSet = false;

		while (stateBuffer.Count > 1 || parent.currentDepth != 0) {

			child = (GameState)stateBuffer.Dequeue();


			if(parent != child.parent) {
				parent = child.parent;
				parentScoreSet = false;
				stateBuffer.Enqueue(parent);
			}



			if(child.currentDepth == maxDepth && child.score == 0)
				EvaluateState(child);

			if(!parentScoreSet || (parent.isMin && child.score < parent.score) || (!parent.isMin && child.score > parent.score)) {
				parent.score = child.score;
				parent.childDirection = child.maxSnake.direction;
				parentScoreSet = true;
			}



			

		}

		//yield break;
	}

	
	private void FindSolutionAlphaBeta() {

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
		
		//yield break;
	}



	void  EvaluateState(GameState state) {
		float maxDist=0,minDist=0;

		if(state.food != null) {
			maxDist = Vector2.Distance(state.maxSnake.head.First.Value,state.food); 
			minDist = Vector2.Distance(state.minSnake.head.First.Value,state.food);
		}


	
		state.score = (minDist*5 - maxDist*5- state.minSnake.size * 8 + state.maxSnake.size * 8)  ;

		if(state.maxSnake.depthEaten != 0)
			state.score += (maxDepth - state.maxSnake.depthEaten) *1.75f;

		if(state.minSnake.depthEaten != 0)
			state.score -= (maxDepth - state.minSnake.depthEaten) *1.75f;


		//UnityEngine.Debug.Log("DIR: " + state.maxSnake.direction + "  EGGPOS: " + state.food  + "  EV SCORE: "  + state.score + "  MIN: " + minDist + " MAX: " + maxDist);
	}

	void EvaluateStateBalanced(GameState state) {
		float maxDist,minDist,maxDensity,minDensity,maxAteFood=0,minAteFood=0;


		Vector2 maxHead = state.maxSnake.head.First.Value;
		Vector2 minHead = state.minSnake.head.First.Value;

		if(!state.foodEaten) {
			maxDist = Vector2.Distance(maxHead,state.food)/mapScript.mapDistance(); 		
			minDist = Vector2.Distance(minHead,state.food)/mapScript.mapDistance();
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

		return counter/total;
	}

	public void Reset() {
		base.Reset();

		stateBuffer.Clear();
		running = false;


	}


}
