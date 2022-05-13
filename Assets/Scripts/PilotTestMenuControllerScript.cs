using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PilotTestMenuControllerScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void gotoTest1()
    {
        SceneManager.LoadScene("Buzzwire_Pilot_1");
    }

    public void gotoTest2()
    {
        SceneManager.LoadScene("Buzzwire_Pilot_2");
    }
}
