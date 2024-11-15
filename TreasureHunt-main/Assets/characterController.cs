using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float gravity = -90.81f;
    public float speed = 6.0f;
    public float turnSmoothTime = 0.1f;
    public float jumpHeight = 1f;
    public float sensitivity = -1f;
    public Camera cam;
    public GameObject chest;
    public TextMeshProUGUI scoreText;
    public GameObject[] enemies;
    public Slider HealthBar;
    public AudioClip coin;
    private AudioSource treasureGet;
    CharacterController characterController;
    private float turnSmoothVelocity;
    private bool cooldown = false;
    private float verticalVelocity = -1;
    private Vector3 rotate;
    private float x;
    private float y;
    private int health = 10;
    private ParticleSystem footDust;
    private bool carryingChest= false;
    private int score = 0;
    
    Animator animator;



    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        footDust = transform.GetComponentInChildren<ParticleSystem>();
        treasureGet = gameObject.AddComponent<AudioSource>();
        treasureGet.clip = coin;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 input = new Vector2();
        Vector3 move = new Vector3();
        
        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Vertical");
        if(Input.GetMouseButton(1)){
             y = Input.GetAxis("Mouse X");
            x = Input.GetAxis("Mouse Y");
            rotate = new Vector3(x, y * sensitivity, 0);
            cam.transform.eulerAngles = cam.transform.eulerAngles - rotate;
        }
        cam.transform.position = transform.position - 10*cam.transform.forward + Vector3.up * 2;

        Vector3 direction = (cam.transform.forward * input.y - Vector3.Cross(cam.transform.forward, cam.transform.up) * input.x).normalized;
        if (direction.magnitude > 0.1){
            if(carryingChest){
                animator.ResetTrigger("ChestStop");
                animator.SetTrigger("End");
            }
            else{
                animator.ResetTrigger("End");
                animator.SetTrigger("Run");
            }
            footDust.Play(true);
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            move = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            move = move.normalized * speed;
            }
            else {
                if(carryingChest){
                    animator.ResetTrigger("End");
                    animator.SetTrigger("ChestStop");
                }
                else{
                    animator.ResetTrigger("Run");
                    animator.SetTrigger("End"); 
                }
                //footDust.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
        if(characterController.isGrounded){
            verticalVelocity = 0;
        }
        if (Input.GetButton("Jump") && characterController.isGrounded)
        {
            verticalVelocity = Mathf.Sqrt(gravity * -3 * jumpHeight);
        }
        move.y = verticalVelocity;
         verticalVelocity += gravity * Time.deltaTime;
        characterController.Move(move * Time.deltaTime);
    }
    IEnumerator waitCooldown(){
        cooldown = true;
        yield return new WaitForSeconds(0.5f);
        cooldown = false;
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Hitbox") || collision.gameObject.CompareTag("Enemy") && !cooldown){
            health--;
            StartCoroutine(waitCooldown());
            HealthBar.value = (float)health/10;
            if (health <= 0){
                SceneManager.LoadScene(2);
            }
        }
        if(collision.gameObject.CompareTag("Flag") && carryingChest){
            animator.ResetTrigger("ChestGrab");
            animator.SetTrigger("ChestDown");
            treasureGet.Play();
            carryingChest = false;
            chest.SetActive(false);
            score++;
            scoreText.text = "Score:" + score.ToString();
        }
        if(collision.gameObject.CompareTag("Chest") && !carryingChest){
            animator.ResetTrigger("ChestDown");
            animator.SetTrigger("ChestGrab");
            chest.SetActive(true);
            GameObject enemy = Instantiate(enemies[Random.Range(0,enemies.Length)],transform.forward * -10 + transform.right * Random.Range(-5f,5f) + transform.position,Quaternion.identity);
            if(enemy.CompareTag("BigGuy")){
                BigBanditAI ai = enemy.GetComponent<BigBanditAI>();
                ai.player = transform;
            }
            else{
                LankerAI ai = enemy.GetComponent<LankerAI>();
                ai.player = transform;
            }
            carryingChest = true;
            Destroy(collision.gameObject);
        }
    }
}
