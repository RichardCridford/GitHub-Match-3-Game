using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
public class HintIndicator : Singleton<HintIndicator>
{
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private Button hintButton;

    protected override void Init()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
        hintButton.interactable = false;
    }
    public void IndicateHint(Transform hintLocation)
    { 
        transform.position = hintLocation.position;
        spriteRenderer.enabled = true;
    }
    public void CancelHint()
    { 
        spriteRenderer.enabled = false;
        hintButton.interactable = false;

    }
    public void EnableHintButton()
    {
        hintButton.interactable = true; 

    }


}
