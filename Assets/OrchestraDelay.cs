using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrchestraDelay : MonoBehaviour {

    private static OrchestraDelay instance;
    private float[] possibleOrchTimeDelays;
    private int maxIndex = 10;
    private int currentIndex = 0;
    private TextMesh displaytext;

    public static OrchestraDelay Instance {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        possibleOrchTimeDelays = new float[11];
        possibleOrchTimeDelays[0] = 0;
        possibleOrchTimeDelays[1] = 100;
        possibleOrchTimeDelays[2] = 200;
        possibleOrchTimeDelays[3] = 300;
        possibleOrchTimeDelays[4] = 400;
        possibleOrchTimeDelays[5] = 500;
        possibleOrchTimeDelays[6] = 600;
        possibleOrchTimeDelays[7] = 700;
        possibleOrchTimeDelays[8] = 800;
        possibleOrchTimeDelays[9] = 900;
        possibleOrchTimeDelays[10] = 1000;
        displaytext = GetComponent<TextMesh>();
        UpdateText();

        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Update display text on orchdelay component for delay time
    /// </summary>
    private void UpdateText()
    {
        string displayText = string.Format("{0}ms", possibleOrchTimeDelays[currentIndex]);
        displaytext.text = displayText;
    }

    /// <summary>
    /// Increase index of time delay to refer to 
    /// </summary>
    public void IncreaseIndex()
    {
        if(currentIndex <= (maxIndex - 1))
        {
            currentIndex++;
            UpdateText();
        }
        UpdateText();
    }

    /// <summary>
    /// Decrease index of time delay to refer to 
    /// </summary>
    public void DecreaseIndex()
    {
        if (currentIndex >= 1)
        {
            currentIndex--;
            UpdateText();
        }
    }

    /// <summary>
    /// Return current delay time chosen 
    /// </summary>
    /// <returns></returns>
    public float GetCurrentOrchDelay()
    {
        return possibleOrchTimeDelays[currentIndex];
    }
}
