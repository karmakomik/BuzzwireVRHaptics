using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Unity.Mathematics;
using UnityEngine.SceneManagement;

public class PilotTestControllerScript : MonoBehaviour
{
    StreamWriter sw1, sw2;

    //public HapticPlugin HapticDevice = null;

    public float vibrationDuration = 3f;

    public List<GameObject> participantChoiceDropDownLists;
    public GameObject tutorialControlUI;
    public GameObject pilotTest1ControlUI;
    public GameObject pilotTest2ControlUI;

    public Slider vibrationSlider;
    public Slider stiffnessSlider;
    public Slider dampingSlider;
    public Slider staticFrictionSlider;
    public Slider dynamicFrictionSlider;

    float vibrationSliderVal;
    float stiffnessSliderVal;
    float dampingSliderVal;
    float staticFrictionSliderVal;
    float dynamicFrictionSliderVal;

    public GameObject gotoMenuButton;

    public GameObject groundedSurfaceObj;
    public GameObject vibrationSurfaceObj;
    public GameObject testGroundedSurfaceObj;
    public GameObject initTestUI;

    public TMPro.TextMeshProUGUI vibrationSliderValueText;
    public TMPro.TextMeshProUGUI stiffnessSliderValueText;
    public TMPro.TextMeshProUGUI dampingSliderValueText;
    public TMPro.TextMeshProUGUI staticFrictionSliderValueText;
    public TMPro.TextMeshProUGUI dynamicFrictionSliderValueText;

    public GameObject currSurface;
    public GameObject currSurfaceUIText;
    int currTrialNum;
    int currMotorSeqInd;
    int currStiffnessInd;
    int currTrialSurfaceIndex;
    public GameObject currTrialNumUIText;
    public GameObject currVibrationTrialNumUIText;
    public GameObject currStiffnessLevelUIText;

    public string[,] motorSequences = new string[5, 6] {
                                { "a", "f", "b", "d", "c", "e" },
                                { "b", "a", "f", "e", "d", "c" },
                                { "c", "b", "a", "f", "e", "d" },
                                { "e", "c", "b", "a", "f", "d" },
                                { "f", "d", "c", "b", "a", "e" }};

    public string[,] stiffnessSequences = new string[6, 3] {
                                { "Low", "Middle", "High"},
                                { "Low", "Middle", "High"},
                                { "Middle", "Low", "High"},
                                { "High", "Low", "Middle"},
                                { "Middle", "High", "Low"},
                                { "Low", "High", "Middle"}};

    Dictionary<string, float> stiffnessDict = new Dictionary<string, float>()
    {
        {"Low", 0.2f},
        {"Middle", 0.6f},
        {"High", 1.0f}
    };

    Dictionary<string, string> motorDictionary = new Dictionary<string, string>()
    {
        {"a", "Down"},
        {"b", "Left"},
        {"c", "Up"},
        {"d", "Right"},
        {"e", "Front"},
        {"f", "Back"}
    };




    public GameObject hapticArduinoSerialObj;
    public SerialController hapticArduinoSerialController;

    string intensity = "000";
    string motor = "a";

    string timeOfSelection, timeVibrationStart;

    // Start is called before the first frame update
    void Start()
    {
        //initTestUI.SetActive(true);
        //testGroundedSurfaceObj.SetActive(true);
        //tutorialControlUI.SetActive(!true);
        //pilotTest1ControlUI.SetActive(false);
        //pilotTest2ControlUI.SetActive(!false);
        //groundedSurfaceObj.SetActive(!false);
        //vibrationSurfaceObj.SetActive(!false);
        currTrialNum = 0;
        currMotorSeqInd = 0;
        currStiffnessInd = 0;
        //groundedSurfaceObj.GetComponent<HapticSurface>().hlStiffness = stiffnessDict[stiffnessSequences[currTrialNum, currStiffnessInd]];

        if (!Directory.Exists(Application.persistentDataPath + "/PilotTestData"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/PilotTestData");
        }
        sw1 = new StreamWriter(Application.persistentDataPath + "/PilotTestData/HapticsPilotTestData_UserChoices_" + System.DateTime.Now.ToString("dd_MMMM_yyyy_HH_mm_ss") + ".csv");
        sw2 = new StreamWriter(Application.persistentDataPath + "/PilotTestData/HapticsPilotTestData_HapticParams_" + System.DateTime.Now.ToString("dd_MMMM_yyyy_HH_mm_ss") + ".csv");

        //if (HapticDevice == null)
        //HapticDevice = (HapticPlugin)FindObjectOfType(typeof(HapticPlugin));
    }

    public void startTutorial()
    {
        initTestUI.SetActive(false);
        testGroundedSurfaceObj.SetActive(false);
        tutorialControlUI.SetActive(true);        
    }

    public void startTest1()
    {
        initTestUI.SetActive(false);
        testGroundedSurfaceObj.SetActive(false);
        pilotTest1ControlUI.SetActive(true);
    }
    public void startTest2()
    {
        initTestUI.SetActive(false);
        testGroundedSurfaceObj.SetActive(false);
        pilotTest2ControlUI.SetActive(true);
        groundedSurfaceObj.SetActive(true);
        vibrationSurfaceObj.SetActive(true);
    }

    public void startVibration()
    {
        print("Starting vibration for motor " + motorSequences[currTrialNum, currMotorSeqInd]);
        hapticTest_startVibration_mid(motorSequences[currTrialNum, currMotorSeqInd]);
        StartCoroutine(WaitForSeconds(vibrationDuration, motorSequences[currTrialNum, currMotorSeqInd]));
    }

    public void startVibration(string motorName)
    {
        print("Starting vibration for motor " + motorName);
        timeVibrationStart = System.DateTime.Now.ToString("HH:mm:ss:fff");
        hapticTest_startVibration_mid(motorName);
        StartCoroutine(WaitForSeconds(vibrationDuration, motorName));
    }

    public void onVibrationValChange(float val)
    {
        vibrationSliderValueText.text = "" + val;

        vibrationSliderVal = val;
    }
    public void onStiffnessValChange(float val)
    {
        stiffnessSliderValueText.text = "" + val;
        stiffnessSliderVal = math.remap(0, 127, 0, 1, val);
        // groundedSurfaceObj.GetComponent<HapticSurface>().hlStiffness = stiffnessSliderVal;
        //HapticDevice.PhysicsForceDamping = math.remap(0, 127, 0, 1, val);
    }
    public void onDampingValChange(float val)
    {
        dampingSliderValueText.text = "" + val;
        dampingSliderVal = math.remap(0, 127, 0, 1, val);
        //groundedSurfaceObj.GetComponent<HapticSurface>().hlDamping = dampingSliderVal;
        //HapticDevice.PhysicsForceStrength = math.remap(0, 127, 0, 1, val);
    }
    public void onStaticFrictionValChange(float val)
    {
        staticFrictionSliderValueText.text = "" + val;
        staticFrictionSliderVal = math.remap(0, 127, 0, 1, val);
        //groundedSurfaceObj.GetComponent<HapticSurface>().hlStaticFriction = staticFrictionSliderVal;
        //HapticDevice.PhysicsForceStrength = math.remap(0, 127, 0, 1, val);
    }
    public void onDynamicFrictionChange(float val)
    {
        dynamicFrictionSliderValueText.text = "" + val;
        dynamicFrictionSliderVal = math.remap(0, 127, 0, 1, val);
        //groundedSurfaceObj.GetComponent<HapticSurface>().hlDynamicFriction = dynamicFrictionSliderVal;
        //HapticDevice.PhysicsForceStrength = math.remap(0, 127, 0, 1, val);
    }

    /*public void addSelectedHapticValues()
    {
        if (sw2 != null)
        {
            sw2.WriteLine("Vibration = " + vibrationSliderVal + ", Stiffness = " + stiffnessSliderVal + ", Damping = " + dampingSliderVal + ", StaticFriction = " + staticFrictionSliderVal + ", DynamicFriction = " + dynamicFrictionSliderVal);
            sw2.Flush();
            print("Vibration = " + vibrationSliderVal + ", Stiffness = " + stiffnessSliderVal + ", Damping = " + dampingSliderVal + ", StaticFriction = " + staticFrictionSliderVal + ", DynamicFriction = " + dynamicFrictionSliderVal);
            //print("" +
        }
    }*/

    IEnumerator WaitForSeconds(float seconds, string motor)
    {
        yield return new WaitForSeconds(seconds);

        //After x seconds, execute the following code

        hapticTest_stopVibration(motor);
    }


    // Update is called once per frame
    void Update()
    {
        //hapticArduinoSerialController.SendSerialMessage("" + motor + intensity);
    }

    public void recordTimeOfSelection()
    {
        timeOfSelection = System.DateTime.Now.ToString("HH:mm:ss:fff");
    }

    public void addSelectedChoice()
    {
        //++currTrialSurfaceIndex;

        //makeSurfaceActive(currTrialNum, currTrialSurfaceIndex);
        if (sw1 != null)
        {
            var dropDownList = participantChoiceDropDownLists[currMotorSeqInd].GetComponent<TMPro.TMP_Dropdown>();
            sw1.WriteLine("" + currTrialNum + ',' + motorDictionary[motorSequences[currTrialNum, currMotorSeqInd]] + ',' + dropDownList.options[dropDownList.value].text + ',' + timeVibrationStart + ',' + timeOfSelection);
            sw1.Flush();
            print("" + currTrialNum + ',' + motorDictionary[motorSequences[currTrialNum, currMotorSeqInd]] + ',' + dropDownList.options[dropDownList.value].text + ',' + timeVibrationStart + ',' + timeOfSelection);
            ++currMotorSeqInd;
        }

    }


    private void OnApplicationQuit()
    {
        sw1.Close();
        sw2.Close();
    }


    public void hapticTest_startVibration_max(string _motor)
    {
        if (hapticArduinoSerialController != null)
        {
            motor = _motor;
            intensity = "127";
            hapticArduinoSerialController.SendSerialMessage("" + motor + "127");
            print("" + motor + "127");
        }
    }

    public void hapticTest_stopVibration(string _motor)
    {
        if (hapticArduinoSerialController != null)
        {
            hapticArduinoSerialController.SendSerialMessage("" + motor + "000");
            motor = _motor;
            intensity = "000";
            //print("" + motor + "000");
        }
    }

    public void hapticTest_startVibration_custom(string _motor, int level)
    {
        if (hapticArduinoSerialController != null)
        {
            motor = _motor;
            intensity = level.ToString("D3");
            //intensity = "063";
            hapticArduinoSerialController.SendSerialMessage("" + motor + intensity);
        }
    }


    public void hapticTest_startVibration_mid(string _motor)
    {
        if (hapticArduinoSerialController != null)
        {
            motor = _motor;
            intensity = "063";
            hapticArduinoSerialController.SendSerialMessage("" + motor + "063");
        }
    }

    public void gotoMenu()
    {
        //Load scene
        SceneManager.LoadScene("Buzzwire_Pilot_Menu");
    }


    public void gotoNextTrial()
    {
        if (currTrialNum < 4)
        {
            ++currTrialNum;
            currMotorSeqInd = 0;

            //makeSurfaceActive(currTrialNum, currTrialSurfaceIndex);
            currTrialNumUIText.GetComponent<TMPro.TextMeshProUGUI>().text = "Trial No." + (currTrialNum + 1);
            //Reset all dropdown lists in participantChoiceDropDownLists to the first option
            foreach (GameObject dropDownList in participantChoiceDropDownLists)
            {
                dropDownList.GetComponent<TMPro.TMP_Dropdown>().value = 0;
            }
        }
        else
        {
            pilotTest1ControlUI.SetActive(false);

            //pilotTest2ControlUI.SetActive(true);
            //groundedSurfaceObj.SetActive(true);
            //vibrationSurfaceObj.SetActive(true);
            //currTrialNum = 0;
            gotoMenuButton.SetActive(true);
            sw1.Close();
            //gotoNextVibrationEqTrial();
        }
    }

    string prevSeq;

    public void recordVibrationData()
    {
        if (sw2 != null)
        {
            sw2.WriteLine("" + currTrialNum + ',' + prevSeq + ',' + vibrationSliderVal);
            sw2.Flush();
            //print("" + currTrialNum + ',' + stiffnessSequences[currTrialNum, currStiffnessInd] + ',' + vibrationSliderVal);
        }
        if (currTrialNum == stiffnessSequences.GetLength(0) - 1 && currStiffnessInd == stiffnessSequences.GetLength(1) - 1)
        {
            print("Data collection done");
            pilotTest2ControlUI.SetActive(false);
            sw2.Close();
        }
        ++currStiffnessInd;
        
        if (currStiffnessInd < 3)
        {
            //print("currTrialNum = " + currTrialNum + ", currStiffnessInd = " + currStiffnessInd);            
        }
        else
        {
            currStiffnessInd = 0;
            if (currTrialNum < 6)
            {
                ++currTrialNum;
            }
            else
            {
                //print("Data collection done");
                //pilotTest2ControlUI.SetActive(false);
            }
        }

    }

    public void gotoNextVibrationEqTrial()
    {
        print("currTrialNum = " + currTrialNum + ", currStiffnessInd = " + currStiffnessInd);
        currStiffnessLevelUIText.GetComponent<TMPro.TextMeshProUGUI>().text = stiffnessSequences[currTrialNum, currStiffnessInd];
        currVibrationTrialNumUIText.GetComponent<TMPro.TextMeshProUGUI>().text = "Trial No." + (currTrialNum + 1);
        prevSeq = stiffnessSequences[currTrialNum, currStiffnessInd];
        //groundedSurfaceObj.GetComponent<HapticSurface>().hlStiffness = stiffnessDict[stiffnessSequences[currTrialNum, currStiffnessInd]];     
    }

}