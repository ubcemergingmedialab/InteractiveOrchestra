using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
//using ../VelocityTracking/VelocityTracker.ConductorSample;

/// <summary>
/// Updates relevant data being tracked using the controller. This data is displayed and updated dynamically in the inspector and is to be used by team to view the data
/// during screen capture (recording of the gestures) to track gesture points of the user and map to corresponding data.
/// </summary>

public class ControllerDataUpdater : MonoBehaviour
{

    #region Variables
    [SerializeField]
    public string id;
    [SerializeField]
    public string region;
    [SerializeField]
    public Vector3 velocityVector;
    [SerializeField]
    public Vector3 controllerPosition;
    [SerializeField]
    public float velocityMagnitude;
    [SerializeField]
    public float timeRelativeToPrep;
    [SerializeField]
    public float angleToBP1;
    [SerializeField]
    public float distanceCoveredSoFar;
    [SerializeField]
    public float acceleration;
    // Preset variables:
    [SerializeField]
    public char gestureSize;

    [SerializeField]
    public int BPM;

    private Text currentDisplayText;
  

    // private Editor editor;
    // TODO need to define the gesturze variable.
    // private GestureSize gestureSize;
    #endregion

    #region Unity functions

    /// <summary>
    /// initialize all static and dynamic controller variables to be collected at beginning of the VR experience. 
    /// </summary>

    private void Awake()
    {
        id = "";
        region = "";
        velocityVector = new Vector3();
        controllerPosition = new Vector3();
        velocityMagnitude = 0;
        timeRelativeToPrep = 0;
        angleToBP1 = 0;
        distanceCoveredSoFar = 0;
        acceleration = 0;
        gestureSize = 'S';
        BPM = 80;
        currentDisplayText = GetComponent<Text>();
        // CustomWindow.Initialize(this);
    }

    /// <summary>
	/// Update is called once per frame. It is here in case it is needed to update any of the variables every frame in the future. Keep for future use potentially.
	/// </summary>

    private void Update()
    {
        //velocityMagnitude
        if (Input.GetKeyUp("4")) velocityMagnitude = 5;
        if (Input.GetKeyUp("5")) velocityMagnitude = 6;
        if (Input.GetKeyUp("6")) velocityMagnitude = 7;
        if (Input.GetKeyUp("7")) velocityMagnitude = 8;
        if (Input.GetKeyUp("8")) velocityMagnitude = 9;
        if (Input.GetKeyUp("9")) velocityMagnitude = 0;
        // gestureVelocity = 2; //get gestureVelocity from ConductorSample structs somewhere to display here
    }
    #endregion

    #region Class functions
    /// <summary>
    /// This function is used to update only the controller variables that are updated dynamically. Use this function if you want to manually update the dynamic variables by specifying each variable.
    /// </summary>
    /// <param name="gestureVelocity"> current velocity of the gesture at a particular data point </param>
    /// <param name="acceleration"> current acceleration of the baton at a particular data point </param>
    /// <param name="positionVector"> current xyz coordinates of the occulus controller (i.e. baton) in the world space </param>
    /// <param name="directionVector"> current direction coordinates of the occulus controller (i.e. baton) in the world space </param>
    /// <param name="distanceCoveredP1"> amount of distance (from full trial) that the baton has covered in the P1 region</param>
    /// <param name="distanceCoveredP2"> amount of distance (from full trial) that the baton has covered in the P2 region </param>
    /// <param name="timeRelativeToPrep"> Time runner starting from the beginning of the prep. beat to current time </param>
    /// <param name="region"> "" if current data point is in neither region. Otherwise, either "P1" or "P2". </param>
    /// <param name="velocityMagnitude"> Magnitude of the velocity </param>
    /// <param name="angleToBP1"> Angle to point BP1 on the plane relative to horizontal plane </param>


    public void updateDynamicValues(float gestureVelocity, float acceleration, Vector3 positionVector,
        Vector3 directionVector, float distanceCoveredP1, float distanceCoveredP2, float timeRelativeToPrep, string region, float velocityMagnitude,
        float angleToBP1)
    {

        //this.gestureVelocity = gestureVelocity;
        //this.acceleration = acceleration;
        //this.positionVector = positionVector;
        //this.directionVector = directionVector;
        //this.distanceCoveredP1 = distanceCoveredP1;
        //this.distanceCoveredP2 = distanceCoveredP2;
        //this.timeRelativeToPrep = timeRelativeToPrep;
        //this.region = region;
        //this.velocityMagnitude = velocityMagnitude;
        //this.angleToBP1 = angleToBP1;
        //this.displayValuesInConsole();

    }

    /// <summary> 
    /// This function is used to update the two preset static values in the inspector. Can be used to manually update the values by providing the direct values
    /// </summary>
    /// <param name="BPM"> preset static BPM of the song </param>
    /// <param name="gestureSize"> preset static size of the gesture being used for the VR </param> // TODO need to add this variable to this function

    public void updateStaticValues(int BPM, char gestureSize) //TODO need to add gesture size parameter for this function
    {
        this.BPM = BPM;
        this.gestureSize = gestureSize;
        this.displayValuesInConsole();
    }

    /// <summary>
    /// This function is used to update SOME of the controller collected variables in the inspector using an instance of a ConductorSample as an input. 
    /// It also displays all the updated values onto the console using the displayValuesInConsole() local function.
    /// </summary>
    /// <param name="sample"> an instance of a ConductorSample object with its relevant variables </param>
    /// <returns> the input ConductorSample object for reference and for use where this function is called </returns>

    public void updateValuesWithConductorSample(OVRVelocityTracker.ConductorSample sample)
    {
        //this.directionVector = sample.direction;
        //this.positionVector = sample.position;
        //this.gestureVelocity = sample.velocity;
        //this.timeRelativeToPrep = sample.time;
        //this.BPM = sample.beat;
        //this.displayValuesInConsole();
        //return sample;
        // Debug.Log("=========================");
        this.id = sample.id;
        // Debug.Log(this.id);
        this.velocityVector = sample.velocityVector;
        this.controllerPosition = sample.position;
        this.velocityMagnitude = sample.velocityMagnitude;
        this.timeRelativeToPrep = sample.timeRelativeToPrep;
        this.angleToBP1 = sample.angleToBP1;
        this.acceleration = sample.acceleration;
        this.distanceCoveredSoFar = sample.distanceCoveredSoFar;
       // this.gestureSize = sample.gestureSize;
        this.BPM = sample.BPM;
        if (currentDisplayText == null)
        {
            currentDisplayText = GetComponent<Text>();
        }
        else
        {

            currentDisplayText.text = "Test";
        }
        
    }

    /// <summary>
    /// This function is used to reinitialize the values to the default starting values specified in the Awake() method.
    /// </summary>
    public void reInitializeValues()
    {
        this.Awake();
    }

    /// <summary>
    /// displays all the variables in this Class into the console for reference to the user in a neat format that is easy to comprehend.
    /// </summary>
    /// <returns> void </returns>

    public void displayValuesInConsole()
    {
        //Debug.Log("Gesture Velocity = " + this.gestureVelocity);
        //Debug.Log("Gesture Acceleration = " + this.acceleration);
        //Debug.Log("Position Vector = " + this.positionVector);
        //Debug.Log("Direction Vector = " + this.directionVector);
        //Debug.Log("Distance covered in P1 = " + this.distanceCoveredP1);
        //Debug.Log("Distance covered in P2 = " + this.distanceCoveredP2);
        //Debug.Log("Time relative to Prep beat = " + this.timeRelativeToPrep);
        //Debug.Log("Preset BPM = " + this.BPM);
        //Debug.Log("Preset gesture = " + ""); // TODO need to add logging gesturSize value to the console
    }

    public void reRenderOnCustomWindow()
    {

    }
    #endregion
}
