using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelLoader : MonoBehaviour
{
/*    public float wait_time = 5f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(wait_for_seconds());
    }
    IEnumerator wait_for_seconds(){
        yield return new WaitForSeconds(wait_time);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1 );
    }*/
    public Animator transition;
    public float wait_time_long = 5f;
    public float wait_time_small = 1f;
    // Start is called before the first frame update

    void Update()
    {
        if(Time.deltaTime>wait_time_long){
              LoadNextLevel();
        }
    }

    void LoadNextLevel()
    {
        //StartCoroutine(wait_for_seconds());
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }
   /* IEnumerator wait_for_seconds(){
        yield return new WaitForSeconds(wait_time_long);
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1 );
    }*/
   IEnumerator LoadLevel(int levelIndex){
     transition.SetTrigger("Start");
     yield return new WaitForSeconds(wait_time_small);
     SceneManager.LoadScene(levelIndex);

}
}