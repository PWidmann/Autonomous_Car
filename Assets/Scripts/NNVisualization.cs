using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class NNVisualization : MonoBehaviour
{
    public static NNVisualization Instance;

    [SerializeField] GameObject neuronPanel;
    [SerializeField] GameObject neuronPrefab;
    [SerializeField] Transform InterfaceAnker;
    [SerializeField] GameObject outputTitle;
    [SerializeField] GameObject textMotor;
    [SerializeField] GameObject textSteering;
    [SerializeField] Text generationText;
    [SerializeField] Text populationText;
    [SerializeField] Text bestScore;

    public bool netInitialized = false;
    private Network net;

    public List<GameObject> inputNeurons = new List<GameObject>();
    public GameObject[][] layers;

    Vector2 panelStartPosition;
    float panelStartWidth;

    void Start()
    {
        if (Instance == null)
            Instance = this;

        GameInterface.Instance.NNVisualisation.SetActive(false);
        panelStartWidth = neuronPanel.GetComponent<RectTransform>().rect.width;
        panelStartPosition = neuronPanel.GetComponent<RectTransform>().position;
    }

    private void Update()
    {
        UpdateNeuronValues();

        if (NeuralController.networks.Length > 0)
        {
            generationText.text = "Generation: " + (NeuralController.generation + 1);
            populationText.text = "Population: " + (NeuralController.currentNeuralNetwork + 1) + " / " + NeuralController.maxPopulation;
            bestScore.text = "Current best score: " + NeuralController.currentSessionBestScore;
        }
    }

    void DestroyInterfaceNeurons()
    {
        foreach (Transform child in InterfaceAnker.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void NewNetInitialization(Network currentNet)
    {
        if (InterfaceAnker != null)
        {
            DestroyInterfaceNeurons();

            inputNeurons.Clear();
        }
        

        net = currentNet;

        
        // input layers
        for (int i = 0; i < NeuralController.sensors.Length; i++)
        {
            GameObject temp = Instantiate(neuronPrefab, InterfaceAnker);
            temp.transform.position = new Vector3(temp.transform.position.x, (temp.transform.position.y-50) - i * 50, temp.transform.position.z);
            inputNeurons.Add(temp);
        }


        // hidden layers & output
        if (net != null)
        {
            layers = new GameObject[net.layers.Length][];

            for (int x = 0; x < net.layers.Length; x++) // For each layer
            {
                layers[x] = new GameObject[net.layers[x].neurons.Length];

                for (int y = 0; y < layers[x].Length; y++) // For each neuron
                {
                    if (x == net.layers.Length -1)
                    {
                        GameObject temp = Instantiate(neuronPrefab, InterfaceAnker);
                        temp.transform.position = new Vector3((temp.transform.position.x + 110) + x * 50, (temp.transform.position.y - 100) - y * 50, temp.transform.position.z);
                        layers[x][y] = temp;
                        outputTitle.transform.position = new Vector3(temp.transform.position.x - 26, temp.transform.position.y + 90, temp.transform.position.z);
                        textSteering.transform.position = new Vector3(temp.transform.position.x + 110, temp.transform.position.y + 42, temp.transform.position.z);
                        textMotor.transform.position = new Vector3(temp.transform.position.x + 110, temp.transform.position.y -5, temp.transform.position.z);
                    }
                    else
                    {
                        GameObject temp = Instantiate(neuronPrefab, InterfaceAnker);
                        temp.transform.position = new Vector3((temp.transform.position.x + 80) + x * 50, temp.transform.position.y - y * 50, temp.transform.position.z);
                        layers[x][y] = temp;
                    }
                }
            }
        }
    }

    public void UpdateNeuronValues()
    {
        if (layers != null)
        {
            // Update neural net input values
            for (int i = 0; i < inputNeurons.Count; i++)
            {
                inputNeurons[i].GetComponentInChildren<Text>().text = Math.Round(NeuralController.sensors[i], 1).ToString();
            }

            // Update Hidden & output layer
            for (int x = 0; x < NeuralController.hiddenLayerCount + 1; x++)
            {
                for (int y = 0; y < layers[x].Length; y++)
                {
                    if(NeuralController.currentNeuralNet != null)
                        layers[x][y].GetComponentInChildren<Text>().text = Math.Round((float)NeuralController.currentNeuralNet.layers[x].neurons[y].outputValue, 1).ToString();
                }
            }
        }
    }
}
