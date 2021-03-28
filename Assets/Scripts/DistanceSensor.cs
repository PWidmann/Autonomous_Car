using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceSensor : MonoBehaviour
{
    private float distance;
    private Vector3 targetPoint;
    private bool debugMode = true;
    private LineRenderer lineRenderer;

    public bool DebugMode { get => debugMode; set => debugMode = value; }
    public float Distance { get => distance; set => distance = value; }

    private RaycastHit hit;
    Vector3[] positions = new Vector3[2];

    Material yellowMat;
    Material greenMat;
    Material redMat;

    private void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = yellowMat;
        lineRenderer.startWidth = 0.2f;
        lineRenderer.endWidth = 0.2f;

        yellowMat = new Material(Shader.Find("Unlit/Color"));
        greenMat = new Material(Shader.Find("Unlit/Color"));
        redMat = new Material(Shader.Find("Unlit/Color"));
        yellowMat.color = Color.yellow;
        greenMat.color = Color.green;
        redMat.color = Color.red;
    }

    public void UpdatePosition()
    {

        Vector3 forward = (transform.position + transform.forward) - transform.position;
        forward.y = 0; // always straight forward, ignore suspension car torque
        if (Physics.Raycast(transform.position, forward, out hit, 2000f))
        {
            lineRenderer.enabled = true;
            targetPoint = hit.point;
            distance = Vector3.Distance(transform.position, targetPoint);
            
        }
        else
        {
            lineRenderer.enabled = false;
            distance = 2000f;
        }


        if (DebugMode)
        {
            positions[0] = transform.position;
            positions[1] = targetPoint;
            lineRenderer.SetPositions(positions);

            if (distance > 30f)
            {
                lineRenderer.material = greenMat;
            }
            if (distance <= 30 && distance > 15)
            {
                lineRenderer.material = yellowMat;
            }
            if (distance <= 15 && distance > 0)
            {
                lineRenderer.material = redMat;
            }
        }
    }
}
