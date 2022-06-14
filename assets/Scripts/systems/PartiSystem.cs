using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class PartiSystem : SystemBase
{
    ParticleSystem ps;
    ParticleSystem ps1;
    ParticleSystem ps2;
    ParticleSystem ps3;
    ParticleSystem ps4;

    ParticleSystem.EmitParams emitParams;
    bool enabled; 
    const float particlesPerSecond = 1f;
    const float interval = 1f / particlesPerSecond;
    double nextSpawnTime;
    public void Init(ParticleSystem pas, ParticleSystem pas1, ParticleSystem pas2, ParticleSystem pas3, ParticleSystem pas4)
    {
        ps = pas;
        ps1 = pas1;
        ps2 = pas2;
        ps3 = pas3;
        ps4 = pas4;
        enabled = true; // Now we have a ParticleSystem, can begin running the system
    }
    protected override void OnUpdate()
    {

        if (!enabled) return;
        // Assign values to local variables captured in your job here, so that it has
        // everything it needs to do its work when it runs later.
        // For example,
        //     float deltaTime = Time.DeltaTime;

        // This declares a new kind of job, which is a unit of work to do.
        // The job is declared as an Entities.ForEach with the target components as parameters,
        // meaning it will process all entities in the world that have both
        // Translation and Rotation components. Change it to process the component
        // types you want.

        var time = Time.ElapsedTime;
        if (false && time >= nextSpawnTime)
        {
            nextSpawnTime = time + interval;
            int count = 0;
            int ps_number = 0;
            Entities.WithAll<PartiEmitter>().ForEach((in Translation position) =>
            {
                // Implement the work to perform for each entity here.
                // You should only access data that is local or that is a
                // field on this job. Note that the 'rotation' parameter is
                // marked as 'in', which means it cannot be modified,
                // but allows this job to run in parallel with other jobs
                // that want to read Rotation component data.
                // For example,
                //     translation.Value += math.mul(rotation.Value, new float3(0, 0, 1)) * deltaTime;

                emitParams.position = position.Value;
                if (count >= 4000)
                {
                    count = 0;
                    ps_number++;
                }
                if (ps_number == 0)
                {
                    ps.Emit(emitParams, 1);

                }
                if (ps_number == 1)
                {
                    ps1.Emit(emitParams, 1);

                }
                if (ps_number == 2)
                {

                    ps2.Emit(emitParams, 1);
                }
                if (ps_number == 3)
                {

                    ps3.Emit(emitParams, 1);
                }
                if (ps_number == 4)
                {

                    ps4.Emit(emitParams, 1);
                }
            }).WithoutBurst().Run();
        }

    
    }
}
