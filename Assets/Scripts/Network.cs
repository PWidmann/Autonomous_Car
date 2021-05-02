using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Network {

	public Layer[] layers; // layers without input layer
	double[] output; 

	public Network(int [] _layers)
	{
		layers = new Layer[_layers.Length -1]; // without input layer
		output = new double[_layers[layers.Length]];

		for (int l = 0; l < layers.Length; l++)
		{
			// if first hidden layer -> input count from sensors
			if(l == 0)
				layers[l] = new Layer(_layers[1], _layers[0]);
			else
				layers[l] = new Layer(_layers[l + 1], _layers[l]);
		}
	}

	public Network(Network Dad, Network Mom)
	{

		int[] childLayers = new int[Mom.layers.Length + 1];
		childLayers[0] = 4;
		for (int i = 1; i < childLayers.Length; i++)
		{
			childLayers[i] = Mom.layers[i-1].neurons.Length;
		}

		layers = new Layer[childLayers.Length - 1];
		output = new double[childLayers[layers.Length]];

		for (int l = 0; l < layers.Length; l++)
		{
			// if first hidden layer -> input count from sensors
			if (l == 0)
				layers[l] = new Layer(childLayers[1], childLayers[0]);
			else
				layers[l] = new Layer(childLayers[l + 1], childLayers[l]);
		}

		for (int l = 0; l < layers.Length; l++) // for every layer
		{
			for (int n = 0; n < layers[l].neurons.Length; n++) // for every neuron
			{
				for (int w = 0; w < layers[l].neurons[n].weights.Length; w++) // for every weight
				{
					// random weight crossing
					if (Random.Range(0, 2) == 0)
					{
						layers[l].neurons[n].weights[w] = Mom.layers[l].neurons[n].weights[w];
					}
					else
					{
						layers[l].neurons[n].weights[w] = Dad.layers[l].neurons[n].weights[w];
					}
				}
			}
		}


        for (int i = 0; i < 3; i++)
        {
			// Mutation
			int mutationLayer = Random.Range(0, layers.Length);
			int mutationNeuron = Random.Range(0, layers[mutationLayer].neurons.Length);
			int mutationWeight = Random.Range(0, layers[mutationLayer].neurons[mutationNeuron].weights.Length);

			layers[mutationLayer].neurons[mutationNeuron].RandomWeightValue(mutationWeight);
		}
	}

	public double[] Process(double[] neuralNetInput)
	{
		// Forward propagation
        // Iterate through every layer and every neuron
        for (int l = 0; l < layers.Length; l++) // for every layer
        {
            for (int n = 0; n < layers[l].neurons.Length; n++) // for every neuron
            {
				// get input value from previous layer output value
				if (l == 0)
				{
					layers[l].neurons[n].inputValues = neuralNetInput; // first layer gets input from sensors
					layers[l].neurons[n].Process();
				}
				else if (l == layers.Length - 1) // Last layer with activation
				{
					layers[l].neurons[n].GetInputValues(layers[l - 1]);
					layers[l].neurons[n].Process();

					if(layers[l].neurons[n] == layers[layers.Length -1].neurons[layers[layers.Length - 1].neurons.Length -1]) // if motor output // Last neuron of network
						layers[l].neurons[n].ActivationSigmoid(); // Motor output
					else
						layers[l].neurons[n].ActivationSigmoid(); // Steering

				}
				else
				{
					layers[l].neurons[n].GetInputValues(layers[l - 1]);
					layers[l].neurons[n].Process();
				}
            }
        }

		// Get Network output values from last layer output		
        for (int o = 0; o < output.Length; o++)
        {
			output[o] = layers[layers.Length-1].neurons[o].outputValue;
		}

		return output;
	}

	
}
