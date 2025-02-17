using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// UIEnforceConstraints is a class that maintains the structure of one UI box. It scales the box to match the positions of the circles and vice versa. It also keeps track of the box dimensions for other classes to use.
public class UIEnforceConstraints : MonoBehaviour
{
	/// The left circle
	public GameObject left;

	/// The right circle
	public GameObject right;

	/// The top circle
	public GameObject top;

	/// The bottom circle
	public GameObject bottom;

	/// The square component
	public GameObject square;

	/// Local Position of the box
	public Vector3 savedPos;

	/// Local width of the box
	public float width;

	/// Local height of the box
	public float height;

	float zPos;

	public bool isLeftBox = false; 
	public bool isRightBox = false;
	public Camera leftCam;
	public Camera rightCam;

    // Start is called before the first frame update
    void Start()
    {
        savedPos = square.transform.localPosition;

        //distances from center
        width = square.transform.localScale.x;
        height = square.transform.localScale.y;

        zPos = left.transform.localPosition.z;

    }

    // Update is called once per frame
    void Update()
    {
		CheckBoxBoundary();

    	//check if entire square is transforming
    	if(savedPos != square.transform.localPosition) {
    		savedPos = square.transform.localPosition;
    		return;
    	}

    	savedPos.x = ( right.transform.localPosition.x + left.transform.localPosition.x ) /2;
    	savedPos.y = ( top.transform.localPosition.y + bottom.transform.localPosition.y ) /2;

    	width = ( right.transform.localPosition.x - left.transform.localPosition.x ) /2;
    	height = ( top.transform.localPosition.y - bottom.transform.localPosition.y ) /2;

    	right.transform.localPosition = new Vector3(savedPos.x + width, savedPos.y, zPos);
    	left.transform.localPosition = new Vector3(savedPos.x - width, savedPos.y, zPos);

    	top.transform.localPosition = new Vector3(savedPos.x, savedPos.y + height, zPos);
    	bottom.transform.localPosition = new Vector3(savedPos.x, savedPos.y - height, zPos);


    	square.transform.localPosition = savedPos;
    	square.transform.localScale = new Vector3(width, height, 1);
    }

	void CheckBoxBoundary() {
		if (isLeftBox) {
			// var screenPoint = leftCam.WorldToScreenPoint(transform.position);
			// // Clamp the screen position so that x stays within the left half of the screen.
			// // Here, we assume the left half is defined as x between 0 and Screen.width / 2,
			// // and for y, we keep the entire screen height (from 0 to Screen.height).
			// screenPoint.x = Mathf.Clamp(screenPoint.x, 0, Screen.width / 2);
			// screenPoint.y = Mathf.Clamp(screenPoint.y, 0, Screen.height);

			// // Convert the clamped screen point back to a world position.
			// // Vector3 worldPos = leftCam.ViewportToWorldPoint(screenPoint);
        	// // transform.position = worldPos;
			// // var worldPoint = default(Vector3);

			// // if (RectTransformUtility.ScreenPointToWorldPointInRectangle(transform.parent as RectTransform, screenPoint, leftCam, out worldPoint) == true)
			// // {
			// // 	transform.position = worldPoint;
			// // }
			// transform.position = leftCam.ScreenToWorldPoint(screenPoint);
		}
		if (isRightBox) {
			// var screenPoint = rightCam.WorldToScreenPoint(transform.position);
			// screenPoint.x = Mathf.Clamp(screenPoint.x, Screen.width / 2, Screen.width);
			// screenPoint.y = Mathf.Clamp(screenPoint.y, 0, Screen.height);
			// transform.position = rightCam.ScreenToWorldPoint(screenPoint);
		}
	}
}
