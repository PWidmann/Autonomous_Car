using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Network {

	public double [][][] weights;
	public int [] parameters;

	public int lenght;

    double sigmoid(double x)
    {
		return 1 / (1 + Mathf.Exp(-(float)x));
    }

	double ReLU(float s)
	{
		//leaky relu to prevent
		//dying of the neurons
		float leakyReLU = Mathf.Max(0.001f * s, s);
		return Mathf.Clamp((leakyReLU), -1, 1);
	}

	float eLU(float s)
	{
		//if (s > 0) return s;
		return Mathf.Clamp(0.3f * (Mathf.Exp(s) - 1), -1, 1) / 10f;
	}

	float SoftSign(float s)
	{
		float product = s / (1 + Mathf.Abs(s));
		return Mathf.Clamp(product, -1, 1);
	}


	void initializeVariables()
	{
		this.weights = new double[parameters.Length - 1][][];
		this.lenght = parameters.Length;
	}

	public Network(Network Dad, Network Mom)
	{
		this.parameters = Mom.parameters;
		initializeVariables ();

		for (int i = 0; i < parameters.Length - 1; i++) { // layers

			weights [i] = new double[parameters [i]][];

			for (int j = 0; j < parameters [i]; j++) { // neurons

				weights [i] [j] = new double[parameters [i + 1]];

				for (int k = 0; k < parameters [i + 1]; k++) { // connections

					// crossing
					if (Random.Range (0, 2) == 0) {
						weights [i] [j] [k] = Mom.weights [i] [j] [k];
					} else {
						weights [i] [j] [k] = Dad.weights [i] [j] [k];		
					}
				}
			}
		}

		int mutationLayer = Random.Range(0, weights.Length);
		int mutationLeft  = Random.Range(0, weights[mutationLayer].Length);
		int mutationRight = Random.Range(0, weights[mutationLayer][mutationLeft].Length);

		weights [mutationLayer] [mutationLeft] [mutationRight] = getRandomWeight ();
		//Debug.Log (mutationLayer + " " + mutationLeft + " " + mutationRight);
	}

	public Network(int [] parameters)
	{
		this.parameters = parameters;
		//int a = 0;
		//{3,5,2}

		initializeVariables ();

		for (int i = 0; i < parameters.Length - 1 ; i++) {
			
			weights[i] = new double[parameters[i]][];

			for (int j = 0; j < parameters [i]; j++) {
				
				weights[i][j] =  new double[parameters[i+1]];

				for (int k = 0; k < parameters [i + 1]; k++) {
				
					weights [i] [j] [k] = getRandomWeight ();
				}
			}
		}

	}

	public double [] process(double [] inputs) // propagation
	{
		//int a = 0;
		
		if (inputs.Length != parameters [0]) {
		
			Debug.Log ("wrong input lenght!");
			return null;
		}
			
		double[] outputs;
		//Debug.Log (lenght);
		//for each layer
		for (int i = 0; i < (lenght-1); i++) {
			
			//output values, they all start at 0 by default, checked that in C# Documentation ;)
			outputs = new double[parameters [i+1]];

			//for each input neuron
			for (int j = 0; j < inputs.Length; j++) {
			
				//and for each output neuron
				for (int k = 0; k < outputs.Length; k++) {

					//increase the load of an output neuron by the value of each input neuron multiplied by the weight between them
					outputs [k] += inputs [j] * weights [i] [j] [k];
				}
			}

			//we have the proper output values, now we have to use them as inputs to the next layer and so on, until we hit the last layer
			inputs = new double[outputs.Length];

			//after all output neurons have their values summed up, apply the activation function and save the value into new inputs
			for (int l = 0; l < outputs.Length; l++) {
				inputs [l] = sigmoid(outputs [l] * 5);
				//inputs[l] = (float)ReLU((float)outputs[l] * 5);
				//inputs[l] = eLU((float)outputs[l] * -5);
			}
		}
		return inputs;
	}

	//	this is DEPRECATED
	public double [] processRecurrent(double [] inputs, int layer)
	{
		if (layer == parameters.Length -1 ) {
			//Debug.Log (layer);
			return inputs;
		}

		layer++;

		double [] outputs = new double[parameters[layer]];
	
		for (int i = 0; i < parameters [layer]; i++) {
			outputs [i] = 0;
		}

		for (int i = 0; i < parameters[layer] ; i++) {

			for (int j = 0; j < inputs.Length; j++) {
				
				outputs [i] += inputs [j] * weights [layer - 1] [j] [i];

			}
				
		}

		for (int i = 0; i < parameters [layer]; i++) {
			outputs [i] = sigmoid(outputs[i]);
			//outputs[i] = (float)ReLU((float)outputs[i] * 5);
			//outputs[i] = eLU((float)outputs[i] * -5);
		}

		return processRecurrent (outputs, layer);

	}

	double getRandomWeight()
    {
		return Random.Range(-1.0f, 1.0f);
	}

}
