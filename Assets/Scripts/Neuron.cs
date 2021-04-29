using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neuron
{
    public double[] inputValues;
    public double[] weights;
    public double outputValue;
    public double bias = 0;

    public Neuron(int _inputValues)
    {
        inputValues = new double[_inputValues];
        weights = new double[_inputValues];

        PopulateWeights();
    }

    public void PopulateWeights()
    {
        // Initialize
        for (int i = 0; i < weights.Length; i++)
        {
            weights[i] = Random.Range(-1.0f, 1.0f);
        }
    }

    public void Process()
    {
        double sum = 0;

        for (int i = 0; i < inputValues.Length - 1; i++)
        {
            sum += inputValues[i] * weights[i];
        }

        sum += bias;

        //Activation
        //outputValue = (float)sigmoid(sum);
        outputValue = sum;
    }

    public void GetInputValues(Layer previousLayer)
    {
        double[] values = new double[previousLayer.neurons.Length];

        for (int n = 0; n < previousLayer.neurons.Length; n++)
        {
            values[n] = previousLayer.neurons[n].outputValue;
        }

        inputValues = values;
    }

    public void ActivationTanH()
    {
        outputValue = System.Math.Tanh(outputValue);
    }

    public void ActivationSigmoid()
    {
        outputValue = sigmoid(outputValue);
    }

    double sigmoid(double value)
    {
        return 1 / (1 + Mathf.Exp(-(float)value));
    }

    public void RandomWeightValue(int weightIndex)
    {
        weights[weightIndex] = Random.Range(-1.0f, 1.0f);
    }
}
