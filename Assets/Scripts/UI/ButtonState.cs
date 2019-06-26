/*

This manages the button state of a gameobject that represents a button. It can be updated manually but it should really
be updated by way of the ButtonActions.cs

It is used to keep track of the state of the button and update button visuals

This needs the Renderer to be on the same gameobject

! You should be using VRTK_InteractableObject to easily turn a gameobject into a button BUT it currently forces a color change
(an obsolete feature that'll be removed in future versions of VRTK). Make sure the 'highlighting' sections of VRTK_InteractableObject 
is commented out
 
*/


using Sirenix.OdinInspector;
using UnityEngine;

public class ButtonState : MonoBehaviour
{
    // We don't want to directly change the state of the button because we want something to happen when the state is changed
    [ReadOnly]
    public bool currentState;

    // Sometimes buttons should already be in an activated state at runtime. This helps to set what the starting state should be
    public bool startingState;

    // We're swapping the material instead of using VRTK's Object Highlighter because we may want to do more with the visuals than to change color
    public Material activatedState, deactivatedState;

    private Renderer buttonRenderer;

    void Start()
    {
        buttonRenderer = GetComponent<Renderer>();

        currentState = startingState;
        UpdateMaterialBasedOnState();
    }

    public void ToggleButtonState()
    {
        currentState = !currentState;
        UpdateMaterialBasedOnState();
    }

    // It might be useful in certain edge cases to directly update the state to what we want
    public void ToggleButtonState(bool state)
    {
        currentState = state;
        UpdateMaterialBasedOnState();
    }

    private void UpdateMaterialBasedOnState()
    {
        if (currentState)
        {
            buttonRenderer.material = activatedState;
        }

        else
        {
            buttonRenderer.material = deactivatedState;
        }
    }
}