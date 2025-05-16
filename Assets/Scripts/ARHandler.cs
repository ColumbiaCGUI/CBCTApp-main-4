using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using TMPro;

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
    // public TextMeshProUGUI debugText; 

    [SerializeField]
    ARRaycastManager raycastManager;

    public Camera ARCam;

    public Camera MainCam;

    public Camera XRayCam;

    List<ARRaycastHit> hits = new List<ARRaycastHit>();

    bool isARMode = false;

    bool placed = false;

    float x_diff;
    private float previousDistance = 1.0f;
    private Vector3 originalScale;

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
        // update original scale and previous distance
        originalScale = xRayHead.transform.localScale;
        previousDistance = Vector3.Distance(ARCam.transform.position, xRayHead.transform.position);

        if (isARMode && Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            // debugText.text += "widthSlider x: " + widthSlider.transform.position.x;

            // Debug.Log("touch position x: " + Input.GetTouch(0).position.x); 
            // Debug.Log("widthSlider x: " + widthSlider.transform.position.x);

            // ensure the finger has to touch within a bound to place the head
            if (Input.GetTouch(0).position.x < widthSlider.transform.position.x + 100 || Input.GetTouch(0).position.x > (transparencySlider.transform.position.x - x_diff - 100))
            {
                return;
            }
            if (raycastManager.Raycast(Input.GetTouch(0).position, hits))
            {
                Collider skullCollider = xRayHead.GetComponent<Collider>();

                if (skullCollider != null)
                {
                    // Using ClosestPoint: if the hit point is inside the collider, the closest point equals the hit point
                    Vector3 closestPoint = skullCollider.ClosestPoint(hits[0].pose.position);
                    if (Vector3.Distance(closestPoint, hits[0].pose.position) < 0.001f)
                    {
                        // Hit is inside the skull's collider, so do nothing.
                        return;
                    }
                }

                if (!placed)
                {
                    // record original scale and distance when first spawning the head
                    originalScale = xRayHead.transform.localScale;
                    previousDistance = Vector3.Distance(ARCam.transform.position, xRayHead.transform.position);

                    xRayHead.SetActive(true);
                    placed = true;
                }

                // allow repeated placement upon tap, however this will increase false positives
                xRayHead.transform.position = hits[0].pose.position;

                // readapt visual size when spawning at new location
                float newDistance = Vector3.Distance(ARCam.transform.position, hits[0].pose.position);
                float scaleFactor = newDistance / previousDistance;
                xRayHead.transform.localScale = originalScale * scaleFactor;

                // update state
                originalScale = xRayHead.transform.localScale;
                previousDistance = newDistance;

                //xRayHead.transform.LookAt(ARCam.transform, Vector3.up);

                // ensure the x-ray head is upright 
                // Quaternion targetRotation = Quaternion.Euler(0, hits[0].pose.rotation.eulerAngles.y, 0);
                // xRayHead.transform.rotation = targetRotation;
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

        // xRayHead.transform.rotation = Quaternion.Euler(90.0f, 0.0f, 0);

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
        xRayHead.transform.rotation = Quaternion.Euler(0.0f, 0, 0);

        // if already scaled during AR mode, this resets the scale
        // xRayHead.transform.localScale = new Vector3(0.004f, 0.004f, 0.004f);
    }

    public void UIMode ()
    {
        NonARMode();
        sceneHandler.SetUIAsMainPage();
    }
}
