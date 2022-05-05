using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StylusCollisionTestScript : MonoBehaviour
{
    public HapticPlugin HapticDevice = null;
    public GameObject pilotTestControllerObj;
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

    void OnTriggerEnter(Collider other)
    {        
        Debug.Log("Velocity of stylus - " + HapticDevice.stylusVelocityRaw.normalized);
        if (other.gameObject.name.StartsWith("Up"))
        {
            if (HapticDevice.stylusVelocityRaw.normalized.y > 0)
            {
                Debug.Log("Collision with " + other.gameObject.name);
                pilotTestControllerObj.GetComponent<PilotTestControllerScript>().hapticTest_startVibration_mid("b");
                StartCoroutine(WaitForSeconds(1f, "b"));
            }
        }

        if (other.gameObject.name.StartsWith("Down"))
        {
            if (HapticDevice.stylusVelocityRaw.normalized.y < 0)
            {
                Debug.Log("Collision with " + other.gameObject.name);
                pilotTestControllerObj.GetComponent<PilotTestControllerScript>().hapticTest_startVibration_mid("c");
                StartCoroutine(WaitForSeconds(1f, "c"));
            }
        }
    }


    IEnumerator WaitForSeconds(float seconds, string motor)
    {
        yield return new WaitForSeconds(seconds);

        //After x seconds, execute the following code
        pilotTestControllerObj.GetComponent<PilotTestControllerScript>().hapticTest_stopVibration(motor);
    }


}