using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]

public class Cursor : Singleton<Cursor>
{
    private SpriteRenderer spriteRenderer;

    private Matchable[] selected;
    
    protected override void Init()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        //Cursor sprite is off initially (only shows when selecting matchables)
        spriteRenderer.enabled = false;

        selected = new Matchable[2];
    }

    public void SelectFirst(Matchable toSelect)
    {
        // put the matchable selected into the array 
        selected[0] = toSelect;

        // move the cursor to the position of the selected matchable
        transform.position = toSelect.transform.position;

        // turn the cursor sprite on
        spriteRenderer.enabled = true;
    
    }


}
