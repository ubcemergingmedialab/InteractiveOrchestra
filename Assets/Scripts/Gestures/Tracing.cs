using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class Tracing : MonoBehaviour
{
    public bool isFirst = false;
    public int traceableIndex;
    public bool canTrace = false;
    public bool isTracing = false;
    public bool tracedThrough = false;
    Material TracingMaterial;
    Vector3 planePoint;
    Vector3 defaultPoint;
    Vector3 afterPoint;
    GestureTrace gestureTrace;
    // Start is called before the first frame update
    void Awake()
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
    }

    private void Start()
    {
        Plane clipPlane = new Plane(transform.up, planePoint);
        Vector4 plane = new Vector4(clipPlane.normal.x, clipPlane.normal.y, clipPlane.normal.z, clipPlane.distance);
        TracingMaterial.SetVector("_TracingPlane", plane);
        if (isFirst)
        {
            canTrace = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isTracing && canTrace && !tracedThrough)
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
        if (other.gameObject.name == "Baton_Tip")
        {
            if(ReadyToTrace())
            {
                isTracing = true;
            }
            if (isFirst)
            {
                foreach(Tracing trace in gestureTrace.GetComponentsInChildren<Tracing>())
                {
                    trace.ResetTrace();
                }
                Tracing nextTrace = gestureTrace.GetNextTraceable(traceableIndex);
                if (nextTrace != null)
                {
                    nextTrace.StartTracing();
                }
            } else
            {
                Tracing lastTrace = gestureTrace.GetPreviousTraceable(traceableIndex);
                if(lastTrace != null)
                {
                    lastTrace.FinishTracing();
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Baton_Tip" && canTrace == true)
        {
            if (ReadyToTrace())
            {
                FinishTracing();
            }
            Debug.Log("Baton Collider Exit");
        }
    }

    public void ResetTrace()
    {
        Debug.Log("resetting " + gameObject.name);
        Plane clipPlane = new Plane(transform.up, defaultPoint);
        Vector4 plane = new Vector4(clipPlane.normal.x, clipPlane.normal.y, clipPlane.normal.z, clipPlane.distance);
        TracingMaterial.SetVector("_TracingPlane", plane);
        tracedThrough = false;
        canTrace = false;
    }

    public void FinishTracing()
    {
        planePoint = defaultPoint;
        isTracing = false;
        canTrace = false;
        tracedThrough = true;
        Plane clipPlane = new Plane(transform.up, afterPoint);
        Vector4 plane = new Vector4(clipPlane.normal.x, clipPlane.normal.y, clipPlane.normal.z, clipPlane.distance);
        TracingMaterial.SetVector("_TracingPlane", plane);
    }

    private bool ReadyToTrace()
    {
        Tracing prev = gestureTrace.GetPreviousTraceable(traceableIndex);
        if(prev == null)
        {
            return true;
        } else if(prev.isTracing && prev.canTrace)
        {
            Debug.Log(gameObject.name + " asking for " + prev.name);
            return true;
        } else
        {
            return false;
        }
    }
}
