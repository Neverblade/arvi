using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

/**
 * Main controller for ARVI.
 */
public class ModeManager : MonoBehaviour {

    // Singleton
    public static ModeManager instance;

    // UI Elements
    public delegate void Func();
    public struct Element {
        public string name;
        public Func function;

        public Element(string name, Func function) {
            this.name = name;
            this.function = function;
        }
    }
    public List<Element> elements;
    public int index;

    // Modes
    public Mode startingMode;
    private Mode currentMode;

    private void Start() {
        // Singleton handling
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(this);
        }

        // Start at the main menu
        startingMode.SetupMode();
        currentMode = startingMode;

    }

    public void SwitchModes(Mode nextMode) {
        currentMode.CleanupMode();
        nextMode.SetupMode();
        currentMode = nextMode;
    }

    // TODO: Add text-to-speech support
    public void OutputText(string text) {
        Debug.Log("AUDIO: " + text);
    }
}
