using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This class will draw the cursor where it needs to be and handle input processing
 * This class is a Singleton so any other sclass can get a reference to this through instance
 * It requires a sprite renderer to be used with a 9 slice 1 Unity unit in size
 * 
 */

[RequireComponent(typeof(SpriteRenderer))]

public class Cursor : Singleton<Cursor>
{
    public bool cheatMode;

    private MatchablePool pool;
    private MatchableGrid grid;

    private SpriteRenderer spriteRenderer;

    // which 2 matchables are currently selected?
    private Matchable[] selected;

   // variables to stretch the size of the cursor
   [SerializeField] private Vector2Int verticalStretch = new Vector2Int (1, 2);
   [SerializeField] private Vector2Int horizontalStretch = new Vector2Int (2, 1);

   
   // offset variables to adjust how the cursor looks onscreen
   // Vector3 with .direction is 1 in game unit
   [SerializeField]
   private Vector3  halfUp      = Vector3.up    / 2,
                    halfDown    = Vector3.down  / 2,
                    halfLeft    = Vector3.left  / 2,
                    halfRight   = Vector3.right / 2;


    protected override void Init()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        //Cursor sprite is off initially (only shows when selecting matchables)
        spriteRenderer.enabled = false;

        selected = new Matchable[2];
    }

    private void Start()
    {
        pool = (MatchablePool)MatchablePool.Instance;
        grid = (MatchableGrid)MatchableGrid.Instance;
    }

    private void Update()
    {
        if (!cheatMode || selected[0] == null)
            return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            pool.ChangeType(selected[0], 0);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            pool.ChangeType(selected[0], 1);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            pool.ChangeType(selected[0], 2);

        if (Input.GetKeyDown(KeyCode.Alpha4))
            pool.ChangeType(selected[0], 3);

        if (Input.GetKeyDown(KeyCode.Alpha5))
            pool.ChangeType(selected[0], 4);

        if (Input.GetKeyDown(KeyCode.Alpha6))
            pool.ChangeType(selected[0], 5);

        if (Input.GetKeyDown(KeyCode.Alpha7))
            pool.ChangeType(selected[0], 6);

        if (Input.GetKeyDown(KeyCode.Alpha8))
            pool.ChangeType(selected[0], 7);



    }
    // select the list of 2 mathcbales, move the cursor to it, reset the size, activate the sprite
    public void SelectFirst(Matchable toSelect)
    {
        // put the matchable selected into the array 
        selected[0] = toSelect;

        //Allows the player to deselect
        if (!enabled || selected[0] == null)
            return;

        // move the cursor to the position of the selected matchable
        transform.position = toSelect.transform.position;

        // sets the cursor to it's original size
        spriteRenderer.size = Vector2.one;

        // turn the cursor sprite on
        spriteRenderer.enabled = true;
    
    }

    public void SelectSecond(Matchable toSelect)
    {
        selected[1] = toSelect;

        // Used for deselection and makes sure moving matchables can't be selected
        if (!enabled || selected[0] == null || selected[1] == null || !selected[0].Idle || !selected[1].Idle || selected[0] == selected[1])
            return;

        if (SelectedAreAdjacent())
            StartCoroutine(grid.TrySwap(selected));
            
            //print("Swapping matchables at positions : (" + selected[0].position.x + ", " + selected[0].position.y + " ) and ( " + selected[1].position.x + ", " + selected[1].position.y + ")");

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
                transform.position += halfDown;
                return true;

            }
            else if (selected[0].position.y == selected[1].position.y - 1)
            {
                spriteRenderer.size = verticalStretch;
                transform.position += halfUp;
                return true;
            }

        }
        else if (selected[0].position.y == selected[1].position.y)
        {

            if (selected[0].position.x == selected[1].position.x + 1)
            {
                spriteRenderer.size = horizontalStretch;
                transform.position += halfLeft;
                return true;

            }
            else if (selected[0].position.x == selected[1].position.x - 1)
            {
                spriteRenderer.size = horizontalStretch;
                transform.position += halfRight;
                return true;
            }
        }
        
        return false;
    }

}
