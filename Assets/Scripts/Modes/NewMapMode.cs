using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SM = ScriptManager;

public class NewMapMode : Mode {

    public GameObject newMapPanel;
    public Mode mainMenuMode;
    public Mode audioCueMode;

    public override void CleanupMode() {
        newMapPanel.SetActive(false);

        // Clean up elements
        SM.instance.elements.Clear();

        // Clean up event handler
        TapSwipeDetector.OnSwipe -= OnSwipeDefault;
        TapSwipeDetector.OnTap -= OutputCurrentElement;
        TapSwipeDetector.OnDoubleTap -= OnDoubleTapDefault;
    }

    public override void SetupMode() {
        newMapPanel.SetActive(true);

        // Set up elements
        List<SM.Element> elements = new List<SM.Element>();
        elements.Add(new SM.Element("Add audio cue", OnSelectAddAudioCue));
        elements.Add(new SM.Element("Save map", OnSelectSave));
        elements.Add(new SM.Element("Cancel", OnSelectCancel));
        SM.instance.elements = elements;
        SM.instance.index = 0;

        // Set up event handlers
        TapSwipeDetector.OnSwipe += OnSwipeDefault;
        TapSwipeDetector.OnTap += OutputCurrentElement;
        TapSwipeDetector.OnDoubleTap += OnDoubleTapDefault;

        // Output current element name
        OutputCurrentElement();
    }

    public void OnSelectAddAudioCue() {
        print("Adding audio cue. Moving to Audio Cue Menu.");
        SM.instance.SwitchModes(audioCueMode);
    }

    public void OnSelectCancel() {
        print("Canceling. Moving to main menu.");
        // TODO: Remove any other gameobjects or what not
        SM.instance.SwitchModes(mainMenuMode);
    }

    public void OnSelectSave() {
        print("Saving. Moving to main menu.");
        // TODO: Placenote saving, clean up stuff
        SM.instance.SwitchModes(mainMenuMode);
    }
}
