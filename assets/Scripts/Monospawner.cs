using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using Unity.Physics;
public class Monospawner : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject lider;
    public GameObject follower;
    public int count;
    public float speed = 20f;
    public float separacion = 5f;
    public ParticleSystem ps;
    public ParticleSystem ps1;
    public ParticleSystem ps2;
    public ParticleSystem ps3;
    public ParticleSystem ps4;
    void Start()
    {
        BlobAssetStore bas = new BlobAssetStore();
        GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, bas);
        Entity pref = GameObjectConversionUtility.ConvertGameObjectHierarchy(follower, settings);
        //Entity pref_lider = GameObjectConversionUtility.ConvertGameObjectHierarchy(lider, settings);
        EntityManager em = World.DefaultGameObjectInjectionWorld.EntityManager;
        int fila = 0;
        int columna = 0;
        int depth = 0;
        
        //var instance = em.Instantiate(pref_lider);
        //em.SetComponentData(instance, new Translation { Value = new float3(0,0, 0) });
        // em.GetComponentObject<OffsetSystem_ForEach>().GetSingleton<Lider>() = instance;
        World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<PartiSystem>().Init(ps, ps1,ps2,ps3,ps4);
        double mimi = Mathd.Pow(count, 1d / 3d);
        mimi = Mathd.RoundToInt(mimi);
        Debug.Log(mimi);
        float momo = (float)(mimi*separacion / 2d);
        for (int i = 1; i< count; i++)
        {

            var instance = em.Instantiate(pref);
            float3 pos = new float3(momo - fila* separacion + UnityEngine.Random.Range(-5f, 5f), momo - columna * separacion+ UnityEngine.Random.Range(-5f, 5f), momo - depth * separacion + UnityEngine.Random.Range(-5f, 5f));
            //Debug.Log(pos);
            em.SetComponentData(instance, new Translation { Value = float3.zero });
            
                em.AddComponentData(instance, new Offset { 
                    offset = pos,
                    id = i,
                    vel = float3.zero
                });
            em.AddComponentData(instance, new PartiEmitter());
            columna++;
            if (columna > mimi +1)
            {
                columna = 0;
                fila++;
            }
            if (fila > mimi+1)
            {
                fila = 0;
                depth++;
            }

        }
        bas.Dispose();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
