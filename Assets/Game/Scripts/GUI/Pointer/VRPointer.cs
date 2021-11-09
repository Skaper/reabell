using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VRPointer : MonoBehaviour
{
    public float defaultLength = 5f;
    public GameObject dot;
    public bool useDotPointer = true;
    public Camera Camera { get; private set; } = null;

    public GameObject Target { get; private set; } = null;

    private VRInput vrInput = null;
    private LineRenderer lineRenderer = null;


    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        Camera = GetComponent<Camera>();
        Camera.enabled = false;
        vrInput = GetComponent<VRInput>();//EventSystem.current.gameObject.GetComponent<VRInput>();
    }

    private void Start()
    {
        

       
    }

    private void Update()
    {
        UpdateLine();
    }

    private void UpdateLine()
    {
        PointerEventData data = vrInput.Data;
        RaycastHit hit = CreateRayCast();

        if (hit.collider)
        {
            Target = hit.collider.gameObject;
        }
        else
        {
            Target = null;
        }

        float colliderDistance = hit.distance == 0 ? defaultLength : hit.distance;
        float canvasDistance = data.pointerCurrentRaycast.distance == 0 ? defaultLength : data.pointerCurrentRaycast.distance;
        
        float targetLength = Mathf.Min(colliderDistance, canvasDistance);
        
        Vector3 endPosition = transform.position + (transform.forward * targetLength);

        if(useDotPointer)dot.transform.position = endPosition;

        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, endPosition);
        
    }

    private RaycastHit CreateRayCast()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);
        Physics.Raycast(ray, out hit, defaultLength, -5, QueryTriggerInteraction.Ignore);

        return hit;
    }
}
