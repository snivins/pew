using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Physics;
[BurstCompile]

[UpdateAfter(typeof(LiderSystem))]
public partial class OffsetSystem_ForEach : SystemBase
{
   
    protected override void OnUpdate()
    {
        // Assign values to local variables captured in your job here, so that it has
        // everything it needs to do its work when it runs later.
        // For example,
            float deltaTime = Time.DeltaTime;
            float sped = GameObject.FindObjectOfType<Monospawner>().speed;
       // bool reseting = false;
        float3 reset_offset = float3.zero;
        float3 acel = float3.zero;
        // This declares a new kind of job, which is a unit of work to do.
        // The job is declared as an Entities.ForEach with the target components as parameters,
        // meaning it will process all entities in the world that have both
        // Translation and Rotation components. Change it to process the component
        // types you want.
        //query.
        //UnityEngine.Debug.Log(query);
        
        float3 pos = float3.zero;
        quaternion q = quaternion.identity;
        Entities.ForEach(( in Rotation rotation, in Lider lider) => {
            // Implement the work to perform for each entity here.
            // You should only access data that is local or that is a
            // field on this job. Note that the 'rotation' parameter is
            // marked as 'in', which means it cannot be modified,
            // but allows this job to run in parallel with other jobs
            // that want to read Rotation component data.
            // For example,

            /*
            reseting = lider.reseting;
            if (reseting)
            {
                reset_offset = lider.offset;
            }*/
            //if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            //{
                q = rotation.Value;
            //}
            //position.Value +=  new float3(0, 0, 1) * deltaTime;
            acel = lider.aceleration.ToFloat();
            pos = lider.simulated_pos.ToFloat();
        }).Run();

        //Entity momo = GetSingletonEntity<Lider>();
        //momo.GetSingleton<Rotation>(momo );
        Entities.ForEach((ref Offset offset, ref Translation position, ref Rotation rotation) => {
            // Implement the work to perform for each entity here.
            // You should only access data that is local or that is a
            // field on this job. Note that the 'rotation' parameter is
            // marked as 'in', which means it cannot be modified,
            // but allows this job to run in parallel with other jobs
            // that want to read Rotation component data.
            // For example,
            /*Entity mm = GetSingletonEntity<Lider>();
            pos = GetComponentDataFromEntity<Translation>()[mm].Value;
            q = GetComponentDataFromEntity<Rotation>()[mm].Value;*/
            position.Value += acel * -1f;
            float3 posi_rel = position.Value;
            float3 posi_final = pos + math.mul(q, offset.offset);
            /* Vector3d p_f = Vector3d.Tove3(posi_final);
             Vector3d p_i = Vector3d.Tove3(position.Value);
             Vector3d p_n = Vector3d.MoveTowards(p_i, p_f, deltaTime * 3f);*/
            // Vector3d.Tofl3(p_n);
            // avoid vector ops because current scripting backends are terrible at inlining
            float toVector_x = posi_final.x - posi_rel.x;
                float toVector_y = posi_final.y - posi_rel.y;
                float toVector_z = posi_final.z - posi_rel.z;

                float sqdist = toVector_x * toVector_x + toVector_y * toVector_y + toVector_z * toVector_z;
            float distancia = math.distance(posi_rel, posi_final);
            float speed = 0f;
            if (distancia < sped)
            {
                speed = distancia * deltaTime* 0.5f;
            } else
            {
                 speed = sped * deltaTime * 0.5f;
            }


                var dist = (float)math.sqrt(sqdist);

                float3 final_pos = new float3(posi_rel.x + toVector_x / dist * speed,
                    posi_rel.y + toVector_y / dist * speed,
                    posi_rel.z + toVector_z / dist * speed);
            

                position.Value = final_pos;
            /* if (reseting)
            {
                position.Value += reset_offset;
            }*/

            /*
           emitParams.position = position.Value;
           particleSystem.Emit(emitParams, 1);
            */
            /*
            float3 vector3 = posi - position.Value;
            float magnitude = math.rsqrt( math.sqrt(vector3.x)+ math.sqrt(vector3.y)+ math.sqrt(vector3.z));*/
            //if (magnitude > 0.01f )
            //  position.Value =(vector3 / magnitude) *  (deltaTime * math.distance(posi, position.Value));
            //position.Value = posi;
            rotation.Value = q;
        }).ScheduleParallel();

    }
}
