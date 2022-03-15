using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillCounter : MonoBehaviour
{
    private TMPro.TMP_Text textComp;

    // Start is called before the first frame update
    void Start()
    {
        textComp = gameObject.GetComponent<TMPro.TMP_Text>();
        textComp.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        textComp.text = ScoreTracker.playerKills.ToString();
    }
}
