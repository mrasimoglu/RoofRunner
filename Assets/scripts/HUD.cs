using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ActionTypes {

    public static readonly ActionTypes JUMP = new ActionTypes("Space", "Jump");
    public static readonly ActionTypes Vault = new ActionTypes("Space", "Vault");
    public static readonly ActionTypes HoldLadder = new ActionTypes("F", "Hold Ladder");
    public static readonly ActionTypes HoldWall = new ActionTypes("F", "Hold Edge Of The Wall");
    public static readonly ActionTypes SlideonLadder = new ActionTypes("Space", "Slide On The Ladder");
    public static readonly ActionTypes Slide = new ActionTypes("Ctrl", "Slide");

    public string Key;
    public string description;

    ActionTypes(string key, string desc) => (Key, description) = (key, desc);
}
public class HUD : MonoBehaviour
{
    public Text text;
    public Text textCoin;
    public Text gameFinished;
    public Text youaredeadmyfriend;
    public Image img;
    public Button restartgame;

    public void Start()
    {
        textCoin.text = "Coins : 0";
        gameFinished.enabled = false;
        restartgame.gameObject.SetActive(false);
        youaredeadmyfriend.enabled = false;

        StartCoroutine(FadeImage(true));
    }
    public void setAction(ActionTypes a)
    {
        if (a == ActionTypes.HoldWall)
            text.text = "First Jump then press " + a.Key + " for hold.";
        else
            text.text = "Press " + a.Key + " For " + a.description;
    }

    public void restart()
    {
        SceneManager.LoadScene("SampleScene2");
    }


    public void clearAction()
    {
        text.text = "";
    }

    public void setCoinCount(int count)
    {
        textCoin.text = "Coins : " + count;
    }

    public void GameFinish()
    {
        textCoin.enabled = false;
        text.enabled = false;
        gameFinished.enabled = true;
        restartgame.gameObject.SetActive(true);
        StartCoroutine(FadeImage(false));
    }
    public void Dead()
    {
        youaredeadmyfriend.enabled = true;
        textCoin.enabled = false;
        text.enabled = false;
        restartgame.gameObject.SetActive(true);
        StartCoroutine(FadeImage(false));
        
    }




    IEnumerator FadeImage(bool fadeAway)
    {
        // fade from opaque to transparent
        if (fadeAway)
        {
            // loop over 1 second backwards
            for (float i = 1; i >= 0; i -= Time.deltaTime)
            {
                // set color with i as alpha
                img.color = new Color(1, 1, 1, i);
                yield return null;
            }
        }
        // fade from transparent to opaque
        else
        {
            // loop over 1 second
            for (float i = 0; i <= 1; i += Time.deltaTime)
            {
                // set color with i as alpha
                img.color = new Color(1, 1, 1, i);
                yield return null;
            }
        }
    }
}
