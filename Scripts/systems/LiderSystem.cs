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
        bool movF =  Input.GetKey(KeyCode.Space);
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
         double planet_mass = 0;
         Vector3d planet_pos = Vector3d.zero;
        double distancia_show = 0;
        //if (timerino < 1d) return;
       /* Entities.ForEach((in Lider lider) =>
            {
                posicion = lider.simulated_pos;

            }).Run();*/
        Lider momo = GetSingleton<Lider>();
        //Entity mimi = GetSingletonEntity<Lider>();
        posicion = momo.simulated_pos;
        bool cambio =momo.real_sim;
        double accuracyTolerance = 1e-7d;
        if (movF) cambio = true;
        if ( true || cambio && movF ){
            Entities.ForEach((in Planeta planet, in PlanetarianOrbit po) =>
            {
                if (posicion != Vector3d.zero)
                {
                    Vector3d posicion_sim = posicion + planet.simulated_speed;
                    direccion = planet.simulated_pos- posicion_sim;
                    double distancia = direccion.magnitude;
                    
                    if (distancia < po.influence_distance)
                    {
                        distancia_show = distancia * 1000d;
                        //distancia *= 100;
                        // distancia *= 10;
                        double forceMagnitude = (G * po.masa) / Mathd.Pow(distancia_show, 2);
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
                lider.simulated_pos += planet_speed;
                lider.distancia = distancia_show;
                lider.old_force = lider.speeeeeed;

                //lider.force = math.mul(rotation.Value, new float3(0, 0, 1)) * deltaTime * 5f;
                lider.force = fuerza_g /1000d;// lo paso a km/s^2
                lider.speeeeeed += lider.force * deltaTime ;
                lider.aceleration = lider.speeeeeed   - lider.old_force;
                if (movF){
                    lider.speeeeeed += new Vector3d( math.mul(rotation.Value, new float3(0, 0, 1)) * deltaTime * 5f);
                }
                lider.simulated_pos += lider.speeeeeed * deltaTime;
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
                lider.real_sim = cambio;
            }).Schedule();
        } 
            Entities.ForEach((in Planeta planet, in PlanetarianOrbit po) =>
            {
                if (posicion != Vector3d.zero)
                {
                    Vector3d posicion_sim = posicion + planet.simulated_speed;
                    direccion = planet.simulated_pos- posicion_sim;
                    double distancia = direccion.magnitude;
                    
                    if (distancia < po.influence_distance)
                    {
                        distancia_show = distancia * 1000d;
                        //distancia *= 100;
                        // distancia *= 10;
                        planet_pos = planet.simulated_pos;
                        planet_mass = po.masa;
                        planet_speed = planet.simulated_speed;
                    }
                }

            }).Run();
        
            if (planet_mass == 0)
            {
                direccion =  -posicion;
                double distancia = direccion.magnitude;
                distancia_show = distancia * 1000d;
                //fuerza_g = direccion.normalized * forceMagnitude;
                        planet_pos = Vector3d.zero;
                        planet_mass = sol_mass;
                        planet_speed = Vector3d.zero;
            }
        
        if (false && momo.real_sim && !movF){

            Entities.ForEach((ref PlanetarianOrbit po, in Lider lider) => {

                po.mu = G * planet_mass;
                double fuerza = (Mathd.Pow(lider.speeeeeed.magnitude*1000d, 2) / 2d) - (po.mu / distancia_show);
                po.semiMajorAxis = po.mu / (2 * -fuerza);
                po.n = Mathd.Sqrt(po.mu / Mathd.Pow(po.semiMajorAxis, 3));
                double h = (distancia_show * lider.speeeeeed.magnitude *1000d);
                Vector3d eccentricity_vector = ((Mathd.Pow(lider.speeeeeed.magnitude*1000d, 2)-po.mu/distancia_show)*(direccion*1000d) -Vector3d.Cross(Vector3d.Cross(direccion*1000d,lider.speeeeeed*1000d),lider.speeeeeed*1000d))/ po.mu;
                //Vector3d eccentricity_vector = (Vector3d.Cross(lider.speeeeeed*1000d, momentum)/po.mu)-diferencia/diferencia.magnitude;
                
       // Vector3d eccentricity_vector = ((Mathd.Pow(v.magnitude, 2)-u/diferencia.magnitude)*diferencia -Vector3d.Cross(Vector3d.Cross(diferencia,v),v))/ u;
                double eccentricity = Mathd.Sqrt(1 + ((2 * fuerza * Mathd.Pow(h, 2)) / Mathd.Pow(po.mu, 2))); 
               // double eccentricity = eccentricity_vector.magnitude; 
                if (eccentricity < math.EPSILON) eccentricity = 0;
                po.eccentricity = eccentricity;
                po.trueAnomalyConstant = Mathd.Sqrt((1 + eccentricity) / (1 - eccentricity));
                Vector3d posicion_sim = posicion  + planet_speed;
                Vector3d diferencia = planet_pos- posicion_sim;

                Vector3d momentum = Vector3d.Cross(diferencia*1000d, lider.speeeeeed*1000d);
               po.e_vector = planet_pos;
                double inclinacion = Mathd.Acos(momentum.y/ momentum.magnitude);
                Vector3d n = Vector3d.Cross(Vector3d.up, momentum);
               po.n_vector = n;
                double longitud_an = 0;
                if (n != Vector3d.zero){
                    longitud_an = Mathd.Acos(n.x / n.magnitude);
                    po.masa = longitud_an;
                }
                if (longitud_an < Mathd.Epsilon) longitud_an =0;
                po.cosLOAN = Mathd.Cos(longitud_an);
                po.sinLOAN = Mathd.Sin(longitud_an);
                po.cosI = Mathd.Cos(inclinacion);
                po.sinI = Mathd.Sin(inclinacion);
                po.argumentOfPeriapsis  = 0;
                if (longitud_an == 0d){
                    po.argumentOfPeriapsis = (double)Mathf.Atan2((float)eccentricity_vector.z,(float)eccentricity_vector.x);
                } else {
                    po.argumentOfPeriapsis = Mathd.Acos((Vector3d.Cross(n, eccentricity_vector)/(n.magnitude * eccentricity_vector.magnitude)).magnitude);
                }

            }).Run();
            cambio = false;
        }
        if (false && !cambio && !movF){
            
            Entities.ForEach((ref Lider lider, in PlanetarianOrbit po) => {
                lider.meanAnomaly = po.n * (timerino*100 -0) +  Mathd.PI/2;
                lider.speeeeeed = lider.simulated_pos;
                double E1 = lider.meanAnomaly;   //initial guess
                double difference = 1d;
                for (int i = 0; difference > accuracyTolerance && i < 6; i++)
                {
                    double E0 = E1;
                    double F = lider.meanAnomaly - E0 + po.eccentricity * Mathd.Sin(E0);
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
                double y = -distance * (po.sinI * sinAOPPlusTA);
                lider.simulated_pos = planet_pos +new Vector3d(x,y,z)/1000d  ;
                lider.speeeeeed = (lider.simulated_pos-lider.speeeeeed).normalized*Mathd.Sqrt(po.mu/po.semiMajorAxis);
                lider.speeeeeed = lider.speeeeeed/1000d;
                lider.real_sim = false;
            }).Schedule();
        }
    }
}
