using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

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
    int[] surveyMethodsMediumPreResults, surveyMethodsMediumPostResults, surveyMethodsHiLoPreResults, surveyMethodsHiLoPostResults;
    public int methodCounterMediumPre, methodCounterMediumPost, methodCounterHiLoPre, methodCounterHiLoPost;

    int introChoice, selfEfficacy1Choice, selfEfficacy2Choice, ageChoice, genderChoice, vrExpChoice;
    int presence1Choice, presence2Choice, presence3Choice, presence4Choice, presence5Choice;
    int NASATLXMentalChoice, NASATLXPhysicalChoice, NASATLXTemporalChoice, NASATLXPerformanceChoice, NASATLXEffortChoice, NASATLXFrustrationChoice;
    int samChoice;

    public Material buttonActiveMat;
    public Material buttonInactiveMat;
    public Material submitButtonIncompleteMat;

    public GameObject currentFormUI;
    public HapticsExperimentControllerScript hapticsExperimentController;

    public GameObject submitMediumPreButtonObj, submitMediumPostButtonObj, submitHiLoPreButtonObj, submitHiLoPostButtonObj;

    public GameObject likert5Obj, likert4Obj, likert7Obj, slider3DLineObj, slider3DRootObj;

    public GameObject okButtonObj, cancelButtonObj;//, nextButtonObj, prevButtonObj;

    public GameObject introObj, samScaleObj, selfefficacy1Obj, selfefficacy2Obj, ageObj, genderObj, vrExpObj;

    public GameObject presence1ScaleObj, presence2ScaleObj, presence3ScaleObj, presence4ScaleObj, presence5ScaleObj;

    public GameObject NASATLXMentalObj, NASATLXPhysicalObj, NASATLXTemporalObj, NASATLXPerformanceObj, NASATLXEffortObj, NASATLXFrustrationObj;

    string surveyResponseTxt;
    int prevResponse;

    //public ArrowClickScript arrowScript;

    // Start is called before the first frame update
    void Start()
    {
        surveyResponseTxt = "";

        currLikertMode = LikertMode.NONE;

        methodCounterMediumPre = methodCounterMediumPost = 0;

        surveyMethodsMediumPre = new List<Action>();
        surveyMethodsMediumPre.Add(gotoIntro);
        surveyMethodsMediumPre.Add(gotoSelfEfficacy1);
        surveyMethodsMediumPre.Add(gotoAge);
        surveyMethodsMediumPre.Add(gotoGender);
        surveyMethodsMediumPre.Add(gotoVRExp);
        surveyMethodsMediumPre.Add(showMediumPreSurvey);
        surveyMethodsMediumPreResults = new int[surveyMethodsMediumPre.Count - 1]; // -2 because we don't want to count the gotoIntro and showMediumPreSurvey methods ... Copilot generated this comment WTF!

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
        surveyMethodsMediumPost.Add(showMediumPostSurvey);
        surveyMethodsMediumPostResults = new int[surveyMethodsMediumPost.Count - 1];
        
        //surveyMethodsMediumPost.Add(surveyOver);

        surveyMethodsHiLoPre = new List<Action>(); 
        surveyMethodsHiLoPre.Add(gotoSelfEfficacy1);
        surveyMethodsHiLoPre.Add(showHiLoPreSurvey);
        surveyMethodsHiLoPreResults = new int[surveyMethodsHiLoPre.Count - 1];
        
        //surveyMethodsHiLoPre.Add(surveyOver);

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
        surveyMethodsHiLoPost.Add(showHiLoPostSurvey);
        surveyMethodsHiLoPostResults = new int[surveyMethodsHiLoPost.Count - 1];
        
        //surveyMethodsHiLoPost.Add(surveyOver);


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
                okButtonObj.SetActive(true);
                print("Touching slider line");
                //print("hapticDeviceArrow.clickLoc.x - " + hapticDeviceArrow.clickLoc.x);
                //print("remapping " + hapticDeviceArrow.clickLoc.x + " from " + hapticDeviceArrow.xMin + ',' + hapticDeviceArrow.xMax + " to " + formUIScript.xMin + ',' + formUIScript.xMax);
                int newX = (int)math.remap(hapticDeviceArrow.xMin, hapticDeviceArrow.xMax, formUIScript.xMin, formUIScript.xMax, hapticDeviceArrow.clickLoc.x);
                int clampedX = (int)math.clamp(newX, formUIScript.xMin, formUIScript.xMax);
                int selectedVal = (int)math.remap(hapticDeviceArrow.xMin, hapticDeviceArrow.xMax, 1, 21, hapticDeviceArrow.clickLoc.x);
                int clampedSelectedVal = (int)math.clamp(selectedVal, 1, 21);
                prevResponse = clampedSelectedVal;
                print("clampedSelectedVal " + clampedSelectedVal);
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
                    okButtonObj.SetActive(true);
                    //Set the current likert level to the index of the button in likert5buttons
                    currLikertLevel = buttonList.IndexOf(touchedObj);

                    print("currLikertLevel" + currLikertLevel);
                    prevResponse = (currLikertLevel + 1);
                    //surveyResponseTxt += "" + (currLikertLevel + 1);

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
            if (methodCounterMediumPre > 0 && methodCounterMediumPre < surveyMethodsMediumPreResults.Length)
            {
                print("methodCounterMediumPre " + methodCounterMediumPre);
                surveyMethodsMediumPreResults[methodCounterMediumPre] = prevResponse;
                surveyResponseTxt += "" + prevResponse;
                print("Current surveyResponseTxt - " + surveyResponseTxt);
            }
            methodCounterMediumPre++;
            //touchedObj.GetComponent<Renderer>().material = buttonActiveMat;
            //prevButtonObj.SetActive(false);
            //nextButtonObj.SetActive(false);
            surveyMethodsMediumPre[methodCounterMediumPre]();
            prevResponse = 0;
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
            if (methodCounterMediumPost > -1 && methodCounterMediumPost < surveyMethodsMediumPostResults.Length)
            {
                print("methodCounterMediumPost " + methodCounterMediumPost);
                surveyMethodsMediumPostResults[methodCounterMediumPost] = prevResponse;
                surveyResponseTxt += "" + prevResponse;
                print("Current surveyResponseTxt - " + surveyResponseTxt);
            }

            methodCounterMediumPost++;
            //touchedObj.GetComponent<Renderer>().material = buttonActiveMat;
            //prevButtonObj.SetActive(false);
            //nextButtonObj.SetActive(false);
            surveyMethodsMediumPost[methodCounterMediumPost]();
            prevResponse = 0;
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
            if (methodCounterHiLoPre == 0)
            {
                print("methodCounterHiLoPre " + methodCounterHiLoPre);
                surveyMethodsHiLoPreResults[methodCounterHiLoPre] = prevResponse;
                surveyResponseTxt += "" + prevResponse;
                print("Current surveyResponseTxt - " + surveyResponseTxt);
            }

            methodCounterHiLoPre++;
            //touchedObj.GetComponent<Renderer>().material = buttonActiveMat;
            //prevButtonObj.SetActive(false);
            //nextButtonObj.SetActive(false);
            surveyMethodsHiLoPre[methodCounterHiLoPre]();
            prevResponse = 0;
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
            if (methodCounterHiLoPost > -1 && methodCounterHiLoPost < surveyMethodsHiLoPostResults.Length)
            {
                print("methodCounterHiLoPost " + methodCounterHiLoPost);
                surveyMethodsHiLoPostResults[methodCounterHiLoPost] = prevResponse;
                surveyResponseTxt += "" + prevResponse;
                print("Current surveyResponseTxt - " + surveyResponseTxt);
            }
          
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
        okButtonObj.SetActive(false);
        methodCounterHiLoPre = -1;
        surveyResponseTxt = "HI_LO_PRE_";
        gotoNextHiLoPreQ();
    }

    public void startSurveyHiLoPost()
    {
        okButtonObj.SetActive(false);
        methodCounterHiLoPost = -1;
        surveyResponseTxt = "HI_LO_POST_";
        gotoNextHiLoPostQ();
    }

    public void startSurveyMediumPre()
    {
        okButtonObj.SetActive(false);
        methodCounterMediumPre = -1;
        surveyResponseTxt = "MEDIUM_PRE_";
        gotoNextMediumPreQ();
    }

    public void startSurveyMediumPost()
    {
        okButtonObj.SetActive(false);
        methodCounterMediumPost = -1;
        surveyResponseTxt = "MEDIUM_POST_";
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
        okButtonObj.SetActive(false);
        surveyResponseTxt += ",Eff_1_Res_";
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
        okButtonObj.SetActive(false);
        surveyResponseTxt += ",Age_Res_";
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
        okButtonObj.SetActive(false);
        surveyResponseTxt += ",Gender_Res_";
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
        okButtonObj.SetActive(false);
        surveyResponseTxt += ",VRExp_Res_";
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
        okButtonObj.SetActive(false);
        surveyResponseTxt += ",SAM_Res_";
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
        okButtonObj.SetActive(false);
        surveyResponseTxt += ",Eff_2_Res_";
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
        okButtonObj.SetActive(false);
        surveyResponseTxt += ",Pres_1_Res_";
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
        okButtonObj.SetActive(false);
        surveyResponseTxt += ",Pres_2_Res_";
        presence1ScaleObj.SetActive(false);
        currentFormUI = presence2ScaleObj;
        presence2ScaleObj.SetActive(true);
        presence3ScaleObj.SetActive(false);
        okButtonObj.GetComponent<Renderer>().material = buttonInactiveMat;
        resetLikertButtons();
    }

    public void gotoPresence3()
    {
        okButtonObj.SetActive(false);
        surveyResponseTxt += ",Pres_3_Res_";
        presence2ScaleObj.SetActive(false);
        currentFormUI = presence3ScaleObj;
        presence3ScaleObj.SetActive(true);
        presence4ScaleObj.SetActive(false);
        okButtonObj.GetComponent<Renderer>().material = buttonInactiveMat;
        resetLikertButtons();
    }

    public void gotoPresence4()
    {
        okButtonObj.SetActive(false);
        surveyResponseTxt += ",Pres_4_Res_";
        presence3ScaleObj.SetActive(false);
        currentFormUI = presence4ScaleObj;
        presence4ScaleObj.SetActive(true);
        presence5ScaleObj.SetActive(false);
        okButtonObj.GetComponent<Renderer>().material = buttonInactiveMat;
        resetLikertButtons();
    }
    public void gotoPresence5()
    {
        okButtonObj.SetActive(false);
        surveyResponseTxt += ",Pres_5_Res_";
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
        okButtonObj.SetActive(false);
        surveyResponseTxt += ",NASATLX_Mental_Res_";
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
        okButtonObj.SetActive(false);
        surveyResponseTxt += ",NASATLX_Physical_Res_";
        NASATLXMentalObj.SetActive(false);
        currentFormUI = NASATLXPhysicalObj;
        NASATLXPhysicalObj.SetActive(true);
        NASATLXTemporalObj.SetActive(false);
        okButtonObj.GetComponent<Renderer>().material = buttonInactiveMat;
        hapticDeviceArrow.resetSlider();
    }
    public void gotoNASATLX_Temporal()
    {
        okButtonObj.SetActive(false);
        surveyResponseTxt += ",NASATLX_Temporal_Res_";
        NASATLXPhysicalObj.SetActive(false);
        currentFormUI = NASATLXTemporalObj;
        NASATLXTemporalObj.SetActive(true);
        NASATLXPerformanceObj.SetActive(false);
        okButtonObj.GetComponent<Renderer>().material = buttonInactiveMat;
        hapticDeviceArrow.resetSlider();
    }

    public void gotoNASATLX_Performance()
    {
        okButtonObj.SetActive(false);
        surveyResponseTxt += ",NASATLX_Performance_Res_";
        NASATLXTemporalObj.SetActive(false);
        currentFormUI = NASATLXPerformanceObj;
        NASATLXPerformanceObj.SetActive(true);
        NASATLXEffortObj.SetActive(false);
        okButtonObj.GetComponent<Renderer>().material = buttonInactiveMat;
        hapticDeviceArrow.resetSlider();
    }
    public void gotoNASATLX_Effort()
    {
        okButtonObj.SetActive(false);
        surveyResponseTxt += ",NASATLX_Effort_Res_";
        NASATLXPerformanceObj.SetActive(false);
        currentFormUI = NASATLXEffortObj;
        NASATLXEffortObj.SetActive(true);
        NASATLXFrustrationObj.SetActive(false);
        okButtonObj.GetComponent<Renderer>().material = buttonInactiveMat;
        hapticDeviceArrow.resetSlider();
    }

    public void gotoNASATLX_Frustration()
    {
        okButtonObj.SetActive(false);
        surveyResponseTxt += ",NASATLX_Frustration_Res_";
        NASATLXEffortObj.SetActive(false);
        currentFormUI = NASATLXFrustrationObj;
        NASATLXFrustrationObj.SetActive(true);
        slider3DRootObj.SetActive(true);
        okButtonObj.GetComponent<Renderer>().material = buttonInactiveMat;
        hapticDeviceArrow.resetSlider();
    }

    static string removeDuplicates(string s)
    {
        char[] S = s.ToCharArray();
        int n = S.Length;

        // We don't need to do anything for
        // empty or single character string.
        if (n < 2)
        {
            return "";
        }

        // j is used to store index is result
        // string (or index of current distinct
        // character)
        int j = 0;

        // Traversing string
        for (int i = 1; i < n; i++)
        {
            // If current character S[i]
            // is different from S[j]
            if (S[j] != S[i])
            {
                j++;
                S[j] = S[i];
            }
        }
        char[] A = new char[j + 1];
        Array.Copy(S, 0, A, 0, j + 1);
        return new string(A);
    }

    /*public void surveyOver()
    {
        //surveyResponseTxt = removeDuplicates(surveyResponseTxt);
        print("surveyResponseTxt " + surveyResponseTxt);
        if (hapticsExperimentController.client != null)
            hapticsExperimentController.client.Write("M;1;;;" + surveyResponseTxt + ";\r\n");

        surveyResponseTxt = "";
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
    }*/

    public void showMediumPreSurvey()
    {
        submitMediumPreButtonObj.SetActive(true);
    }

    //Print contents of array
    public void printArray(int[] arr)
    {
        string s = "";
        for (int i = 0; i < arr.Length; i++)
        {
            s += arr[i] + " ";
        }
        print(s);
    }
    
    public void mediumPreSurveyOver()
    {
        var surveyMethodsMediumPreResultsNew = new List<int>(surveyMethodsMediumPreResults).GetRange(1, surveyMethodsMediumPreResults.Length-1).ToArray();
        //Check if surveyMethodsMediumPreResults has any 0 in it
        if (ContainsZero(surveyMethodsMediumPreResultsNew, 0))
        {
            printArray(surveyMethodsMediumPreResultsNew);
            print("Some answers were not answered in medium pre");
            //print("surveyResponseTxt " + surveyResponseTxt);
            surveyResponseTxt = "MEDIUM_PRE_";
            surveyMethodsMediumPreResults = new int[surveyMethodsMediumPre.Count - 1];
            submitMediumPreButtonObj.GetComponent<Image>().color = Color.red;
        }
        else
        {
            print("mediumPreSurveyOver");
            submitMediumPreButtonObj.SetActive(false);
            print("surveyResponseTxt " + surveyResponseTxt);
            //submitMediumPreButtonObj.SetActive(false);
            if (hapticsExperimentController.client != null)
                hapticsExperimentController.client.Write("M;1;;;" + surveyResponseTxt + ";\r\n");


            surveyResponseTxt = "";
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
        }
    }

    public void showMediumPostSurvey()
    {
        submitMediumPostButtonObj.SetActive(true);
    }

    public void mediumPostSurveyOver()
    {        
        if (ContainsZero(surveyMethodsMediumPostResults,0))
        {
            printArray(surveyMethodsMediumPostResults);
            print("Some answers were not answered in medium post");
            print("surveyResponseTxt " + surveyResponseTxt);
            surveyResponseTxt = "MEDIUM_POST_";
            surveyMethodsMediumPostResults = new int[surveyMethodsMediumPost.Count - 1];
            submitMediumPostButtonObj.GetComponent<Image>().color = Color.red;
            //submitMediumPostButtonObj.GetComponent<Renderer>().material = submitButtonIncompleteMat;
        }
        else
        {
            print("mediumPostSurveyOver");
            submitMediumPostButtonObj.SetActive(false);
            print("surveyResponseTxt " + surveyResponseTxt);
            if (hapticsExperimentController.client != null)
                hapticsExperimentController.client.Write("M;1;;;" + surveyResponseTxt + ";\r\n");


            surveyResponseTxt = "";
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
        }
    }

    public void showHiLoPreSurvey()
    {
        submitHiLoPreButtonObj.SetActive(true);
    }

    public static bool ContainsZero(Array a, object val)
    {
        return Array.IndexOf(a, val) != -1;
    }

    public void hiloPreSurveyOver()
    {
        if (ContainsZero(surveyMethodsHiLoPreResults,0))
        {
            print("Some answers were not answered in hilo pre");
            print("surveyResponseTxt " + surveyResponseTxt);
            surveyResponseTxt = "HILO_PRE_";
            surveyMethodsHiLoPreResults = new int[surveyMethodsHiLoPre.Count - 1];
            submitHiLoPreButtonObj.GetComponent<Image>().color = Color.red;
            //submitHiLoPreButtonObj.GetComponent<Renderer>().material = submitButtonIncompleteMat;
        }
        else
        {
            print("hiloPreSurveyOver");
            submitHiLoPreButtonObj.SetActive(false);
            print("surveyResponseTxt " + surveyResponseTxt);
            if (hapticsExperimentController.client != null)
                hapticsExperimentController.client.Write("M;1;;;" + surveyResponseTxt + ";\r\n");

            surveyResponseTxt = "";
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
        }
    }

    public void showHiLoPostSurvey()
    {
        submitHiLoPostButtonObj.SetActive(true);
    }

    public void hiloPostSurveyOver()
    {
        if (ContainsZero(surveyMethodsHiLoPostResults,0))
        {
            //print(surveyMethodsHiLoPostResults);
            print("Some answers were not answered in Hilo post");
            //print("surveyResponseTxt " + surveyResponseTxt);
            surveyResponseTxt = "HILO_POST_";
            surveyMethodsHiLoPostResults = new int[surveyMethodsHiLoPost.Count - 1];

            submitHiLoPostButtonObj.GetComponent<Image>().color = Color.red;
            //submitHiLoPostButtonObj.GetComponent<Renderer>().material = submitButtonIncompleteMat;
        }
        else
        {
            print("hiloPostSurveyOver");
            submitHiLoPostButtonObj.SetActive(false);
            print("surveyResponseTxt " + surveyResponseTxt);
            if (hapticsExperimentController.client != null)
                hapticsExperimentController.client.Write("M;1;;;" + surveyResponseTxt + ";\r\n");


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
        }
    }

}
