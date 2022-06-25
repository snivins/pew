using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class PlanetariumSystem : SystemBase
{
    protected override void OnUpdate()
    {
        // Assign values to local variables captured in your job here, so that it has
        // everything it needs to do its work when it runs later.
        // For example,
             double timerino = Time.ElapsedTime;

        // This declares a new kind of job, which is a unit of work to do.
        // The job is declared as an Entities.ForEach with the target components as parameters,
        // meaning it will process all entities in the world that have both
        // Translation and Rotation components. Change it to process the component
        // types you want.

        double accuracyTolerance = 1e-7d;

        //if (timerino < 1d) return;
        Entities.ForEach((ref Planeta planeta, in PlanetarianOrbit po) => {
            // Implement the work to perform for each entity here.
            // You should only access data that is local or that is a
            // field on this job. Note that the 'rotation' parameter is
            // marked as 'in', which means it cannot be modified,
            // but allows this job to run in parallel with other jobs
            // that want to read Rotation component data.
            // For example,
            //     translation.Value += math.mul(rotation.Value, new float3(0, 0, 1)) * deltaTime;

            planeta.meanAnomaly = po.n * (timerino - 0);
            planeta.simulated_speed = planeta.simulated_pos;
            double E1 = planeta.meanAnomaly;   //initial guess
            double difference = 1d;
            for (int i = 0; difference > accuracyTolerance && i < 6; i++)
            {
                double E0 = E1;
                double F = planeta.meanAnomaly - E0 + po.eccentricity * Mathd.Sin(E0);
                double DF = (-1f) + po.eccentricity * Mathd.Cos(E0);
                E1 = E0 - F / DF;
                difference = Mathd.Abs(E1 - E0);
            }
            double EccentricAnomaly = E1;

            double trueAnomaly = 2 * Mathd.Atan(po.trueAnomalyConstant * Mathd.Tan(EccentricAnomaly / 2d));
            double distance = po.semiMajorAxis * (1d - po.eccentricity * Mathd.Cos(EccentricAnomaly));

            double cosAOPPlusTA = Mathd.Cos(po.argumentOfPeriapsis + trueAnomaly);
            double sinAOPPlusTA = Mathd.Sin(po.argumentOfPeriapsis + trueAnomaly);

            double x = distance * ((po.cosLOAN * cosAOPPlusTA) - (po.sinLOAN * sinAOPPlusTA * po.cosI));
            double z = distance * ((po.sinLOAN * cosAOPPlusTA) + (po.cosLOAN * sinAOPPlusTA * po.cosI));      //Switching z and y to be aligned with xz not xy
            double y = distance * (po.sinI * sinAOPPlusTA);
            planeta.simulated_pos = new Vector3d(x,y,z);
            planeta.simulated_speed = planeta.simulated_pos - planeta.simulated_speed;

        }).Schedule();
    }
}
