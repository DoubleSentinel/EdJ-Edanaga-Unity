﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TMPro
{
    public enum Emotion { happy, sad, surprised, angry };
    [System.Serializable] public class EmotionEvent : UnityEvent<Emotion> { }

    [System.Serializable] public class ActionEvent : UnityEvent<string> { }

    [System.Serializable] public class TextRevealEvent : UnityEvent<char> { }

    [System.Serializable] public class DialogueEvent : UnityEvent { }

    /// <summary>
    /// Class derivated from TextMeshProUGUI used to add the ability to display text with a typewriter effect and add more tag for action and emotion
    /// </summary>
    public class TextMeshProAnimated : TextMeshProUGUI
    {
        [SerializeField] private float speed = 50;
        public EmotionEvent onEmotionChange;
        public ActionEvent onAction;
        public TextRevealEvent onTextReveal;
        public DialogueEvent onDialogueFinish;

        // Helper for text skip
        private string currentText = "";
        private bool isInCoroutine = false;

        public string[] ParseText(string text)
        {
            this.text = string.Empty;
            // split the whole text into parts based off the <> tags 
            // even numbers in the array are text, odd numbers are tags
            string[] subTexts = text.Split('<', '>');

            // textmeshpro still needs to parse its built-in tags, so we only include noncustom tags
            string displayText = "";
            for (int i = 0; i < subTexts.Length; i++)
            {
                if (i % 2 == 0)
                    displayText += subTexts[i];
                else if (!isCustomTag(subTexts[i].Replace(" ", "")))
                    displayText += $"<{subTexts[i]}>";
            }
            // check to see if a tag is our own
            bool isCustomTag(string tag)
            {
                return tag.StartsWith("speed=");
            }
            
            // send that string to textmeshpro and hide all of it, then start reading
            this.text = displayText;
            maxVisibleCharacters = 0;
            return subTexts;
        }

        public void ReadText(string newText)
        {
            // Display error if the newText is empty
            if (newText.Trim() == "") newText = "ERROR This text should not be seen !";
            currentText = newText;
            StartCoroutine(Read(ParseText(newText)));
        }

        IEnumerator Read(string[] subTexts)
        {
            isInCoroutine = true;

            int subCounter = 0;
            int visibleCounter = 0;
            while (subCounter < subTexts.Length)
            {
                // if 
                if (subCounter % 2 == 1)
                {
                    yield return EvaluateTag(subTexts[subCounter].Replace(" ", ""));
                }
                else
                {
                    while (visibleCounter < subTexts[subCounter].Length)
                    {
                        onTextReveal.Invoke(subTexts[subCounter][visibleCounter]);
                        visibleCounter++;
                        maxVisibleCharacters++;
                        yield return new WaitForSeconds(1f / speed);
                    }
                    visibleCounter = 0;
                }
                subCounter++;
            }
            isInCoroutine = false;
            yield return null;

            WaitForSeconds EvaluateTag(string tag)
            {
                if (tag.Length > 0)
                {
                    if (tag.StartsWith("speed="))
                    {
                        speed = float.Parse(tag.Split('=')[1]);
                    }
                    else if (tag.StartsWith("pause="))
                    {
                        return new WaitForSeconds(float.Parse(tag.Split('=')[1]));
                    }
                    else if (tag.StartsWith("emotion="))
                    {
                        onEmotionChange.Invoke((Emotion)System.Enum.Parse(typeof(Emotion), tag.Split('=')[1]));
                    }
                    else if (tag.StartsWith("action="))
                    {
                        onAction.Invoke(tag.Split('=')[1]);
                    }
                }
                return null;
            }
            onDialogueFinish.Invoke();
        }

        private void Update()
        {
            // Allow to skip the animation of the text
            if (Input.GetKeyDown(KeyCode.Space) && isInCoroutine)
            {
                isInCoroutine = false;
                StopAllCoroutines();
                maxVisibleCharacters = currentText.Length;

                // Wait for next frame to avoid skipping a text too early
                StartCoroutine(InvokeFinish());
                IEnumerator InvokeFinish()
                {
                    yield return new WaitForEndOfFrame();
                    onDialogueFinish.Invoke();
                }
            }
        }
    }
}