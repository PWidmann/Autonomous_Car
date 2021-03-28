using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameInterface : MonoBehaviour
{
    public static GameInterface Instance;

    [SerializeField] private Text speedText;



    void Start()
    {
        if (Instance == null)
            Instance = this;
    }

    
    void Update()
    {
        
    }

    public void SetSpeedText(float _speed)
    {
        speedText.text = Mathf.Floor(_speed / 10).ToString() + " km/h";
    }
}
