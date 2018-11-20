using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MM = ModeManager;
using AM = AudioCueManagerV2;

public class NewMapMode : Mode {

    public GameObject newMapPanel;
    public Mode mainMenuMode;
    public Mode saveMapMode;
    public Mode audioCueMode;

    public override void CleanupMode() {
        newMapPanel.SetActive(false);

        // Clean up elements
        MM.instance.elements.Clear();
        UnhighlightElement(newMapPanel);

        // Clean up event handlers
        TapSwipeDetector.OnSwipe -= OnSwipeDefault;
        TapSwipeDetector.OnTap -= OutputCurrentElement;
        TapSwipeDetector.OnDoubleTap -= OnDoubleTapDefault;
    }

    public override void SetupMode() {
        newMapPanel.SetActive(true);

        // Set up elements
        List<MM.Element> elements = new List<MM.Element>();
        elements.Add(new MM.Element("Add audio cue", OnSelectAddAudioCue));
        elements.Add(new MM.Element("Cancel", OnSelectCancel));
        elements.Add(new MM.Element("Save map", OnSelectSave));
        MM.instance.elements = elements;
        MM.instance.index = 0;
        MM.instance.currentPanel = newMapPanel;
        HighlightElement(newMapPanel);

        // Set up event handlers
        TapSwipeDetector.OnSwipe += OnSwipeDefault;
        TapSwipeDetector.OnTap += OutputCurrentElement;
        TapSwipeDetector.OnDoubleTap += OnDoubleTapDefault;

        // Output current element name
        MM.OutputText("Creating a new map. Please scan the environment slowly to collect data.");
    }

    public void OnSelectAddAudioCue() {
        //Debug.Log("Adding audio cue. Moving to Audio Cue Menu.");
        AudioCueManagerV2.instance.PlaceCandidateAudioCue(Camera.main.transform.position);
        MM.instance.SwitchModes(audioCueMode);
    }

    public void OnSelectCancel() {
        //Debug.Log("Canceling. Moving to main menu.");
        AM.instance.ClearAudioCues();
        MM.instance.SwitchModes(mainMenuMode);
    }

    public void OnSelectSave() {
        //Debug.Log("Saving. Moving to save map mode.");
        MM.instance.SwitchModes(saveMapMode);
    }
}
