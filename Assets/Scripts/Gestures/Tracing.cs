using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the heart of the tracing mechanic, attach one of these to each trace element
/// </summary>
public class Tracing : MonoBehaviour
{
    // the first traceable behaves differently from the rest, resets trace elements
    public bool isFirst = false;
    // isTracing means that the baton is currently intersecting the trace
    public bool isTracing = false;
    // canTrace means that the trace element is in a state ready to trace (we define rules)
    public bool canTrace = false;
    // tracedThrough used to mark trace elements already traced so they stop changing
    public bool tracedThrough = false;
    // traceableIndex used to get next and previous from GestureTrace in parent
    public int traceableIndex;
    // the renderer that does the magic (has a special shader)
    public Renderer traceRenderer;
    // used to decide what color the trace should be, (does it match?)
    public int beatSegment;

    // reference for material from traceRenderer
    Material TracingMaterial;
    // where the "plane" currently is for the clip shader
    Vector3 planePoint;
    // where the plane starts
    Vector3 defaultPoint;
    // where the plane ends
    Vector3 afterPoint;
    // parent for talking to other trace elements
    GestureTrace gestureTrace;
    // transform that we use to define the plane (the actual renderer is a child of this GameObject)
    private Transform transformTrace;
    
    // Color that the trace will be if the beat segment matches
    private Color activeColor = new Color(0.01201493f, 0.7834063f, 0.8490566f);
    // Color that the trace will be if the beat segment doesn't match
    private Color inactiveColor = new Color(0.3018868f, 0.3018868f, 0.3018868f);

    void Awake()
    {
        if(traceRenderer == null)
        {
            Debug.Log("No trace renderer assigned at " + name);
            return;
        }

        TracingMaterial = traceRenderer.material;
        gestureTrace = transform.parent.GetComponent<GestureTrace>();
        transformTrace = transform.Find("Trace"); // there will always be a child with name "Trace"
        if(gestureTrace == null)
        {
            Debug.Log("No GestureTrace found in parent");
        }
        defaultPoint = transformTrace.position + (transformTrace.up * 10); //use child transform to initialize plane
        afterPoint = transformTrace.position - (transformTrace.up * 10);
        planePoint = defaultPoint;
        SegmentController controller = GetComponentInParent<SegmentController>();
        foreach(Renderer rend in GetComponentsInChildren<Renderer>())
        {
            rend.material.EnableKeyword("_EMISSION"); // allowing us to change emission in a renderer as well as color
        }
        if (controller != null && !isFirst)
        {
            // event handler for segment
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
        // these are the requirements for being a trace element actively tracing
        if (isTracing && canTrace && !tracedThrough)
        {
            planePoint = gestureTrace.GetBatonTip().transform.position;
            Plane clipPlane = new Plane(transformTrace.up, planePoint);
            Vector4 plane = new Vector4(clipPlane.normal.x, clipPlane.normal.y, clipPlane.normal.z, clipPlane.distance);
            TracingMaterial.SetVector("_TracingPlane", plane);
        }
    }
    
    /// <summary>
    /// sets canTrace to true which is for final check before segment can start clipping
    /// </summary>
    public void StartTracing()
    {
        canTrace = true;
    }

    /// <summary>
    /// This is the core of the tracing logic:
    /// 1. touching the first element (isFirst == true) resets all traces
    /// 2. otherwise, an element can start tracing if it is being touched, isnt traced through, and passes ReadyToTrace
    ///   2a. if the next trace is the first trace, reset it so that it can be passed through again
    ///   2b. else, just continue by preparing the next element to be traced
    ///   2c. also, finish tracing the previous traceable, since we have reached this one already (dont want two tracing at the same time)
    /// 3. if this gameobject is on the border between beats, publish the beat we are entering
    /// </summary>
    /// <param name="other">A reference to the collider we interact with</param>
    public void TriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Baton_Tip")
        {
            if (isFirst) // 1
            {
                foreach (Tracing trace in gestureTrace.GetComponentsInChildren<Tracing>())
                {
                    trace.ResetTrace();
                }
                StartTracing();
            }
            if(!tracedThrough && ReadyToTrace(gestureTrace.traceDifficulty)) // 2
            {
                Tracing nextTrace = gestureTrace.GetNextTraceable(traceableIndex);
                if (nextTrace != null)
                {
                    if(nextTrace.isFirst) // 2a
                    {
                        Debug.Log("resetting first trace");
                        nextTrace.ResetTrace();
                    } else //2b
                    {
                        nextTrace.StartTracing();
                    }
                }
                Tracing prevTrace = gestureTrace.GetPreviousTraceable(traceableIndex);
                if(prevTrace != null) //2c
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

    /// <summary>
    /// Whenever the baton leaves a segment, that segment should stay "full" until the whole trace resets (back to first)
    /// </summary>
    /// <param name="other">the collider we interact with</param>
    public void TriggerExit(Collider other)
    {
        if (other.gameObject.name == "Baton_Tip" && canTrace == true)
        {
            FinishTracing();
            Debug.Log("Baton Collider Exit");
        }
    }

    /// <summary>
    /// Set a segment back to how it is when first instantiated
    /// </summary>
    public void ResetTrace()
    {
        //Debug.Log("resetting " + gameObject.name);
        Plane clipPlane = new Plane(transformTrace.up, defaultPoint);
        Vector4 plane = new Vector4(clipPlane.normal.x, clipPlane.normal.y, clipPlane.normal.z, clipPlane.distance);
        TracingMaterial.SetVector("_TracingPlane", plane);
        tracedThrough = false;
    }

    /// <summary>
    /// Stop segment from actively tracing and put in a "finished" state, do the same for all "previous" segments
    /// </summary>
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

    /// <summary>
    /// recursive function that checks whether the n previous segments have been started,
    /// because sometimes the the user will skip segments. the greater n is, the more
    /// segments the user can skip.
    /// </summary>
    /// <param name="difficulty">accumulator, called at first with traceDifficulty from GestureTrace.cs</param>
    /// <returns></returns>
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
