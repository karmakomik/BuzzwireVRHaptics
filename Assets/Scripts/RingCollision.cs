using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingCollision : MonoBehaviour
{
    public GameObject gameController;
    Collider currCollider;
    Collider oldCollider;
    Vector3 loc;
    public GameObject startStopLight;
    int numCollidersInContact;
    public GameObject mistakeLineObj;

    // Start is called before the first frame update
    void Start()
    {
        numCollidersInContact = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameController.GetComponent<HapticsController>().isFeedbackOnNow)
        {
            //mistakeLineObj.GetComponent<LineRenderer>().;
            //Set the start and end points of the line
            mistakeLineObj.GetComponent<LineRenderer>().SetPosition(0, transform.position);
            mistakeLineObj.GetComponent<LineRenderer>().SetPosition(1, gameController.GetComponent<HapticsController>().projectedHookPos);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Number of colliders in contact - " + numCollidersInContact);
        ++numCollidersInContact;
        //Debug.Log("Object in contact - " + other.gameObject);
        if (other.tag != "StartZone" && other.tag != "StopZone" && gameController.GetComponent<HapticsController>().feedbackEnabled)
        {
            /*if (currCollider != null)
            {
                //currCollider.enabled = false;
                //delayEnableOldCollider(currCollider);
                currCollider = other;
            }
            else
            {
                currCollider = other;
            }*/
            if ((gameController.GetComponent<HapticsController>().currLevel == 2 || gameController.GetComponent<HapticsController>().currLevel == 3) && other.gameObject.name == "Part6")
            {
                Debug.Log("Entry into " + other.gameObject);
                other.gameObject.transform.parent.Find("StopCylinder").GetComponent<Collider>().enabled = true;
            }

            if (gameController.GetComponent<HapticsController>().feedbackEnabled && oldCollider!= null)
            {
                //if(oldCollider.gameObject.name.Substring()
                //Find substring of oldcollider game object name after 'Part' and convert it to an integer
                int oldColliderPartNum = int.Parse(oldCollider.gameObject.name.Substring(4));
                int currColliderPartNum = int.Parse(other.gameObject.name.Substring(4));
                //Print oldColliderPartNum and currColliderPartNum
                Debug.Log("Old collider part number - " + oldColliderPartNum);
                Debug.Log("Current collider part number - " + currColliderPartNum);
                if (currColliderPartNum == oldColliderPartNum || currColliderPartNum == oldColliderPartNum + 1 || currColliderPartNum == oldColliderPartNum - 1) //Is it the same part, the next part or the previous part I am moving to?
                {
                    //Skip the rest of the code
                    //return;
                    gameController.GetComponent<HapticsController>().doControllerReattachOperations(other.gameObject.tag);
                    gameController.GetComponent<HapticsController>().stopMistakeFeedback();
                    mistakeLineObj.SetActive(false);
                }
                else
                {
                    Debug.Log("You jumped!");
                }
            }
            else
            {
                /*gameController.GetComponent<HapticsController>().doControllerReattachOperations(other.gameObject.tag);
                gameController.GetComponent<HapticsController>().stopMistakeFeedback();
                mistakeLineObj.SetActive(false);*/
            }
        }
        else if (other.tag == "StopZone")
        {
            gameController.GetComponent<HapticsController>().doControllerReattachOperations("null");
            mistakeLineObj.SetActive(false);
            gameController.GetComponent<HapticsController>().feedbackEnabled = false;
            //gameController.GetComponent<HapticsController>().startStopRefController.SetActive(true);
            //gameController.GetComponent<HapticsController>().startStopRefController.transform.position = gameController.GetComponent<HapticsController>().stopPositions[gameController.GetComponent<HapticsController>().currLevel - 1];
            gameController.GetComponent<HapticsController>().solidRightHandController.SetActive(false);
            gameController.GetComponent<HapticsController>().ghostRightHandController.SetActive(true);
        }
        else if (other.tag == "StartZone")
        {
            gameController.GetComponent<HapticsController>().doControllerReattachOperations("null");
            mistakeLineObj.SetActive(false);
            gameController.GetComponent<HapticsController>().feedbackEnabled = false;
            //gameController.GetComponent<HapticsController>().startStopRefController.SetActive(false);            
            gameController.GetComponent<HapticsController>().solidRightHandController.SetActive(true);
            gameController.GetComponent<HapticsController>().ghostRightHandController.SetActive(false);
        }
    }


    /*public void delayEnableOldCollider(Collider collider)
    {
        StartCoroutine(delayEnableColliderCoRoutine(collider));
    }

    //Placing this here because I disable all coroutines in gamemanager during every transition
    public IEnumerator delayEnableColliderCoRoutine(Collider collider)
    {
        int seconds = 1;
        while (seconds > 0)
        {
            yield return new WaitForSecondsRealtime(1);
            seconds--;
        }
        collider.enabled = true;
        //Debug.Log("delayResetNewTasksFlag");
    }*/

    private void OnTriggerStay(Collider other)
    {

        if (other.tag != "StartZone" && other.tag != "StopZone")
        {
            //startStopLight.SetActive(false);
            //Debug.Log("Collision with " + other.gameObject);
            loc = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
            //Debug.Log(loc);       
        }
        else if(other.tag == "StartZone")
        {
            //startStopLight.SetActive(true);
            gameController.GetComponent<HapticsController>().stopMistakeFeedback();
            mistakeLineObj.SetActive(false);
            //if (gameController.GetComponent<HapticsController>().client != null && !gameController.GetComponent<HapticsController>().tutorialPhase)
            //    gameController.GetComponent<HapticsController>().client.Write("M;1;;;LeftSwitchPressed;Left Switch Pressed\r\n");
        }
        else if (other.tag == "StopZone")
        {
            //startStopLight.SetActive(true);
            gameController.GetComponent<HapticsController>().stopMistakeFeedback();
            mistakeLineObj.SetActive(false);
            //if (gameController.GetComponent<HapticsController>().client != null && !gameController.GetComponent<HapticsController>().tutorialPhase)
            //gameController.GetComponent<HapticsController>().client.Write("M;1;;;RightSwitchPressed;Right Switch Pressed\r\n");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        --numCollidersInContact;
        if (other.tag != "StartZone" && other.tag != "StopZone" && gameController.GetComponent<HapticsController>().feedbackEnabled)
        {
            //startStopLight.SetActive(false);

            Debug.Log("Exit from " + other.gameObject);
            Debug.Log("Collider tag - " + other.gameObject.tag);
            oldCollider = other;
            if (numCollidersInContact < 1)
            {
                Debug.Log("Number of colliders in contact is less than 1. Triggering feedback");
                //currCollider = null;
                gameController.GetComponent<HapticsController>().doControllerDetachOperations((CapsuleCollider)other, other.gameObject.tag, loc);
                gameController.GetComponent<HapticsController>().triggerMistakeFeedback();
                mistakeLineObj.SetActive(true);
            }
        }
        else if (other.tag == "StartZone")
        {
            Debug.Log("Level started!");
            if(!gameController.GetComponent<HapticsController>().tutorialPhase) other.enabled = false;
            gameController.GetComponent<HapticsController>().feedbackEnabled = true;
            //gameController.GetComponent<HapticsController>().startStopRefController.SetActive(false);
            gameController.GetComponent<HapticsController>().solidRightHandController.SetActive(true);
            gameController.GetComponent<HapticsController>().ghostRightHandController.SetActive(false);
        }
    }

    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(loc, 0.005f);
    }*/
}
