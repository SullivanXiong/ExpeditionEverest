using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class trackingsyster : MonoBehaviour
{
    public NavMeshAgent enemy;
    public Transform Player;
    public GameObject enemyUI;
    public GameObject p1;
    float _nextShootTime;
    [SerializeField] GameObject _bulletPrefab;
    [SerializeField] Transform _shootPoint;
    [SerializeField] float _delay = 0.5f;
    [SerializeField] float _bulletSpeed = 5f;
    Vector3 _direction;
    public float bulletDamage = 5f;
    //Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        enemy.SetDestination(Player.position);
        //RaycastHit hit;
        //_direction = hit.point - _shootPoint.position;
        //_direction.Normalize();
        //_direction = new Vector3(_direction.x,0,_direction.z);
        //transform.forward = _direction;
        
        if(CanShoot()){
          Shoot();
        }
        AdjustUI();
    }

    void Shoot(){
        _nextShootTime = Time.time + _delay;
        var bullet = Instantiate(_bulletPrefab, _shootPoint.position, _shootPoint.rotation);
        //bullet.GetComponent<Rigidbody>().velocity = _direction * _bulletSpeed;
        //bullet.Translate(Vector3.forward * (Time.deltaTime * _bulletSpeed));
       // bullet.velocity =transform.TransformDirection(Vector3(0,0, intialSpeed));
    }

    bool CanShoot(){
        return Time.time >= _nextShootTime;
    }
    private void AdjustUI()
    {
        enemyUI.transform.position = new Vector3(transform.position.x, transform.position.y + 4, transform.position.z);
    }
   /* private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
                    Debug.Log("BulletHit");
            other.gameObject.GetComponent<PlayerController>().curHealth -= bulletDamage;
            Destroy(gameObject);
        }
    }*/
}


