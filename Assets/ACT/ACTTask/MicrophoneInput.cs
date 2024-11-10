using NBitcoin;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Whisper;
using Whisper.Utils;
using Button = UnityEngine.UI.Button;
using Toggle = UnityEngine.UI.Toggle;

    /// <summary>
    /// Record audio clip from microphone and make a transcription.
    /// </summary>
    public class MicrophoneInput : MonoBehaviour
    {
        public WhisperManager whisper;
        public MicrophoneRecord microphoneRecord;
        public bool streamSegments = true;
        public bool printLanguage = true;

        [Header("UI")] 
        public Text buttonText;
        public Text outputText;
        public Text timeText;
        public ScrollRect scroll;
        
        private string _buffer;

    

        
        
        
        
        
    }
