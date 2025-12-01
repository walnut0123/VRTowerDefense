using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class DroneAI : MonoBehaviour
{

    enum DroneState
    {
        Idle,
        Move,
        Attack,
        Damage,
        Die
    }


    DroneState state = DroneState.Idle;
    public float idleDelayTime = 2;
    float currentTime;
    //
    public float moveSpeed=1;
    Transform tower;
    UnityEngine.AI.NavMeshAgent agent;
    //
    public float attackRange = 3;
    //
    public float attackDelayTime = 2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tower = GameObject.Find("Tower location").transform;
        print("location"+tower.transform.position);
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.enabled = false;
        agent.speed = moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        print("current State: "+state);
       switch(state)
       {
            case DroneState.Idle:
            Idle();
            break;
            case DroneState.Move:
            Move();
            break;
            case DroneState.Attack:
            Attack();
            break;
            case DroneState.Damage:
            //Damage();
            break;
            case DroneState.Die:
            Die();
            break;
       }


    }

    
    private void Idle()
    {
        currentTime+=Time.deltaTime;
        if(currentTime > idleDelayTime)
        {
            state = DroneState.Move;
            agent.enabled = true;
        }
    }
       
    private void Move()
    {
        agent.SetDestination(tower.position);
        if(Vector3.Distance(transform.position,tower.position) < attackRange)
        {
            state = DroneState.Attack;
            agent.enabled = false;
            //
            currentTime = attackDelayTime;
        }
    }
    private void Attack()
    {
        currentTime += Time.deltaTime;
        if(currentTime > attackDelayTime)
        {
            print("Drone is now attacking");
            Tower.Instance.HP--;
            currentTime=0;
        }
    }
       
    IEnumerator Damage()
    {
        //print("drone got damage");
        agent.enabled = false;
        Material mat = GetComponentInChildren<MeshRenderer>().material;
        Color originalColor = mat.color;
        mat.SetColor("_Color", Color.red);
        yield return new WaitForSeconds(0.1f);
        mat.SetColor("_Color", originalColor);
        state = DroneState.Idle;
        currentTime =0;
    }

    private void Die()
    {

    }

    [SerializeField]
    int hp = 3;

    public void OnDamageProcess()
    {
        hp--;

        if(hp>0)
        {
            state=DroneState.Damage;
            StopAllCoroutines();
            StartCoroutine(Damage());
        }
    }
}
