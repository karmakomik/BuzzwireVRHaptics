using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowClickScript : MonoBehaviour
{
    Vector3 handleInitPos;

    public GameObject touchingObj;
    public Vector3 clickLoc;
    public GameObject sliderCylinderLine;
    public GameObject sliderHandle;

    public float xMin = -0.1285837f, xMax = 0.379902f;

    // Start is called before the first frame update
    void Start()
    {
        handleInitPos = sliderHandle.transform.position;
        touchingObj = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Arrow Clicked " + other.gameObject.name);

        touchingObj = other.gameObject;
    }

    public void resetSlider()
    {
        sliderHandle.transform.position = handleInitPos;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.name == "SliderLine")
        {
            clickLoc = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
            sliderHandle.transform.position = new Vector3(clickLoc.x + 0.018f, sliderHandle.transform.position.y, sliderHandle.transform.position.z);
        }
        //formHandler.moveCrossToX((int)sliderHandle.transform.position.x);
    }

    void OnTriggerExit(Collider other)
    {
        touchingObj = null;
    }
}
