using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class LankerAI : MonoBehaviour
{
    private bool cooldown = false;
    private Vector3 goTo;
    private Enum state;
    private Animator animator;
    private Rigidbody rigidbody;
    private BoxCollider boxCollider;
    public float speed;
    public float attackSpeed;
    public float grabRange;
    public Transform player;
    public float attackRange;
    public float attackRange2;
    public int health = 3; 

    enum States{
        Attack,
        Chase,
        Grab
    }
    
    void Start()
    {
        state = States.Chase;
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
    }

    void Update(){
        switch(state){
            case States.Attack:
            Attack();
            break;
            case States.Chase:
            Chase();
            break;
            case States.Grab:
            Grab();
            break;
        }
    }

    void Chase(){
        goTo = player.position;
        this.transform.position = Vector3.MoveTowards(this.transform.position, goTo, speed * Time.deltaTime);
        if((transform.position-player.position).magnitude < attackRange){
            state = States.Attack;
            return;
        }
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
    }

    void Attack(){
        if((transform.position-player.position).magnitude > attackRange){
            animator.ResetTrigger("SlowWalk");
            animator.SetTrigger("End");
            state = States.Chase;
            return;
        }
        goTo = -Vector3.Cross(transform.position - player.position, transform.up) + transform.position;
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        if((transform.position-player.position).magnitude < grabRange && !cooldown){
            animator.SetTrigger("Grab");
            goTo = goTo + transform.forward * 1000;
            state = States.Grab;
            boxCollider.isTrigger = true;
            rigidbody.useGravity = false;
            StartCoroutine(grabSwitch());
            speed = 45;
            return;
        }
        animator.SetTrigger("SlowWalk");
        if((transform.position - player.position).magnitude > attackRange2){
            goTo = player.position;
        }
        this.transform.position = Vector3.MoveTowards(this.transform.position,  goTo, attackSpeed * Time.deltaTime);
       
        /*GameObject projectile = Instantiate(bullet, transform.position, Quaternion.identity);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.velocity = (player.position - transform.position).normalized * projectileSpeed -  0.5f * Physics.gravity * ((player.position - transform.position).magnitude)/projectileSpeed;
        Destroy(projectile, 2f);
        StartCoroutine(waitCooldown());*/

    }
    void Grab(){
        transform.position = Vector3.MoveTowards(this.transform.position, goTo, speed * Time.deltaTime);
        speed = speed * (1-Time.deltaTime);
        transform.LookAt(goTo);
    }
    IEnumerator grabSwitch(){
        float oldSpeed = speed;
        cooldown = true;
        yield return new WaitForSeconds(2f);
        animator.ResetTrigger("SlowWalk");
        animator.SetTrigger("End");
        cooldown = false;
        state = States.Chase;
        speed = oldSpeed;
        boxCollider.isTrigger = false;
        rigidbody.useGravity = true;
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Projectile")){
            health--;
            Destroy(collision.gameObject);

            if (health <= 0){
                Destroy(gameObject);
            }

        }
    }
}
