using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PilotTestControllerScript : MonoBehaviour
{
    StreamWriter sw;

    public float vibrationDuration = 3f;

    public List<GameObject> participantChoiceDropDownLists;
    public GameObject pilotTest1ControlUI;
    public GameObject pilotTest2ControlUI;

    public Slider vibrationSlider;
    public Slider stiffnessSlider;
    public TMPro.TextMeshProUGUI vibrationSliderValueText;
    public TMPro.TextMeshProUGUI stiffnessSliderValueText;

    public GameObject currSurface;
    public GameObject currSurfaceUIText;
    int currTrialNum;
    int currMotorSeqInd;
    int currTrialSurfaceIndex;
    public GameObject currTrialNumUIText;


    public string[,] motorSequences = new string[5,6] {
                                { "a", "f", "b", "d", "c", "e" },
                                { "b", "a", "f", "e", "d", "c" },
                                { "c", "b", "a", "f", "e", "d" },
                                { "e", "c", "b", "a", "f", "d" },
                                { "f", "d", "c", "b", "a", "e" }};      

    public List<GameObject> trial2Surfaces;
    public List<GameObject> trial3Surfaces;
    public List<GameObject> trial4Surfaces;
    public List<GameObject> trial5Surfaces;

    public GameObject hapticArduinoSerialObj;
    public SerialController hapticArduinoSerialController;

    string intensity = "000";
    string motor = "a";

    string timeOfSelection, timeVibrationEnd;

    // Start is called before the first frame update
    void Start()
    {
        pilotTest1ControlUI.SetActive(true);
        pilotTest2ControlUI.SetActive(false);
        currTrialNum = 0;
        currMotorSeqInd = 0;
        //currTrialSurfaceIndex = 0;
        //currSurface = trial1Surfaces[0];
        //makeSurfaceActive(currTrialNum, currTrialSurfaceIndex);
        //currTrialNumUIText.GetComponent<TMPro.TextMeshProUGUI>().text = "Trial No." + currTrialNum;
        //currSurfaceUIText.GetComponent<TMPro.TextMeshProUGUI>().text = "Current Surface Name" + currSurface.gameObject.name;

        if (!Directory.Exists(Application.persistentDataPath + "/PilotTestData"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/PilotTestData");
        }
        sw = new StreamWriter(Application.persistentDataPath + "/PilotTestData/HapticsPilotTestData_" + System.DateTime.Now.ToString("dd_MMMM_yyyy_HH_mm_ss") + ".csv");
    }

    /*private void makeSurfaceActive(int _currTrialNum, int _currTrialSurfaceIndex)
    {
        if (_currTrialSurfaceIndex < trial1Surfaces.Count)
        {            
            switch (_currTrialNum)
            {
                case 1:
                    //setOtherSurfacesInactive();
                    currSurface = trial1Surfaces[_currTrialSurfaceIndex].gameObject;
                    trial1Surfaces[_currTrialSurfaceIndex].gameObject.SetActive(true);
                    break;
                case 2:
                    //setOtherSurfacesInactive();
                    currSurface = trial2Surfaces[_currTrialSurfaceIndex].gameObject;
                    trial2Surfaces[_currTrialSurfaceIndex].gameObject.SetActive(true);
                    break;
                case 3:
                    //setOtherSurfacesInactive();
                    currSurface = trial3Surfaces[_currTrialSurfaceIndex].gameObject;
                    trial3Surfaces[_currTrialSurfaceIndex].gameObject.SetActive(true);
                    break;
                case 4:
                    //setOtherSurfacesInactive();
                    currSurface = trial4Surfaces[_currTrialSurfaceIndex].gameObject;
                    trial4Surfaces[_currTrialSurfaceIndex].gameObject.SetActive(true);
                    break;
                case 5:
                    //setOtherSurfacesInactive();
                    currSurface = trial5Surfaces[_currTrialSurfaceIndex].gameObject;
                    trial5Surfaces[_currTrialSurfaceIndex].gameObject.SetActive(true);
                    break;
            }

            currSurfaceUIText.GetComponent<TMPro.TextMeshProUGUI>().text = "Current Surface Name" + currSurface.gameObject.name;
        }
    }

    private void setOtherSurfacesInactive()
    {
        //Make all gameobjects in trial1Surfaces, trial2Surfaces, trial3Surfaces, trial4Surfaces, trial5Surfaces inactive
        foreach (GameObject surface in trial1Surfaces)
        {
            surface.gameObject.SetActive(false);
        }
        foreach (GameObject surface in trial2Surfaces)
        {
            surface.gameObject.SetActive(false);
        }
        foreach (GameObject surface in trial3Surfaces)
        {
            surface.gameObject.SetActive(false);
        }
        foreach (GameObject surface in trial4Surfaces)
        {
            surface.gameObject.SetActive(false);
        }
        foreach (GameObject surface in trial5Surfaces)
        {
            surface.gameObject.SetActive(false);
        }
        
    }*/
    
    public void startVibration()
    {
        print("Starting vibration for motor " + motorSequences[currTrialNum, currMotorSeqInd]);
        hapticTest_startVibration_mid(motorSequences[currTrialNum, currMotorSeqInd]);
        StartCoroutine(WaitForSeconds(vibrationDuration, motorSequences[currTrialNum, currMotorSeqInd]));
    }

    public void onVibrationValChange(float val)
    {
        vibrationSliderValueText.text = "" + val;
    }
    public void onStiffnessValChange(float val)
    {
        stiffnessSliderValueText.text = "" + val;
    }

    IEnumerator WaitForSeconds(float seconds, string motor)
    {
        yield return new WaitForSeconds(seconds);

        //After x seconds, execute the following code
        timeVibrationEnd = System.DateTime.Now.ToString("HH:mm:ss:fff");
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
        if (sw != null)
        {
            var dropDownList = participantChoiceDropDownLists[currMotorSeqInd].GetComponent<TMPro.TMP_Dropdown>();
            sw.WriteLine("" + currTrialNum + ',' + motorSequences[currTrialNum, currMotorSeqInd] + ',' + dropDownList.options[dropDownList.value].text + ',' + timeVibrationEnd + ',' + timeOfSelection);
            sw.Flush();
            print("" + currTrialNum + ',' + motorSequences[currTrialNum, currMotorSeqInd] + ',' + dropDownList.options[dropDownList.value].text + ',' + timeVibrationEnd + ',' + timeOfSelection);
            ++currMotorSeqInd;
        }
        
    }

    private void OnApplicationQuit()
    {
        sw.Close();
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
            print("" + motor + "000");
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
            pilotTest2ControlUI.SetActive(true);
        }
    }
    
}
