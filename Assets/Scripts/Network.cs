using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Network {

	public double [][][] weights;
	public double[][] neuronOutputs;
	public int [] layers;

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
		this.weights = new double[layers.Length - 1][][];
		this.lenght = layers.Length;
	}

	public Network(Network Dad, Network Mom)
	{
		this.layers = Mom.layers;
		initializeVariables ();

		for (int l = 0; l < layers.Length - 1; l++) { // layers

			weights [l] = new double[layers [l]][];

			for (int n = 0; n < layers [l]; n++) { // neurons

				weights [l] [n] = new double[layers [l + 1]];

				for (int c = 0; c < layers [l + 1]; c++) { // connections

					// crossing
					if (Random.Range (0, 2) == 0) {
						weights [l] [n] [c] = Mom.weights [l] [n] [c];
					} else {
						weights [l] [n] [c] = Dad.weights [l] [n] [c];		
					}
				}
			}
		}

		// Mutation

		// wieviele gewichte

		int mutationLayer = Random.Range(0, weights.Length);
		int mutationNeuron  = Random.Range(0, weights[mutationLayer].Length);
		int mutationConnection = Random.Range(0, weights[mutationLayer][mutationNeuron].Length);

		weights [mutationLayer] [mutationNeuron] [mutationConnection] = getRandomWeight ();
	}

	public Network(int [] _layers)
	{
		this.layers = _layers;

		initializeVariables ();

		for (int l = 0; l < _layers.Length - 1 ; l++) { // layers
			
			weights[l] = new double[_layers[l]][];

			for (int n = 0; n < _layers [l]; n++) { // Neurons
				
				weights[l][n] =  new double[_layers[l+1]];

				for (int c = 0; c < _layers [l + 1]; c++) { // connections weight
				
					weights [l] [n] [c] = getRandomWeight ();
				}
			}
		}

	}

	public double [] process(double [] inputs) // propagation
	{
		//int a = 0;
		
		if (inputs.Length != layers [0]) {
		
			Debug.Log ("wrong input lenght!");
			return null;
		}
			
		double[] outputs;
		//Debug.Log (lenght);
		//for each layer
		for (int i = 0; i < (lenght-1); i++) {
			
			//output values, they all start at 0 by default, checked that in C# Documentation ;)
			outputs = new double[layers [i+1]];

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

				//neuronOutputs[i][l] = inputs[l];
			}
		}
		return inputs;
	}

	//	this is DEPRECATED
	public double [] processRecurrent(double [] inputs, int layer)
	{
		if (layer == layers.Length -1 ) {
			//Debug.Log (layer);
			return inputs;
		}

		layer++;

		double [] outputs = new double[layers[layer]];
	
		for (int i = 0; i < layers [layer]; i++) {
			outputs [i] = 0;
		}

		for (int i = 0; i < layers[layer] ; i++) {

			for (int j = 0; j < inputs.Length; j++) {
				
				outputs [i] += inputs [j] * weights [layer - 1] [j] [i];

			}
		}

		for (int i = 0; i < layers [layer]; i++) {
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
