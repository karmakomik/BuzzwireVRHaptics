using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class FormHandlerScript : MonoBehaviour
{
    public enum LikertMode {NONE, LIKERT_5, LIKERT_7};
    LikertMode currLikertMode;

    int currLikertLevel;

    List<GameObject> likert5Buttons;
    List<GameObject> likert7Buttons;
    List<GameObject> likert4Buttons;

    List<GameObject> buttonList;

    //HapticPlugin hapticDevice;
    public ArrowClickScript hapticDeviceArrow;

    GameObject touchedObj;

    List<Action> surveyMethods1Medium, surveyMethods2;
    public int methodCounter1, methodCounter2;

    public Material buttonActiveMat;
    public Material buttonInactiveMat;

    public GameObject currentFormUI;
    public HapticsExperimentControllerScript hapticsExperimentController;

    public GameObject likert5Obj, likert4Obj, likert7Obj, slider3DLineObj, slider3DRootObj;

    public GameObject okButtonObj, cancelButtonObj;//, nextButtonObj, prevButtonObj;

    public GameObject introObj, samScaleObj, selfefficacy1Obj, selfefficacy2Obj, ageObj, genderObj, vrExpObj;

    public GameObject presence1ScaleObj, presence2ScaleObj, presence3ScaleObj, presence4ScaleObj, presence5ScaleObj;

    public GameObject NASATLXMentalObj, NASATLXPhysicalObj, NASATLXTemporalObj, NASATLXPerformanceObj, NASATLXEffortObj, NASATLXFrustrationObj;



    //public ArrowClickScript arrowScript;

    // Start is called before the first frame update
    void Start()
    {
        currLikertMode = LikertMode.NONE;

        methodCounter1 = methodCounter2 = 0;

        surveyMethods1Medium = new List<Action>();
        surveyMethods1Medium.Add(gotoIntro);
        surveyMethods1Medium.Add(gotoSelfEfficacy1);
        surveyMethods1Medium.Add(gotoAge);
        surveyMethods1Medium.Add(gotoGender);
        surveyMethods1Medium.Add(gotoVRExp);

        surveyMethods2 = new List<Action>();
        surveyMethods2.Add(gotoSAM);
        surveyMethods2.Add(gotoSelfEfficacy2);
        surveyMethods2.Add(gotoPresence1);
        surveyMethods2.Add(gotoPresence2);
        surveyMethods2.Add(gotoPresence3);
        surveyMethods2.Add(gotoPresence4);
        surveyMethods2.Add(gotoPresence5);
        surveyMethods2.Add(gotoNASATLX_Mental);
        surveyMethods2.Add(gotoNASATLX_Physical);
        surveyMethods2.Add(gotoNASATLX_Temporal);
        surveyMethods2.Add(gotoNASATLX_Performance);
        surveyMethods2.Add(gotoNASATLX_Effort);
        surveyMethods2.Add(gotoNASATLX_Frustration);
        surveyMethods2.Add(surveyOver);

        //Remove this
        //changeLikertScale(LikertMode.LIKERT_5);

        likert5Buttons = new List<GameObject>();
        likert4Buttons = new List<GameObject>();
        likert7Buttons = new List<GameObject>();
        buttonList = new List<GameObject>();

        foreach (Transform child in likert4Obj.transform)
        {
            likert4Buttons.Add(child.gameObject);
        }

        foreach (Transform child in likert5Obj.transform)
        {
            likert5Buttons.Add(child.gameObject);
        }

        foreach (Transform child in likert7Obj.transform)
        {
            likert7Buttons.Add(child.gameObject);
        }

    }
    
    // Update is called once per frame
    void Update()
    {
        if (hapticDeviceArrow.touchingObj != null)
        {
            //Check if hapticDevice.touching is in likert5buttons list
            touchedObj = hapticDeviceArrow.touchingObj;
            FormUIScript formUIScript = currentFormUI.GetComponent<FormUIScript>();
            buttonList = null;

            if (touchedObj == slider3DLineObj)
            {
                print("Touching slider line");
                print("remapping " + hapticDeviceArrow.clickLoc.x + " from " + hapticDeviceArrow.xMin + ',' + hapticDeviceArrow.xMax + " to " + formUIScript.xMin + ',' + formUIScript.xMax);
                int newX = (int)math.remap(hapticDeviceArrow.xMin, hapticDeviceArrow.xMax, formUIScript.xMin, formUIScript.xMax, hapticDeviceArrow.clickLoc.x);
                int clampedX = (int)math.clamp(newX, formUIScript.xMin, formUIScript.xMax);
                print("clamped " + clampedX + " from " + newX);
                //int newX = (int)math.clamp(formUIScript.xMin, formUIScript.xMax, (int) math.remap(hapticDeviceArrow.xMin, hapticDeviceArrow.xMax, formUIScript.xMin, formUIScript.xMax, hapticDeviceArrow.clickLoc.x));
                //print("newX: " + newX);
                formUIScript.moveCrossToX(clampedX);
            }
            else if (touchedObj.transform.parent.gameObject == likert4Obj)
            {
                buttonList = likert4Buttons;
            }
            else if (touchedObj.transform.parent.gameObject == likert5Obj)
            {
                buttonList = likert5Buttons;
            }
            else if (touchedObj.transform.parent.gameObject == likert7Obj)
            {
                buttonList = likert7Buttons;
            }
            else if (touchedObj == okButtonObj)
            {
                print("Touching ok button");
                touchedObj.GetComponent<Renderer>().material = buttonActiveMat;
            }

            if (buttonList != null) //arrow is touching a button
            {
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
                    //if (formUIScript == null) print("formUIScript is null");
                    if (currentFormUI.name == "SAM_AS_Scale" || currentFormUI.name == "Age" || currentFormUI.name == "Gender" || currentFormUI.name == "VRExperience")
                        formUIScript.showArrow(currLikertLevel);
                }
            }
        }
    }

    void resetAllScales()
    {
        methodCounter1 = methodCounter2 = -1;
        okButtonObj.GetComponent<Renderer>().material = buttonInactiveMat;
        resetLikertButtons();
        hapticDeviceArrow.resetSlider();
    }

    void resetLikertButtons()
    {
        buttonList = likert5Buttons;
        foreach (GameObject button in buttonList)
        {
            button.GetComponent<Renderer>().material = buttonInactiveMat;
        }

        buttonList = likert7Buttons;
        foreach (GameObject button in buttonList)
        {
            button.GetComponent<Renderer>().material = buttonInactiveMat;
        }

        buttonList = likert4Buttons;
        foreach (GameObject button in buttonList)
        {
            button.GetComponent<Renderer>().material = buttonInactiveMat;
        }
    }

    public void gotoPreviousQ_1()
    {
        if (methodCounter1 >= 1)
        {
            methodCounter1--;
            //touchedObj.GetComponent<Renderer>().material = buttonActiveMat;
            surveyMethods1Medium[methodCounter1]();
        }
        else
            print("No previous question");
    }

    public void gotoNextQ_1()
    {
        if (methodCounter1 < surveyMethods1Medium.Count - 1)
        {
            methodCounter1++;
            //touchedObj.GetComponent<Renderer>().material = buttonActiveMat;
            //prevButtonObj.SetActive(false);
            //nextButtonObj.SetActive(false);
            surveyMethods1Medium[methodCounter1]();
        }
        else
            print("No next question");
    }

    public void gotoPreviousQ_2()
    {
        if (methodCounter2 >= 1)
        {
            methodCounter2--;
            //touchedObj.GetComponent<Renderer>().material = buttonActiveMat;
            surveyMethods2[methodCounter2]();
        }
        else
            print("No previous question");
    }

    public void gotoNextQ_2()
    {
        if (methodCounter2 < surveyMethods2.Count - 1)
        {
            methodCounter2++;
            //touchedObj.GetComponent<Renderer>().material = buttonActiveMat;
            //prevButtonObj.SetActive(false);
            //nextButtonObj.SetActive(false);
            surveyMethods2[methodCounter2]();
        }
        else
            print("No next question");
    }

    public void startSurveyPart1()
    {
        gotoIntro();
    }

    public void gotoIntro()
    {
        okButtonObj.SetActive(true);
        okButtonObj.GetComponent<Renderer>().material = buttonInactiveMat;
        introObj.SetActive(true);
        selfefficacy1Obj.SetActive(false);
        likert7Obj.SetActive(false);
    }

    public void gotoSelfEfficacy1()
    {        
        selfefficacy1Obj.SetActive(true);
        ageObj.SetActive(false);
        introObj.SetActive(false);
        currentFormUI = selfefficacy1Obj;
        likert5Obj.SetActive(false);
        likert7Obj.SetActive(true);
        likert4Obj.SetActive(false);        
        okButtonObj.GetComponent<Renderer>().material = buttonInactiveMat;
        resetLikertButtons();
    }

    public void gotoAge()
    {
        ageObj.SetActive(true);
        selfefficacy1Obj.SetActive(false);
        genderObj.SetActive(false);
        currentFormUI = ageObj;
        likert5Obj.SetActive(false);
        likert7Obj.SetActive(true);
        likert4Obj.SetActive(false);
        okButtonObj.GetComponent<Renderer>().material = buttonInactiveMat;
        resetLikertButtons();
    }

    public void gotoGender()
    {
        genderObj.SetActive(true);
        ageObj.SetActive(false);
        vrExpObj.SetActive(false);
        currentFormUI = genderObj;
        likert5Obj.SetActive(false);
        likert7Obj.SetActive(false);
        likert4Obj.SetActive(true);
        okButtonObj.GetComponent<Renderer>().material = buttonInactiveMat;
        resetLikertButtons();
    }

    public void gotoVRExp()
    {
        vrExpObj.SetActive(true);
        genderObj.SetActive(false);
        currentFormUI = vrExpObj;
        likert5Obj.SetActive(false);
        likert7Obj.SetActive(false);
        likert4Obj.SetActive(true);
        okButtonObj.GetComponent<Renderer>().material = buttonInactiveMat;
        resetLikertButtons();
    }

    public void startSurveyPart2()
    {
        gotoSAM();
    }
    
    public void gotoSAM()
    {
        vrExpObj.SetActive(false);
        samScaleObj.SetActive(true);
        currentFormUI = samScaleObj;
        likert5Obj.SetActive(true);
        likert7Obj.SetActive(false);
        selfefficacy2Obj.SetActive(false);
        okButtonObj.GetComponent<Renderer>().material = buttonInactiveMat;
        resetLikertButtons();
    }

    public void gotoSelfEfficacy2()
    {
        samScaleObj.SetActive(false);
        selfefficacy2Obj.SetActive(true);
        currentFormUI = selfefficacy2Obj;
        likert5Obj.SetActive(false);
        likert7Obj.SetActive(true);
        presence1ScaleObj.SetActive(false);
        okButtonObj.GetComponent<Renderer>().material = buttonInactiveMat;
        resetLikertButtons();        
    }

    public void gotoPresence1()
    {
        selfefficacy2Obj.SetActive(false);
        likert5Obj.SetActive(false);
        currentFormUI = presence1ScaleObj;
        likert7Obj.SetActive(true);
        presence1ScaleObj.SetActive(true);
        presence2ScaleObj.SetActive(false);
        okButtonObj.GetComponent<Renderer>().material = buttonInactiveMat;
        resetLikertButtons();
    }

    public void gotoPresence2()
    {
        presence1ScaleObj.SetActive(false);
        currentFormUI = presence2ScaleObj;
        presence2ScaleObj.SetActive(true);
        presence3ScaleObj.SetActive(false);
        okButtonObj.GetComponent<Renderer>().material = buttonInactiveMat;
        resetLikertButtons();
    }

    public void gotoPresence3()
    {
        presence2ScaleObj.SetActive(false);
        currentFormUI = presence3ScaleObj;
        presence3ScaleObj.SetActive(true);
        presence4ScaleObj.SetActive(false);
        okButtonObj.GetComponent<Renderer>().material = buttonInactiveMat;
        resetLikertButtons();
    }

    public void gotoPresence4()
    {
        presence3ScaleObj.SetActive(false);
        currentFormUI = presence4ScaleObj;
        presence4ScaleObj.SetActive(true);
        presence5ScaleObj.SetActive(false);
        okButtonObj.GetComponent<Renderer>().material = buttonInactiveMat;
        resetLikertButtons();
    }
    public void gotoPresence5()
    {
        presence4ScaleObj.SetActive(false);
        currentFormUI = presence5ScaleObj;
        presence5ScaleObj.SetActive(true);
        presence3ScaleObj.SetActive(false);
        NASATLXMentalObj.SetActive(false);
        okButtonObj.GetComponent<Renderer>().material = buttonInactiveMat;
        resetLikertButtons();
    }

    public void gotoNASATLX_Mental()
    {
        likert7Obj.SetActive(false);
        presence5ScaleObj.SetActive(false);
        currentFormUI = NASATLXMentalObj;
        slider3DRootObj.SetActive(true);
        NASATLXMentalObj.SetActive(true);
        NASATLXPhysicalObj.SetActive(false);
        okButtonObj.GetComponent<Renderer>().material = buttonInactiveMat;
        hapticDeviceArrow.resetSlider();
    }

    public void gotoNASATLX_Physical()
    {
        NASATLXMentalObj.SetActive(false);
        currentFormUI = NASATLXPhysicalObj;
        NASATLXPhysicalObj.SetActive(true);
        NASATLXTemporalObj.SetActive(false);
        okButtonObj.GetComponent<Renderer>().material = buttonInactiveMat;
        hapticDeviceArrow.resetSlider();
    }
    public void gotoNASATLX_Temporal()
    {
        NASATLXPhysicalObj.SetActive(false);
        currentFormUI = NASATLXTemporalObj;
        NASATLXTemporalObj.SetActive(true);
        NASATLXPerformanceObj.SetActive(false);
        okButtonObj.GetComponent<Renderer>().material = buttonInactiveMat;
        hapticDeviceArrow.resetSlider();
    }

    public void gotoNASATLX_Performance()
    {
        NASATLXTemporalObj.SetActive(false);
        currentFormUI = NASATLXPerformanceObj;
        NASATLXPerformanceObj.SetActive(true);
        NASATLXEffortObj.SetActive(false);
        okButtonObj.GetComponent<Renderer>().material = buttonInactiveMat;
        hapticDeviceArrow.resetSlider();
    }
    public void gotoNASATLX_Effort()
    {
        NASATLXPerformanceObj.SetActive(false);
        currentFormUI = NASATLXEffortObj;
        NASATLXEffortObj.SetActive(true);
        NASATLXFrustrationObj.SetActive(false);
        okButtonObj.GetComponent<Renderer>().material = buttonInactiveMat;
        hapticDeviceArrow.resetSlider();
    }

    public void gotoNASATLX_Frustration()
    {
        NASATLXEffortObj.SetActive(false);
        currentFormUI = NASATLXFrustrationObj;
        NASATLXFrustrationObj.SetActive(true);
        slider3DRootObj.SetActive(true);
        okButtonObj.GetComponent<Renderer>().material = buttonInactiveMat;
        hapticDeviceArrow.resetSlider();
    }

    public void surveyOver()
    {
        NASATLXFrustrationObj.SetActive(false);
        currentFormUI = null;
        slider3DRootObj.SetActive(false);
        okButtonObj.SetActive(false);
        resetAllScales();
        //Send results to imotions
    }

}
