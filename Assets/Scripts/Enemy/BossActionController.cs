using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossActionController : MonoBehaviour
{
    [Header("Boss Wave details")]
    public WaveSpawner[] waveSpawners;
    public WaveSpawner.Wave[] wave;

    public enum ActionState { ATTACKING1, SPAWNING, ATTACKING2, CANCELLING };
    [Header("Action Cycle Variables")]
    public float Attack1BufferTime = 15f;
    public float SpawningBufferTime = 1f;
    public float Attack2BufferTime = 5f;
    public float CancellingBufferTime = 2f;

    [Header("Ranged Enemy Trigger")]
    public bool attacking1 = false;
    public bool attacking2 = false;

    [Header("References")]
    public NewController playerController;
    public GameObject playerObject;
    public NewGrapplingGun grappleGunScript;

    private float time = 0f;
    private ActionState state;
    private int waveCount;

    // Start is called before the first frame update
    void Start()
    {
        waveCount = wave.Length;
        state = ActionState.ATTACKING1;
    }

    // Update is called once per frame
    void Update()
    {
        if (time <= Attack1BufferTime) {
            state = ActionState.ATTACKING1;
        }
        else if (time <= Attack1BufferTime + SpawningBufferTime) {
            state = ActionState.SPAWNING;
        }
        else if (time <= Attack1BufferTime + SpawningBufferTime + Attack2BufferTime) {
            state = ActionState.ATTACKING2;
        }
        else if (time <= Attack1BufferTime + SpawningBufferTime + Attack2BufferTime + CancellingBufferTime) {
            state = ActionState.CANCELLING;
        }
        else {
            time = 0f;
        }
        updateState();

        time += Time.deltaTime;
    }

    void updateState() {
        if (state == ActionState.ATTACKING1) {
            playerController.canGrapple = true;
            playerController.canClimb = true;
            Debug.Log("Attacking1");
            attacking1 = true;
        }
        else if (state == ActionState.SPAWNING) {
            Debug.Log("Spawning");
            attacking1 = false;
            SpawnEnemiesFromSpawnPoints();
        }
        else if (state == ActionState.ATTACKING2) {
            if (waveCount == 0)
            {
                waveCount = wave.Length;
            }
            Debug.Log("Attacking2");
            attacking2 = true;
        }
        else if (state == ActionState.CANCELLING) {
            Debug.Log("Cancelling");
            playerController.canGrapple = false;
            playerController.canClimb = false;
            if (playerController.isGrappling)
            {
                playerController.isGrappling = false;
                grappleGunScript.StopGrapple();
            }
            else if (playerController.isClimbing)
            {
                playerController.isClimbing = false;
            }
            attacking2 = false;
        }
    }

    void SpawnEnemiesFromSpawnPoints()
    {
        if (waveCount > 0)
        {
            for (int i = 0; i < waveSpawners.Length; i++)
            {
                waveSpawners[i].SpawnWave(wave[wave.Length - waveCount]);
            }
            waveCount--;
        }
    }
}
