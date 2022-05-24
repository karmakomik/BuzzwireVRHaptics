using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormUIScript : MonoBehaviour
{
    // Start is called before the first frame update
    public int xMin = -1740, xMax = 1808;

    public List<GameObject> arrows;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void showArrow(int index)
    {
        // Disable all other gameobjects
        foreach (GameObject obj in arrows)
            obj.SetActive(false);
        arrows[index].SetActive(true);
    }

    public void moveCrossToX(int x)
    {
        // Move cross to x
        Vector3 pos = arrows[0].transform.localPosition;
        pos.x = x;
        arrows[0].transform.localPosition = pos;
    }
}
