using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class PlacenoteManager : MonoBehaviour, PlacenoteListener {

    // Singleton
    public static PlacenoteManager instance;

    // Audio Cues
    public AudioCueManager audioCueManager;

    // Placenote / ARKit
    private UnityARSessionNativeInterface arSession;
    private bool ARInit = false;
    private LibPlacenote.MapMetadataSettable currMapDetails;
    private LibPlacenote.MapInfo selectedMapInfo;
    private string selectedMapId {
        get {
            return selectedMapInfo != null ? selectedMapInfo.placeId : null;
        }
    }
    private string saveMapId = null;

    private void Start() {
        // Singleton handling
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(this);
        }

        Input.location.Start();
        arSession = UnityARSessionNativeInterface.GetARSessionNativeInterface();
        StartARKit();
        FeaturesVisualizer.EnablePointcloud();
        LibPlacenote.Instance.RegisterListener(this);
    }

    private void Update() {
        if (!ARInit && LibPlacenote.Instance.Initialized()) {
            ARInit = true;
            OutputText("Ready to start!");
        }
    }

    private void StartARKit() {
#if !UNITY_EDITOR
        Debug.Log("Initializing ARKit");
        Application.targetFrameRate = 60;
        ConfigureSession();
#endif
    }

    private void ConfigureSession() {
#if !UNITY_EDITOR
        ARKitWorldTrackingSessionConfiguration config = new ARKitWorldTrackingSessionConfiguration();
        config.alignment = UnityARAlignment.UnityARAlignmentGravity;
        config.getPointCloudData = true;
        config.enableLightEstimation = true;
        config.planeDetection = UnityARPlaneDetection.Horizontal;
        arSession.RunWithConfig(config);
#endif
    }

    public void OnPose(Matrix4x4 outputPose, Matrix4x4 arkitPose) { }

    public void OnStatusChange(LibPlacenote.MappingStatus prevStatus, LibPlacenote.MappingStatus currStatus) {
        Debug.Log("prevStatus: " + prevStatus.ToString() + " currStatus: " + currStatus.ToString());
        if (currStatus == LibPlacenote.MappingStatus.RUNNING && prevStatus == LibPlacenote.MappingStatus.LOST) {
            OutputText("Localized");
            //mAudioCueManager.LoadAudioCuesJSON(mSelectedMapInfo.metadata.userdata); // CHANGED
        }
        else if (currStatus == LibPlacenote.MappingStatus.RUNNING && prevStatus == LibPlacenote.MappingStatus.WAITING) {
            OutputText("Mapping: Tap to add audio cues");
        }
        else if (currStatus == LibPlacenote.MappingStatus.LOST) {
            OutputText("Searching for position lock");
        }
        else if (currStatus == LibPlacenote.MappingStatus.WAITING) {
            if (audioCueManager.audioCueObjList.Count != 0) { // CHANGED
                audioCueManager.ClearAudioCues(); // CHANGED
            }
        }
    }

    /**
     * For outputting status updates and other information.
     * This might turn into text-to-speech, or a label up top, etc.
     */
    public void OutputText(string text) {
        Debug.Log("PLACENOTE: " + text);
    }
}
