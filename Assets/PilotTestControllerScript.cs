using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PilotTestControllerScript : MonoBehaviour
{
    StreamWriter sw;

    public List<GameObject> participantChoiceDropDownLists;

    GameObject currSurface;
    int currTrialNum;
    public GameObject currTrialNumUIText;

    public List<GameObject> trial1Surfaces;
    public List<GameObject> trial2Surfaces;
    public List<GameObject> trial3Surfaces;
    public List<GameObject> trial4Surfaces;
    public List<GameObject> trial5Surfaces;

    public GameObject hapticArduinoSerialObj;
    public SerialController hapticArduinoSerialController;

    string intensity = "000";
    string motor = "a";

    // Start is called before the first frame update
    void Start()
    {
        currTrialNum = 1;
        
        if (!Directory.Exists(Application.persistentDataPath + "/PilotTestData"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/PilotTestData");
        }
        sw = new StreamWriter(Application.persistentDataPath + "/PilotTestData/" + System.DateTime.Now.ToString("HapticsPilotTestData_dd_MMMM_yyyy_HH_mm_ss") + ".csv");
    }

    // Update is called once per frame
    void Update()
    {
        //hapticArduinoSerialController.SendSerialMessage("" + motor + intensity);
    }
    
    public void addSelectedChoice(char trial_num, string surface)
    {
        if (sw != null)
        {
            sw.WriteLine(trial_num + ',' + surface + gameObject.name);
        }
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
        ++currTrialNum;
        if (currTrialNum < 5)
        {
            currTrialNumUIText.GetComponent<TMPro.TextMeshProUGUI>().text = "Trial No." + currTrialNum;
            //Reset all dropdown lists in participantChoiceDropDownLists to the first option
            foreach (GameObject dropDownList in participantChoiceDropDownLists)
            {
                dropDownList.GetComponent<TMPro.TMP_Dropdown>().value = 0;
            }
        }
    }
    
}
