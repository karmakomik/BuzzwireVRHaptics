using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class RingCollision : MonoBehaviour
{
    public GameObject experimentController;
    HapticsExperimentControllerScript experimentControllerScript;
    Collider currCollider;
    Collider oldCollider;
    Vector3 loc;
    public GameObject startStopLight;
    int numCollidersInContact;
    public GameObject mistakeLineObj;
    private Vector3 mistakeVector;
    Vector3 mistakeDirection;
    

    // Start is called before the first frame update
    void Start()
    {
        numCollidersInContact = 0;
        experimentControllerScript = experimentController.GetComponent<HapticsExperimentControllerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (experimentControllerScript.isFeedbackOnNow)
        {
            //mistakeLineObj.GetComponent<LineRenderer>().;
            //Set the start and end points of the line
            mistakeLineObj.GetComponent<LineRenderer>().SetPosition(0, transform.position);
            mistakeLineObj.GetComponent<LineRenderer>().SetPosition(1, experimentControllerScript.projectedHookPos);
            mistakeVector = experimentControllerScript.projectedHookPos - transform.position;            
            print("mistakeVector.magnitude" + mistakeVector.magnitude);
            float intensity = math.remap(0, 0.1f, 0, 1, mistakeVector.magnitude);
            float clampedIntensity = math.clamp(intensity, 0, 1);

            experimentControllerScript.changeIntensityOfGhost(clampedIntensity);

            //print("mistakeDirection - " + mistakeDirection);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Number of colliders in contact - " + numCollidersInContact);
        ++numCollidersInContact;
        //Debug.Log("Object in contact - " + other.gameObject);
        if (other.tag != "StartZone" && other.tag != "StopZone" && experimentControllerScript.feedbackEnabled)
        {

            if (experimentControllerScript.feedbackEnabled && oldCollider!= null)
            {
                //if(oldCollider.gameObject.name.Substring()
                //Find substring of oldcollider game object name after 'Part' and convert it to an integer
                int oldColliderPartNum = int.Parse(oldCollider.gameObject.name.Substring(4));
                int currColliderPartNum = int.Parse(other.gameObject.name.Substring(4));
                //Print oldColliderPartNum and currColliderPartNum
                Debug.Log("Old collider part number - " + oldColliderPartNum);
                Debug.Log("Current collider part number - " + currColliderPartNum);
                //if (currColliderPartNum == oldColliderPartNum || currColliderPartNum == oldColliderPartNum + 1 || currColliderPartNum == oldColliderPartNum - 1) //Is it the same part, the next part or the previous part I am moving to?
                {
                    //Skip the rest of the code
                    //return;
                    experimentControllerScript.doControllerReattachOperations(other.gameObject.tag);
                    experimentControllerScript.stopMistakeFeedback();
                    
                    mistakeLineObj.SetActive(false);
                }
                /*else
                //{
                    Debug.Log("You jumped!");
                }*/
            }
            else
            {
                /*experimentControllerScript.doControllerReattachOperations(other.gameObject.tag);
                experimentControllerScript.stopMistakeFeedback();
                mistakeLineObj.SetActive(false);*/
            }
        }
        else if (other.tag == "StopZone")
        {
            experimentControllerScript.doControllerReattachOperations("null");
            mistakeLineObj.SetActive(false);
            experimentControllerScript.stopMistakeFeedback();
            experimentControllerScript.changeIntensityOfGhost(1);
            experimentControllerScript.feedbackEnabled = false;
            //experimentControllerScript.startStopRefController.SetActive(true);
            //experimentControllerScript.startStopRefController.transform.position = experimentControllerScript.stopPositions[experimentControllerScript.currLevel - 1];
            experimentControllerScript.solidRightHandController.SetActive(false);
            experimentControllerScript.ghostRightHandController.SetActive(true);
        }
        else if (other.tag == "StartZone")
        {
            experimentControllerScript.doControllerReattachOperations("null");
            mistakeLineObj.SetActive(false);
            experimentControllerScript.feedbackEnabled = false;
            //experimentControllerScript.startStopRefController.SetActive(false);            
            experimentControllerScript.solidRightHandController.SetActive(true);
            experimentControllerScript.ghostRightHandController.SetActive(false);
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
            experimentControllerScript.stopMistakeFeedback();
            mistakeLineObj.SetActive(false);
            if (experimentControllerScript.client != null && experimentControllerScript.expState != ExperimentState.VR_TUTORIAL)
                experimentControllerScript.client.Write("M;1;;;LeftSwitchPressed;Left Switch Pressed\r\n");
        }
        else if (other.tag == "StopZone")
        {
            //startStopLight.SetActive(true);
            experimentControllerScript.stopMistakeFeedback();
            mistakeLineObj.SetActive(false);
            if (experimentControllerScript.client != null && experimentControllerScript.expState != ExperimentState.VR_TUTORIAL)
                experimentControllerScript.client.Write("M;1;;;RightSwitchPressed;Right Switch Pressed\r\n");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        --numCollidersInContact;
        if (other.tag != "StartZone" && other.tag != "StopZone" && experimentControllerScript.feedbackEnabled)
        {
            //startStopLight.SetActive(false);

            //Debug.Log("Exit from " + other.gameObject);
            //Debug.Log("Collider tag - " + other.gameObject.tag);
            oldCollider = other;
            if (numCollidersInContact < 1)
            {
                //Debug.Log("Number of colliders in contact is less than 1. Triggering feedback");
                //currCollider = null;
                experimentControllerScript.doControllerDetachOperations((CapsuleCollider)other, other.gameObject.tag, loc);
                //experimentControllerScript.triggerMistakeFeedback();
                //Find the vector between the two points
                mistakeVector = experimentControllerScript.projectedHookPos - transform.position;
                //experimentControllerScript.changeIntensityOfGhost(math.remap(0, 127, 0, 1, mistakeVector.magnitude));

                mistakeDirection = mistakeVector.normalized;
                float mistakeDepth = mistakeVector.magnitude;
                experimentControllerScript.triggerMistakeFeedback(mistakeDirection, mistakeDepth);
                mistakeLineObj.SetActive(true);
            }
        }
        else if (other.tag == "StartZone")
        {
            Debug.Log("Level started!");
            //if(!experimentControllerScript.tutorialPhase) other.enabled = false;
            experimentControllerScript.feedbackEnabled = true;
            //experimentControllerScript.startStopRefController.SetActive(false);
            experimentControllerScript.solidRightHandController.SetActive(true);
            experimentControllerScript.ghostRightHandController.SetActive(false);
        }
    }

    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(loc, 0.005f);
    }*/
}
