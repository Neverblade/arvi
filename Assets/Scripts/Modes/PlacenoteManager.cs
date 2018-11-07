using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class PlacenoteManager : MonoBehaviour, PlacenoteListener {

    // Singleton
    public static PlacenoteManager instance;

    private UnityARSessionNativeInterface arSession;

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
            //mLabelText.text = "Localized";
            //mAudioCueManager.LoadAudioCuesJSON(mSelectedMapInfo.metadata.userdata); // CHANGED
        }
        else if (currStatus == LibPlacenote.MappingStatus.RUNNING && prevStatus == LibPlacenote.MappingStatus.WAITING) {
            //mLabelText.text = "Mapping: Tap to add audio cues"; // CHANGED
        }
        else if (currStatus == LibPlacenote.MappingStatus.LOST) {
            //mLabelText.text = "Searching for position lock";
        }
        else if (currStatus == LibPlacenote.MappingStatus.WAITING) {
            //if (mAudioCueManager.audioCueObjList.Count != 0) { // CHANGED
            //    mAudioCueManager.ClearAudioCues(); // CHANGED
            //}
        }
    }
}
