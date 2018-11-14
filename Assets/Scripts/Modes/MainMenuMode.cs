using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MM = ModeManager;
using PM = PlacenoteManager;

public class MainMenuMode : Mode {

    public GameObject mainMenuPanel;
    public Mode scanningMode;
    public Mode localizingMode;

    public override void CleanupMode() {
        mainMenuPanel.SetActive(false);

        // Clean up elements
        MM.instance.elements.Clear();

        // Clean up event handlers
        TapSwipeDetector.OnSwipe -= OnSwipeDefault;
        TapSwipeDetector.OnTap -= OutputCurrentElement;
        TapSwipeDetector.OnDoubleTap -= OnDoubleTapDefault;
    }

    public override void SetupMode() {
        mainMenuPanel.SetActive(true);
        
        // Set up elements
        List<MM.Element> elements = new List<MM.Element>();
        elements.Add(new MM.Element("Scanning", OnSelectScanning));
        elements.Add(new MM.Element("Localizing", OnSelectLocalizing));
        MM.instance.elements = elements;
        MM.instance.index = 0;

        // Set up event handlers
        TapSwipeDetector.OnSwipe += OnSwipeDefault;
        TapSwipeDetector.OnTap += OutputCurrentElement;
        TapSwipeDetector.OnDoubleTap += OnDoubleTapDefault;

        // Output current element name
        OutputCurrentElement();
    }

    public void OnSelectScanning() {
        bool success = PM.instance.CreateNewSession();
        if (success) {
            Debug.Log("Scanning. Moving to New Map Mode.");
            MM.instance.SwitchModes(scanningMode);
        } else {
            MM.OutputText("ARVI is still loading, please wait.");
        }
    }

    public void OnSelectLocalizing() {
        Debug.Log("Localizing. Moving to Load Map Menu.");
        MM.instance.SwitchModes(localizingMode);
    }
}
