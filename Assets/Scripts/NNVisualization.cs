using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NNVisualization : MonoBehaviour
{
    [SerializeField] GameObject neuronPanel;

    [SerializeField] NeuralController neuralController;

    void Start()
    {
        
    }


    void Update()
    {
        Network currentNet = neuralController.currentNeuralNet;
        int inputNeurons = currentNet.parameters[0];
        Debug.Log("Inputneurons: " + inputNeurons);

        int hiddenlayers = currentNet.parameters.Length - 2;
        Debug.Log("Hiddenlayers: " + hiddenlayers);

        int outputNeurons = currentNet.parameters[currentNet.parameters.Length];
        Debug.Log("Outputneurons: " + outputNeurons);
    }
}
