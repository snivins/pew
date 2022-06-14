using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(PlanetariumSystem))]
[BurstCompile(FloatPrecision.High, FloatMode.Strict)]
public partial class LiderSystem : SystemBase
{
    protected override void OnUpdate()
    {
        // Assign values to local variables captured in your job here, so that it has
        // everything it needs to do its work when it runs later.
        // For example,
        float deltaTime = Time.DeltaTime;
        double timerino = Time.ElapsedTime;
        float movH = Input.GetAxis("Horizontal");
        float movV =  Input.GetAxis("Vertical");
        Vector3d direccion = Vector3d.zero;
        double G = 6.6743e-11;
        // double G_km = 6.6743e-5;
    double sol_mass = 1.989 * 10e30;
        Vector3d posicion = Vector3d.zero;
        Vector3d fuerza_g = Vector3d.zero;
        Vector3d planet_speed = Vector3d.zero;
        // This declares a new kind of job, which is a unit of work to do.
        // The job is declared as an Entities.ForEach with the target components as parameters,
        // meaning it will process all entities in the world that have both
        // Translation and Rotation components. Change it to process the component
        // types you want.
        /* if (timerino > 20d)
         {*/
        double distancia_show = 0;
        //if (timerino < 1d) return;
       /* Entities.ForEach((in Lider lider) =>
            {
                posicion = lider.simulated_pos;

            }).Run();*/
        Lider momo = GetSingleton<Lider>();
        //Entity mimi = GetSingletonEntity<Lider>();
        posicion = momo.simulated_pos;
            Entities.ForEach((in Planeta planet, in PlanetarianOrbit po) =>
            {
                if (posicion != Vector3d.zero)
                {
                    posicion += planet.simulated_speed;
                    direccion = planet.simulated_pos- posicion;
                    double distancia = direccion.magnitude;
                    
                    if (distancia < po.influence_distance)
                    {
                        distancia_show = distancia * 1000d;
                        //distancia *= 100;
                        // distancia *= 10;
                        double forceMagnitude = G * po.masa / Mathd.Pow(distancia_show, 2);
                        fuerza_g = direccion.normalized * forceMagnitude;
                        planet_speed = planet.simulated_speed;
                    }
                }

            }).Run();
        
        if (fuerza_g == Vector3d.zero)
        {
            direccion =  -posicion;
            double distancia = direccion.magnitude;
            double forceMagnitude = G * sol_mass / Mathd.Pow(distancia, 2);
            fuerza_g = direccion.normalized * forceMagnitude;
        }
        
        Entities.ForEach((ref Rotation rotation, ref Lider lider) => {
            // Implement the work to perform for each entity here.
            // You should only access data that is local or that is a
            // field on this job. Note that the 'rotation' parameter is
            // marked as 'in', which means it cannot be modified,
            // but allows this job to run in parallel with other jobs
            // that want to read Rotation component data.
            // For example,
            /*
            if (lider.reseting)
            {
                lider.reseting = false;
            }*/
            lider.distancia = distancia_show;
            lider.old_force = lider.speeeeeed;

            //lider.force = math.mul(rotation.Value, new float3(0, 0, 1)) * deltaTime * 5f;
            lider.force = fuerza_g * deltaTime/1000d;// lo paso a km/s^2
            lider.speeeeeed += lider.force;
            lider.aceleration = lider.speeeeeed   - lider.old_force;
            lider.simulated_pos += lider.speeeeeed * deltaTime;
            lider.simulated_pos += planet_speed ;
            /*if (math.abs(pos.x) > 100f || math.abs(pos.y) > 100f || math.abs(pos.z) > 100f)
            {
                lider.reseting = true;
                lider.offset = -pos;
                translation.Value = float3.zero;
            }*/
            // distancia inicial prox 721588993.247152

            if (movH != 0)
            {
                rotation.Value = math.mul(rotation.Value, quaternion.RotateY(math.radians(movH * deltaTime * 5f)));
            }

            if (movV != 0)
            {
                rotation.Value = math.mul(rotation.Value, quaternion.RotateX(math.radians(movV * deltaTime * 5f)));
            }
        }).Schedule();
    }
}
