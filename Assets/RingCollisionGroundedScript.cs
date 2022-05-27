using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingCollisionGroundedScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameControllerScript gameController;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        print("Euler angles are currently " + gameObject.transform.eulerAngles);
        
    }
    private void OnTriggerEnter(Collider other)
    {

    }

    private void OnTriggerStay(Collider other)
    {
        //print("Collision! Euler angles are currently " + gameObject.transform.eulerAngles);

    }

    private void OnTriggerExit(Collider other)
    {
     
    }
}
