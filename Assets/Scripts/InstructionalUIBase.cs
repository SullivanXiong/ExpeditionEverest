using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstructionalUIBase : MonoBehaviour
{

    public GameObject background;
    // Start is called before the first frame update
    public bool somethingDisplayed;

    void Start()
    {
        somethingDisplayed = false;
        background.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (somethingDisplayed)
        {
            background.SetActive(true);
        }
        else
        {
            background.SetActive(false);
        }
    }
}
