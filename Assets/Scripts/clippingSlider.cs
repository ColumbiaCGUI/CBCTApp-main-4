using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

  /// This function sets the position of the clipping planes based on the value of the slider 
public class clippingSlider : MonoBehaviour
{


    /// The camera whose  clipping  is being changed 
    public Camera cam; 

    /// defualt value for clipping planes 
    public float near = 0.15f; 
    public float far = 100f;

    /// The sliders controlling the clipping values
    public Slider near_slider;
    public Slider width_slider;

    public GameObject head; 
    float i_dist; 
    float offset; 
    public Camera MainCam;  
    public Camera ARCam;
    public Camera xRayCam;


    void Start()
    {
        i_dist = Vector3.Distance(head.transform.position, cam.transform.position);
        offset = 0; 
    }

    void Update() {
        if(MainCam.CompareTag("MainCamera")) {
            // if the maincamera tag is in action I think we want to have default value on the xray cam clipping 
             ChangeClipping(near, far, cam);
        } else if (xRayCam.CompareTag("MainCamera")) {
            float cur_dist = Vector3.Distance(head.transform.position, cam.transform.position);

            offset = cur_dist - i_dist; 


            ChangeClipping(near_slider.value + offset, width_slider.value, cam);
        } else if (ARCam.CompareTag("MainCamera")) {
            if (head.activeSelf == false)
            {
                Debug.Log("Default clipping");
                ChangeClipping(0.05f, far, ARCam);
            }
            else
            {

                float cur_dist = Vector3.Distance(head.transform.position, ARCam.transform.position);

                offset = cur_dist - i_dist;
             

                ChangeClipping(Mathf.Max(near_slider.value + offset, 0.03f), width_slider.value, ARCam);
            }
        } 
    

       
    }

    void ChangeClipping(float near, float width, Camera cam)
    {
        cam.nearClipPlane = near; 
        cam.farClipPlane = near + width; 
    }
}
