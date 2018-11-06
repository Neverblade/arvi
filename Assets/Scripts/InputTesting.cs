using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputTesting : MonoBehaviour {

    private void Start() {
        TapSwipeDetector.OnSwipe += ReceiveSwipe;
        TapSwipeDetector.OnTap += ReceiveTap;
        TapSwipeDetector.OnDoubleTap += ReceiveDoubleTap;
    }

    private void ReceiveSwipe(SwipeData swipe) {
        print("Received swipe: " + swipe.Direction);
    }

    private void ReceiveTap() {
        print("Received single tap.");
    }

    private void ReceiveDoubleTap() {
        print("Received double tap.");
    }
}
