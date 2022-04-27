
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using SimpleTCP;
using UnityEngine.UI;

public class HapticsController : MonoBehaviour
{
    //public SimpleTcpClient client;

    public int currLevel;

    public Vector3[] startPositions;
    public Vector3[] stopPositions;

    public GameObject level1, tutorial;

    public GameObject env;
    //public AudioSource beepsound;
    //public AudioSource goSound;
    public GameObject hookRoot;
    //public GameObject originalHookModel;
    //public GameObject hookChildWithColliders;

    public Quaternion hookRootDefaultRot;
    public Vector3 hookRootDefaultPos;
    public Quaternion solidRightHandControllerDefaultRot;
    public Vector3 solidRightHandControllerDefaultPos;

    //public GameObject righthandController;
    public GameObject startStopRefController;
    public GameObject ghostRightHandController;
    public GameObject solidRightHandController;
    //public GameObject snapSlot;
    //public GameObject rightHandAnchor;
    //bool checkSnapCondition;
    Vector3 detachPt;
    Vector3 offsetGhostDistance;
    Vector3 oldGhostPos;
    GameObject detachPivot;
    public bool isFeedbackOnNow = false;

    public Vector3 projectedHookPos;

    //public GameObject testArduinoSerialControllerObj;
    //public SerialController testArduinoSerialController;

    //UI
    /*public TMPro.TMP_Text modeTxt;
    public TMPro.TMP_Text iMotionsConnText;
    public Image leftSwitchIndicator, rightSwitchIndicator, mistakeIndicator;
    public GameObject baselineOverIndicator;
    public GameObject restOverIndicator;*/


    public bool feedbackEnabled = false;

    public bool isDetached = false;
    enum Direction { xDir, yDir, zDir };
    CapsuleCollider currCollider;
    string currDragDir;
    public Vector3 offsetPivotAng;

    public bool trainingPhase, tutorialPhase;

    //[Header("Materials")]
    //public Material lightOffMat;
    //public Material lightOnMat;

    //public GameObject mistakeLight;

    private void Awake()
    {
        //testArduinoSerialController.portName = PlayerPrefs.GetString("testCOMPort", "not_set");
        //Debug.Log("Test COM port set - " + testArduinoSerialController.portName);

        //testArduinoSerialControllerObj.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        trainingPhase = true;
        tutorialPhase = false;
        //beepsound.mute = true;
        currLevel = 1;

        solidRightHandControllerDefaultRot = solidRightHandController.transform.localRotation;
        solidRightHandControllerDefaultPos = solidRightHandController.transform.localPosition;
        detachPivot = new GameObject("DetachPivot");
        detachPivot.transform.position = Vector3.zero;
        oldGhostPos = ghostRightHandController.transform.position;

        //startStopRefController.transform.position = startPositions[currLevel - 1];
        solidRightHandController.SetActive(false);
        ghostRightHandController.SetActive(true);

        //checkSnapCondition = false;
        //doControllerDetachOperations(null, "",  startPositions[currLevel - 1], true);
        //Debug.Log("startPositions[currLevel - 1].transform.position" + transform.TransformPoint(startPositions[currLevel - 1]));
        //client = new SimpleTcpClient().Connect("127.0.0.1", 8089);
    }

    float translateFactor = 0.001f;

    // Update is called once per frame
    void Update()
    {


    }

    public void startBaseline()
    {
        //if (client != null)
        //client.Write("M;1;;;baseline_started;Baseline started\r\n");

        StartCoroutine(startBaselineCounterCoroutine());
    }

    public IEnumerator startBaselineCounterCoroutine()
    {
        //modeTxt.text = "Baseline Active";
        Debug.Log("Baseline started");
        int seconds = 180;
        while (seconds > 0)
        {
            yield return new WaitForSecondsRealtime(1);
            //baselineOverIndicator.GetComponentInChildren<Text>().text = "" + seconds + "s";
            seconds--;
        }
        //if (client != null)
        //client.Write("M;1;;;baseline_over;Baseline over\r\n");

        //baselineOverIndicator.SetActive(true);
        //baselineOverIndicator.GetComponentInChildren<Text>().text = "Baseline over";
        //Debug.Log("delayResetNewTasksFlag");
    }

    /*public void startTutorial()
    {
        modeTxt.text = "Tutorial Mode On";
        tutorialPhase = true;
    }*/

    public void moveToLevel(int level)
    {
        //modeTxt.text = "Training Mode On";
        //trainingPhase = true;
        if (level == 0)
        {
            currLevel = 1;
            //modeTxt.text = "Tutorial Mode On";
            tutorialPhase = true;
            tutorial.SetActive(true);
            level1.SetActive(false);
            env.transform.eulerAngles = new Vector3(0, 0, 0);
        }
        if (level == 1)
        {
            tutorialPhase = false;
            currLevel = 1;
            tutorial.SetActive(false);
            level1.SetActive(true);
            env.transform.eulerAngles = new Vector3(0, 0, 0);
            //goSound.Play();
            //if (client != null)
            //client.Write("M;1;;;level_1_started;Level 1 started\r\n");
        }
        if (level == 2)
        {
            tutorialPhase = false;
            currLevel = 2;
            env.transform.eulerAngles = new Vector3(0, -90, 0);
            //goSound.Play();
            //if (client != null)
            //client.Write("M;1;;;level_2_started;Level 2 started\r\n");
        }
        if (level == 3)
        {
            tutorialPhase = false;
            currLevel = 3;
            env.transform.eulerAngles = new Vector3(0, -180, 0);
            //goSound.Play();
            //if (client != null)
            //client.Write("M;1;;;level_3_started;Level 3 started\r\n");
        }
        if (level == 4)
        {
            tutorialPhase = false;
            currLevel = 4;
            env.transform.eulerAngles = new Vector3(0, -270, 0);
            //goSound.Play();
            //if (client != null)
            //client.Write("M;1;;;level_4_started;Level 4 started\r\n");
        }

        //startStopRefController.transform.position = startPositions[currLevel - 1];

    }

    public void startTestMode()
    {
        trainingPhase = false;
    }

    public void startTest(int stage)
    {
        //goSound.Play();
        if (stage == 1)
        {
            //modeTxt.text = "Pre Test Active";
            trainingPhase = false;
            //if (client != null)
            //client.Write("M;1;;;pre_test_started;Test (pre) started\r\n");
        }
        if (stage == 2)
        {
            //modeTxt.text = "Post Test Active";
            trainingPhase = false;
            //if (client != null)
            //client.Write("M;1;;;post_test_started;Test (post) started\r\n");
        }
    }

    public void startRest()
    {
        StartCoroutine(startRestCoroutine());
    }

    public IEnumerator startRestCoroutine()
    {
        int seconds = 30;
        while (seconds > 0)
        {
            yield return new WaitForSecondsRealtime(1);
            //restOverIndicator.GetComponentInChildren<Text>().text = "" + seconds + "s";
            seconds--;
        }
        //if (client != null)
        //client.Write("M;1;;;rest_over;Rest over\r\n");
        //goSound.Play();
        //restOverIndicator.GetComponentInChildren<Text>().text = "Rest over";
        //Debug.Log("delayResetNewTasksFlag");
    }

    public void startRest(int level)
    {
        //modeTxt.text = "Level " + level + " Active";
        trainingPhase = true;
        if (level == 1)
        {
            startRest();
            //if (client != null)
            {
                //   client.Write("M;1;;;level_1_started;Level 1 started\r\n");
                // client.Write("M;1;;;level_1_rest_started;Level 1 rest started\r\n");
            }
        }
        if (level == 2)
        {
            startRest();
            //if (client != null)
            {
                // client.Write("M;1;;;level_2_started;Level 2 started\r\n");
                // client.Write("M;1;;;level_2_rest_started;Level 2 rest started\r\n");
            }
        }
        if (level == 3)
        {
            startRest();
            //if (client != null)
            {
                //client.Write("M;1;;;level_3_started;Level 3 started\r\n");
                // client.Write("M;1;;;level_3_rest_started;Level 3 rest started\r\n");
            }
        }
        if (level == 4)
        {
            startRest();
            //if (client != null)
            {
                // client.Write("M;1;;;level_4_started;Level 4 started\r\n");
                //client.Write("M;1;;;level_4_rest_started;Level 4 rest started\r\n");
            }
        }
    }

    /*public void gotoNextLevel()
    {
        ++currLevel;
        if (currLevel == 5) currLevel = 1;
        startStopRefController.transform.position = startPositions[currLevel - 1];
        env.transform.Rotate(0, -90, 0);
    }*/

    public void stopFeedback()
    {

    }

    private void FixedUpdate()
    {
        /*if (client == null)
        {
            iMotionsConnText.color = Color.red;
            iMotionsConnText.text = "iMotions Disconnected";
        }
        else
        {
            iMotionsConnText.color = Color.green;
            iMotionsConnText.text = "iMotions Connected";
        }*/


        /*if (checkSnapCondition)
        {
            if (Vector3.Distance(solidRightHandController.transform.position, ghostRightHandController.transform.position) < 0.01f)
            {
                //Debug.Log("Snap!");
            }
        }*/

        offsetGhostDistance = ghostRightHandController.transform.position - oldGhostPos;
        oldGhostPos = ghostRightHandController.transform.position;

        string message;

        if (trainingPhase || tutorialPhase)
        {
            message = null;
            if (isDetached && feedbackEnabled)
            {
                //StartCoroutine(Haptics(1, 1, 0.1f, true, false));
                //if (client != null && !tutorialPhase)
                //client.Write("M;1;;;BuzzWireHit;Buzz wire was hit\r\n");
                //Debug.Log("isDetached = true");
                if (currDragDir == "x-axis")
                {
                    //Vector3 projectedPos = new Vector3(ghostRightHandController.transform.position.x, solidRightHandController.transform.position.y, solidRightHandController.transform.position.z);
                    //if (projectedPos.x < currCollider.bounds.max.x && projectedPos.x > currCollider.bounds.min.x)
                    //    solidRightHandController.transform.position = projectedPos;
                    projectedHookPos = detachPivot.transform.position + new Vector3(offsetGhostDistance.x, 0, 0);
                    if (projectedHookPos.x < currCollider.bounds.max.x && projectedHookPos.x > currCollider.bounds.min.x)
                        detachPivot.transform.position = projectedHookPos;

                    //detachPivot.transform.eulerAngles = ghostRightHandController.transform.eulerAngles;// + offsetPivotAng;

                }
                else if (currDragDir == "y-axis")
                {
                    //solidRightHandController.transform.position = new Vector3(solidRightHandController.transform.position.x, ghostRightHandController.transform.position.y, solidRightHandController.transform.position.z);
                    projectedHookPos = detachPivot.transform.position + new Vector3(0, offsetGhostDistance.y, 0);
                    if (projectedHookPos.y < currCollider.bounds.max.y && projectedHookPos.y > currCollider.bounds.min.y)
                        detachPivot.transform.position = projectedHookPos;
                    //detachPivot.transform.eulerAngles = ghostRightHandController.transform.eulerAngles;
                }
                else if (currDragDir == "z-axis")
                {
                    //solidRightHandController.transform.position = new Vector3(solidRightHandController.transform.position.x, solidRightHandController.transform.position.y, ghostRightHandController.transform.position.z);
                    projectedHookPos = detachPivot.transform.position + new Vector3(0, 0, offsetGhostDistance.z);
                    if (projectedHookPos.z < currCollider.bounds.max.z && projectedHookPos.z > currCollider.bounds.min.z)
                        detachPivot.transform.position = projectedHookPos;
                }
            }
            else
            {
                //Debug.Log("isDetached = false");
            }
        }
        else
        {
            //message = testArduinoSerialController.ReadSerialMessage();
        }

        /*if (message == null)
        {
            //leftSwitchIndicator.color = Color.gray;
            //rightSwitchIndicator.color = Color.gray;
            //mistakeIndicator.color = Color.gray;
            if (!trainingPhase && !tutorialPhase)
            {
                //beepsound.mute = true;
            }
            return;
        }

        // Check if the message is plain data or a connect/disconnect event.
        if (ReferenceEquals(message, SerialController.SERIAL_DEVICE_CONNECTED))
            Debug.Log("Connection established");
        else if (ReferenceEquals(message, SerialController.SERIAL_DEVICE_DISCONNECTED))
            Debug.Log("Connection attempt failed or disconnection detected");
        else
        {
            //Debug.Log("Message arrived: " + message);
            if (message == "1")
            {
                //mistakeIndicator.color = Color.red;
                //if (client != null)
                //client.Write("M;1;;;BuzzWireHit;Buzz wire was hit\r\n");
            }
            else
            {
                //beepsound.mute = true;
            }

            if (message == "+")
            {
                //leftSwitchIndicator.color = Color.green;
                //if (client != null)
                //client.Write("M;1;;;LeftSwitchPressed;Left Switch Pressed\r\n");
            }
            else if (message == "*")
            {
                //rightSwitchIndicator.color = Color.green;
                //if (client != null)
                //client.Write("M;1;;;RightSwitchPressed;Right Switch Pressed\r\n");
            }
            else
            {
                //leftSwitchIndicator.color = Color.gray;
                //rightSwitchIndicator.color = Color.gray;
            }
            message = "";
        }
        */

    }

    public void doControllerDetachOperations(CapsuleCollider _collider, string tag, Vector3 _detachPt)
    {
        currCollider = _collider;
        //Debug.Log("isDetached = true, collision with " + tag);
        isDetached = true;
        currDragDir = tag; //x-dir, y-dir or z-dir

        detachPt = _detachPt;
        detachPivot.transform.position = detachPt;

        solidRightHandController.SetActive(true);
        solidRightHandController.transform.SetParent(detachPivot.transform);
        ghostRightHandController.SetActive(true);
    }

    public Vector3 rotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        return Quaternion.Euler(angles) * (point - pivot) + pivot;
    }

    public void doControllerReattachOperations(string tag)
    {        
        //Debug.Log("isDetached = false, collision with " + tag);
        isDetached = false;

        ghostRightHandController.SetActive(false);

        solidRightHandController.transform.SetParent(hookRoot.transform);
        solidRightHandController.transform.localRotation = solidRightHandControllerDefaultRot;
        solidRightHandController.transform.localPosition = solidRightHandControllerDefaultPos;

    }

    public void triggerMistakeFeedback()
    {
        isFeedbackOnNow = true;
        Debug.Log("triggerMistakeFeedback");
        //GetComponent<VibrationDemoScript>().TurnEffectOn();
        //StartCoroutine(Haptics(1, 1, 0.1f, true, false));
        //beepsound.mute = false;
        //mistakeLight.GetComponent<MeshRenderer>().material = lightOnMat;
        //mistakeLight.SetActive(true);

    }

    public void stopMistakeFeedback()
    {
        isFeedbackOnNow = false;
        //beepsound.mute = true;
        //GetComponent<VibrationDemoScript>().TurnEffectOff();


        //mistakeLight.SetActive(false);
    }

    /*IEnumerator Haptics(float frequency, float amplitude, float duration, bool rightHand, bool leftHand)
    {
        if (rightHand) OVRInput.SetControllerVibration(frequency, amplitude, OVRInput.Controller.RTouch);
        if (leftHand) OVRInput.SetControllerVibration(frequency, amplitude, OVRInput.Controller.LTouch);

        yield return new WaitForSeconds(duration);

        if (rightHand) OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
        if (leftHand) OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.LTouch);
    }*/

    private void OnApplicationQuit()
    {
        //client.Disconnect();
    }

}
