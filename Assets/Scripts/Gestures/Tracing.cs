using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class Tracing : MonoBehaviour
{
    public bool isFirst = false;
    bool canTrace = false;
    bool isTracing = false;
    Material TracingMaterial;
    Vector3 planePoint;
    Vector3 defaultPoint;
    Vector3 afterPoint;
    GestureTrace gestureTrace;
    // Start is called before the first frame update
    void Start()
    {
        TracingMaterial = GetComponent<Renderer>().material;
        gestureTrace = transform.parent.GetComponent<GestureTrace>();
        if(gestureTrace == null)
        {
            Debug.Log("No GestureTrace found in parent");
        }
        defaultPoint = transform.position + (transform.up * 10);
        afterPoint = transform.position - (transform.up * 10);
        planePoint = defaultPoint;
        if(isFirst)
        {
            canTrace = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isTracing)
        {
            planePoint = gestureTrace.GetBatonTip().transform.position;
            Plane clipPlane = new Plane(transform.up, planePoint);
            Vector4 plane = new Vector4(clipPlane.normal.x, clipPlane.normal.y, clipPlane.normal.z, clipPlane.distance);
            TracingMaterial.SetVector("_TracingPlane", plane);
        }
    }

    public void StartTracing()
    {
        canTrace = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Baton_Tip" && canTrace == true)
        {
            isTracing = true;
            if(isFirst)
            {
                foreach(Tracing trace in gestureTrace.GetComponentsInChildren<Tracing>())
                {
                    trace.ResetTrace();
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Baton_Tip" && canTrace == true)
        {
            Debug.Log("Baton Collider Exit");
            planePoint = defaultPoint;
            isTracing = false;
            canTrace = false;
            gestureTrace.GetNextTraceable().StartTracing();
            Plane clipPlane = new Plane(transform.up, afterPoint);
            Vector4 plane = new Vector4(clipPlane.normal.x, clipPlane.normal.y, clipPlane.normal.z, clipPlane.distance);
            TracingMaterial.SetVector("_TracingPlane", plane);
        }
    }

    public void ResetTrace()
    {
        Debug.Log("resetting " + gameObject.name);
        Plane clipPlane = new Plane(transform.up, defaultPoint);
        Vector4 plane = new Vector4(clipPlane.normal.x, clipPlane.normal.y, clipPlane.normal.z, clipPlane.distance);
        TracingMaterial.SetVector("_TracingPlane", plane);
    }
}
