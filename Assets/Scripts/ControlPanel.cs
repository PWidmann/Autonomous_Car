using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlPanel : MonoBehaviour
{
    [SerializeField] Slider timeScaleSlider;
    [SerializeField] Text timeScaleSliderText;

    [SerializeField] Dropdown aiSelector;
    [SerializeField] Dropdown classicAIselector;

    [SerializeField] CarController carController;
    [SerializeField] AIController aiController;

    private void FixedUpdate()
    {
        Time.timeScale = timeScaleSlider.value;
        timeScaleSliderText.text = "Timescale: " + timeScaleSlider.value;
    }

    public void StartButton()
    {
        // Reset car
        aiController.gameObject.transform.position = NeuralController.startPosition;
        aiController.gameObject.transform.rotation = NeuralController.startRotation;
        carController.rb.velocity = Vector3.zero;
        carController.rb.angularVelocity = Vector3.zero;
        aiController.currentWayPoint = 0;

        aiController.autoPilotOn = true;

        switch (aiSelector.value)
        {
            case 0:
                aiController.steeringMode = AIController.SteeringMode.ClassicAI;
                GameInterface.Instance.NNVisualisation.SetActive(false);
                break;
            case 1:
                aiController.steeringMode = AIController.SteeringMode.NeuralNet;
                NNVisualization.Instance.NewNetInitialization(NeuralController.networks[0]);
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
    }
}
