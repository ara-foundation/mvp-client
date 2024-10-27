using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EventController : MonoBehaviour
{
    private int index;
    private string actualText = "";

    public TextMeshProUGUI logTextBox;
    public Event actualEvent;
    public PauseInfo pauseInfo;

    private Action OnEndCallback;
    private Coroutine runningCoroutine;

    public void StartTexting(Event eventToText, Action onEndCallback)
    {
        actualEvent = eventToText;
        index = 0;
        logTextBox.text = "";
        actualText = "";
        OnEndCallback = onEndCallback;
        ReproduceText();
    }

    public void StopTexting()
    {
        if (runningCoroutine != null)
        {
            StopCoroutine(runningCoroutine);
            runningCoroutine = null;
            OnEndCallback?.Invoke();
        }
    }

    public bool IsTexting()
    {
        return runningCoroutine != null;
    }

    private void ReproduceText()
    {
        runningCoroutine = null;
        //if not readied all letters
        if (index < actualEvent.finalText.Length)
        {
            //get one letter
            char letter = actualEvent.finalText[index];

            //Actualize on screen
            logTextBox.text = Write(letter);

            //set to go to the next
            index += 1;
            runningCoroutine = StartCoroutine(PauseBetweenChars(letter));
        } else
        {
            OnEndCallback?.Invoke();
        }
    }

    private string Write(char letter)
    {
        actualText += letter;
        return actualText;
    }

    private void OnDisable()
    {
        if (runningCoroutine != null)
        {
            StopCoroutine(runningCoroutine);
            runningCoroutine = null;
        }
    }

    private IEnumerator PauseBetweenChars(char letter)
    {
        switch (letter)
        {
            case '.':
                yield return new WaitForSeconds(pauseInfo.dotPause);
                ReproduceText();
                yield break;
            case '!':
                yield return new WaitForSeconds(pauseInfo.dotPause);
                ReproduceText();
                yield break;
            case '?':
                yield return new WaitForSeconds(pauseInfo.dotPause);
                ReproduceText();
                yield break;
            case ',':
                yield return new WaitForSeconds(pauseInfo.commaPause);
                ReproduceText();
                yield break;
            case ' ':
                yield return new WaitForSeconds(pauseInfo.spacePause);
                ReproduceText();
                yield break;
            default:
                yield return new WaitForSeconds(pauseInfo.normalPause);
                ReproduceText();
                yield break;
        }
    }
}

[Serializable]
public class PauseInfo
{
    public float dotPause;
    public float commaPause;
    public float spacePause;
    public float normalPause;
}