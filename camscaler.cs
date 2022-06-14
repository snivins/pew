using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camscaler : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject solar_ref;
    public GameObject othercam;
    public Transform tierra;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3d.ToFloaterino(solar_ref.GetComponent<SimulatedSpace>().sim_pos );// + othercam.transform.position /10000f;
        transform.rotation = othercam.transform.rotation;
        Debug.Log((transform.position-tierra.position).z);
    }
}
