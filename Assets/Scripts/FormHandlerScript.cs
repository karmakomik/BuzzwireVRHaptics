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

    List<Action> surveyMethodsMediumPre, surveyMethodsMediumPost, surveyMethodsHiLoPre, surveyMethodsHiLoPost;
    public int methodCounterMediumPre, methodCounterMediumPost, methodCounterHiLoPre, methodCounterHiLoPost;

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

        methodCounterMediumPre = methodCounterMediumPost = 0;

        surveyMethodsMediumPre = new List<Action>();
        surveyMethodsMediumPre.Add(gotoIntro);
        surveyMethodsMediumPre.Add(gotoSelfEfficacy1);
        surveyMethodsMediumPre.Add(gotoAge);
        surveyMethodsMediumPre.Add(gotoGender);
        surveyMethodsMediumPre.Add(gotoVRExp);
        surveyMethodsMediumPre.Add(surveyOver);

        surveyMethodsMediumPost = new List<Action>();
        surveyMethodsMediumPost.Add(gotoSAM);
        surveyMethodsMediumPost.Add(gotoSelfEfficacy2);
        surveyMethodsMediumPost.Add(gotoPresence1);
        surveyMethodsMediumPost.Add(gotoPresence2);
        surveyMethodsMediumPost.Add(gotoPresence3);
        surveyMethodsMediumPost.Add(gotoPresence4);
        surveyMethodsMediumPost.Add(gotoPresence5);
        surveyMethodsMediumPost.Add(gotoNASATLX_Mental);
        surveyMethodsMediumPost.Add(gotoNASATLX_Physical);
        surveyMethodsMediumPost.Add(gotoNASATLX_Temporal);
        surveyMethodsMediumPost.Add(gotoNASATLX_Performance);
        surveyMethodsMediumPost.Add(gotoNASATLX_Effort);
        surveyMethodsMediumPost.Add(gotoNASATLX_Frustration);
        surveyMethodsMediumPost.Add(surveyOver);

        surveyMethodsHiLoPre = new List<Action>(); 
        surveyMethodsHiLoPre.Add(gotoSelfEfficacy1);
        surveyMethodsHiLoPre.Add(surveyOver);

        surveyMethodsHiLoPost = new List<Action>();
        surveyMethodsHiLoPost.Add(gotoSAM);
        surveyMethodsHiLoPost.Add(gotoSelfEfficacy2);
        surveyMethodsHiLoPost.Add(gotoPresence1);
        surveyMethodsHiLoPost.Add(gotoPresence2);
        surveyMethodsHiLoPost.Add(gotoPresence3);
        surveyMethodsHiLoPost.Add(gotoPresence4);
        surveyMethodsHiLoPost.Add(gotoPresence5);        
        surveyMethodsHiLoPost.Add(gotoNASATLX_Mental);
        surveyMethodsHiLoPost.Add(gotoNASATLX_Physical);
        surveyMethodsHiLoPost.Add(gotoNASATLX_Temporal);
        surveyMethodsHiLoPost.Add(gotoNASATLX_Performance);
        surveyMethodsHiLoPost.Add(gotoNASATLX_Effort);
        surveyMethodsHiLoPost.Add(gotoNASATLX_Frustration);
        surveyMethodsHiLoPost.Add(surveyOver);


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
                //print("hapticDeviceArrow.clickLoc.x - " + hapticDeviceArrow.clickLoc.x);
                //print("remapping " + hapticDeviceArrow.clickLoc.x + " from " + hapticDeviceArrow.xMin + ',' + hapticDeviceArrow.xMax + " to " + formUIScript.xMin + ',' + formUIScript.xMax);
                int newX = (int)math.remap(hapticDeviceArrow.xMin, hapticDeviceArrow.xMax, formUIScript.xMin, formUIScript.xMax, hapticDeviceArrow.clickLoc.x);
                int clampedX = (int)math.clamp(newX, formUIScript.xMin, formUIScript.xMax);
                //print("clamped " + clampedX + " from " + newX);
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
        print("Resetting all likert scales");
        okButtonObj.GetComponent<Renderer>().material = buttonInactiveMat;
        resetLikertButtons();
        hapticDeviceArrow.resetSlider();
    }

    void resetLikertButtons()
    {
        if (likert5Buttons != null)
        {
            buttonList = likert5Buttons;
            foreach (GameObject button in buttonList)
            {
                button.GetComponent<Renderer>().material = buttonInactiveMat;
            }
        }

        if (likert7Buttons != null)
        {
            buttonList = likert7Buttons;
            foreach (GameObject button in buttonList)
            {
                button.GetComponent<Renderer>().material = buttonInactiveMat;
            }
        }

        if (likert4Buttons != null)
        {
            buttonList = likert4Buttons;
            foreach (GameObject button in buttonList)
            {
                button.GetComponent<Renderer>().material = buttonInactiveMat;
            }
        }
    }

    public void gotoPreviousMediumPreQ()
    {
        if (methodCounterMediumPre >= 1)
        {
            methodCounterMediumPre--;
            //touchedObj.GetComponent<Renderer>().material = buttonActiveMat;
            surveyMethodsMediumPre[methodCounterMediumPre]();
        }
        else
            print("No previous question");
    }

    public void gotoNextMediumPreQ()
    {
        if (methodCounterMediumPre < surveyMethodsMediumPre.Count - 1)
        {
            methodCounterMediumPre++;
            //touchedObj.GetComponent<Renderer>().material = buttonActiveMat;
            //prevButtonObj.SetActive(false);
            //nextButtonObj.SetActive(false);
            surveyMethodsMediumPre[methodCounterMediumPre]();
        }
        else
            print("No next question");
    }

    public void gotoPreviousMediumPostQ()
    {
        if (methodCounterMediumPost >= 1)
        {
            methodCounterMediumPost--;
            //touchedObj.GetComponent<Renderer>().material = buttonActiveMat;
            surveyMethodsMediumPost[methodCounterMediumPost]();
        }
        else
            print("No previous question");
    }

    public void gotoNextMediumPostQ()
    {
        if (methodCounterMediumPost < surveyMethodsMediumPost.Count - 1)
        {
            methodCounterMediumPost++;
            //touchedObj.GetComponent<Renderer>().material = buttonActiveMat;
            //prevButtonObj.SetActive(false);
            //nextButtonObj.SetActive(false);
            surveyMethodsMediumPost[methodCounterMediumPost]();
        }
        else
            print("No next question");
    }

    public void gotoPreviousHiLoPreQ()
    {
        print("methodCounterHiLoPre" + methodCounterHiLoPre);
        if (methodCounterHiLoPre >= 1)
        {
            methodCounterHiLoPre--;
            //touchedObj.GetComponent<Renderer>().material = buttonActiveMat;
            surveyMethodsHiLoPre[methodCounterHiLoPre]();
        }
        else
            print("No previous question");
    }

    public void gotoNextHiLoPreQ()
    {
        if (methodCounterHiLoPre < surveyMethodsHiLoPre.Count - 1)
        {
            methodCounterHiLoPre++;
            //touchedObj.GetComponent<Renderer>().material = buttonActiveMat;
            //prevButtonObj.SetActive(false);
            //nextButtonObj.SetActive(false);
            surveyMethodsHiLoPre[methodCounterHiLoPre]();
        }
        else
            print("No next question");
    }

    public void gotoPreviousHiLoPostQ()
    {
        if (methodCounterHiLoPost >= 1)
        {
            methodCounterHiLoPost--;
            //touchedObj.GetComponent<Renderer>().material = buttonActiveMat;
            surveyMethodsHiLoPost[methodCounterHiLoPost]();
        }
        else
            print("No previous question");
    }

    public void gotoNextHiLoPostQ()
    {
        if (methodCounterHiLoPost < surveyMethodsHiLoPost.Count - 1)
        {
            methodCounterHiLoPost++;
            //touchedObj.GetComponent<Renderer>().material = buttonActiveMat;
            //prevButtonObj.SetActive(false);
            //nextButtonObj.SetActive(false);
            surveyMethodsHiLoPost[methodCounterHiLoPost]();
        }
        else
            print("No next question");
    }

    public void startSurveyHiLoPre()
    {
        okButtonObj.SetActive(true);
        methodCounterHiLoPre = -1;
        gotoNextHiLoPreQ();
    }

    public void startSurveyHiLoPost()
    {
        okButtonObj.SetActive(true);
        methodCounterHiLoPost = -1;
        gotoNextHiLoPostQ();
    }

    public void startSurveyMediumPre()
    {
        okButtonObj.SetActive(true);
        methodCounterMediumPre = -1;
        gotoNextMediumPreQ();
    }

    public void startSurveyMediumPost()
    {
        okButtonObj.SetActive(true);
        methodCounterMediumPost = -1;   
        gotoNextMediumPostQ();    
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

   
    public void gotoSAM()
    {
        vrExpObj.SetActive(false);
        samScaleObj.SetActive(true);
        currentFormUI = samScaleObj;
        likert5Obj.SetActive(true);
        likert7Obj.SetActive(false);
        likert4Obj.SetActive(false);
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
        selfefficacy1Obj.SetActive(false);
        vrExpObj.SetActive(false);
        NASATLXFrustrationObj.SetActive(false);
        //currentFormUI = null;
        likert4Obj.SetActive(false);
        likert5Obj.SetActive(false);
        likert7Obj.SetActive(false);
        
        slider3DRootObj.SetActive(false);
        okButtonObj.GetComponent<Renderer>().material = buttonInactiveMat;
        okButtonObj.SetActive(false);
        resetAllScales();
        //Send results to imotions
    }

}
