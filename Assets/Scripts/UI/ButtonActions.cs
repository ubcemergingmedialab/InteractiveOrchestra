/*

This holds a list of operations that fires when a button is activated
 
*/

using Sirenix.OdinInspector;
using UnityEngine;

public class ButtonActions : MonoBehaviour
{
    [InfoBox("If this is empty, script will look for the first ButtonState script it finds on this Gameobject")]
    public ButtonState buttonStateScript;

    void Start()
    {
        if (!buttonStateScript)
        {
            buttonStateScript = GetComponent<ButtonState>();
        }
    }

    // Activates or disables target gameobject
    public void ToggleGameobject(GameObject go)
    {
        // The operation to fire
        go.SetActive(!go.activeSelf);

        // Set the button state
        buttonStateScript.ToggleButtonState();
    }
}