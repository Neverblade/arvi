using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoodEnough.TextToSpeech;
using System.Linq;

/**
 * Controls all text-to-speech operations and settings.
 */
public class TextToSpeechManager : MonoBehaviour {

    public static TextToSpeechManager instance;

    public float pitch = 1;
    public float rate = 0.5f;

	void Awake () {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(this);
        }

        List<ISpeechSynthesisVoice> voices = TTS.AllAvailableVoices.ToList();
        TTS.DefaultParameters.Voice = null; // Default voice
        TTS.DefaultParameters.PitchMultiplier = pitch;
        TTS.DefaultParameters.SpeechRate = rate;
	}

    public void TextToSpeech(string text) {
        TTS.Stop(SpeechBoundary.Immediate);
        TTS.Speak(text);
    }
}
