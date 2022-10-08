using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using TMPro;
[UpdateInGroup(typeof(LateSimulationSystemGroup))]
public class SimulatedSpace : MonoBehaviour
{
    // Starbefore the first frame update
    public Entity ent;
    public float3 offset;
    private EntityManager em;
    public Vector3d sim_pos;
    public TextMeshProUGUI tmp;
    double vel;
    public static string Map_mode = "solar_scale";
    public CustomCamerino camerino;

    public GameObject followerino;
    public Vector3 sped_show;
    void Start()
    {
        em = World.DefaultGameObjectInjectionWorld.EntityManager;
        tmp = GameObject.Find("Text_velocidad").GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        /*
        if (ent == Entity.Null) return;
        sim_pos = em.GetComponentData<Lider>(ent).simulated_pos;
        transform.position =-( sim_pos).ToFloat();
        */
        
        if (ent == Entity.Null) return;
        /*Vector3d pos_offset = Vector3d.zero;
        if (followerino != null) pos_offset = followerino.GetComponent<OnRailsdObject>().sped;
        //sim_pos = em.GetComponentData<Lider>(ent).simulated_pos + camerino.sim_pos;
        sped_show = Vector3d.Tofl3(pos_offset);
       // sim_pos =  ;
        sim_pos -= pos_offset + camerino.sim_pos;
        // camerino.sim_pos = sim_pos;

        //sim_pos = camerino.sim_pos;
        transform.position =(sim_pos/1000d).ToFloat();


        */

        vel = em.GetComponentData<Lider>(ent).speeeeeed.magnitude;
        if (vel< 100)
        {
            vel = vel * 1000;
            tmp.text = vel.ToString("#.00") + " m/s";
        } else
        {

            tmp.text = vel.ToString("#.00") + " Km/s";
        }
    }
}
