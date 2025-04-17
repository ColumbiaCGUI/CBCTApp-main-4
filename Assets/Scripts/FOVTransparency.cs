using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FOVTransparency : MonoBehaviour
{
    /// The material whose transparency is being changed 
    public Material mat; 

    /// defualt value for transparency
    public float alpha = 0.15f; 

    /// The slider controlling the alpha value
    public Slider slider;

    void Start()
    {
        
    }

    void Update() {
        ChangeTransparency(slider.value);
        // Debug.Log("Mat _Color = " + mat.GetColor("_Color"));
    }

    void ChangeTransparency(float alphaVal)
    {
        Color oldColor = mat.color;
        if (alphaVal > 0.01) {
            Color newColor = new Color(oldColor.r, oldColor.g, oldColor.b, alphaVal);
            mat.SetColor("_Color", newColor);
        } else {
            Color newColor = new Color(oldColor.r, oldColor.g, oldColor.b, 0);
            mat.SetColor("_Color", newColor);
        }
    }
}
