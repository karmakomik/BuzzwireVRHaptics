using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomButtonScript : MonoBehaviour
{
    public delegate void ClickAction();
    public static event ClickAction OnClicked;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
