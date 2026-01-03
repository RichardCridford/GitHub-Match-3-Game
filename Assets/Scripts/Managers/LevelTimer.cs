using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelTimer : MonoBehaviour
{
    private GameManager gm;
    
    
    [SerializeField] 
    private Text timerText;

    private float timeRemainging;
    private string timeAsString;

    private bool counting;

    private void Start()
    {
        gm = GameManager.Instance;
    }



    public void SetTimer(float t)
    {
        StopAllCoroutines();
        timeRemainging = t;
        UpdateText();
    }

    private void UpdateText()
    {
        timeAsString = (int)timeRemainging / 60 + " : ";
        timeAsString += timeRemainging % 60 < 10 ? "0" : "";
        timerText.text = timeAsString + (int) timeRemainging % 60;
    }

    public IEnumerator Countdown()
    { 
        counting = true;
        do
        { 
            timeRemainging -= Time.deltaTime;
            UpdateText() ;
            yield return null;
        }
        while (timeRemainging > 0);

        counting = false;

        gm.GameOver();
    }

}
