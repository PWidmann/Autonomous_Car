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
    new Rigidbody rigidbody;
	CarController carController;
	SteeringBehavior steeringBehavior;


	public FitnessMeasure fitnessMeasure;

    public int timeScale;
	public float mutationPercent = 0.1f;

	public int population;
	public static int staticPopulation;

    public double driveTime = 0;
	public static double steering;
	public static double braking;
	public static double motor;

	public static int generation = 0;
	public double [] points;
	public double [] results;
	double [] sensors;
	public static int currentNeuralNetwork = 0;

	public static float bestDistance = 0;

	Vector3 startPosition;
	Quaternion startRotation;
	int heavyImpacts = 0;
	float brakingPoints = 1;

	public int[] layers;

	Network [] networks; // Genotypen -> gespeicherte gewichtungen
	public Network currentNeuralNet;
	public Network bestNeuralNet;
	RaycastHit hit;

	Vector3 position;

    // Use this for initialization
    void Start()
    {
		//parameters = { 4, 6, 6, 2 }; // 4 input, 1 hidden layer with 10 neurons, 3 outputs
		staticPopulation = population;

        Time.timeScale = timeScale;

        Debug.Log("Generation " + generation);
        rigidbody = GetComponent<Rigidbody>();
		carController = GetComponent<CarController>();
		steeringBehavior = GetComponent<SteeringBehavior>();

		results = new double[2];
        points = new double[population]; // Bookkeeping
        sensors = new double[3];
       
        position = transform.position;
        networks = new Network[population];

		startPosition = transform.position;
		startRotation = transform.rotation;

        for (int i = 0; i < population; i++)
        {
			networks[i] = new Network(layers);
        }

    }

	void FixedUpdate()
	{
		sensors = steeringBehavior.GetSensorValues();

		results = networks[currentNeuralNetwork].process(sensors);
		steering =((double)results [0] - 0.5f);
		motor = (double)results [1];
		//braking = (double)results[2] - 0.5f;

		driveTime += Time.deltaTime;

		points[currentNeuralNetwork] += Vector3.Distance(position, transform.position);
		position = transform.position;
		currentNeuralNet = networks[currentNeuralNetwork];
	}
	
	// Update is called once per frame
	void Update () {
        
		Time.timeScale = timeScale;

		//if (sensors[1] < 0.15f && results[2] > 0 && sensors[3] > 0.15f)
		//	brakingPoints += Time.deltaTime;
        
		//check if the network is moving
		if(driveTime > 3 && rigidbody.velocity.magnitude<0.005)
        {
			//Debug.Log ("This one stands still!");
            OnCollisionEnter(null);
        }

		// Next population after driving for 60s
		if (driveTime > 60f && steeringBehavior.steeringMode == SteeringBehavior.SteeringMode.NeuralNet)
		{
			OnCollisionEnter(null);
		}

	}

	//game over, friend :/
	void OnCollisionEnter (Collision col)
	{
		if (results[1] > 0.3f) // If there is an impact while high motor torque
		{
			heavyImpacts = 3;
		}

		//Debug.Log ("end!");
		resetCarPosition();

		switch (fitnessMeasure)
		{
			case FitnessMeasure.distance2byTime:
				points[currentNeuralNetwork] *= points[currentNeuralNetwork];
				points[currentNeuralNetwork] /= driveTime;
				if (heavyImpacts != 0)
					points[currentNeuralNetwork] /= heavyImpacts;
				if (brakingPoints != 0)
					points[currentNeuralNetwork] *= brakingPoints;
				break;
			case FitnessMeasure.distanceByTime:
				points[currentNeuralNetwork] /= driveTime;
				break;
			default:
				break;
		}

		heavyImpacts = 0;
		brakingPoints = 1;
		driveTime = 0;

		//Debug.Log("network " + currentNeuralNetwork + " scored " + points[currentNeuralNetwork]);


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
				}
			}

			Debug.Log("first parent is " + maxIndex);

			if (points[maxIndex] > bestDistance)
			{

				bestDistance = (float)points[maxIndex];

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

			Debug.Log("second parent is " + maxIndex);

			points[maxIndex] = -10;

			Network father = networks[maxIndex];


			for (int i = 0; i < population; i++)
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

	//TODO: sometimes the velocity is not reseted.. for some reason
    void resetCarPosition()
    {
        rigidbody.velocity = Vector3.zero;
		rigidbody.angularVelocity = Vector3.zero;
		transform.position = startPosition;
        transform.rotation = startRotation;
		steeringBehavior.currentWayPoint = 0;
    }
}

