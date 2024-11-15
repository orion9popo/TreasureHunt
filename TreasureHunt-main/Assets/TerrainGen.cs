using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;

public class TerrainGen : MonoBehaviour
{
    public GameObject[] tiles;
    public GameObject[] props;
    public int row;
    public int column;
    public int beginningRow;
    public int beginningColumn;
    float deltax = 5.36f;
    float deltaz = 4.65f;
    void Start()
    {
        for (int i = beginningRow; i < row; i++){
            for (int j = beginningColumn; j < column; j++){
                RaycastHit hit;
                UnityEngine.Vector3 pos = new UnityEngine.Vector3(i*deltax + deltax*0.5f*(j%2),1000,j*deltaz);
                if(Physics.Raycast(pos, UnityEngine.Vector3.down, out hit)){
                    Instantiate(tiles[0],hit.point,UnityEngine.Quaternion.identity);
                    if(Random.Range(0f,1f) > 0.9f){
                        GameObject prop = Instantiate(props[Random.Range(0,props.Length)], hit.point, UnityEngine.Quaternion.Euler(0,Random.Range(0,6)*15,0));
                        float Scale = Random.Range(2f,3f);
                        prop.transform.localScale = new UnityEngine.Vector3(Scale,Scale,Scale);
                    } 
                }
            }
        }
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
