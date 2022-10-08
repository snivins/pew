using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class followerinoCam : MonoBehaviour
{
    // Start is called before the first frame update
    public Entity ent;
    public float3 offset;
    private EntityManager em;
    public Vector3d sim_pos;
    void Start()
    {
        em = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (ent == Entity.Null) return;
        Translation tn = em.GetComponentData<Translation>(ent);
        sim_pos = em.GetComponentData<Lider>(ent).simulated_pos;
        transform.position = tn.Value + offset;
    }
}
