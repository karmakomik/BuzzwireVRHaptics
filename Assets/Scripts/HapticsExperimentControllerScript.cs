
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleTCP;
using UnityEngine.UI;

public enum ExperimentState
{
    INIT,
    BASELINE,
    PRE_TEST,
    MEDIUM_PRE_SURVEY,
    VR_TUTORIAL,
    TRAINING_1_MEDIUM_FEEDBACK,
    MEDIUM_POST_SURVEY,
    POST_TEST_1,
    HILO_PRE_SURVEY,
    TRAINING_2_HIGH_FEEDBACK,
    TRAINING_2_LOW_FEEDBACK,
    HILO_POST_SURVEY,
    POST_TEST_2
};
public enum FeedbackLevel
{
    LOW,
    MEDIUM,
    HIGH
};

public enum ExperimentalCondition
{ 
    GROUNDED,
    VIBRATION,
    NO_HAPTICS
}

public class HapticsExperimentControllerScript : MonoBehaviour
{
    public FeedbackLevel feedbackLevel;
    public ExperimentState expState;
    public static ExperimentalCondition expCondition;
    public SimpleTcpClient client;

    public int currLevel;

    public Vector3[] startPositions;
    public Vector3[] stopPositions;

    public GameObject level1, level2, level3, tutorial, surveyPanel;

    public GameObject configMenu;
    public Slider avatar_x_slider, avatar_y_slider, avatar_z_slider, hapticEnv_x_slider, hapticEnv_y_slider, hapticEnv_z_slider;

    //public GameObject 

    public GameObject env;

    public AudioSource goSound;
    public GameObject hookRoot, hook_difficulty_1_root, hook_difficulty_2_root, hook_difficulty_3_root;
    public GameObject cylinderPointer;


    public Quaternion hookRootDefaultRot;
    public Vector3 hookRootDefaultPos;
    public Quaternion solidRightHandControllerDefaultRot;
    public Vector3 solidRightHandControllerDefaultPos;

    //public GameObject righthandController;
    public GameObject startStopRefController;
    public GameObject ghostRightHandController;
    public GameObject solidRightHandController;
    public GameObject ghost_difficulty_1;//, ghost_difficulty_2, ghost_difficulty_3;
    public GameObject ghost_wire, ghost_handle;
    public GameObject solid_difficulty_1;//, solid_difficulty_2, solid_difficulty_3;

    public GameObject levelTimeResultsObj;
    public int levelTimeResult;

    //public GameObject snapSlot;
    //public GameObject rightHandAnchor;
    //bool checkSnapCondition;
    Vector3 detachPt;
    Vector3 offsetGhostDistance;
    Vector3 oldGhostPos;
    GameObject detachPivot;
    public bool isFeedbackOnNow = false;

    public Vector3 projectedHookPos;
    public GameObject mistakeLineObj;
    public GameObject lishengDeviceObj;
    public LishengDeviceControllerScript lishengDeviceController;
    public FormHandlerScript formHandler;

    public GameObject hapticEnv, avatar;
    
    //public SerialController hapticArduinoSerialController;


    public GameObject testArduinoSerialControllerObj;
    public SerialController testArduinoSerialController;

    //UI
    public TMPro.TMP_Text modeTxt;
    public TMPro.TMP_Text conditionTxt;
    public TMPro.TMP_Text iMotionsConnText;
    public Image leftSwitchIndicator, rightSwitchIndicator, mistakeIndicator;
    public GameObject baselineOverIndicator;
    public GameObject restOverIndicator;


    public bool feedbackEnabled = false;

    public bool isDetached = false;
    enum Direction { xDir, yDir, zDir };
    CapsuleCollider currCollider;
    string currDragDir;
    public Vector3 offsetPivotAng;


    private void Awake()
    {
        testArduinoSerialController.portName = PlayerPrefs.GetString("testCOMPort", "not_set");
        
        Debug.Log("Test COM port set - " + testArduinoSerialController.portName);

        testArduinoSerialControllerObj.SetActive(true);

        lishengDeviceObj.GetComponent<SerialController>().portName = PlayerPrefs.GetString("hapticCOMPort", "not_set");
        Debug.Log("Lisheng Device COM port set - " + lishengDeviceObj.GetComponent<SerialController>().portName);

        lishengDeviceObj.SetActive(true);

        if (PlayerPrefs.GetString("avatar_x", "not_set") == "not_set" || PlayerPrefs.GetString("avatar_y", "not_set") == "not_set" || PlayerPrefs.GetString("avatar_z", "not_set") == "not_set")
        {
            Debug.Log("Avatar pos not set");
        }
        else
        {
            //print("Player prefs avatar_z" + PlayerPrefs.GetString("avatar_z", "not_set"));
            //avatar.transform.position = new Vector3(float.Parse(PlayerPrefs.GetString("avatar_x", "not_set")), float.Parse(PlayerPrefs.GetString("avatar_y", "not_set")), float.Parse(PlayerPrefs.GetString("avatar_z", "not_set")));
            avatar_x_slider.value = float.Parse(PlayerPrefs.GetString("avatar_x", "not_set"));
            avatar_y_slider.value = float.Parse(PlayerPrefs.GetString("avatar_y", "not_set"));
            avatar_z_slider.value = float.Parse(PlayerPrefs.GetString("avatar_z", "not_set"));
        }

        if (PlayerPrefs.GetString("hapticEnv_x", "not_set") == "not_set" || PlayerPrefs.GetString("hapticEnv_y", "not_set") == "not_set" || PlayerPrefs.GetString("hapticEnv_z", "not_set") == "not_set")
        {
            Debug.Log("Haptic Env pos not set");
        }
        else
        {
            hapticEnv.transform.position = new Vector3(float.Parse(PlayerPrefs.GetString("hapticEnv_x", "not_set")), float.Parse(PlayerPrefs.GetString("hapticEnv_y", "not_set")), float.Parse(PlayerPrefs.GetString("hapticEnv_z", "not_set")));
            hapticEnv_x_slider.value = float.Parse(PlayerPrefs.GetString("hapticEnv_x", "not_set"));
            hapticEnv_y_slider.value = float.Parse(PlayerPrefs.GetString("hapticEnv_y", "not_set"));
            hapticEnv_z_slider.value = float.Parse(PlayerPrefs.GetString("hapticEnv_z", "not_set"));            
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        expState = ExperimentState.INIT;
        feedbackLevel = FeedbackLevel.MEDIUM;

        if(expCondition == ExperimentalCondition.GROUNDED)
            conditionTxt.text = "Grounded";
        else if (expCondition == ExperimentalCondition.NO_HAPTICS)
            conditionTxt.text = "No Haptics";
        else if (expCondition == ExperimentalCondition.VIBRATION)
            conditionTxt.text = "Vibration";
        //expCondition = ExperimentalCondition.VIBRATION; //REMOVE!!

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
        client = new SimpleTcpClient().Connect("127.0.0.1", 8089);

        changeState("INIT");
    }

    float translateFactor = 0.001f;

    // Update is called once per frame
    void Update()
    {


    }

    public void selectExperimentCondition(string expCondStr)
    {
        switch (expCondStr)
        {
            case "GROUNDED":
                expCondition = ExperimentalCondition.GROUNDED;
                break;
            case "VIBRATION":
                expCondition = ExperimentalCondition.VIBRATION;
                break;
            case "NO_HAPTICS":
                expCondition = ExperimentalCondition.NO_HAPTICS;
                break;
            default:
                print("Invalid experimental condition");
                break;
        }
    }

    public void toggleConfigMenu()
    {
        if (configMenu.activeSelf)
        {
            configMenu.SetActive(false);
        }
        else
        {
            configMenu.SetActive(true);
        }
    }

    public void saveConfig()
    {
        PlayerPrefs.SetString("avatar_x", avatar_x_slider.value.ToString());
        PlayerPrefs.SetString("avatar_y", avatar_y_slider.value.ToString());
        PlayerPrefs.SetString("avatar_z", avatar_z_slider.value.ToString());
        PlayerPrefs.SetString("hapticEnv_x", hapticEnv_x_slider.value.ToString());
        PlayerPrefs.SetString("hapticEnv_y", hapticEnv_y_slider.value.ToString());
        PlayerPrefs.SetString("hapticEnv_z", hapticEnv_z_slider.value.ToString());
        PlayerPrefs.Save();
    }


    public void showLevelResult(int _levelTimeResult)
    {
        levelTimeResultsObj.SetActive(true);
        levelTimeResultsObj.transform.GetChild(0).GetChild(0).GetComponent<TMPro.TMP_Text>().text = "You took " + _levelTimeResult + " seconds. \n Come on, you can do it faster!";
    }


    public void changeState(string state)
    {
        switch (state)
        {
            case "INIT":
                expState = ExperimentState.INIT;
                break;
            case "BASELINE":
                expState = ExperimentState.BASELINE;
                startBaseline();
                break;
            case "PRE_TEST":
                expState = ExperimentState.PRE_TEST;
                goSound.Play();                    
                modeTxt.text = "Pre Test Active";                    
                if (client != null)
                    client.Write("M;1;;;pre_test_started;\r\n");                
                break;
            case "VR_TUTORIAL":
                env.SetActive(true);
                surveyPanel.SetActive(false);
                modeTxt.text = "Tutorial Mode On";
                hookRoot.SetActive(true);
                cylinderPointer.SetActive(false);
                expState = ExperimentState.VR_TUTORIAL;
                modeTxt.text = "Tutorial";
                setAllLevelsInactive();
                tutorial.SetActive(true);
                break;
            case "TRAINING_1_MEDIUM_FEEDBACK":
                env.SetActive(true);
                hookRoot.SetActive(true);
                cylinderPointer.SetActive(false);
                expState = ExperimentState.TRAINING_1_MEDIUM_FEEDBACK;
                modeTxt.text = "Training Medium Feedback";
                lishengDeviceController.vibration_level = 60;
                if (client != null)
                    client.Write("M;1;;;medium_training_started;\r\n");
                break;
            case "MEDIUM_PRE_SURVEY":
                env.SetActive(false);
                hookRoot.SetActive(false);
                levelTimeResultsObj.SetActive(false);
                cylinderPointer.SetActive(true);
                surveyPanel.SetActive(true);
                formHandler.startSurveyMediumPre();
                modeTxt.text = "Pre Survey";
                expState = ExperimentState.MEDIUM_PRE_SURVEY;
                if (client != null)
                    client.Write("M;1;;;medium_pre_survey_started;\r\n");
                break;
            case "MEDIUM_POST_SURVEY":
                env.SetActive(false);
                hookRoot.SetActive(false);
                levelTimeResultsObj.SetActive(false);
                cylinderPointer.SetActive(true);
                surveyPanel.SetActive(true);
                formHandler.startSurveyMediumPost();
                modeTxt.text = "Post Survey";
                expState = ExperimentState.MEDIUM_POST_SURVEY;
                if (client != null)
                    client.Write("M;1;;;medium_post_survey_started;\r\n");
                break;
            case "HILO_PRE_SURVEY":
                env.SetActive(false);
                hookRoot.SetActive(false);
                levelTimeResultsObj.SetActive(false);
                cylinderPointer.SetActive(true);
                surveyPanel.SetActive(true);
                formHandler.startSurveyHiLoPre();
                modeTxt.text = "Pre Survey";
                expState = ExperimentState.HILO_PRE_SURVEY;
                if (client != null)
                    client.Write("M;1;;;hilo_pre_survey_started;\r\n");
                break;
            case "HILO_POST_SURVEY":
                env.SetActive(false);
                hookRoot.SetActive(false);
                levelTimeResultsObj.SetActive(false);
                cylinderPointer.SetActive(true);
                surveyPanel.SetActive(true);
                formHandler.startSurveyHiLoPost();
                modeTxt.text = "Post Survey";
                expState = ExperimentState.HILO_POST_SURVEY;
                if (client != null)
                    client.Write("M;1;;;hilo_post_survey_started;\r\n");
                break;
            case "POST_TEST_1":
                surveyPanel.SetActive(false);
                expState = ExperimentState.POST_TEST_1;
                modeTxt.text = "Post Test Active";                
                if (client != null)
                    client.Write("M;1;;;post_test_1_started;\r\n");
                break;
            case "TRAINING_2_HIGH_FEEDBACK":
                env.SetActive(true);
                hookRoot.SetActive(true);
                cylinderPointer.SetActive(false);
                lishengDeviceController.vibration_level = 127;
                modeTxt.text = "Training High Feedback";
                expState = ExperimentState.TRAINING_2_HIGH_FEEDBACK;
                if (client != null)
                    client.Write("M;1;;;training_high_started;\r\n");
                break;
            case "TRAINING_2_LOW_FEEDBACK":
                hookRoot.SetActive(true);
                cylinderPointer.SetActive(false);
                expState = ExperimentState.TRAINING_2_LOW_FEEDBACK;
                modeTxt.text = "Training Low Feedback";
                lishengDeviceController.vibration_level = 20;
                if (client != null)
                    client.Write("M;1;;;training_low_started;\r\n");
                break;
            case "POST_TEST_2":
                surveyPanel.SetActive(false);
                expState = ExperimentState.POST_TEST_2;
                modeTxt.text = "Post Test Active";
                if (client != null)
                    client.Write("M;1;;;post_test_2_started;\r\n");
                break;
            default:
                break;
        }
    }

    public void changeFeedbackLevel(string level)
    {
        /*
        switch (level)
        {
            case "HIGH":
                feedbackLevel = FeedbackLevel.HIGH;
                break;
            case "MEDIUM":
                feedbackLevel = FeedbackLevel.MEDIUM;
                break;
            case "LOW":
                feedbackLevel = FeedbackLevel.LOW;
                break;
            default:
                break;
        }*/

    }

    public void changeIntensityOfGhost(float level)
    {
        // Change alpha of ghostRightHandController material
        ghostRightHandController.GetComponent<Renderer>().material.color = new Color(ghostRightHandController.GetComponent<Renderer>().material.color.r, ghostRightHandController.GetComponent<Renderer>().material.color.g, ghostRightHandController.GetComponent<Renderer>().material.color.b, level);
        //ghost_handle.GetComponent<Renderer>().material.color = new Color(ghostRightHandController.GetComponent<Renderer>().material.color.r, ghostRightHandController.GetComponent<Renderer>().material.color.g, ghostRightHandController.GetComponent<Renderer>().material.color.b, level);
        ghost_wire.GetComponent<Renderer>().material.color = new Color(ghostRightHandController.GetComponent<Renderer>().material.color.r, ghostRightHandController.GetComponent<Renderer>().material.color.g, ghostRightHandController.GetComponent<Renderer>().material.color.b, level);
    }

    public void startBaseline()
    {
        if (client != null)
            client.Write("M;1;;;baseline_started;Baseline started\r\n");

        StartCoroutine(startBaselineCounterCoroutine());
    }

    public IEnumerator startBaselineCounterCoroutine()
    {
        modeTxt.text = "Baseline Active";
        Debug.Log("Baseline started");
        int seconds = 180;
        while (seconds > 0)
        {
            yield return new WaitForSecondsRealtime(1);
            baselineOverIndicator.GetComponentInChildren<Text>().text = "" + seconds + "s";
            seconds--;
        }
        if (client != null)
            client.Write("M;1;;;baseline_over;Baseline over\r\n");

        baselineOverIndicator.SetActive(true);
        baselineOverIndicator.GetComponentInChildren<Text>().text = "Baseline over";
        //Debug.Log("delayResetNewTasksFlag");
    }

    /*public void startTutorial()
    {
        modeTxt.text = "Tutorial Mode On";
        tutorialPhase = true;
    }*/

    void setAllLevelsInactive()
    {
        tutorial.SetActive(false);
        level1.SetActive(false);
        level2.SetActive(false);
        level3.SetActive(false);
    }

    void disableAllHookModels()
    {

        
    }
    
    public void moveToLevel(int level)
    {
        mistakeLineObj.GetComponent<LineRenderer>().startColor = Color.black;
        mistakeLineObj.GetComponent<LineRenderer>().endColor = Color.black;
        //modeTxt.text = "Training Mode On";
        //trainingPhase = true;
        if (level == 0)
        {
            setAllLevelsInactive();
            tutorial.SetActive(true);
            hookRoot.SetActive(false);
            hookRoot = hook_difficulty_1_root;
            ghostRightHandController = ghost_difficulty_1;
            solidRightHandController = solid_difficulty_1;
            hookRoot.SetActive(true);
            /*currLevel = 1;
            //modeTxt.text = "Tutorial Mode On";
            tutorialPhase = true;
            tutorial.SetActive(true);
            level1.SetActive(false);
            //env.transform.eulerAngles = new Vector3(0, 0, 0);*/
        }
        if (level == 1)
        {
            setAllLevelsInactive();
            levelTimeResultsObj.SetActive(false);
            level1.SetActive(true);
            hookRoot.SetActive(false);
            hookRoot = hook_difficulty_1_root;
            ghostRightHandController = ghost_difficulty_1;
            solidRightHandController = solid_difficulty_1;
            hookRoot.SetActive(true);
            /*tutorialPhase = false;
            currLevel = 1;
            tutorial.SetActive(false);
            level1.SetActive(true);
            env.transform.eulerAngles = new Vector3(0, 0, 0);
            //goSound.Play();
            /if (client != null)
            //client.Write("M;1;;;level_1_started;Level 1 started\r\n");
            */
            if (client != null)
                client.Write("M;1;;;level_1_started;\r\n");
        }
        if (level == 2)
        {
            setAllLevelsInactive();
            levelTimeResultsObj.SetActive(false);
            level2.SetActive(true);
            hookRoot.SetActive(false);
            hookRoot = hook_difficulty_1_root;
            ghostRightHandController = ghost_difficulty_1;
            solidRightHandController = solid_difficulty_1;
            hookRoot.SetActive(true);
            /*tutorialPhase = false;
            currLevel = 2;
            env.transform.eulerAngles = new Vector3(0, -90, 0);
            //goSound.Play();
            //if (client != null)
            //client.Write("M;1;;;level_2_started;Level 2 started\r\n");*/
            if (client != null)
                client.Write("M;1;;;level_2_started;\r\n");
        }
        if (level == 3)
        {
            setAllLevelsInactive();
            levelTimeResultsObj.SetActive(false);
            level3.SetActive(true);
            hookRoot.SetActive(false);
            hookRoot = hook_difficulty_1_root;
            ghostRightHandController = ghost_difficulty_1;
            solidRightHandController = solid_difficulty_1;
            hookRoot.SetActive(true);
            /*tutorialPhase = false;
            currLevel = 3;
            env.transform.eulerAngles = new Vector3(0, -180, 0);
            //goSound.Play();
            //if (client != null)
            //client.Write("M;1;;;level_3_started;Level 3 started\r\n");*/
            if (client != null)
                client.Write("M;1;;;level_3_started;\r\n");
        }

        //startStopRefController.transform.position = startPositions[currLevel - 1];

    }


    public void changeHapticEnv_x(float x)
    {
        hapticEnv.transform.position = new Vector3(x, hapticEnv.transform.position.y, hapticEnv.transform.position.z);
    }

    public void changeHapticEnv_y(float y)
    {
        hapticEnv.transform.position = new Vector3(hapticEnv.transform.position.x, y, hapticEnv.transform.position.z);
    }

    public void changeHapticEnv_z(float z)
    {
        hapticEnv.transform.position = new Vector3(hapticEnv.transform.position.x, hapticEnv.transform.position.y, z);
    }

    

    public void changeAvatar_x(float x)
    {
        avatar.transform.position = new Vector3(x, avatar.transform.position.y, avatar.transform.position.z);
    }

    public void changeAvatar_y(float y)
    {
        avatar.transform.position = new Vector3(avatar.transform.position.x, y, avatar.transform.position.z);
    }

    public void changeAvatar_z(float z)
    {
        avatar.transform.position = new Vector3(avatar.transform.position.x, avatar.transform.position.y, z);
    }


    public void startTest(int stage)
    {

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
            restOverIndicator.GetComponentInChildren<Text>().text = "" + seconds + "s";
            seconds--;
        }
        if (client != null)
            client.Write("M;1;;;rest_over;Rest over\r\n");
        goSound.Play();
        restOverIndicator.GetComponentInChildren<Text>().text = "Rest over";
        //Debug.Log("delayResetNewTasksFlag");
    }

    public void startRest(int level)
    {
        //modeTxt.text = "Level " + level + " Active";
        //trainingPhase = true;
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
        if (client == null)
        {
            iMotionsConnText.color = Color.red;
            iMotionsConnText.text = "iMotions Disconnected";
        }
        else
        {
            iMotionsConnText.color = Color.green;
            iMotionsConnText.text = "iMotions Connected";
        }


        offsetGhostDistance = ghostRightHandController.transform.position - oldGhostPos;
        oldGhostPos = ghostRightHandController.transform.position;

        string message;

        if (expState == ExperimentState.TRAINING_1_MEDIUM_FEEDBACK || expState == ExperimentState.TRAINING_2_HIGH_FEEDBACK || expState == ExperimentState.TRAINING_2_LOW_FEEDBACK || expState == ExperimentState.VR_TUTORIAL)
        {
            message = null;
            if (isDetached && feedbackEnabled)
            {
                mistakeLineObj.SetActive(true);
                //StartCoroutine(Haptics(1, 1, 0.1f, true, false));
                if (client != null && expState != ExperimentState.VR_TUTORIAL)
                    client.Write("M;1;;;BuzzWireHitVR;\r\n");
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
                //mistakeLineObj.SetActive(true);
            }
            else
            {
                mistakeLineObj.SetActive(false);
                //Debug.Log("isDetached = false");
            }
        }
        else
        {
            message = testArduinoSerialController.ReadSerialMessage();
        }

        if (message == null)
        {
            leftSwitchIndicator.color = Color.gray;
            rightSwitchIndicator.color = Color.gray;
            mistakeIndicator.color = Color.gray;
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
                mistakeIndicator.color = Color.red;
                if (client != null)
                    client.Write("M;1;;;BuzzWireHitTest;Buzz wire was hit\r\n");
            }
            else
            {
                //beepsound.mute = true;
            }

            if (message == "+")
            {
                leftSwitchIndicator.color = Color.green;
                if (client != null)
                    client.Write("M;1;;;LeftSwitchPressedTest;Left Switch Pressed\r\n");
            }
            else if (message == "*")
            {
                rightSwitchIndicator.color = Color.green;
                if (client != null)
                    client.Write("M;1;;;RightSwitchPressedTest;Right Switch Pressed\r\n");
            }
            else
            {
                leftSwitchIndicator.color = Color.gray;
                rightSwitchIndicator.color = Color.gray;
            }
            message = "";
        }       

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

    public void vibrateInDirection(Vector3 direction)
    {
        lishengDeviceController.vibrateInDirection(direction);
    }
    
    public void triggerMistakeFeedback(string dir, Vector3 _mistakeDir, Vector3 _mistakeVector)
    {
        isFeedbackOnNow = true;
        //print("mistakeVector" + _mistakeVector);
        if (expCondition == ExperimentalCondition.VIBRATION)
        {
            vibrateInDirection(_mistakeDir);
        }
        else if(expCondition == ExperimentalCondition.GROUNDED)
        {
            //print("Depth" + _depth);
            //double[] zero = { 0.0, 0.0, 0.0 };
            //double[] force = { 0.1 * _mistakeDir.x, 0.1 * _mistakeDir.y, 0.1 * _mistakeDir.z };
            //HapticPlugin.setForce("Default Device", force, zero);
            if (dir == "x-axis") _mistakeVector.x = 0;
            else if (dir == "y-axis") _mistakeVector.y = 0;
            else if (dir == "z-axis") _mistakeVector.z = 0;
            HapticsDeviceManager.SetForce(40f * _mistakeVector);
            Debug.Log("triggerMistakeFeedback");
        }


        //hapticArduinoSerialController.SendSerialMessage("1");
        //GetComponent<VibrationDemoScript>().TurnEffectOn();
        //StartCoroutine(Haptics(1, 1, 0.1f, true, false));
        //beepsound.mute = false;
        //mistakeLight.GetComponent<MeshRenderer>().material = lightOnMat;
        //mistakeLight.SetActive(true);

    }

    void OnMessageArrived(string msg)
    {
        Debug.Log("Message from Arduino - " + msg);
    }

    // Invoked when a connect/disconnect event occurs. The parameter 'success'
    // will be 'true' upon connection, and 'false' upon disconnection or
    // failure to connect.
    void OnConnectionEvent(bool success)
    {
        Debug.Log("Arduino connected");
    }    

    public void stopMistakeFeedback()
    {
        isFeedbackOnNow = false;
        vibrateInDirection(new Vector3(0, 0, 0));
        HapticsDeviceManager.SetForce(new Vector3(0,0,0));
        //double[] zero = { 0.0, 0.0, 0.0 };
        //HapticPlugin.setForce("Default Device", zero, zero);
        //hapticArduinoSerialController.SendSerialMessage("0");

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

    public void closeApp()
    {
        Application.Quit();
    }

}
