using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MM = ModeManager;

public class SaveMapMode : Mode {

    public static string inputFieldElementName = "Map name input field";

    public GameObject saveMapPanel;
    public Mode newMapMode;
    public Mode mainMenuMode;

    private InputField inputField;

    private void Start() {
        inputField = saveMapPanel.transform.Find("InputField").GetComponent<InputField>();
    }

    public override void CleanupMode() {
        saveMapPanel.SetActive(false);

        // Clean up elements
        MM.instance.elements.Clear();

        // Clean up event handlers
        TapSwipeDetector.OnSwipe -= OnSwipeDefault;
        TapSwipeDetector.OnSwipe -= OnSwipeInputField;
        TapSwipeDetector.OnTap -= OutputCurrentElement;
        TapSwipeDetector.OnDoubleTap -= OnDoubleTapDefault;
    }

    public override void SetupMode() {
        saveMapPanel.SetActive(true);

        // Set up elements
        List<MM.Element> elements = new List<MM.Element>();
        elements.Add(new MM.Element(inputFieldElementName, OnSelectInputField));
        elements.Add(new MM.Element("Cancel", OnSelectCancel));
        MM.instance.elements = elements;
        MM.instance.index = 0;

        // Set up event handlers
        TapSwipeDetector.OnSwipe += OnSwipeDefault;
        TapSwipeDetector.OnSwipe += OnSwipeInputField;
        TapSwipeDetector.OnTap += OutputCurrentElement;
        TapSwipeDetector.OnDoubleTap += OnDoubleTapDefault;

        // Output current element name
        OutputCurrentElement();

        // Activate input field
        inputField.interactable = true;
        inputField.Select();
        inputField.ActivateInputField();
    }

    /**
     * A potential function for forcing focus on the input field.
     */
    public void OnTap() {
        if (MM.instance.elements[MM.instance.index].name.Equals(inputFieldElementName)) {
            // TODO: Not sure how to implement this function
        }
    }

    /**
     * Selects and de-selects the input field on swipes.
     * Counts on the element being switched by OnSwipeDefault() first.
     */
    public void OnSwipeInputField(SwipeData swipeData) {
        // Only trigger on horizontal swipes
        if (swipeData.Direction != SwipeDirection.Left
            && swipeData.Direction != SwipeDirection.Right) {
            return;
        }

        if (MM.instance.elements[MM.instance.index].name.Equals(inputFieldElementName)) {
            inputField.interactable = true;
            inputField.Select();
            inputField.ActivateInputField();
        } else {
            inputField.DeactivateInputField();
            inputField.interactable = false;
        }
    }

    public void OnSelectInputField() {
        Debug.Log("Submitting input field. Moving to Main Menu Mode.");
        string name = inputField.text;
        if (name.Equals("")) {
            MM.instance.OutputText("Invalid scan name.");
        } else {
            PlacenoteManager.instance.SaveMap(name);
            MM.instance.SwitchModes(mainMenuMode);
        }
    }

    public void OnSelectCancel() {
        Debug.Log("Canceling. Moving to New Map Mode");
        MM.instance.SwitchModes(newMapMode);
    }
}
