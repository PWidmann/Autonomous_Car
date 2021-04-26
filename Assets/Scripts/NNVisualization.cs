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
        int inputNeurons = currentNet.layers[0];
        Debug.Log("Inputneurons: " + inputNeurons);

        int hiddenlayers = currentNet.layers.Length - 2;
        Debug.Log("Hiddenlayers: " + hiddenlayers);

        int outputNeurons = currentNet.layers[currentNet.layers.Length -1];
        Debug.Log("Outputneurons: " + outputNeurons);
    }
}
