using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * This script allows displaying a sprite over a transform
 * either when a button is pressed or automatically after a time dely
 * or enabling the button after a time delay
 * 
 * This is a singleton and requires a SpriteRenderer component
 */



[RequireComponent(typeof(SpriteRenderer))]
public class HintIndicator : Singleton<HintIndicator>
{
    // the sprite renderer component attached to a gameobject
    private SpriteRenderer spriteRenderer;

    // where the hint will be displayed
    private Transform hintLocation;

    // the coroutine that that delays before autohint
    private Coroutine autoHintCR;

    // UI button component that displays a hint when pressed
    [SerializeField]
    private Button hintButton;

    
    // how many seconds should we wait before offering a hint
    [SerializeField]
    private float delayBeforeAutoHint;

    // disable by default get sprite renderer
    protected override void Init()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
        hintButton.interactable = false;
    }
    // force the hint to dispaly on desired transform
    public void IndicateHint(Transform hintLocation)
    {
        transform.position = hintLocation.position;
        spriteRenderer.enabled = true;
    }
    // remove the hint and interrupt whatever coroutine might be running
    public void CancelHint()
    {
        spriteRenderer.enabled = false;
        hintButton.interactable = false;
        
        if (autoHintCR != null)
            StopCoroutine(autoHintCR);
        autoHintCR = null;

    }
    // enable the hint button
    public void EnableHintButton()
    {
        hintButton.interactable = true;

    }
    // start a coroutine that will wait before offering the hint
    public void StartAutoHint(Transform hintLocation)
    { 
        this.hintLocation = hintLocation;

        autoHintCR = StartCoroutine(WaitAndIndicateHint());
    
    }
    // the coroutine that waits before dispalying the hint
    private IEnumerator WaitAndIndicateHint()
    {
        yield return new WaitForSeconds(delayBeforeAutoHint);
        EnableHintButton();
        
        //IndicateHint(hintLocation);
    }


}
