using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlPanel : MonoBehaviour
{
    [SerializeField] Slider timeScaleSlider;
    [SerializeField] Text timeScaleSliderText;

    [SerializeField] Slider hiddenlayerSlider;
    [SerializeField] Text hiddenlayerValueText;

    [SerializeField] Slider neuronlayerSlider;
    [SerializeField] Text neuronlayerValueText;

    [SerializeField] Slider mutationSlider;
    [SerializeField] Text mutationValueText;

    [SerializeField] Dropdown aiSelector;
    [SerializeField] Dropdown classicAIselector;

    [SerializeField] CarController carController;
    [SerializeField] AIController aiController;

    private void FixedUpdate()
    {
        Time.timeScale = timeScaleSlider.value;
        timeScaleSliderText.text = "Timescale: " + timeScaleSlider.value;

        ManageInterfaceVisibility();
    }

    void ManageInterfaceVisibility()
    {
        if (aiSelector.value == 1) 
        {
            // If neural net is selected
            classicAIselector.transform.gameObject.SetActive(false);
            hiddenlayerSlider.transform.gameObject.SetActive(true);
            neuronlayerSlider.transform.gameObject.SetActive(true);
            mutationSlider.transform.gameObject.SetActive(true);

            hiddenlayerValueText.text = hiddenlayerSlider.value.ToString();
            neuronlayerValueText.text = neuronlayerSlider.value.ToString();

            NeuralController.mutationPercent = mutationSlider.value / 100;
            mutationValueText.text = (NeuralController.mutationPercent * 100).ToString() + "%";
        }
        else
        {
            // if classic AI selected
            classicAIselector.transform.gameObject.SetActive(true);
            hiddenlayerSlider.transform.gameObject.SetActive(false);
            neuronlayerSlider.transform.gameObject.SetActive(false);
            mutationSlider.transform.gameObject.SetActive(false);
        }
    }

    public void StartButton()
    {
        // Reset car
        aiController.gameObject.transform.position = NeuralController.startPosition;
        aiController.gameObject.transform.rotation = NeuralController.startRotation;
        carController.rb.velocity = Vector3.zero;
        carController.rb.angularVelocity = Vector3.zero;
        aiController.currentWayPoint = 0;
        aiController.ResetLapTimer();

        aiController.autoPilotOn = true;

        switch (aiSelector.value)
        {
            case 0:
                aiController.steeringMode = AIController.SteeringMode.ClassicAI;
                GameInterface.Instance.NNVisualisation.SetActive(false);
                break;
            case 1:
                aiController.steeringMode = AIController.SteeringMode.NeuralNet;
                NeuralController.resetNetWork = true;
                GameInterface.Instance.NNVisualisation.SetActive(true);
                break;
        }

        switch (classicAIselector.value)
        {
            case 0:
                aiController.classicAIMode = AIController.ClassicAIMode.Waypoints;
                break;
            case 1:
                aiController.classicAIMode = AIController.ClassicAIMode.Sensors;
                break;
        }

        aiController.aiControllerActive = true;
        aiController.lapStarted = true;

        NeuralController.hiddenLayerCount = (int)hiddenlayerSlider.value;
        NeuralController.neuronPerLayerCount = (int)neuronlayerSlider.value;
        NeuralController.currentSessionBestScore = 0;
    }

    public void ShowNNButton()
    {
        GameInterface.Instance.NNVisualisation.SetActive(!GameInterface.Instance.NNVisualisation.activeSelf);
    }
}
