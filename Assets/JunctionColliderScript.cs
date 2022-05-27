using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JunctionColliderScript : MonoBehaviour
{
    public GameObject surface1, surface2;
    public GameObject hook;
    bool switchedToSurface2, switchedToSurface1;
    // Start is called before the first frame update
    void Start()
    {
        switchedToSurface2 = false;
        switchedToSurface1 = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {

    }

    private void OnTriggerStay(Collider other)
    {
        //print("Collision with " + other.gameObject.transform.parent);
        if (other.gameObject.transform.parent.gameObject == hook)
        {
            //print("Collision with hook");
            if (hook.transform.eulerAngles.z > 45 && !switchedToSurface2)
            {
                switchedToSurface2 = true;
                switchedToSurface1 = false;
                print("Switch haptics to surface 2");
                switchActiveHapticSurface(surface2);
            }
            
            if(hook.transform.eulerAngles.z <= 45 && !switchedToSurface1) 
            {
                switchedToSurface2 = false;
                switchedToSurface1 = true;
                print("Switch haptics to surface 1");
                switchActiveHapticSurface(surface1);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {

    }
    public void switchActiveHapticSurface(GameObject surface)
    {
        if (surface == surface2)
        {
            surface1.GetComponent<HapticSurface>().hlStiffness = 0.002f;
            surface1.GetComponent<HapticSurface>().hlPopThrough = 0.001f;
            surface2.GetComponent<HapticSurface>().hlStiffness = 0.7f;
            surface2.GetComponent<HapticSurface>().hlPopThrough = 0;
        }
        else if(surface == surface1)
        {
            surface1.GetComponent<HapticSurface>().hlStiffness = 0.7f;
            surface1.GetComponent<HapticSurface>().hlPopThrough = 0;
            surface2.GetComponent<HapticSurface>().hlStiffness = 0.002f;
            surface2.GetComponent<HapticSurface>().hlPopThrough = 0.001f;
        }        
    }

}
