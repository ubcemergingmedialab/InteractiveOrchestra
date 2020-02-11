using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class Tracing : MonoBehaviour
{

    public GameObject batonTip;
    bool isTracing = false;
    Material TracingMaterial;
    Vector3 planePoint;
    Vector3 defaultPoint;
    // Start is called before the first frame update
    void Start()
    {
        TracingMaterial = GetComponent<Renderer>().material;
        defaultPoint = new Vector3(-999, -999, -999);
        planePoint = defaultPoint;
    }

    // Update is called once per frame
    void Update()
    {
        if(isTracing)
        {
            planePoint = batonTip.transform.position;
            Plane clipPlane = new Plane(transform.up, planePoint);
            Vector4 plane = new Vector4(clipPlane.normal.x, clipPlane.normal.z, clipPlane.normal.z, clipPlane.distance);
            TracingMaterial.SetVector("_TracingPlane", plane);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Baton_Tip")
        {
            isTracing = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
       if(other.gameObject.name == "Baton_Tip")
        {
            Debug.Log("Baton Collide Exit");
            planePoint = defaultPoint;
            isTracing = false;
        }
    }
}
