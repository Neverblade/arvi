﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MM = ModeManager;

public class AudioCueMode : Mode {

    public GameObject audioCuePanel;
    public Mode newMapMode;

    public override void CleanupMode() {
        audioCuePanel.SetActive(false);

        // Clean up elements
        MM.instance.elements.Clear();

        // Clean up event handler
        TapSwipeDetector.OnSwipe -= OnSwipeDefault;
        TapSwipeDetector.OnTap -= OutputCurrentElement;
        TapSwipeDetector.OnDoubleTap -= OnDoubleTapDefault;
    }

    public override void SetupMode() {
        audioCuePanel.SetActive(true);

        // Set up elements
        List<MM.Element> elements = new List<MM.Element>();
        elements.Add(new MM.Element("Audio cue list", OnSelectAudioCueList));
        elements.Add(new MM.Element("Cancel", OnSelectCancel));
        MM.instance.elements = elements;
        MM.instance.index = 0;

        // Set up event handlers
        TapSwipeDetector.OnSwipe += OnSwipeDefault;
        TapSwipeDetector.OnTap += OutputCurrentElement;
        TapSwipeDetector.OnDoubleTap += OnDoubleTapDefault;

        // Output current element name
        OutputCurrentElement();
    }

    public void OnSelectAudioCueList() {
        Debug.Log("Selecting audio cue. Moving to New Map Mode.");
        MM.instance.SwitchModes(newMapMode);
    }

    public void OnSelectCancel() {
        Debug.Log("Cancelling. Moving to New Map Mode.");
        MM.instance.SwitchModes(newMapMode);
    }
}
