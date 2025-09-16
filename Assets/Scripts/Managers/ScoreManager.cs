using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/*
 * This class will manage the player's score and resolving matches.
 * 
 * This is a Singleton, only one exists in a scene and can be accessed through Instance
 * 
 * This is attached to a Text UI element 
 */



[RequireComponent(typeof(Text))] 
public class ScoreManager : Singleton<ScoreManager>
{
    private MatchablePool pool;
    private MatchableGrid grid;
    private AudioMixer audioMixer;

    [SerializeField]
    private Transform collectionPoint;

    // UI element for displaying the score and combo multiplier
    [SerializeField]
    private Text scoreText,
                 comboText;

    // UI slider for displaying the time reamining in the combo
    [SerializeField]
    private Slider comboSlider;

    // actual score, and a combo multiplier
    private int score, 
                comboMultiplier;

    //getter for score
    public int Score
    {
        get 
        {
            return score;
        }
    
    }

    // how much time since the player last scored?
    private float timeSinceLastScore;

    // how much time should we allow before resetting the combo multiplier? 
    [SerializeField]
    private float   maxComboTime,
                    currentComboTime;

    // is the combo timer currently running?
    private bool timerIsActive;


    private void Start()
    {
        pool = (MatchablePool) MatchablePool.Instance;
        grid = (MatchableGrid) MatchableGrid.Instance;
        audioMixer = AudioMixer.Instance;

        comboText.enabled = false;
        comboSlider.gameObject.SetActive(false);
    }

    // add an amount to the score and update the UI Text
    public void AddScore(int amount)
    {
        score += amount * IncreaseCombo();
        scoreText.text = score.ToString();

        timeSinceLastScore = 0;

        if (!timerIsActive)
            StartCoroutine(ComboTimer());

        // play score sound
        audioMixer.PlaySound(SoundEffects.score);
    }

    // Combo timer coroutine, counts up to max combo time before resetting the combo multiplier
    private IEnumerator ComboTimer()
    {
        timerIsActive = true;
        comboText.enabled = true;
        comboSlider.gameObject.SetActive(true);

        do
        {
            timeSinceLastScore += Time.deltaTime;
            comboSlider.value = 1 - timeSinceLastScore / currentComboTime ;
            yield return null;
        }
        while (timeSinceLastScore < currentComboTime);

        comboMultiplier = 0;
        comboText.enabled = false;
        comboSlider.gameObject.SetActive(false);

        timerIsActive = false;

    }

    private int IncreaseCombo()
    {
        comboText.text = "Combo X" + ++comboMultiplier;

        // the piece of math to help with the combo multiplier getting quicker each time.
        currentComboTime = maxComboTime - Mathf.Log(comboMultiplier) / 2;
        
        return comboMultiplier;
    
    }

    // coroutine for resolving a match
    public IEnumerator ResolveMatch(Match toResolve, MatchType powerupUsed = MatchType.invalid)
    {
        Matchable powerupFormed = null;
        Matchable matchable;

        Transform target = collectionPoint;

        // if no powerup was used to trigger this match and a larger match is made, create a powerup
        if (powerupUsed == MatchType.invalid && toResolve.Count > 3)
        {
            powerupFormed = pool.UpgradeMatchable(toResolve.ToBeUpgraded, toResolve.Type);

            toResolve.RemoveMatchable(powerupFormed);

            target = powerupFormed.transform;

            powerupFormed.SortingOrder = 3;

            // play upgrade sound
            audioMixer.PlaySound(SoundEffects.upgrade);
        }
        else
        {
            // play resolve sound
            audioMixer.PlaySound(SoundEffects.resolve);
        }


        // iterate through every matchable in a match
        for (int i = 0; i != toResolve.Count; ++i)
            {
                matchable = toResolve.Matchables[i];

                // only allow games used as powerups to resolve gems
                // it can stay on the grid until the player decides to use it
                if (powerupUsed != MatchType.match5 && matchable.isGem)
                    continue;



                // remove the matchables from the grid 
                grid.RemoveItemAt(matchable.position);

                // move them off to the side of the screen 
                // and wait for the last one to finish
                if (i == toResolve.Count - 1)
                    yield return StartCoroutine(matchable.Resolve(target));

                else
                    StartCoroutine(matchable.Resolve(target));

            }

        // update the player's score
        // This algorithm Will allow bigger matches to be worth more points
        AddScore(toResolve.Count * toResolve.Count);

        // if there was a powerup, reset the sorting order
        if (powerupFormed != null)
        {
            powerupFormed.SortingOrder = 1;
        }

        yield return null;    
    }
}
