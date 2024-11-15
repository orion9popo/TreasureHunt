using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Hitbox : MonoBehaviour
{
    void OnTriggerEnter(Collider collision){
        if(collision.gameObject.CompareTag("Player")){
            Destroy(gameObject);
        }
        else StartCoroutine(waitSome());

    }
    IEnumerator waitSome(){
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }
}
