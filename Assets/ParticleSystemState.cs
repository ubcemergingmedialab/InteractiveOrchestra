using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemState : MonoBehaviour {
    
    private OVRVelocityTracker velocityTracker;
    public Vector3 speed;

	// Use this for initialization
	void Start () {

        speed.Set(0, velocityTracker.currentBPMToRecord, 0);
	}
	
	// Update is called once per frame
	void Update () {

        transform.Rotate(speed * Time.deltaTime);
		
	}
}
