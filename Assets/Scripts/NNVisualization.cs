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
        //Network currentNet = neuralController.currentNeuralNet;
        //int inputNeurons = currentNet.layerValues[0];
        //Debug.Log("Inputneurons: " + inputNeurons);
        //
        //int hiddenlayers = currentNet.layerValues.Length - 2;
        //Debug.Log("Hiddenlayers: " + hiddenlayers);
        //
        //int outputNeurons = currentNet.layerValues[currentNet.layerValues.Length -1];
        //Debug.Log("Outputneurons: " + outputNeurons);
    }
}
