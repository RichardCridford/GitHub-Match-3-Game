using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]

public class Cursor : Singleton<Cursor>
{
    private SpriteRenderer spriteRenderer;

    private Matchable[] selected;

    private Vector2Int verticalStretch = new Vector2Int(1, 2);
    private Vector2Int horizontalStretch = new Vector2Int(2, 1);

    
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

        //Allows the player to deselect
        if (!enabled || selected[0] == null)
            return;

        // move the cursor to the position of the selected matchable
        transform.position = toSelect.transform.position;

        // turn the cursor sprite on
        spriteRenderer.enabled = true;
    
    }

    public void SelectSecond(Matchable toSelect)
    {
        selected[1] = toSelect;

        // Used for deselection and makes sure moving matchables can't be selected
        if (!enabled || selected[0] == null || selected[1] == null || !selected[0].Idle || !selected[1].Idle)
            return;

        if(SelectedAreAdjacent())
        print("Swapping matchables at positions : (" + selected[0].position.x + ", " + selected[0].position.y + " ) and ( " + selected[1].position.x + ", " + selected[1].position.y + ")");

        // Used for deselection
        SelectFirst(null);

    }

    // Checking that matchables are next to each other so player can't swap matchables far away or on the diagonal 
    // Also changes cursor sprite size so it looks like it encapsualtes matchables
    private bool SelectedAreAdjacent()
    {
        if (selected[0].position.x == selected[1].position.x)
        {
            if (selected[0].position.y == selected[1].position.y + 1)
            {
                spriteRenderer.size = verticalStretch;
                return true;

            }
            else if (selected[0].position.y == selected[1].position.y - 1)
            {
                spriteRenderer.size = verticalStretch;
                return true;
            }

        }
        else if (selected[0].position.y == selected[1].position.y)
        {

            if (selected[0].position.x == selected[1].position.x + 1)
            {
                spriteRenderer.size = horizontalStretch;
                return true;

            }
            else if (selected[0].position.x == selected[1].position.x - 1)
            {
                spriteRenderer.size = horizontalStretch;
                return true;
            }
        }
        
        return false;
    }

}
