using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BPMCircleOscillate : MonoBehaviour {

    private float timeCounter;
    private float speed;
    private float width;
    private float height; 

    // Use this for initialization
    void Start () {
        speed = 5;
        width = 4;
        height = 7;
    }
	
	// Update is called once per frame
	void Update () {
        timeCounter += Time.deltaTime * speed;

        float x = Mathf.Cos(timeCounter) * width;
        float y = Mathf.Sin(timeCounter) * height;
        float z = 10;

        transform.position = new Vector3(x, y, z);
    } 
}
