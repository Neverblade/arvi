using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DumbMode : Mode {
    public override void CleanupMode() {
        print("Cleaning up dumb mode on obj: " + gameObject.name);
    }

    public override void SetupMode() {
        print("Setting up dumb mode: " + gameObject.name);
    }
}
