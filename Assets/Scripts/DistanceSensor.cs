using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceSensor : MonoBehaviour
{
    public float length = 3f;
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
        lineRenderer.enabled = true;

        yellowMat = new Material(Shader.Find("Unlit/Color"));
        greenMat = new Material(Shader.Find("Unlit/Color"));
        redMat = new Material(Shader.Find("Unlit/Color"));
        yellowMat.color = Color.yellow;
        greenMat.color = Color.green;
        redMat.color = Color.red;
    }

    public float GetNormalizedValue()
    {
        return distance / length;
    }

    public void UpdatePosition()
    {

        Vector3 forward = (transform.position + transform.forward) - transform.position;
        forward.y = 0; // always straight forward, ignore suspension car torque
        if (Physics.Raycast(transform.position, forward, out hit, length))
        {
            distance = Vector3.Distance(transform.position, hit.point);
        }
        else
        {
            distance = length;
        }


        if (DebugMode)
        {
            positions[0] = transform.position;
            positions[1] = transform.position + forward.normalized * length;
            lineRenderer.SetPositions(positions);

            if (distance == length)
            {
                lineRenderer.material = greenMat;
            }
            else
            {
                lineRenderer.material = redMat;
            }
        }
    }
}
