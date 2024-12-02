using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using TMPro;
using System;

//[RequireComponent(typeof(ARRaycastManager))]
//[RequireComponent(typeof(ARPlaneManager))]
//[RequireComponent(typeof(ARAnchorManager))]
//[RequireComponent(typeof(ARPointCloudManager))]
[RequireComponent(typeof(ARTrackedImageManager))]
public class ARSceneHandler : MonoBehaviour
{
    // <summary>
    //Needed for Anchor Detection & Management
    // </summary>
    ARTrackedImageManager _artrackImageManager = null;

    /// <summary>
    /// Reposition The Visual and Walls
    /// </summary>
    //public Image aimPoint;

    /// <summary>
    /// InfoBar
    /// </summary>
    public Text infobar;
    public TextMeshProUGUI anchorInfo;

    /// <summary>
    /// Warning Bar
    /// </summary>
    public Text WarningBar;

    /// <summary>
    /// The Background Walls prefab.
    /// </summary>
   
    //public GameObject walls_prefab;

    /// <summary>
    /// Parent of the created Background Walls.
    /// </summary>
    //public GameObject bgwalls;

    /// <summary>
    /// The Background Walls gameObject.
    /// </summary>
    //private GameObject walls;

    /// <summary>
    /// Scene (Visual) gameObject.
    /// </summary>
    //private fi.Scene sceneobj;
    private GameObject sceneobj;

    /// <summary>
    /// 2 Background Buttons
    /// </summary>
    //public Button BGSettingsBtn;
    //public Button BGPanelBtn;

    /// <summary>
    /// Reposition Window GameObject
    /// </summary>
    //public GameObject RepoBox;
    public GameObject Settings;

    /// <summary>
    /// Manipulation Variables Start
    /// </summary>
    
    // For Scaling
    //public float m_minScale = 0.003F;
    //public float m_maxScale = 0.025F;

    // For Demo Module
    //public float d_minScale = 1F;
    //public float d_maxScale = 5F;

    //For BGWalls
    //private Vector3 w_InitialScale = new Vector3(2F, 2F, 2F);

    //private float initialFingersDistance;
    //private Vector3 initialScale;
    private Vector3 m_InitialScale = new Vector3 (0.003f, 0.003f, 0.003f);
    private Vector3 d_InitialScale = new Vector3(0.75F, 0.75F, 0.75F);

    //For Rotation
    private readonly float rotationLimit = 180f;
    public GameObject rotationX;
    public GameObject rotationY;
    public GameObject rotationZ;
    Slider sliderX;
    Slider sliderY;
    Slider sliderZ; 

    /// <summary>
    /// Manipulation Variables End
    /// </summary>

    /// <summary>
    /// Check to Adjust Visual only once
    /// </summary>
    private static bool place_visual = true;

    /// <summary>
    /// Flag to Check if any Active Visuals are Present to Avoid the expensive GameObject.Find call
    /// </summary>
    //static bool activeStudy = false;

    /// <summary>
    /// For managing Visualization
    /// </summary>
    private IReferenceImageLibrary refLib;
    private Dictionary<string, ARTrackedImage> allTracked;


    /// <summary>
    /// Background Walls Check Variables 
    /// </summary>
    //static bool showbackground = false;
    //static bool isbackground = false;

    // Start is called before the first frame update
    void Start()
    {
        // Log 1st message of the session.
        App.LogMessage(string.Format("{0} | Background | Starting Log for screen size {1}x{2}", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now), Screen.width, Screen.height));
        Debug.Log(string.Format("{0} | Background | Starting Log for screen size {1}x{2}", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now), Screen.width, Screen.height));

        refLib = _artrackImageManager.referenceLibrary;
        allTracked = new Dictionary<string, ARTrackedImage>();
    }

    void Awake()
    {
        _artrackImageManager = GetComponent<ARTrackedImageManager>();

        sliderX = rotationX.transform.Find("SliderX").GetComponentInChildren<Slider>();
        sliderY = rotationY.transform.Find("SliderY").GetComponentInChildren<Slider>();
        sliderZ = rotationZ.transform.Find("SliderZ").GetComponentInChildren<Slider>();

        sliderX.onValueChanged.AddListener(delegate { setSliderValue(); });
        sliderY.onValueChanged.AddListener(delegate { setSliderValue(); });
        sliderZ.onValueChanged.AddListener(delegate { setSliderValue(); });
    }

    void OnEnable() => _artrackImageManager.trackedImagesChanged += OnChanged;

    void OnDisable() => _artrackImageManager.trackedImagesChanged -= OnChanged;

    // Update is called once per frame
    void Update()
    {
        if (GameObject.Find("Scene(Clone)") == null)
            return;

        //activeStudy = true;
        sceneobj = GameObject.Find("Scene(Clone)").transform.parent.gameObject;
            
        
        if (place_visual)
        {
            //Debug.Log("Placing Visuals");
            //if (sceneobj.transform.Find("Scene(Clone)").childCount > 0 && allTracked.Count>0)
            if (allTracked.Count > 0)
            {
                WarningBar.gameObject.SetActive(false);
                Debug.Log("Images Tracked");
                string _image = refLib[0].name;
                Vector3 pos = allTracked[_image].transform.position;

                // Adjust Height
                //Vector3 cpos = Camera.main.transform.position;
                //sceneobj.transform.position = new Vector3(pos.x, (cpos.y + pos.y) / 2, pos.z);

                sceneobj.transform.position = pos;
                infobar.text = infobar.text + "\nVisual Placed At: " + sceneobj.transform.position;

                place_visual = false;
                _artrackImageManager.enabled = false;

                Debug.Log($"Place Visual {place_visual} Manager {_artrackImageManager.isActiveAndEnabled}");
            }
            else
                WarningBar.gameObject.SetActive(true);
        }

        // Check Swipe Gesture For Rotation
        /*if (Input.touchCount == 1)
        {
            _Rotation();
            return;
        }

        // Check Pinch to zoom gesture for Scaling
        if (Input.touches.Length == 2)
        {
            _Scaling();
            return;
        }*/
    }

    /// <summary>
    /// Rotate Object on swi[pe gesture.
    /// </summary>
    public void setSliderValue()
    {
        Vector3 rotation = new Vector3(
                sliderX.value * rotationLimit,
                sliderY.value * rotationLimit,
                sliderZ.value * rotationLimit);

        rotationX.transform.Find("ValueX").GetComponent<TextMeshProUGUI>().text = rotation.x.ToString("F");
        rotationY.transform.Find("ValueY").GetComponent<TextMeshProUGUI>().text = rotation.y.ToString("F");
        rotationZ.transform.Find("ValueZ").GetComponent<TextMeshProUGUI>().text = rotation.z.ToString("F");

        rotateObject(rotation);
    }

    public void rotateObject(Vector3 rotation)
    {
        if (GameObject.Find("Scene(Clone)") != null)
        {
            GameObject rotateObj = GameObject.Find("Scene(Clone)");
            Debug.Log(string.Format("Found GameObject: {0}", rotateObj.name));

            App.LogMessage(string.Format("{0} | UI Visual | Slider Rotate", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));

            rotateObj.transform.localEulerAngles = rotation;

        }
    }

    /// <summary>
    /// Sclae Object on pinch to zoom gesture.
    /// </summary>
    /*
    public void scaleObject()
    {
        float minScale;
        float maxScale;

        //Check Which Object Is Targetted
        if (Input.touches.Length == 2)
        {
            Debug.Log("Pinch to Zoom Detected.");
            Touch t1 = Input.touches[0];
            Touch t2 = Input.touches[1];

            App.LogMessage(string.Format("{0} | UI Visual | Pinch to Zoom Object Detected", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
            
            GameObject scaleObj = GameObject.Find("Scene(Clone)");

            // Override Scale Values 
            if (scaleObj.transform.parent.name == "DemoController(Clone)")
            {
                minScale = d_minScale;
                maxScale = d_maxScale;
            }
            else
            {
                minScale = m_minScale;
                maxScale = m_maxScale;
            }

            if (t1.phase == TouchPhase.Began || t2.phase == TouchPhase.Began)
            {
                initialFingersDistance = Vector2.Distance(t1.position, t2.position);
                initialScale = scaleObj.transform.localScale;
            }
            else if (t1.phase == TouchPhase.Moved || t2.phase == TouchPhase.Moved)
            {
                float currentFingersDistance = Vector2.Distance(t1.position, t2.position);
                var scaleFactor = currentFingersDistance / initialFingersDistance;
                Vector3 m_scale = initialScale * scaleFactor;
                m_scale.x = Mathf.Clamp(m_scale.x, minScale, maxScale);
                m_scale.y = Mathf.Clamp(m_scale.y, minScale, maxScale);
                m_scale.z = Mathf.Clamp(m_scale.z, minScale, maxScale);
                    
                scaleObj.transform.localScale = m_scale;
                Debug.Log(string.Format("Increase Visual Size of {0} from {1} to {2}", scaleObj.name, scaleObj.transform.localScale.x, initialScale.x));

                //Set coresponding Value on The Slider
            }
            
        }
    }
    */
    /// <summary>
    /// Reset Check when unsubscribe to modules.
    /// </summary>
    public static void Reset()
    {
        GameObject settings = GameObject.Find("PhysicalSettings").transform.Find("AR Settings").transform.Find("Scroll View").transform.Find("Viewport").transform.Find("Content").transform.Find("Hologram").gameObject;
        Slider size = settings.transform.Find("VisualSize").transform.Find("HoloSize").GetComponent<Slider>();
        if (size != null)
            size.value = 1;

        Slider xRot = settings.transform.Find("Rotation").transform.Find("XAxis").transform.Find("SliderX").GetComponent<Slider>();
        if (xRot != null)
            xRot.value = 0;

        Slider yRot = settings.transform.Find("Rotation").transform.Find("YAxis").transform.Find("SliderY").GetComponent<Slider>();
        if (yRot != null)
            yRot.value = 0;

        Slider zRot = settings.transform.Find("Rotation").transform.Find("ZAxis").transform.Find("SliderZ").GetComponent<Slider>();
        if (zRot != null)
            zRot.value = 0;

        place_visual = true;

        //activeStudy = false;

        //Debug.Log(string.Format("Reset:: Size: {0} place_visual: {1} activeStudy: {2}", size, place_visual, activeStudy));
        Debug.Log(string.Format("Reset:: Size: {0} place_visual: {1}", size, place_visual));
    }

    /// <summary>
    /// Visual Resize Slider Value Change
    /// </summary>
    public void scaleVisual(Slider size)
    {
        Vector3 initialScale;
        
        if (GameObject.Find("Scene(Clone)") == null)
            return;


        App.LogMessage(string.Format("{0} | UI Visual | Scale Visual Slider Change", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));

        GameObject scaleObj = GameObject.Find("Scene(Clone)");
        
        // Override Scale Values 
        if (scaleObj.transform.parent.name == "DemoController(Clone)")
        {
            initialScale = d_InitialScale;
        }
        else
        {
            initialScale = m_InitialScale;
        }

        Vector3 sceneScale = new Vector3(
                initialScale.x * size.value,
                initialScale.y * size.value,
                initialScale.z * size.value
                ) ;
        scaleObj.transform.localScale = sceneScale;
    }

    // Reposition Window Functions Start
    /*
    /// <summary>
    /// Function finds a new point on plane to place the visual.
    /// </summary>
    public void onRepositionClick()
    {
        App.LogMessage(string.Format("{0} | Navigation | Open Reposition Hologram Window Button Click", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
        Debug.Log(string.Format("{0} | Navigation | Open Reposition Hologram Window Button Click", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
        VisualSettingsWindow.gameObject.SetActive(false);
        RepoBox.gameObject.SetActive(true);
    }

    /// <summary>
    /// Function finds a new point on plane to place the visual.
    /// </summary>
    public void onPlaceClick()
    {
        App.LogMessage(string.Format("{0} | UI Visual| Place Hologram in New Position Button Click", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
        Debug.Log(string.Format("{0} | UI Visual| Place Hologram in New Position Button Click", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));

        Vector2 aimpose = new Vector2(aimPoint.gameObject.transform.position.x, aimPoint.gameObject.transform.position.y);

        if (_arRaycastManager.Raycast(aimpose, arRaycastHits, TrackableType.PlaneWithinBounds | TrackableType.PlaneWithinPolygon))
        {
            Debug.Log(string.Format("{0} | UI | Placing Hologram in New Position Button Click", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
            var pose = arRaycastHits[0].pose;

            sceneobj.transform.position = pose.position;
            sceneobj.transform.localPosition = pose.position;

            if (bgwalls.transform.Find("SceneBackground(Clone)") != null)
            {
                Vector3 pos = new Vector3(pose.position.x - 0.5f, pose.position.y - 0.5f, pose.position.z - 0.5f);
                walls.transform.position = pos;
            }
            Debug.Log(string.Format("Pos: {0}", pose.position));

            Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(aimPoint.gameObject.transform.position.x, aimPoint.gameObject.transform.position.y, Camera.main.nearClipPlane + 1.5f));

            Debug.Log("Position On New Location");
            sceneobj.transform.position = pos;
            if (bgwalls.transform.Find("SceneBackground(Clone)") != null)
            {
                walls.transform.position = pos;
            }
            Debug.Log(string.Format("Pos: {0}", pos));

            ARAnchor anchor = null;
            anchor = sceneobj.GetComponent<ARAnchor>();
            if (anchor == null)
            {
                Debug.Log("AddComponent ARAnchor");
                anchor = sceneobj.AddComponent<ARAnchor>();

            }

            anchors.Add(anchor);

            Debug.Log(string.Format("Anchor {0})", anchor.transform.position));

            if (anchor)
                place_visual = false;
            else
                Debug.Log("Error Creating Anchor");
            
        }
    }
    
    /// <summary>
    /// Okay/Close Button on Repostion Window Function
    /// </summary>
    public void onRepoCloseClick()
    {
        App.LogMessage(string.Format("{0} | Navigation | Close Reposition Hologram Window Button Click", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
        Debug.Log(string.Format("{0} | Navigation | Close Reposition Hologram Window Button Click", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
        RepoBox.gameObject.SetActive(false);
    }
    
    // Reposition Window Function Ends

    /// <summary>
    /// Create and Delete Background Walls Manually.
    /// </summary>
    public void onBckgroundButtonClick(Slider slider)
    {
        string text;

        if (isbackground)
            text = "Delete";
        else
        {
            text = "Create";

            if (GameObject.Find("Scene(Clone)") == null)
                return;
        }

        App.LogMessage(string.Format("{0} | UI Background| {1} Background Walls", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now), text));
        Debug.Log(string.Format("{0} Background Walls", text));

        

        // Create new Background
        if (!isbackground)
        {
            sceneobj = GameObject.Find("Scene(Clone)").transform.parent.gameObject;
            Vector3 pos = new Vector3(sceneobj.transform.position.x - 0.5f, sceneobj.transform.position.y - 0.5f, sceneobj.transform.position.z + 0.25f);
            Quaternion rot = sceneobj.transform.rotation;

            walls = Instantiate(walls_prefab, pos, rot);
            walls.transform.SetParent(bgwalls.transform);
            //walls.AddComponent<ARAnchor>();
            walls.transform.localScale = w_InitialScale;
            text = "Delete Walls";
            isbackground = true;
            showbackground = true;
        }
        else
        {
            Destroy(bgwalls.transform.Find("SceneBackground(Clone)").gameObject);
            text = "Create Walls";
            isbackground = false;
            showbackground = false;

            slider.value = 1;
        }

        //Change Text For Both The Buttons
        BGPanelBtn.transform.GetComponentInChildren<TextMeshProUGUI>().text = text;
        BGSettingsBtn.transform.GetComponentInChildren<TextMeshProUGUI>().text = text;
    }

    /// <summary>
    /// Show/Hide Background Walls
    /// </summary>
    public void onBgWallsToggle(Toggle bgtoggle)
    {
        string text;
        if (showbackground)
            text = "Disable";
        else
            text = "Enable";

        Debug.Log(string.Format("{0} | UI Background | {1} BGWalls Toggle Pressed", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now), text));
        App.LogMessage(string.Format("{0} | UI Background| {1} BGWalls Toggle Pressed", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now), text));

        if (bgwalls.transform.Find("SceneBackground(Clone)") == null)
            return;

        walls = bgwalls.transform.Find("SceneBackground(Clone)").gameObject;
        sceneobj = GameObject.Find("Scene(Clone)").transform.parent.gameObject;

        if (bgtoggle.isOn == true)
        {
            if (sceneobj != null)
            {
                Vector3 pos = sceneobj.transform.position;
                Quaternion rot = sceneobj.transform.rotation;
                walls.transform.SetPositionAndRotation(pos, rot);
            }

            walls.SetActive(true);
            showbackground = true;
        }
        else if (walls.activeSelf)
        {
            walls.SetActive(false);
            showbackground = false;
        }
    }

    /// <summary>
    /// Visual Resize Slider Value Change
    /// </summary>
    public void scaleBackground(Slider size)
    {
        Debug.Log(string.Format("{0} | UI Background| Change Background Size", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
        App.LogMessage(string.Format("{0} | UI Background| Change Background Size", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));

        if (bgwalls.transform.Find("SceneBackground(Clone)") == null)
            return;

        walls = bgwalls.transform.Find("SceneBackground(Clone)").gameObject;

        Vector3 sceneScale = new Vector3(
                w_InitialScale.x * size.value,
                w_InitialScale.y * size.value,
                w_InitialScale.z * size.value
                );
        walls.transform.localScale = sceneScale;
    }

    /// <summary>
    /// Change Background Color
    /// </summary>
    public void changeColor()
    {
        if (bgwalls.transform.Find("SceneBackground(Clone)") == null)
            return;

        Debug.Log(string.Format("{0} | UI Background| Change Background Color", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
        App.LogMessage(string.Format("{0} | UI Background| Change Background Color", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));

        walls = bgwalls.transform.Find("SceneBackground(Clone)").gameObject;

        if (walls.transform.Find("BackWall").GetComponent<Renderer>().material.color == Color.white)
        {
            walls.transform.Find("BackWall").GetComponent<Renderer>().material.SetColor("_Color", Color.gray);
            walls.transform.Find("LeftWall").GetComponent<Renderer>().material.SetColor("_Color", Color.gray);
            walls.transform.Find("RightWall").GetComponent<Renderer>().material.SetColor("_Color", Color.gray);
        }
        else
        {
            walls.transform.Find("BackWall").GetComponent<Renderer>().material.SetColor("_Color", Color.white);
            walls.transform.Find("LeftWall").GetComponent<Renderer>().material.SetColor("_Color", Color.white);
            walls.transform.Find("RightWall").GetComponent<Renderer>().material.SetColor("_Color", Color.white);
        }

    }
    */
    /// <summary>
    /// Reset Size and Rotation.
    /// </summary>
    public void onResetButtonClick()
    {
        Vector3 initialScale;
        App.LogMessage(string.Format("{0} | UI Visual| Reset Visual to Original Size and Orientation", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));

        if (GameObject.Find("Scene(Clone)") == null)
            return;

        GameObject visualObj = GameObject.Find("Scene(Clone)");

        if (visualObj.transform.parent.name == "DemoController(Clone)")
            initialScale = d_InitialScale;
        else
            initialScale = m_InitialScale;

        Debug.Log(string.Format("Reset Visual Size from {0} to {1}", visualObj.transform.localScale.x, initialScale.x));
        visualObj.transform.localScale = initialScale;
        visualObj.transform.rotation = Quaternion.identity;

        //Reset Sliders
        Slider size = Settings.transform.Find("VisualSize").transform.Find("HoloSize").GetComponent<Slider>();
        if (size != null)
            size.value = 1;

        Slider xRot = Settings.transform.Find("Rotation").transform.Find("XAxis").transform.Find("SliderX").GetComponent<Slider>();
        if (xRot != null)
            xRot.value = 0;

        Slider yRot = Settings.transform.Find("Rotation").transform.Find("YAxis").transform.Find("SliderY").GetComponent<Slider>();
        if (yRot != null)
            yRot.value = 0;

        Slider zRot = Settings.transform.Find("Rotation").transform.Find("ZAxis").transform.Find("SliderZ").GetComponent<Slider>();
        if (zRot != null)
            zRot.value = 0;
    }

    /// <summary>
    ///  Toggle Info Bar
    /// </summary>
    public void infoButtonClick()
    {
        if (infobar.gameObject.activeSelf)
            infobar.gameObject.SetActive(false);
        else
            infobar.gameObject.SetActive(true);
    }


    /// <summary>
    /// On AR TrackedImage Change Function
    /// </summary>
    /// <param name="eventArgs"></param>
    public void OnChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var newImage in eventArgs.added)
        {
            allTracked.Add(newImage.referenceImage.name, newImage);
            infobar.text = "Found Anchor At: " + newImage.transform.position;
            anchorInfo.text = "Anchor Registered";
        }

        foreach (var updatedImage in eventArgs.updated)
        {
            allTracked[updatedImage.referenceImage.name].transform.position = updatedImage.transform.position;
            allTracked[updatedImage.referenceImage.name].transform.rotation = updatedImage.transform.rotation;
            infobar.text = "Anchor Position: " + updatedImage.transform.position;
            anchorInfo.text = "Anchor Registered";
        }
    }
}
