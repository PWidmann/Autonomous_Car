using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FitnessMeasure{
	distance,
	distance2byTime,
	distanceByTime
}
	

public class NeuralController : MonoBehaviour 
{
	[SerializeField] ScoreManager scoreManager;
	new Rigidbody rigidbody;
	CarController carController;
	SteeringBehavior steeringBehavior;
	AIController aIController;
	


	public FitnessMeasure fitnessMeasure;

	public static float mutationPercent = 0.03f;

    public int population;
    public static int maxPopulation;

    public double driveTime = 0;
	public static double steering;
	public static double braking;
	public static double motor;

	public static int generation = 0;
	public static double [] points;
	public double [] results;
	public static double [] sensors;
	public static int currentNeuralNetwork = 0;

	public static float bestDistance = 0;

	public static Vector3 startPosition;
	public static Quaternion startRotation;

	public int[] layers;
	public static int[] getLayers;

	public static Network [] networks; 
	public static Network currentNeuralNet;
	public Network bestNeuralNet;
	public static int hiddenLayerCount = 0;
	public static int neuronPerLayerCount = 0;

	Vector3 position;

	int heavyImpacts = 0;
	//float brakePoints = 0;
	float wobbleTimer = 0;
	float lastSteerAngle = 0;
	float antiWobbleBonus = 0;

	public static bool resetNetWork = false;
	public static int currentGenBestScore = 0;
	public static int currentSessionBestScore = 0;
	public static Network bestNetwork;

    void Start()
    {
        Debug.Log("Generation " + generation);
        rigidbody = GetComponent<Rigidbody>();
		carController = GetComponent<CarController>();
		steeringBehavior = GetComponent<SteeringBehavior>();
		aIController = GetComponent<AIController>();

		startPosition = transform.position;
		startRotation = transform.rotation;

		sensors = new double[4];
	}

	public void ResetNetwork()
	{
		results = new double[2];
		points = new double[population]; // Bookkeeping current generation

		ResizeLayerArray();
		

		position = transform.position;
		networks = new Network[population];

		maxPopulation = population;

		for (int i = 0; i < population; i++)
		{
			networks[i] = new Network(layers);
		}

		currentNeuralNetwork = 0;
		generation = 0;
		currentNeuralNet = networks[0];
		NNVisualization.Instance.NewNetInitialization(currentNeuralNet);
	}

	void ResizeLayerArray()
	{
		int[] newArray = new int[hiddenLayerCount + 2]; // Plus 1 for output layer

		newArray[0] = layers[0];
        for (int i = 1; i < newArray.Length -1; i++)
        {
			newArray[i] = neuronPerLayerCount;
        }
		
		newArray[newArray.Length -1] = 2;

		layers = newArray;
		getLayers = newArray;
	}

	void FixedUpdate()
	{
		if (aIController.steeringMode == AIController.SteeringMode.NeuralNet && aIController.aiControllerActive)
		{
			results = networks[currentNeuralNetwork].Process(sensors);
			steering = ((double)results[0]);
			motor = (double)results[1] * 0.5f;
			//braking = (double)results[2] - 0.5f;

			driveTime += Time.deltaTime;

			points[currentNeuralNetwork] += Vector3.Distance(position, transform.position);
			position = transform.position;
			currentNeuralNet = networks[currentNeuralNetwork];
		}
	}
	
	void Update () {
		if (aIController.steeringMode == AIController.SteeringMode.NeuralNet && aIController.aiControllerActive)
		{
			if (resetNetWork)
			{
				ResetNetwork();
				NNVisualization.Instance.NewNetInitialization(networks[0]);
				resetNetWork = false;
			}

			//if (sensors[1] < 0.2f && results[2] > 0 && sensors[3] > 0.15f)
			//brakePoints += Time.deltaTime;

			//check if the network is moving
			if (driveTime > 3 && (carController.Velocity * 3.6) < 5) //if less than 2 km/h
			{
				if (aIController.steeringMode == AIController.SteeringMode.NeuralNet)
					OnCollisionEnter(null);
			}

			// Next population agent after driving for 60s
			if (driveTime > 60f)
			{
				OnCollisionEnter(null);
			}

			// Speed cap
			if (carController.Velocity * 3.6f < 80f)
				points[currentNeuralNetwork] += 10f * Time.deltaTime;

			wobbleTimer += Time.deltaTime;
			
			if (wobbleTimer >= 0.2f)
			{
				// if not wobbling
				if(lastSteerAngle > carController.wheels[0].collider.steerAngle - 3 || lastSteerAngle < carController.wheels[0].collider.steerAngle + 3)
				{
					antiWobbleBonus += Time.deltaTime;
					points[currentNeuralNetwork] += antiWobbleBonus * 5;
					antiWobbleBonus = 0;
				}
			
				lastSteerAngle = carController.wheels[0].collider.steerAngle;
				wobbleTimer = 0;
			}

		}
	}

	//game over, friend :/
	void OnCollisionEnter (Collision col)
	{
		if (aIController.steeringMode == AIController.SteeringMode.NeuralNet && aIController.aiControllerActive)
		{
			aIController.lapTimer = 0;

			if (results[1] > 0.3f) // If there is an impact while high motor torque
			{
				// decrease weight on motor torque neuron
				heavyImpacts = 5;
			}

			//Debug.Log ("end!");
			resetCarPosition();

			switch (fitnessMeasure)
			{
				case FitnessMeasure.distance2byTime:
					
					points[currentNeuralNetwork] *= points[currentNeuralNetwork];
					points[currentNeuralNetwork] /= driveTime;
					//if (heavyImpacts != 0)
					//	points[currentNeuralNetwork] /= heavyImpacts;
					//points[currentNeuralNetwork] *= brakePoints;
					break;
				case FitnessMeasure.distanceByTime:
					points[currentNeuralNetwork] /= driveTime;
					break;
				default:
					break;
			}

			points[currentNeuralNetwork] = Mathf.Round((float)points[currentNeuralNetwork]);

			heavyImpacts = 0;
			//brakePoints = 0;
			driveTime = 0;
			//Debug.Log("Wobble Bonus" + wobbleBonus);
			antiWobbleBonus = 0;

			//now we reproduce
			if (currentNeuralNetwork == population - 1)
			{
				

				double maxValue = points[0];
				int maxIndex = 0;

				//looking for the two best networks in the generation

				for (int i = 1; i < population; i++)
				{
					if (points[i] > maxValue)
					{
						maxIndex = i;
						maxValue = points[i];
						if (maxValue < 0)
							maxValue = 0;
					}
				}

				scoreManager.AddScoreEntry(generation, (int)maxValue); // Best score from this generation

				Debug.Log("first parent is has " + maxValue + " points");

				if (points[maxIndex] > bestDistance)
				{
					bestDistance = (float)points[maxIndex];
				}

				if (points[maxIndex] > currentSessionBestScore)
				{
					currentSessionBestScore = (int)points[maxIndex];
				}


				points[maxIndex] = -10;

				Network mother = networks[maxIndex];

				

				//Set best current Network


				maxValue = points[0];
				maxIndex = 0;

				for (int i = 1; i < population; i++)
				{
					if (points[i] > maxValue)
					{
						maxIndex = i;
						maxValue = points[i];
					}
				}

				Debug.Log("second parent is has " + maxValue + " points");

				points[maxIndex] = -10;

				Network father = networks[maxIndex];


				networks[0] = mother; // Best network goes directly into the next generation unmutated
				points[0] = 0; 

				for (int i = 1; i < population; i++)
				{
					points[i] = 0;
					//creating new generation of networks with random combinations of genes from two best parents
					networks[i] = new Network(father, mother);
				}

				generation++;
				Debug.Log("generation " + generation + " is born");
				
				
				
				//because we increment it at the beginning, that's why -1
				currentNeuralNetwork = -1;
			}

			currentNeuralNetwork++;

			//position reset is pretty important, don't forget it :*
			position = transform.position;
		}
	}

    void resetCarPosition()
    {
        rigidbody.velocity = Vector3.zero;
		rigidbody.angularVelocity = Vector3.zero;
		transform.position = startPosition;
        transform.rotation = startRotation;
		aIController.currentWayPoint = 0;
    }
}

