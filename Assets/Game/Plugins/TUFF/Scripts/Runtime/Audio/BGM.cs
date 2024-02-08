using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [CreateAssetMenu(fileName = "BGM", menuName = "TUFF/Audio/BGM")]
    public class BGM : ScriptableObject
    {
        [Tooltip("The music Audio Clip to play.")]
        public AudioClip clip;

        [Header("Loop")]
        [Tooltip("The sample where the loop section of the song begins. Set both values to 0 to loop the entire song.")] 
        public int loopStart = 0;
        [Tooltip("The sample where the loop section of the song ends. Set both values to 0 to loop the entire song.")] 
        public int loopEnd = 0;

        public AudioClip prebakedLoopIntro = null;
        public AudioClip prebakedLoopMain = null;

        [Header("Song Info")]
        [Tooltip("The song's name.")]
        public string songName = "";
        [Tooltip("Credits for the people involved in the song.")]
        public string author = "";
    }
}