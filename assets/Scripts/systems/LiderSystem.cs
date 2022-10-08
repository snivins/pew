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
        Vector3d diff = Vector3d.zero;
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
        FixedString64Bytes planeta_orbitado = "";
        if (movF) cambio = true;
        Entities.ForEach((in Planeta planet, in PlanetarianOrbit po) =>
        {
            if (posicion != Vector3d.zero)
            {
                Vector3d posicion_sim = posicion + planet.simulated_speed;
                direccion = planet.simulated_pos - posicion_sim;
                double distancia = direccion.magnitude;

                if (distancia < po.influence_distance)
                {
                    distancia_show = distancia * 1000d;
                    //distancia *= 100;
                    // distancia *= 10;
                    /*if (!movF)
                    {*/
                        diff = direccion;
                        planet_pos = planet.simulated_pos;
                        planet_mass = po.masa;
                    planeta_orbitado = po.nombre;
                   /* }
                    else
                    {*/

                        double forceMagnitude = (G * po.masa) / Mathd.Pow(distancia_show, 2);
                        fuerza_g = direccion.normalized * forceMagnitude;
                        planet_speed = planet.simulated_speed;
                   // }
                }
            }

        }).Run();

        /*if (fuerza_g == Vector3d.zero)
        {
            direccion = -posicion;
            double distancia = direccion.magnitude;
            double forceMagnitude = G * sol_mass / Mathd.Pow(distancia, 2);
            fuerza_g = direccion.normalized * forceMagnitude;
        }*/

        if ( true || cambio && movF ){
            
            
            Entities.ForEach((ref Translation trans, ref Rotation rotation, ref Lider lider) => {
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
                //trans.Value = (lider.simulated_pos  ).ToFloat();
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
            
        
        if (/*cambio &&*/false && !movF){

            Entities.ForEach((ref PlanetarianOrbit po, in Lider lider) => {

                po.mu = G * planet_mass;
                Vector3d v_vector = lider.speeeeeed * 1000d;
                po.n_vector = diff *1000d;
                Vector3d r_vector = diff * 1000d;
                double fuerza = (Mathd.Pow(v_vector.magnitude, 2) / 2d) - (po.mu / r_vector.magnitude);
                po.semiMajorAxis = po.mu / (2 * -fuerza);
                po.n = Mathd.Sqrt(po.mu / Mathd.Pow(math.abs(po.semiMajorAxis), 3));
                Vector3d h_vector = Vector3d.Cross(r_vector, v_vector);
                Vector3d n_vector = Vector3d.Cross(Vector3d.up, h_vector);

                Vector3d e_vector = (((Mathd.Pow(v_vector.magnitude, 2) / po.mu) - (1 / r_vector.magnitude)) * r_vector) - ((Mathd.Dot(r_vector, v_vector) * v_vector) / po.mu);
                double eccentricity = e_vector.magnitude; 
               // double eccentricity = eccentricity_vector.magnitude; 

                po.eccentricity = eccentricity;
                po.trueAnomalyConstant = Mathd.Sqrt((1 + eccentricity) / (1 - eccentricity));
               po.e_vector = planet_pos;
                double specific_me = (Mathd.Pow(v_vector.magnitude, 2) / 2d) - (po.mu / r_vector.magnitude);
                double semimayoraxis = 0;
                double apoapsis = 0;
                double periapsis = 0;
                if (eccentricity < 1)
                {
                    semimayoraxis = -po.mu / (2 * specific_me);
                    apoapsis = semimayoraxis * (1 + eccentricity);
                    periapsis = semimayoraxis * (1 - eccentricity);
                }
                else
                {
                    semimayoraxis = double.PositiveInfinity;
                    periapsis = Mathd.Pow(h_vector.magnitude, 2) / po.mu;
                    apoapsis = double.PositiveInfinity;
                }
                double inclinacion = math.acos(h_vector.y / h_vector.magnitude);
                double loa = math.acos(n_vector.x / n_vector.magnitude);//longitud of ascending node
                double aop = math.acos(Mathd.Dot(n_vector, e_vector) / (n_vector.magnitude * e_vector.magnitude)); //argument of periapsis
                if (inclinacion == 0)
                {

                    loa = 0;//longitud of ascending node
                    aop = 0; //argument of periapsis
                }
                po.cosLOAN = math.cos(loa);
                po.sinLOAN = math.sin(loa);
                po.sinI = math.sin(inclinacion);
                po.cosI = math.cos(inclinacion);
                po.argumentOfPeriapsis = aop;
                po.masa = math.acos(r_vector.x / r_vector.magnitude);
                po.nombre = planeta_orbitado;
            }).Run();
            cambio = false;
        }
        if (false && movF){
            
            Entities.ForEach((ref Lider lider, in PlanetarianOrbit po) => {
                lider.meanAnomaly = po.n * (timerino *100) +  Mathd.PI/2;
                Vector3d old_pos = lider.simulated_pos;
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
                lider.simulated_pos = planet_pos +(new Vector3d(x,y,z)/1000d)  ;
                lider.speeeeeed = (old_pos - lider.simulated_pos).normalized*Mathd.Sqrt(po.mu/po.semiMajorAxis);
                lider.speeeeeed = lider.speeeeeed/1000d;
                lider.real_sim = false;
            }).Schedule();
        }
    }
}
