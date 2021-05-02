using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NNVisualization : MonoBehaviour
{
    public static NNVisualization Instance;

    [SerializeField] GameObject neuronPanel;
    [SerializeField] GameObject neuronPrefab;
    [SerializeField] Transform InterfaceAnker;
    
    public bool netInitialized = false;
    private Network net;

    void Start()
    {
        if (Instance == null)
            Instance = this;
    }

    public void NewNetInitialization(Network currentNet)
    {
        if (InterfaceAnker != null)
        {
            foreach (Transform child in InterfaceAnker.transform)
            {
                Destroy(child.gameObject);
            }
        }
        

        net = currentNet;

        for (int x = 0; x < net.layers.Length; x++) // For each layer
        {
            for (int y = 0; y < net.layers[x].neurons.Length; y++) // For each neuron
            {
                GameObject temp = Instantiate(neuronPrefab, InterfaceAnker);
                temp.transform.position = new Vector3(temp.transform.position.x + x * 30, temp.transform.position.y + y * 30, temp.transform.position.z);
            }
        }
    }



    public void UpdateNeuronValues()
    {

    }
}
