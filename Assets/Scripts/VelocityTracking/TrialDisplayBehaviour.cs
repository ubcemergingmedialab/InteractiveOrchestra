using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// This script handles display relevant information during conducting sample data collection.
/// </summary>
public class TrialDisplayBehaviour : MonoBehaviour {

    private Text textComponent;

    void Start () {
        textComponent = GetComponent<Text>();
        Debug.Log("Got textComponent");
    }
    /// <summary>
    /// Change display text of the current trial that is being recorded
    /// </summary>
    /// <param name="trialNumber"> Current trial number </param>
    /// <param name="BPM"<> Current BPM being recorded </param>
    /// <param name="size"> Current gesture size being measure </param>
    public void ChangeTrial(int trialNumber,string BPM, string size)
    {
        textComponent.text = "Trial: " + (trialNumber - 1) + " of " +"20" + "\n" + "BPM: " + BPM + " Size: " + size ;
    }

    /// <summary>
    /// Display recording text
    /// </summary>
    public void DisplayRecordScreen()
    {
        textComponent.text = "Data has been recorded!"+"\n" + "Thank you very much!";
        Debug.Log("data has been recorded");
    }

    /// <summary>
    /// Changes display values with new ConductorSample input.
    /// <param name="sample"> The sample in which the display values are being used to update.</param>
    /// </summary>
    public void UpdateValuesWithConductorSample(OVRVelocityTracker.ConductorSample sample)
    {
        textComponent.text = "Trial:" + sample.trial + "\n" +
        "ID:" + sample.id + "\n" +
        "VelocityVector:" + sample.velocityVector+"\n" +
        "Position:" + sample.position +"\n" +
        "VelocityMagnitude:" + sample.velocityMagnitude + "\n" +
        "TimeRelativeToPrep:" + sample.timeRelativeToPrep +"\n" +
        "AngleToBP1:" + sample.angleToBP1 + "\n" +
        "Acceleration:" + sample.acceleration +"\n" +
        "DistanceCoveredSoFar:" + sample.distanceCoveredSoFar +"\n" +
        "GestureSize:" + sample.gestureSize;
    }

}
