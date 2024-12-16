using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement : MonoBehaviour
{
    public float speed = 10.0f;

    // Update is called once per frame
    void Update()
    {
        float verticalTrans = Input.GetAxis("Vertical") * speed; 
        float horizonatalTrans = Input.GetAxis("Horizontal") * speed;

        transform.Translate(0,0, -verticalTrans);
        transform.Translate(horizonatalTrans, 0, 0);
    }
}
