using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class BigBanditAI : MonoBehaviour
{
    private bool cooldown = false;
    private Vector3 goTo;
    private Animator animator;
    private AudioSource punchSFX;
    public float speed;
    public Transform player;
    public float attackRange;
    public int health = 3; 
    public GameObject hitbox;
    void Start()
    {
        animator = GetComponent<Animator>();
        punchSFX = GetComponent<AudioSource>();
    }

    void Update(){
        Chase();
    }

    void Chase(){
        goTo = player.position;
        this.transform.position = Vector3.MoveTowards(this.transform.position, goTo, speed * Time.deltaTime);
        if((player.position-transform.position).magnitude < attackRange && !cooldown){
            StartCoroutine(PunchSwitch());
            animator.SetTrigger("Punch");
        }
        if(!cooldown)
        transform.LookAt(new Vector3(goTo.x, transform.position.y, goTo.z));
    }
    void Punch(){
        GameObject box = Instantiate(hitbox);
        box.transform.position =  transform.position +transform.forward * 3 + Vector3.down * 2;
        box.transform.LookAt(transform.forward*5 + Vector3.down*2);
        punchSFX.Play();
    }
    IEnumerator PunchSwitch(){
        cooldown = true;
        float oldSpeed = speed;
        speed = 0;
        yield return new WaitForSeconds(1.5f);
        animator.SetTrigger("End");
        speed = oldSpeed;
        cooldown = false;
    }
}
