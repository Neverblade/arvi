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
    private bool reportDebug = false;
    private LibPlacenote.MapInfo selectedMapInfo;
    private string selectedMapId {
        get {
            return selectedMapInfo != null ? selectedMapInfo.placeId : null;
        }
    }
    private string saveMapId = null;

    #region Public Functions

    /**
     * Starts a new map for the scanning process.
     * Returns true iff successful.
     */
    public bool CreateNewSession() {
        ConfigureSession();
        if (!LibPlacenote.Instance.Initialized()) {
            OutputPlacenoteText("SDK not yet initialized");
            return false;
        }

        OutputPlacenoteText("Started Session");
        LibPlacenote.Instance.StartSession();

        if (reportDebug) {
            LibPlacenote.Instance.StartRecordDataset(
                (completed, faulted, percentage) => {
                    if (completed) {
                        Debug.Log("Dataset Upload Complete");
                    }
                    else if (faulted) {
                        Debug.Log("Dataset Upload Faulted");
                    }
                    else {
                        Debug.Log("Dataset Upload: (" + percentage.ToString("F2") + "/1.0)");
                    }
                });
            Debug.Log("Started Debug Report");
        }

        return true;
    }

    #endregion

    #region Boilerplate

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
            OutputPlacenoteText("ARVI has finished loading, ready to start!");
        }
    }

    /**
     * For outputting status updates and other information.
     * This might turn into text-to-speech, or a label up top, etc.
     */
    private void OutputPlacenoteText(string text) {
        Debug.Log("PLACENOTE: " + text);
    }

    #endregion

    #region Placenote Magic

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
            OutputPlacenoteText("Localized");
            //mAudioCueManager.LoadAudioCuesJSON(mSelectedMapInfo.metadata.userdata); // CHANGED
        }
        else if (currStatus == LibPlacenote.MappingStatus.RUNNING && prevStatus == LibPlacenote.MappingStatus.WAITING) {
            OutputPlacenoteText("Mapping: Tap to add audio cues");
        }
        else if (currStatus == LibPlacenote.MappingStatus.LOST) {
            OutputPlacenoteText("Searching for position lock");
        }
        else if (currStatus == LibPlacenote.MappingStatus.WAITING) {
            if (audioCueManager.audioCueObjList.Count != 0) { // CHANGED
                audioCueManager.ClearAudioCues(); // CHANGED
            }
        }
    }

    #endregion
}
