using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tracing : MonoBehaviour
{
    public bool isFirst = false;
    public bool isTracing = false;
    public bool canTrace = false;
    public bool tracedThrough = false;
    public int traceableIndex;
    public Renderer traceRenderer;
    public int beatSegment;
    Material TracingMaterial;
    Vector3 planePoint;
    Vector3 defaultPoint;
    Vector3 afterPoint;
    GestureTrace gestureTrace;
    private Transform transformTrace;
    
    private Color activeColor = new Color(0.01201493f, 0.7834063f, 0.8490566f);
    private Color inactiveColor = new Color(0.3018868f, 0.3018868f, 0.3018868f);
    // Start is called before the first frame update
    void Awake()
    {
        if(traceRenderer == null)
        {
            Debug.Log("No trace renderer assigned at " + name);
            return;
        }

        TracingMaterial = traceRenderer.material;
        gestureTrace = transform.parent.GetComponent<GestureTrace>();
        transformTrace = transform.Find("Trace");
        if(gestureTrace == null)
        {
            Debug.Log("No GestureTrace found in parent");
        }
        defaultPoint = transformTrace.position + (transformTrace.up * 10);
        afterPoint = transformTrace.position - (transformTrace.up * 10);
        planePoint = defaultPoint;
        SegmentController controller = GetComponentInParent<SegmentController>();
        foreach(Renderer rend in GetComponentsInChildren<Renderer>())
        {
            rend.material.EnableKeyword("_EMISSION");
        }
        if (controller != null && !isFirst)
        {
            controller.SegmentEvent += (int segment) => {
                if(beatSegment == segment)
                { 
                    foreach(Renderer rend in GetComponentsInChildren<Renderer>())
                    {
                        rend.material.SetColor("_Emission", activeColor);
                        rend.material.SetColor("_EmissionColor", activeColor);
                        rend.material.color = activeColor;
                    }
                } else
                {
                    foreach(Renderer rend in GetComponentsInChildren<Renderer>())
                    {
                        rend.material.SetColor("_Emission", inactiveColor);
                        rend.material.SetColor("_EmissionColor", inactiveColor);
                        rend.material.color = inactiveColor;
                    }
                }
                TracingMaterial = traceRenderer.material;
            };
        }
    }

    private void Start()
    {
        Plane clipPlane = new Plane(transformTrace.up, planePoint);
        Vector4 plane = new Vector4(clipPlane.normal.x, clipPlane.normal.y, clipPlane.normal.z, clipPlane.distance);
        TracingMaterial.SetVector("_TracingPlane", plane);
    }

    // Update is called once per frame
    void Update()
    {
        if (isTracing && canTrace && !tracedThrough)
        {
            planePoint = gestureTrace.GetBatonTip().transform.position;
            Plane clipPlane = new Plane(transformTrace.up, planePoint);
            Vector4 plane = new Vector4(clipPlane.normal.x, clipPlane.normal.y, clipPlane.normal.z, clipPlane.distance);
            TracingMaterial.SetVector("_TracingPlane", plane);
        }
    }

    public void StartTracing()
    {
        canTrace = true;
    }

    public void TriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Baton_Tip")
        {
            if (isFirst)
            {
                foreach (Tracing trace in gestureTrace.GetComponentsInChildren<Tracing>())
                {
                    trace.ResetTrace();
                }
                StartTracing();
            }
            if(!tracedThrough && ReadyToTrace(gestureTrace.traceDifficulty))
            {
                Tracing nextTrace = gestureTrace.GetNextTraceable(traceableIndex);
                if (nextTrace != null)
                {
                    if(nextTrace.isFirst)
                    {
                        Debug.Log("resetting first trace");
                        nextTrace.ResetTrace();
                    } else
                    {
                        nextTrace.StartTracing();
                    }
                }
                Tracing prevTrace = gestureTrace.GetPreviousTraceable(traceableIndex);
                if(prevTrace != null)
                {
                    prevTrace.FinishTracing();
                }
            }
            Segmentation segmentation = GetComponent<Segmentation>();
            if (segmentation != null)
            {
                segmentation.publishSegment();
            }
            isTracing = true;
        }
    }

    public void TriggerExit(Collider other)
    {
        if (other.gameObject.name == "Baton_Tip" && canTrace == true)
        {
            FinishTracing();
            Debug.Log("Baton Collider Exit");
        }
    }

    public void ResetTrace()
    {
        //Debug.Log("resetting " + gameObject.name);
        Plane clipPlane = new Plane(transformTrace.up, defaultPoint);
        Vector4 plane = new Vector4(clipPlane.normal.x, clipPlane.normal.y, clipPlane.normal.z, clipPlane.distance);
        TracingMaterial.SetVector("_TracingPlane", plane);
        tracedThrough = false;
    }

    public void FinishTracing()
    {
        planePoint = defaultPoint;
        isTracing = false;
        canTrace = false;
        tracedThrough = true;
        Plane clipPlane = new Plane(transformTrace.up, afterPoint);
        Vector4 plane = new Vector4(clipPlane.normal.x, clipPlane.normal.y, clipPlane.normal.z, clipPlane.distance);
        TracingMaterial.SetVector("_TracingPlane", plane);
        Tracing nextTrace = gestureTrace.GetNextTraceable(traceableIndex);
        if(nextTrace != null)
        {
            if(!nextTrace.isFirst)
            {
                Tracing prevTrace = gestureTrace.GetPreviousTraceable(traceableIndex);
                if (prevTrace != null)
                {
                    if (!prevTrace.isFirst)
                    {
                        prevTrace.FinishTracing();
                    }
                }
            }
        }
    }

    public bool ReadyToTrace(int difficulty)
    {
        Tracing prev = gestureTrace.GetPreviousTraceable(traceableIndex);
        if(prev == null)
        {
            return true;
        } else if(prev.canTrace || prev.tracedThrough)
        {
           // Debug.Log(gameObject.name + " asking for " + prev.name);
            return true;
        } else
        {
            if(difficulty <= 0)
            {
               // Debug.Log("not ready to trace yet " + name + " " + prev.name);
                return false;
            } else
            {
                return prev.ReadyToTrace(difficulty - 1);
            }
        }
    }
}
