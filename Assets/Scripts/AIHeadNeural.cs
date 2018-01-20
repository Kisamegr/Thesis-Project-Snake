using UnityEngine;
using System.Collections;
using System.Diagnostics;



public class AIHeadNeural : Head {

	public bool ai_running;
	public bool alive;
	public int[] N;
	public float beta;
	public float hl;

	private int layerNumber;

	private double[][] a ; // The output value of each neuron, in each layer
	private double[][][] w ; // The Weight values of the neurons synapses of this layer, with each of the previous one
	private double[][] b ; // The biases weight values for each neuron, in each layer
	private double[][] d; // The delta values for each neuron, in each layer 

	private float lastFoodDistance;
	private bool foodEaten;
	private int directionChosen;

	protected override void Start () {
		base.Start();

		alive = true;
		lastFoodDistance = -1;
		layerNumber = N.Length;
		foodEaten = false;
		directionChosen = -1;

		a = new double[layerNumber][]; 
		w = new double[layerNumber - 1][][];
		b = new double[layerNumber - 1][]; 
		d = new double[layerNumber - 1][]; 


		// For each layer
		for (int l = 0; l < layerNumber; l++)
		{
			a[l] = new double[N[l]]; // Create the output array
			
			if (l != 0)
			{
				w[l - 1] = new double[N[l]][]; // Create the weights array
				b[l - 1] = new double[N[l]]; // Create the bias weights array
				d[l - 1] = new double[N[l]]; // Create the d array
				
				// For each neuron in the l layer
				for (int i = 0; i < N[l]; i++)
					w[l - 1][i] = new double[N[l - 1]]; // Create each neuron input-weights array
			}
			
		}

		// Initialize the arrays values
		for (int l = 1; l < layerNumber; l++) // For each layer...
		{
			for (int i = 0; i < N[l]; i++) // For each neuron in the l layer...
			{
				b[l - 1][i] = -1; // Initialize the bias to -1
				double g = 2.38 / Mathf.Sqrt(N[l - 1]); // Calculate the range of the random weights for each layer, based on the number of neurons from the previous layer
				
				for (int j = 0; j < N[l - 1]; j++) // For each neuron in the l-1 layer...
				{
					double r = Random.value*g - g/2; // Calculate a random value within the g range, centered to 0
					w[l - 1][i][j] = r; // Set the weight
				}			
			}
		}

	}
	
	public IEnumerator Step () {

		ai_running = true;

		Cell[,] map = mapScript.GetMap(); 
		Stopwatch watch = Stopwatch.StartNew();

		double squared_error = 0; // The variable to store the squared error
		int successes = 0; // Count the successes of each epoch

		
		/******************
		 * BACKWARDS PHASE *
		*******************/

		if(lastFoodDistance != -1) {

			float foodDistanceChange = Vector2.Distance(gridPosition,mapScript.GetFoodPosition()) - lastFoodDistance;

			UnityEngine.Debug.Log("Dist:  " + foodDistanceChange);

			if(Mathf.Abs(direction-oldDirection) == 2)
			{
				alive = false;
				gameScript.GameOver(this);
			}

			int max_num; // Values for the successes
			double max_value = -1;
			
			for (int i = 0; i < N[layerNumber - 1]; i++) // For each neuron in the last-output layer...
			{

				int target;
				
				if(i==directionChosen ) {
					if(!alive )
						target = -5;
					else if (foodDistanceChange > 0)
						target = -1;
					else if (foodDistanceChange < 0 && !foodEaten)
						target = 1;
					else {
						target = 5;
						foodEaten = false;
					}

					//UnityEngine.Debug.Log("Target: " + target + "  Dir: " + i);
				}
				//else if(i==oppositeDirection) {
				//	target = 0;
				//}
				else
				{
					if(foodEaten)
						target = -2;
					else if(foodDistanceChange < 0)
						target = 0;
					else
						target = 2;
				}


				// The idea is that, if the label is the number X, then the neuron X of the output layer has target 1, and the rest 0
				// Likewise, if we are at the i neuron in the loop (out of 10), and the label is i, then this neuron must have target=1

				// Calculate the error
				double error = target - a[layerNumber - 1][i];
				
				// Calculate the delta
				//if (logistic)
					d[layerNumber - 2][i] = hl * error * a[layerNumber - 1][i] * (1 - a[layerNumber - 1][i]);
				//else
				//	d[layerNumber - 2][i] = ht * error * (1 - a[layerNumber - 1][i]) * (1 + a[layerNumber - 1][i]);
				
				// Add to the squared error
				//squared_error += error*error;
				
				// Find the biggest value in order to see if it succeeded finding the number
				//if (a[layerNumber - 1][i] > max_value)
				//{
				//	max_value = a[layerNumber - 1][i];
				//	max_num = i;
				//}
			}
			
			// If it succeedded, raise the successes
			//if (max_num == (int)trainLabelFile->GetLabel(n))
			//	successes++;
			
			
			for (int l = layerNumber - 2; l > 0; l--) // For each hidden layer...
			{
				/*for (int c = 0; c < cores; c++) // For each core...
				{
					results.emplace_back(
						pool.enqueue([w, a, l, N, d,c,cores,logistic]{ // Enqueue task to the thread pool
						
						// Calculate the r (range) and the dr (default range)
						int r, dr;
						dr = N[l] / cores; // dr is the div
						
						if (c < cores - 1)
							r = dr;
						else
							r = dr + N[l] % cores; // r contains the mod, if the div N[l]/cores is not perfect
						
						// Split the for loop into (#cores) parts
						for (int i = c*dr; i < c*dr + r; i++) // For each neuron in this layer*/

						for (int i = 0; i < N[l]; i++) // For each neuron in this layer*/
						{
							double sum = 0;
							
							for (int j = 0; j < N[l + 1]; j++) // For each neuron in the previous layer
								sum += d[l][j] * w[l][j][i]; // Sum up all the mults between the deltas and the weights of each neuron in the next layer, with the neuron in this layer
							
							// Calculate the delta
							//if (logistic)
								d[l - 1][i] = hl * sum * a[l][i] * (1 - a[l][i]);
							//else // tanh
							//	d[l - 1][i] = ht * sum * (1 - a[l][i]) * (1 + a[l][i]);
						}
						
						//return 0; // Return nothing
						
					//}));
					
				//}
				
				//for (auto && result : results) // Wait each task to finish
				//	result.get();
				
				//results.clear();
			}
			
			
			
			
				/***************
				  UPDATE PHASE *
				****************/
			
			for (int l = 1; l < layerNumber; l++) // For each layer...
			{
				for (int i = 0; i < N[l]; i++) // For each neuron in this layer...
				{
					for (int j = 0; j < N[l - 1]; j++) // For each neuron in the previous layer...
						w[l - 1][i][j] += beta * d[l - 1][i] * a[l - 1][j]; // Calculate the new weight
					
					b[l - 1][i] += beta * d[l - 1][i]; // Caltulate the new bias weight
				}
				
			}
		

		}

		lastFoodDistance = Vector2.Distance(gridPosition,mapScript.GetFoodPosition());

		// Initialize the first input values
		for (int i = 0; i < N[0]-3; i++) // For each input...
			a[0][i] = (double) map[i/(int)mapScript.gridSize.x, i%(int)mapScript.gridSize.y]/3;

		a[0][N[0]-3] = (double)direction/3;
		a[0][N[0]-2] = (double)mapScript.GetFoodPosition().x/mapScript.gridSize.x;
		a[0][N[0]-1] = (double)mapScript.GetFoodPosition().y/mapScript.gridSize.x;

			
			
		/******************
	 	 * FORWARDS PHASE *
		 ******************/
		
		for (int l = 1; l < layerNumber; l++) // For each layer...
		{
			/*for (int c = 0; c < cores; c++) // For each core...
			{
				results.emplace_back(
					pool.enqueue([w, b, a, l, N, c, cores,logistic]{ // Enqueue task to the thread pool
					
					// Calculate the r (range) and the dr (default range)
					int r,dr;
					dr = N[l] / cores; // dr is the div
					
					if (c < cores -1)
						r = dr;
					else
						r = dr + N[l] % cores; // r contains the mod, if the div N[l]/cores is not perfect
					
					// Split the for loop into (#cores) parts
					for (int i = c*dr; i < c*dr + r; i++) // For each neuron in this layer*/

					for (int i = 0; i < N[l]; i++) // For each neuron in this layer*/
					{
						double sum = 0;
						
						for (int j = 0; j < N[l - 1]; j++) // For each neuron in the previous layer
							sum += w[l - 1][i][j] * a[l - 1][j]; // Sum up all the outputs*weights
						
						sum += b[l - 1][i]; // Add the bias
						
						//if (logistic)
							a[l][i] = Logistic(sum); // Pass it through the Logistic function
						//else
						//	a[l][i] = Tanh(sum); // Or pass it through the Tanh function
						
					}
					
				//	return 0; // Return nothing
				//}));
				
			//}
			
			//for (auto && result : results) // Wait each task to finish
			//	result.get();
			
			//results.clear();
		}
		
		
		double maxValue=-1;
		int maxDir=-1;

		for(int i=0 ; i < N[layerNumber-1] ; i++) {
			if(a[layerNumber-1][i] > maxValue)
			{
				maxValue = a[layerNumber-1][i];
				maxDir = i;
			}
		}

		directionChosen = maxDir;

		//UnityEngine.Debug.Log("MAXDIR: " + maxDir);

		switch(maxDir) {
		case 0:
			newDirection = direction - 1;
			break;
		case 1: 
			newDirection = direction;
			break;
		case 2:
			newDirection = direction + 1;
			break;
		}

		if(newDirection < 0)
			newDirection += 4;
		if(newDirection >= 4)
			newDirection -= 4;

		watch.Stop();

		long time = watch.ElapsedMilliseconds;

		UnityEngine.Debug.Log("Left:  " + a[layerNumber-1][0]);
		UnityEngine.Debug.Log("Strt:  " + a[layerNumber-1][1]);
		UnityEngine.Debug.Log("Right: " + a[layerNumber-1][2]);
		//UnityEngine.Debug.Log("Bot:   " + a[layerNumber-1][3]);
		UnityEngine.Debug.Log("Time:  " + time);

		

		yield return StartCoroutine(base.Step());

		ai_running = false;
	}

	double Logistic(double x)
	{
		return ((double)1 ) /((double) (1 + System.Math.Exp(-hl*x)));
	}

	public void ExtendSnake () {
		base.ExtendSnake();

		foodEaten = true;
	}

	public void Reset() {
		base.Reset();

		alive = true;
		ai_running = false;
		lastFoodDistance = -1;
	}

}
