using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class maintain_scale : MonoBehaviour
{
    public Vector3 scale_Rect = new Vector3(0.3f, 0.3f, 1f); 
    public Vector3 scale_Button = new Vector3(0.1f, 0.1f, 1f);
    
    private GameObject leftButton; 
    private GameObject rightButton; 
    private GameObject downButton; 
    private GameObject upButton; 

    void Start()
    {
        leftButton = gameObject.transform.Find("CircleLeft").gameObject; 
        rightButton = gameObject.transform.Find("CircleRight").gameObject;
        downButton = gameObject.transform.Find("CircleBottom").gameObject;
        upButton = gameObject.transform.Find("CircleTop").gameObject;

        // Debug.Log(gameObject);
        // Debug.Log(upButton);
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 rs = transform.localScale;

        Vector3 change = Vector3.Scale(rs, new Vector3(1/scale_Rect.x, 1/scale_Rect.y, 1/scale_Rect.z)); 
        Vector3 scale = Vector3.Scale(scale_Button, new Vector3(1/change.x, 1/change.y, 1/change.z));

        leftButton.transform.localScale = scale;
        rightButton.transform.localScale = scale;
        downButton.transform.localScale = scale;
        upButton.transform.localScale = scale;
    }
}
