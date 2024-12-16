using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;


/// CameraDrag handles the rotation of the camera in front of the x-ray head. It listens for user inputs and rotates the camera around the x-ray head based on user direction.
public class CameraDrag : MonoBehaviour
{
    /// The speed of how fast to rotate the camera.
    public float speed = 100;
    private float X;
    private float Y;
    private float startingPosition;
    private Vector3 pos;
    private bool hit_head; 

    /// The target of the camera to rotate around.
    public GameObject target;
    /// The camera that is rotating.
    public Camera camera;

    public Camera ARCam;

    public GameObject sessionOrigin;

    void Start() {
        hit_head = true;
    }
    

    void Update()
    {
    	if(!SceneHandler.isXRay)
    		return;
        if (Input.touchCount > 0)
        {
            int fingerCount = 0;
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
                {
                    fingerCount++;
                }
            }
            
            if (fingerCount == 1) {
                Touch touch = Input.GetTouch(0);
                switch (touch.phase)
                {
                case TouchPhase.Began:
                    startingPosition = touch.position.x;
                    pos = touch.position;
                    Debug.Log("Touch Phase Started");
                    break;
                case TouchPhase.Moved:

                    /*
                    var pointerEventData = new pointerEventData{ position = pos};
                    var raycastResults = new List<RaycastResult>();
                    boolean hit_slider = false;
                    EventSystem.current.RaycastAll(pointerEventData, raycastResults);

                    
                    
                    foreach(var result in RaycastResults) {
                         if (var.gameObject == near_slider|| var.gameObject == width_slider || transparency_slider) {
                             hit_slider = true;
                         } 
                    }
        
                    Ray ray = camera.ScreenPointToRay(pos);
                    Collider near_col = near_slider.GetComponent<Collider>();
                    Collider width_col = width_slider.GetComponent<Collider>();
                    Collider transparency_col = transparency_slider.GetComponent<Collider>();
                    Debug.Log("Mep");
                    Debug.Log(near_col.bounds.IntersectRay(ray));
                    */
                    if (hit_head) {
                            camera.transform.RotateAround(new Vector3(0,0,0), Vector3.up, Input.GetAxis("Mouse X") * speed / 10 * Time.deltaTime);
                            Debug.Log("Rotating AR origin");
                            sessionOrigin.transform.RotateAround(target.transform.position, Vector3.up, Input.GetAxis("Mouse X") * speed / 10 * Time.deltaTime);

                            //target.transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X") * speed/10 *Time.deltaTime, 0), Space.World);

                            /*if (startingPosition > touch.position.x)
                            {
                                target.transform.Rotate(Vector3.up, -speed * Time.deltaTime);
                            }
                            else if (startingPosition < touch.position.x)
                            {
                                target.transform.Rotate(Vector3.up, speed * Time.deltaTime);
                            }
                            */
                        }
                    break;
                case TouchPhase.Ended:
                    Debug.Log("Touch Phase Ended.");
                    break;
                }
            }
        } else if(Input.GetMouseButton(0))
        {   
            /*
            PointerEventData pointerEventData = new pointerEventData{ position = pos};
            var raycastResults = new List<RaycastResult>();
            bool hit_slider = false;
            EventSystem.current.RaycastAll(pointerEventData, raycastResults);


            foreach(var result in RaycastResults) {
                    if (var.gameObject == near_slider|| var.gameObject == width_slider || transparency_slider) {
                        hit_slider = true;
                    } 
            }
            
            Debug.Log("Mouse Phase Started");
             Ray ray = camera.ScreenPointToRay(Input.mousePosition);
             Collider near_col = near_slider.GetComponent<Collider>();
             Collider width_col = width_slider.GetComponent<Collider>();
             Collider transparency_col = transparency_slider.GetComponent<Collider>();
             Debug.Log("Mep");
             Debug.Log(near_col.bounds.IntersectRay(ray));
             */

          

             if (hit_head) {
                 camera.transform.RotateAround(new Vector3(0,0,0), Vector3.up, Input.GetAxis("Mouse X") * speed *Time.deltaTime);
                 sessionOrigin.transform.RotateAround(target.transform.position, Vector3.up, Input.GetAxis("Mouse X") * speed / 10 * Time.deltaTime);
                //target.transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X") * speed *Time.deltaTime, 0), Space.World);
            }
            
        }
    }

    /// Method to set the location of the camera relative to the target. The Vector3 dist takes in a vector from the target to place the camera.
    public void SetCamera(Vector3 dist)
    {
        camera.transform.position = target.transform.position + dist;
        camera.transform.LookAt(target.transform);
        // Vector3 Dist = Vector3.Distance(Camera.main.transform.position,transform.position) 
    }

    public void set_hit_head_false() {
        hit_head = false;
    }
    public void set_hit_head_true() {
        hit_head = true;
    }

    public bool get_hit_head()
    {
        return hit_head;
    }
 
 
}