using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Layer
{
    public Neuron[] neurons;

    public Layer(int _neuronCount, int _previousLayerNeuronCount)
    {
        neurons = new Neuron[_neuronCount];

        for (int i = 0; i < _neuronCount; i++)
        {
            neurons[i] = new Neuron(_previousLayerNeuronCount);
        }
    }
}
