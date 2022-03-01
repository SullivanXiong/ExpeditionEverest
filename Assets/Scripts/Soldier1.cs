using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; //for NavMesh


public class Soldier1 : MonoBehaviour
{
    CoverManager coverManager;

    Soldier curTarget;
    public Team myTeam;
    public Vitals myVitals;

    Transform myTransform;
    public Transform eyes;

    Animator anim;

    [SerializeField] float minAttackDistance = 10, maxAttackDistance = 25, moveSpeed = 15;

    [SerializeField] float damageDealt = 50F;
    [SerializeField] float fireCooldown = 1F;
    float curFireCooldown = 0;







    // Start is called before the first frame update
    void Start()
    {
        myTeam = GetComponent<Team>();
        myVitals = GetComponent<Vitals>();

 
    }



}