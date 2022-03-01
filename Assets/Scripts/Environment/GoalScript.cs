using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalScript : MonoBehaviour
{
    public GameOverScript gameOver;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            gameOver.Setup();
        }
    }
}
