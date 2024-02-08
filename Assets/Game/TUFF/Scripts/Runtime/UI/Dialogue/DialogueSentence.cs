using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public enum SentenceTextType
    {
        Localized = 0,
        Simple = 1
    }
    [System.Serializable]
    public struct DialogueSentence
    {
        [Tooltip("Localized: Displays the localized text from a Localization Table key.\n" +
            "Simple: Displays a normal string.")]
        public SentenceTextType sentenceTextType;
        [Tooltip("Localization Table key to display.")]
        public string key;
        [Tooltip("Text to display.")]
        [TextArea(3, 12)] public string text;
        [Tooltip("The Voicebank to use for this sentence. Leave to none to use base Voicebank.")]
        public Voicebank voicebank;
        [Tooltip("The Text Speed to use for this sentence. Leave to 0 or less to use base Text Speed.")]
        public float textSpeed;
    }
}
