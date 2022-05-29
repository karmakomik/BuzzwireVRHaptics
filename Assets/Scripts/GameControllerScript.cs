using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControllerScript : MonoBehaviour
{
    public GameObject rightRiftController;
    public GameObject hapticArea;
    Vector3 hapticAreaPosOffset, hapticAreaRotOffset;

    public GameObject surface1, surface2;
    bool hapticEffectToggled = true;

    public HapticPlugin HapticDevice = null;

    // Start is called before the first frame update
    void Start()
    {
        hapticAreaPosOffset = new Vector3(0.032768f, -0.153919f, 0.203074F);//new Vector3(-0.5114737f, -0.0235486f, -2.6125581f);  //new Vector3(1.15f, -0.59f, -2.11f); // 
        // hapticAreaRotOffset = new Vector3(1.15f, -0.59f, -2.11f);

        if (HapticDevice == null)
            HapticDevice = (HapticPlugin)FindObjectOfType(typeof(HapticPlugin));
    }

    public void calibrateHapticArea()
    {
        Debug.Log("rightRiftController.transform.position - " + rightRiftController.transform.position);
        Debug.Log("offset - " + (rightRiftController.transform.position - new Vector3(1.15f, -0.59f, -2.11f)).ToString("F6"));
        hapticArea.transform.rotation = Quaternion.Euler(0, -35.52f, 0);
        hapticArea.transform.position = rightRiftController.transform.position - hapticAreaPosOffset;
    }

    public void toggleHaptics()
    {
        if (hapticEffectToggled == true)
        {
            surface1.GetComponent<HapticSurface>().hlStiffness = 0.002f;
            surface1.GetComponent<HapticSurface>().hlPopThrough = 0.001f;
            surface2.GetComponent<HapticSurface>().hlStiffness = 0.7f;
            surface2.GetComponent<HapticSurface>().hlPopThrough = 0;
        }
        else
        {
            surface1.GetComponent<HapticSurface>().hlStiffness = 0.7f;
            surface1.GetComponent<HapticSurface>().hlPopThrough = 0;
            surface2.GetComponent<HapticSurface>().hlStiffness = 0.002f;
            surface2.GetComponent<HapticSurface>().hlPopThrough = 0.001f;
        }
        hapticEffectToggled = !hapticEffectToggled;
    }



    // Update is called once per frame
    void Update()
    {
        //print("Euler angles are currently " + HapticDevice.stylusRotationWorld.eulerAngles);
    }
}
