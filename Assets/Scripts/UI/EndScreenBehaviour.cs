using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script holds the features of the display at the end of a conducted song.
/// </summary>
public class EndScreenBehaviour : MonoBehaviour
{
    #region Variables
    [SerializeField] private Sprite victoryScreen;
    [SerializeField] private Sprite failScreen;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject GestureRelatedObjects;
    #endregion

    private void Awake()
    {
        TempoController.PieceStop += TriggerEndMenu;
    }

    /// <summary>
    /// Event that spawns the end Menu
    /// </summary>
    /// <param name="dummyParam"> Don't worry about it </param>
    public void TriggerEndMenu(float dummyParam)
    {
        animator.SetBool("ExperienceEnded",true);
        GestureRelatedObjects.SetActive(false);
    }

    /// <summary>
    /// Sets animation bool to false to reverse end screen animation
    /// </summary>
    public void TurnOffMenu()
    {
        animator.SetBool("ExperienceEnded", false);
        OVRGestureHandle.songOver = false;
    }
}

