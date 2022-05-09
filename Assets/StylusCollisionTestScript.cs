using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StylusCollisionTestScript : MonoBehaviour
{
    public HapticPlugin HapticDevice = null;
    public GameObject pilotTestControllerObj;
    float vibrationDuration = 3f;
    // Start is called before the first frame update
    void Start()
    {
        if (HapticDevice == null)
            HapticDevice = (HapticPlugin)FindObjectOfType(typeof(HapticPlugin));
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerStay(Collider other)
    {
        pilotTestControllerObj.GetComponent<PilotTestControllerScript>().hapticTest_startVibration_custom("a", (int)pilotTestControllerObj.GetComponent<PilotTestControllerScript>().vibrationSlider.value);

        //pilotTestControllerObj.GetComponent<PilotTestControllerScript>().currSurface = other.gameObject;

        /*
        Debug.Log("Velocity of stylus - " + HapticDevice.stylusVelocityRaw.normalized);
        if (other.gameObject.name.StartsWith("Up"))
        {
            if (HapticDevice.stylusVelocityRaw.normalized.y > 0)
            {
                other.gameObject.SetActive(false);
                Debug.Log("Collision with " + other.gameObject.name);
                pilotTestControllerObj.GetComponent<PilotTestControllerScript>().hapticTest_startVibration_mid("c");
                StartCoroutine(WaitForSeconds(vibrationDuration, "c"));
            }
        }

        if (other.gameObject.name.StartsWith("Down"))
        {
            if (HapticDevice.stylusVelocityRaw.normalized.y < 0)
            {
                other.gameObject.SetActive(false);                
                Debug.Log("Collision with " + other.gameObject.name);
                pilotTestControllerObj.GetComponent<PilotTestControllerScript>().hapticTest_startVibration_mid("a");
                StartCoroutine(WaitForSeconds(vibrationDuration, "a"));
            }
        }

        if (other.gameObject.name.StartsWith("Left"))
        {
            if (HapticDevice.stylusVelocityRaw.normalized.x < 0)
            {
                other.gameObject.SetActive(false);
                Debug.Log("Collision with " + other.gameObject.name);
                pilotTestControllerObj.GetComponent<PilotTestControllerScript>().hapticTest_startVibration_mid("c");
                StartCoroutine(WaitForSeconds(vibrationDuration, "c"));
            }
        }

        if (other.gameObject.name.StartsWith("Right"))
        {
            if (HapticDevice.stylusVelocityRaw.normalized.x > 0)
            {
                other.gameObject.SetActive(false);
                Debug.Log("Collision with " + other.gameObject.name);
                pilotTestControllerObj.GetComponent<PilotTestControllerScript>().hapticTest_startVibration_mid("d");
                StartCoroutine(WaitForSeconds(vibrationDuration, "d"));
            }
        }

        if (other.gameObject.name.StartsWith("Back"))
        {
            if (HapticDevice.stylusVelocityRaw.normalized.z < 0)
            {
                other.gameObject.SetActive(false);
                Debug.Log("Collision with " + other.gameObject.name);
                pilotTestControllerObj.GetComponent<PilotTestControllerScript>().hapticTest_startVibration_mid("f");
                StartCoroutine(WaitForSeconds(vibrationDuration, "f"));
            }
        }

        if (other.gameObject.name.StartsWith("Front"))
        {
            if (HapticDevice.stylusVelocityRaw.normalized.z > 0)
            {
                other.gameObject.SetActive(false);
                Debug.Log("Collision with " + other.gameObject.name);
                pilotTestControllerObj.GetComponent<PilotTestControllerScript>().hapticTest_startVibration_mid("e");
                
            }
        }
        */
    }

    void OnTriggerExit(Collider other)
    {
        pilotTestControllerObj.GetComponent<PilotTestControllerScript>().hapticTest_stopVibration("a");
    }


}