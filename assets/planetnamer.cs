using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class planetnamer : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject namerino;
    string[] nombres;
    void Start()
    {
        nombres = new string[GameObject.FindGameObjectsWithTag("Planeta").Length];
        int p = 0;
        foreach (GameObject planetita in GameObject.FindGameObjectsWithTag("Planeta"))
        {
            nombres[p] = planetita.name;
            p++;
        }
        for (int i = 0; i < nombres.Length; i++) {
            GameObject go = Instantiate(namerino, Vector3.zero, Quaternion.identity);
            go.GetComponent<planetfoloow>().nombre = nombres[i];
            go.transform.SetParent(transform);
        }
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
