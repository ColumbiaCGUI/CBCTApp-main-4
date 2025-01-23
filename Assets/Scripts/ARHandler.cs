using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

/// ARHandler is in charge of changing between the AR mode and the non-AR mode. 
public class ARHandler : MonoBehaviour
{
    public GameObject xRayHeadEmpty;
    /// 3D model head showing x-ray result
	public GameObject xRayHead;

    // The head model used on the selection screen
    public GameObject head;

    public GameObject widthSlider;
    public GameObject nearSlider;
    public GameObject transparencySlider;

    /// Script to rotate the camera around the head
	public CameraDrag camScript; 
    /// Button to enter or leave AR mode
    public Button yourButton;

    public ARSession arSession;

    public SceneHandler sceneHandler;

    [SerializeField]
    ARRaycastManager raycastManager;

    public Camera ARCam;

    public Camera MainCam;

    public Camera XRayCam;

    List<ARRaycastHit> hits = new List<ARRaycastHit>();

    bool isARMode = false;

    bool placed = false;

    float x_diff;

    void Start()
    {
        x_diff = widthSlider.transform.position.x - nearSlider.transform.position.x;
    	NonARMode();
        sceneHandler.SetUIAsMainPage();

        Button btn = yourButton.GetComponent<Button>();
        btn.onClick.AddListener(ChangeMode);
    }

    void Update()
    {
        if (isARMode && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {

            if (Input.GetTouch(0).position.x < widthSlider.transform.position.x || Input.GetTouch(0).position.x > (transparencySlider.transform.position.x - x_diff))
            {
                return;
            }
            if (raycastManager.Raycast(Input.GetTouch(0).position, hits))
            {
                if (!placed)
                {
                    xRayHead.SetActive(true);
                    placed = true;
                    xRayHead.transform.position = hits[0].pose.position;
                    //xRayHead.transform.LookAt(ARCam.transform, Vector3.up);
                }
            }
        }
    }

    void ChangeMode() 
    {
    	if(isARMode) {
    		NonARMode();
    	} else {
    		ARMode();
    	}
    }

    ///
    /// Make the necessary changes to enter AR mode. 
    ///
    public void ARMode() 
    {
        sceneHandler.SetXRayAsMainPage();
    	isARMode = true;

        xRayHead.transform.rotation = Quaternion.Euler(90.0f, 0.0f, 0);

        xRayHead.SetActive(true);
        //head.SetActive(false);

        MainCam.tag = "Untagged";
        ARCam.tag = "MainCamera";
        XRayCam.enabled = false;
        XRayCam.tag = "Untagged";


        arSession.enabled = true;
        sceneHandler.ARSetup();
    }

    ///
    /// Make the necessary changes to exit AR mode. 
    ///
    public void NonARMode()
    {
        placed = false;

        isARMode = false;
        arSession.enabled = false;
        XRayCam.enabled = true;
        head.SetActive(true);

        ARCam.tag = "Untagged";

        sceneHandler.ARExit();
        sceneHandler.SetXRayAsMainPage();
        xRayHead.transform.position = new Vector3(0, 0, 0);
        xRayHead.transform.rotation = Quaternion.Euler(90.0f, 0, 0);
        xRayHead.transform.localScale = new Vector3(0.004f, 0.004f, 0.004f);
    }

    public void UIMode ()
    {
        NonARMode();
        sceneHandler.SetUIAsMainPage();
    }
}
