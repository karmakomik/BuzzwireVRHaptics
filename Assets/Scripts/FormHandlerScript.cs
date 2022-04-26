using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormHandlerScript : MonoBehaviour
{
    public enum LikertMode {NONE, LIKERT_5, LIKERT_7};
    LikertMode currLikertMode;

    int currLikertLevel;

    public GameObject likert5ButtonsRoot;
    List<GameObject> likert5Buttons;

    public GameObject likert7ButtonsRoot;
    List<GameObject> likert7Buttons;

    List<GameObject> buttonList;

    HapticPlugin hapticDevice;

    GameObject touchedObj;

    public Material buttonActiveMat;
    public Material buttonInactiveMat;

    public GameObject currentFormUI;

    // Start is called before the first frame update
    void Start()
    {
        currLikertMode = LikertMode.NONE;

        //Remove this
        changeLikertScale(LikertMode.LIKERT_5);


        if (hapticDevice == null)
            hapticDevice = (HapticPlugin)FindObjectOfType(typeof(HapticPlugin));

        likert5Buttons = new List<GameObject>();
        likert7Buttons = new List<GameObject>();
        buttonList = new List<GameObject>();

        foreach (Transform child in likert5ButtonsRoot.transform)
        {
            likert5Buttons.Add(child.gameObject);
        }

        foreach (Transform child in likert7ButtonsRoot.transform)
        {
            likert7Buttons.Add(child.gameObject);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (hapticDevice.touching != null)
        {
            //Check if hapticDevice.touching is in likert5buttons list
            touchedObj = hapticDevice.touching;
            if (currLikertMode == LikertMode.LIKERT_5)
            {
                buttonList = likert5Buttons;
            }
            else if (currLikertMode == LikertMode.LIKERT_7)
            {
                buttonList = likert7Buttons;
            }

            if (buttonList.Contains(touchedObj))
            {
                //Set the current likert level to the index of the button in likert5buttons
                currLikertLevel = buttonList.IndexOf(touchedObj);
                
                print("currLikertLevel" + currLikertLevel);
                //Set all objects in buttonList to inactive material
                foreach (GameObject button in buttonList)
                {
                    button.GetComponent<Renderer>().material = buttonInactiveMat;
                }
                //Set the selected button to active material                
                touchedObj.GetComponent<Renderer>().material = buttonActiveMat;

                currentFormUI.GetComponent<FormUIScript>().showArrow(currLikertLevel);
            }            
        }
    }

    public void changeLikertScale(LikertMode mode)
    {
        currLikertMode = mode;
        if(currLikertMode == LikertMode.LIKERT_5)
        {
            likert5ButtonsRoot.SetActive(true);
            likert7ButtonsRoot.SetActive(false);
        }
        else if (currLikertMode == LikertMode.LIKERT_7)
        {
            likert7ButtonsRoot.SetActive(true);
            likert5ButtonsRoot.SetActive(false);
        }
    }
        
}
