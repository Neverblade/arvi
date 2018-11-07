using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DumbMode : Mode {
    public override void CleanupMode() {
        Debug.Log("Cleaning up dumb mode on obj: " + gameObject.name);
    }

    public override void SetupMode() {
        Debug.Log("Setting up dumb mode: " + gameObject.name);
    }
}
