using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; //for NavMesh

public class ShootingSoldier: MonoBehaviour {
    GameObject curTarget;
    //Player curTarget;
    public Team myTeam;
    Transform myTransform;
    Vitals myVitals;

    [SerializeField] float fireCooldown = 1F;
    float curFireCooldown = 0;

    Animator anim;
    [SerializeField] float damageDealt = 50F;
    [SerializeField] float minAttackDistance = 10, maxAttackDistance = 25, moveSpeed = 15;
    public enum ai_states {
        idle,
        move,
        combat
    }
    public ai_states state = ai_states.idle;
    // Start is called before the first frame update
    void Start() {
        myTransform = transform;
        myTeam = GetComponent < Team >();
        myVitals = GetComponent < Vitals >();
        anim = GetComponent < Animator >();
        curTarget = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void update() {
        if (myVitals.getCurHealth() > 0) {
            switch (state) {
            case ai_states.idle:
                stateIdle();
                break;
            case ai_states.move:
                stateMove();
                break;
            case ai_states.combat:
                stateCombat();
                break;
            default:
                break;
            }
        } else {
            Destroy(this.gameObject);
        }
    }
    void stateIdle() {
        if (curTarget != null && curTarget.GetComponent < Vitals >().getCurHealth() > 0) {
            if (Vector3.Distance(myTransform.position, curTarget.transform.position) <= maxAttackDistance && Vector3.Distance(myTransform.position, curTarget.transform.position) >= minAttackDistance) {
                //attack
                state = ai_states.combat;

            } else {
                anim.SetBool("move", true);
                //move
                state = ai_states.move;
            }
        } else {
            //continue pursuing same target
            GameObject bestTarget = GameObject.FindGameObjectWithTag("Player");
        }
    }
    void stateMove() {
        if (curTarget != null && curTarget.GetComponent < Vitals >().getCurHealth() > 0) {

            myTransform.LookAt(curTarget.transform);
            if (Vector3.Distance(myTransform.position, curTarget.transform.position) > maxAttackDistance) {
                // move toward target
                myTransform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            } else if (Vector3.Distance(myTransform.position, curTarget.transform.position) < minAttackDistance) {
                //stay at a safe range from target
                myTransform.Translate(Vector3.forward * -1 * moveSpeed * Time.deltaTime);
            } else {
                anim.SetBool("move", false);
                //attack
                state = ai_states.idle;
            }
        } else {
            anim.SetBool("move", false);
            state = ai_states.idle;
        }

    }
    void stateCombat() {
        if (curTarget != null && curTarget.GetComponent < Vitals >().getCurHealth() > 0) {
            myTransform.LookAt(curTarget.transform);
            if (Vector3.Distance(myTransform.position, curTarget.transform.position) <= maxAttackDistance && Vector3.Distance(myTransform.position, curTarget.transform.position) >= minAttackDistance) {
                if (curFireCooldown <= 0) {
                    //attack
                    anim.SetTrigger("fire");
                    curTarget.GetComponent < Vitals >().getHit(damageDealt);
                    curFireCooldown = fireCooldown;
                } else {
                    curFireCooldown -= 1 * Time.deltaTime;
                }
            } else {
                //move
                anim.SetBool("move", true);
                state = ai_states.move;
            }
        } else {
            state = ai_states.idle;
        }
    }
}