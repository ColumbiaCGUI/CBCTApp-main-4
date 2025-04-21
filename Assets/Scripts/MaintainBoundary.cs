using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaintainBoundary : MonoBehaviour
{
    public bool isLeftBox = true;
    public bool isRightBox = false;
    public Camera UICamera; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RestrictBound();
    }

    private void RestrictBound() {
        if (UICamera == null || UICamera.tag != "MainCamera") {
            return;
        }

        // find the children
        foreach (Transform child in transform) {
            if (child) {
                var childPoint = UICamera.WorldToScreenPoint(child.position);
                if (isLeftBox) {
                    if (childPoint.x < 0 || childPoint.x >= Screen.width / 2 || childPoint.y < 0 || childPoint.y >= Screen.height) {
                        childPoint.x = Mathf.Clamp(childPoint.x, 0, Screen.width / 2);
                        childPoint.y = Mathf.Clamp(childPoint.y, 0, Screen.height);
                        child.position = UICamera.ScreenToWorldPoint(childPoint); 
                    }
                } else if (isRightBox) {
                    if (childPoint.x < Screen.width / 2 || childPoint.x >= Screen.width || childPoint.y < 0 || childPoint.y >= Screen.height) {
                        childPoint.x = Mathf.Clamp(childPoint.x, Screen.width / 2, Screen.width);
                        childPoint.y = Mathf.Clamp(childPoint.y, 0, Screen.height);
                        child.position = UICamera.ScreenToWorldPoint(childPoint); 

                    }
                }
            }
        }
    }
}
