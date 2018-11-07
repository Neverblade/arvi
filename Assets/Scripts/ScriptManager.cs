using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Main controller for ARVI.
 */
public class ScriptManager : MonoBehaviour {

    public static ScriptManager instance;

    // Struct for elements of a mode
    public delegate void Func();
    public struct Element {
        public string name;
        public Func function;

        public Element(string name, Func function) {
            this.name = name;
            this.function = function;
        }
    }

    // Current list of elements in the scene
    public List<Element> elements;

    // Currently focused element
    public int index;

    // Mode that's used when the application starts.
    public Mode startingMode;

    // The current mode
    private Mode currentMode;

    private void Start() {
        if (instance == null) {
            instance = this;
        } else {
            if (instance != this) {
                Destroy(this);
            }
        }

        startingMode.SetupMode();
        currentMode = startingMode;

        // TODO: ARKIt setup, Placenote setup
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
