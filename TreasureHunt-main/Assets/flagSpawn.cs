using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;

public class flagSpawn : MonoBehaviour
{
    public GameObject flag;
    void Start()
    {
        StartCoroutine(waitsome());
    }

    IEnumerator waitsome(){
        yield return new WaitForSeconds(0.01f);
        GameObject flg = Instantiate(flag, transform);
    }
}
