using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TMPro
{
    public enum Emotion
    {
        happy,
        sad,
        surprised,
        angry
    };

    [System.Serializable]
    public class EmotionEvent : UnityEvent<Emotion>
    {
    }

    [System.Serializable]
    public class ActionEvent : UnityEvent<string>
    {
    }

    [System.Serializable]
    public class TextRevealEvent : UnityEvent<char>
    {
    }

    [System.Serializable]
    public class DialogueEvent : UnityEvent
    {
    }

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
        [HideInInspector]
        public bool isWriting = false;

        public string ParseText(string text)
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
                else
                    EvaluateTag(subTexts[i].Replace(" ", ""));
            }

            // check to see if a tag is our own
            bool isCustomTag(string tag)
            {
                return tag.StartsWith("speed=");
            }

            // send that string to textmeshpro and hide all of it, then start reading
            this.text = displayText;
            maxVisibleCharacters = 0;
            return string.Concat(subTexts);
        }

        public IEnumerator ReadPage(int pageNumber)
        {
            if (overflowMode == TextOverflowModes.Page)
            {
                isWriting = true;

                pageToDisplay = pageNumber;
                // the indexes are offset by one between what is displayed as a page
                // and what index values are saved in textInfo.pageInfo because TextMeshPro
                // starts displays at 0 and 1 for the first page.
                int numberOfCharactersToReveal = textInfo.pageInfo[pageNumber - 1].lastCharacterIndex -
                    textInfo.pageInfo[pageNumber - 1].firstCharacterIndex + 1;

                int offset = textInfo.pageInfo[pageNumber - 1].firstCharacterIndex;
                for (int character = offset; character <= numberOfCharactersToReveal + offset; character++)
                {
                    maxVisibleCharacters = character;
                    yield return new WaitForSeconds(1f / speed);
                }

                isWriting = false;
                yield return null;
            }
        }

        public IEnumerator ReadAllPages(UnityEvent unityEvent)
        {
            if (overflowMode == TextOverflowModes.Page)
            {
                for (int page = 1; page <= textInfo.pageCount; page++)
                {
                    pageToDisplay = page;
                    yield return ReadPage(page);
                }

                yield return null;
            }
        }

//        IEnumerator Read(string[] subTexts)
//        {
//            isInCoroutine = true;
//
//            int subCounter = 0;
//            int visibleCounter = 0;
//            while (subCounter < subTexts.Length)
//            {
//                if (subCounter % 2 != 0)
//                {
//                    while (visibleCounter < subTexts[subCounter].Length)
//                    {
//                        onTextReveal.Invoke(subTexts[subCounter][visibleCounter]);
//                        visibleCounter++;
//                        maxVisibleCharacters++;
//                        yield return new WaitForSeconds(1f / speed);
//                    }
//                }
//                subCounter++;
//            }
//
//            isInCoroutine = false;
//            yield return null;
//
//            onDialogueFinish.Invoke();
//        }

        private void EvaluateTag(string tag)
        {
            if (tag.Length > 0)
            {
                if (tag.StartsWith("speed="))
                {
                    speed = float.Parse(tag.Split('=')[1]);
                }
                else if (tag.StartsWith("emotion="))
                {
                    onEmotionChange.Invoke((Emotion) System.Enum.Parse(typeof(Emotion), tag.Split('=')[1]));
                }
                else if (tag.StartsWith("action="))
                {
                    onAction.Invoke(tag.Split('=')[1]);
                }
            }
        }

        // Use this for event based readpage by linking a button click or else
        private IEnumerator WaitUntilEvent(UnityEvent unityEvent)
        {
            var trigger = false;
            Action action = () => trigger = true;
            unityEvent.AddListener(action.Invoke);
            yield return new WaitUntil(() => trigger);
            unityEvent.RemoveListener(action.Invoke);
        }

        private void Update()
        {
            // Allow to skip the animation of the text
            if (Input.GetKeyDown(KeyCode.Space) && isWriting)
            {
                isWriting = false;
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