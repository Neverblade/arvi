using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SM = ModeManager;

public abstract class Mode : MonoBehaviour {

    public abstract void SetupMode();

    public abstract void CleanupMode();

    public void OutputCurrentElement() {
        SM.instance.OutputText(SM.instance.elements[SM.instance.index].name);
    }

    public void OnDoubleTapDefault() {
        SM.instance.elements[SM.instance.index].function();
    }

    public void OnSwipeDefault(SwipeData swipeData) {
        SwipeDirection dir = swipeData.Direction;
        if (dir == SwipeDirection.Left || dir == SwipeDirection.Right) {
            OnSwipeHorizontal(dir);
        }
    }

    public void OnSwipeHorizontal(SwipeDirection dir) {
        if (dir == SwipeDirection.Left) {
            SM.instance.index -= 1;
            SM.instance.index += SM.instance.elements.Count;
        } else {
            SM.instance.index += 1;
        }
        SM.instance.index = SM.instance.index % SM.instance.elements.Count;
        OutputCurrentElement();
    }
}
