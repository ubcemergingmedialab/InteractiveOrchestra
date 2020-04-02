using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// This script holds a list of operations that fires when any button is activated.
/// </summary>
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

    /// <summary>
    /// Activates or disables target gameobject.
    /// </summary>
    public void ToggleGameobject(GameObject go)
    {
        // The operation to fire
        go.SetActive(!go.activeSelf);

        // Set the button state
        buttonStateScript.ToggleButtonState();
    }
}