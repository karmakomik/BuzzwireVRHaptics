using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class StartSceneScript : MonoBehaviour
{
    public TMP_InputField testCOMPort;
    public TMP_InputField hapticCOMPort;

    // Start is called before the first frame update
    void Start()
    {
        testCOMPort.text = PlayerPrefs.GetString("testCOMPort", "not_set");
        hapticCOMPort.text = PlayerPrefs.GetString("hapticCOMPort", "not_set");
        //Debug.Log("testCOMPort" + testCOMPort.text);

        /*if (PlayerPrefs.HasKey("testCOMPort"))
        {
            Debug.Log("The key " + "testCOMPort" + " exists");
        }
        else
            Debug.Log("The key " + "testCOMPort" + " does not exist");*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setTestCOMPort()
    {
        //Debug.Log("Current com port - " + testCOMPort.text);
        PlayerPrefs.SetString("testCOMPort", testCOMPort.text);
        PlayerPrefs.Save();
    }

    public void setHapticCOMPort()
    {
        //Debug.Log("Current com port - " + testCOMPort.text);
        PlayerPrefs.SetString("hapticCOMPort", hapticCOMPort.text);
        PlayerPrefs.Save();
    }

    public void selectExperimentCondition(string expCondStr)
    {
        switch (expCondStr)
        {
            case "GROUNDED":
                HapticsExperimentControllerScript.expCondition = ExperimentalCondition.GROUNDED;
                startExperiment();
                break;
            case "VIBRATION":
                HapticsExperimentControllerScript.expCondition = ExperimentalCondition.VIBRATION;
                startExperiment();
                break;
            case "NO_HAPTICS":
                HapticsExperimentControllerScript.expCondition = ExperimentalCondition.NO_HAPTICS;
                startExperiment();
                break;
            default:
                print("Invalid experimental condition");
                break;
        }
    }

    public void startExperiment()
    {        
        SceneManager.LoadScene("BuzzwireHaptics");
    }

}
