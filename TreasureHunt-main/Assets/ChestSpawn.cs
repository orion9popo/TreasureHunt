using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ChestSpawn : MonoBehaviour
{
    public GameObject chest;
    private Vector3[] spawns;
    private bool chestCooldown = false;
    // Start is called before the first frame update
    void Start()
    {
        spawns = new Vector3[transform.childCount];
        for (int i = 0; i < transform.childCount; i++){
            spawns[i] = transform.GetChild(i).position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!chestCooldown){
            GameObject newChest = Instantiate(chest);
            newChest.transform.position = spawns[Random.Range(0,spawns.Length)];
            StartCoroutine(chestRespawn());
        }
    }
    IEnumerator chestRespawn(){
        chestCooldown = true;
        yield return new WaitForSeconds(5f);
        chestCooldown = false;
    }
}
