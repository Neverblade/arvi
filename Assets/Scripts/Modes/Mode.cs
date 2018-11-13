using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MM = ModeManager;

public abstract class Mode : MonoBehaviour {

    public abstract void SetupMode();

    public abstract void CleanupMode();

    public void OutputCurrentElement() {
        MM.instance.OutputText(MM.instance.elements[MM.instance.index].name);
    }

    public void OnDoubleTapDefault() {
        MM.instance.elements[MM.instance.index].function();
    }

    public void OnSwipeDefault(SwipeData swipeData) {
        SwipeDirection dir = swipeData.Direction;
        if (dir == SwipeDirection.Left || dir == SwipeDirection.Right) {
            SwitchElements(dir);
            OutputCurrentElement();
        }
    }

    public void SwitchElements(SwipeDirection dir) {
        if (dir == SwipeDirection.Left) {
            MM.instance.index -= 1;
            MM.instance.index += MM.instance.elements.Count;
        } else {
            MM.instance.index += 1;
        }
        MM.instance.index = MM.instance.index % MM.instance.elements.Count;
    }
}
