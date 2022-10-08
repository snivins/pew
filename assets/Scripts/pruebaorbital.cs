using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class pruebaorbital : MonoBehaviour
{
    public double escala_temporal = 1;
    public double tiempo_pasado = 0;
    // Start is called before the first frame update
    double meanAnomaly = 0;
    double meanLongitude = 0;
    double maxIterations = 5;
    double eccentricity_o = 0;
    double semiMajorAxis = 0;
    double trueAnomalyConstant = 0;
    double argumentOfPeriapsis = 0;
    double cosLOAN = 0;
    double sinLOAN = 0;
    double cosI = 0;
    double sinI = 0;
    double n = 0;
    double truanomaly = 0;
    double init_vel = 0;
     double accuracyTolerance = 1e-7d;
    Vector3d og_pos = new Vector3d(0, 0, -6779d);
    Vector3d my_pos = new Vector3d(-69693292.5867402d, 16710563.4486748d, 128451602.981214d);
    Vector3d last_pos = new Vector3d(-69693292.5867402d, 16710563.4486748d, 128451602.981214d);

    Vector3d earth_pos = new Vector3d(0, 0, 0);
    double G = 6.6743e-11;
    //double jup_mass = 1.898e27;
    double earth_mass = 5.972e24;
    double timerino = 0;
    double U =0;
    Vector3d v_vector = Vector3d.zero;
    void Start()
    {
        U = G * earth_mass;
        calculateOrbit(true);
    }
    void calculateOrbit(bool inicio)
    {
        Vector3d diferencia = Vector3d.zero;
        Vector3d oldv_vector = Vector3d.zero;
        if (inicio)
        {

            diferencia = (earth_pos * 1000d) - (og_pos * 1000d);
            my_pos = -diferencia;
            
            //transform.position = -diferencia.ToFloat() / 1000000;
            //double init_vel = Mathd.Sqrt((G * jup_mass) / ((jup_pos - my_pos).magnitude));
            init_vel = Mathd.Sqrt((G * earth_mass) / ((diferencia).magnitude));
            v_vector = new Vector3d(init_vel, 0d, 0d); //velocity vector
        } else
        {

            Vector3d earth_pos2 = Vector3d.zero;
            diferencia = -(my_pos);
            //double init_vel = Mathd.Sqrt((G * jup_mass) / ((jup_pos - my_pos).magnitude));
            //init_vel = (my_pos - last_pos).normalized * Mathd.Sqrt(U / semiMajorAxis);
            oldv_vector = v_vector;
            v_vector = (my_pos - last_pos).normalized * Mathd.Sqrt(U / semiMajorAxis); //velocity vector
        }
        Vector3d r_vector = diferencia;//position vector, la posicion relativa del satelite con respecto al planeta
        double fuerza = (Mathd.Pow(v_vector.magnitude, 2) / 2d) - (U / diferencia.magnitude);
        double semiMajorAxi2s = U / (2 * -fuerza);
        Vector3d h_vector = Vector3d.Cross(r_vector, v_vector);//angular momentum 
        double eccentricity2 = Mathd.Sqrt(1 + ((2 * fuerza * Mathd.Pow(h_vector.magnitude, 2)) / Mathd.Pow(U, 2))); // h o esto sta mal
        Vector3d n_vector = Vector3d.Cross(Vector3d.up, h_vector);
        Vector3d e_vector = (((Mathd.Pow(v_vector.magnitude, 2) / U) - (1 / r_vector.magnitude)) * r_vector) - ((Mathd.Dot(r_vector, v_vector) * v_vector) / U);
        Vector3d e_vector2 = (Vector3d.Cross(v_vector, h_vector) / U) - (r_vector / r_vector.magnitude);
        Vector3d e_vector3 = (Mathd.Pow(v_vector.magnitude, 2) * r_vector / U) - ((Mathd.Dot(r_vector, v_vector) * v_vector) / U) - (r_vector / r_vector.magnitude);
        Vector3d e_vector4 = (Mathd.Pow(v_vector.magnitude, 2) * r_vector / U) - Vector3d.right;
        double eccentricity = e_vector.magnitude;
        eccentricity_o = e_vector.magnitude;
        double specific_me = (Mathd.Pow(v_vector.magnitude, 2) / 2d) - (U / r_vector.magnitude);
        double semimayoraxis = 0;
        double apoapsis = 0;
        double periapsis = 0;
        string tipo = "circular";
        double inclinacion = math.acos(h_vector.y / h_vector.magnitude);

        //semiMajorAxis = semimayoraxis;
        double loa = math.acos(n_vector.x / n_vector.magnitude);//longitud of ascending node
        double aop2 = math.acos(Mathd.Dot(n_vector, e_vector) / (n_vector.magnitude * e_vector.magnitude)); //argument of periapsis
        double aop = math.atan(e_vector.z / e_vector.x);
        if (inclinacion == 0)
        {

            loa = 0;//longitud of ascending node
            //aop = 0;// Mathd.PI * 1.5; //argument of periapsis 0.5* pi pi 1.5pi 2pi 
            truanomaly = math.acos(r_vector.x / r_vector.magnitude);
            if (v_vector.x > 0)
            {
                truanomaly = Mathd.TAU - truanomaly;
            }
            if (eccentricity < Mathd.Epsilon)
            {
                aop = 0;
            } else
            {
                aop = math.acos(e_vector.x / e_vector.magnitude);
            }
        } else
        {

            loa = math.acos(n_vector.x / n_vector.magnitude);//longitud of ascending node
            if (n_vector.y < 0)
            {
                loa = Mathd.TAU - loa;
            }
            aop = math.acos(Mathd.Dot(n_vector, e_vector) / (n_vector.magnitude * e_vector.magnitude));
        }
        double lop = loa - aop; //longitud of periapsis
                                //double ma = math.acos(Mathd.Dot(n_vector, r_vector) / (n_vector.magnitude * r_vector.magnitude));

        if (eccentricity < 0.0001d)
        {
            semimayoraxis = -U / (2 * specific_me);
            apoapsis = semimayoraxis * (1 + eccentricity);
            periapsis = semimayoraxis * (1 - eccentricity);

            if (inclinacion < Mathd.Epsilon)
            {

                truanomaly = math.acos(r_vector.x / r_vector.magnitude);
                if (v_vector.x > 0)
                {
                    truanomaly = Mathd.TAU - truanomaly;
                }
            } else
            {

                truanomaly = math.acos(Mathd.Dot(n_vector, r_vector) / (n_vector.magnitude * r_vector.magnitude));
                
                    if(Mathd.Dot(n_vector, v_vector) > 0)
                {
                    truanomaly = Mathd.TAU - truanomaly;
                }
            }
        }else if (eccentricity < 1)
        {
                tipo = "eliptica";
                semimayoraxis = -U / (2 * specific_me);
            apoapsis = semimayoraxis * (1 + eccentricity);
            periapsis = semimayoraxis * (1 - eccentricity);
            
            truanomaly = math.acos(Mathd.Dot(e_vector, r_vector) / (e_vector.magnitude * r_vector.magnitude));
            if (Mathd.Dot(r_vector, v_vector) < 0)
            {
                truanomaly = Mathd.TAU - truanomaly;
            }
            if (e_vector.y > 0)
            {
                aop = Mathd.TAU - aop;
            }
        }
        else
        {
            semimayoraxis = double.PositiveInfinity;
            periapsis = Mathd.Pow(h_vector.magnitude, 2) / U;
            apoapsis = double.PositiveInfinity;
            if (eccentricity == 1)
            {
                tipo = "parabolica";
            }
            else
            {

                tipo = "hiperbolica";
            }
            
        }



        semiMajorAxis = semimayoraxis;

        n = Mathd.Sqrt(U / Mathd.Pow(math.abs(semiMajorAxis), 3));
        cosLOAN = math.cos(loa);
        sinLOAN = math.sin(loa);
        sinI = math.sin(inclinacion);
        cosI = math.cos(inclinacion);
        argumentOfPeriapsis = aop;
        trueAnomalyConstant = Mathd.Sqrt((1 + eccentricity) / (1 - eccentricity));

        //double truanomaly = 2 * Mathd.Atan(trueAnomalyConstant * Mathd.Tan(EccentricAnomaly / 2));
        // truanomaly/2 = Mathd.Atan(trueAnomalyConstant * Mathd.Tan(EccentricAnomaly / 2));
        // Mathd.tan(truanomaly/2) = trueAnomalyConstant * Mathd.Tan(EccentricAnomaly / 2)
        // Mathd.tan(truanomaly/2)/trueAnomalyConstant =  Mathd.Tan(EccentricAnomaly / 2)
        // Mathd.atan(Mathd.tan(truanomaly/2)/trueAnomalyConstant) =  EccentricAnomaly / 2
        // Mathd.atan(Mathd.tan(truanomaly/2)/trueAnomalyConstant) * 2 =  EccentricAnomaly 
        double ecentricAnomaly =math.atan(math.tan(truanomaly / 2) / trueAnomalyConstant) * 2;

        double meanAnomaly2 = ecentricAnomaly - eccentricity_o * math.sin(ecentricAnomaly);

        timerino = Time.time;
         meanLongitude = (-meanAnomaly2 / n) + tiempo_pasado;
        timerino = (-meanAnomaly2 / n);
        Debug.Log("old_v= " + oldv_vector + "v= " + v_vector);
        /*Debug.Log("old_v= " + oldv_vector.magnitude + "v= " + v_vector.magnitude);
         Debug.Log("n= " + n_vector + "v= " + v_vector + " r= " + r_vector + " h= " + h_vector + " e= " + e_vector + " e12= " + e_vector2 + " e13= " + e_vector3 + " e14= " + e_vector4 + " e " + eccentricity2);
         Debug.Log(" ecentricity= " + eccentricity + " specific_me= " + specific_me + " semimayoraxis= " + semimayoraxis + " semimayoraxi2s= " + semiMajorAxi2s + " apoapsis= " + apoapsis + " periapsis= " + periapsis);
         Debug.Log(" tipo de orbita= " + tipo + " inclinacion= " + inclinacion + " longitud of ascending node= " + loa + " argument of periapsis= " + aop + " argument of periapsis2= " + aop2 + " longitud of periapsis= " + lop);
         //Debug.Log(" truanomaly= " + truanomaly + "vmag= " + v_vector.magnitude + "rmag= " + r_vector.magnitude + "hmag= " + h_vector.magnitude + "emag= " + e_vector.magnitude );
         Debug.Log(" ecentricAnomaly= " + ecentricAnomaly + " truanomaly= " + truanomaly + " meanLongitude= " + meanLongitude + " timerino= " + timerino);
        */
        Debug.Log(" ecentricity= " + eccentricity + " e== " + e_vector + " ex= " + e_vector.x + " ey= " + e_vector.y + " ez= " + e_vector.z);

        //meanAnomaly = (double)(n * (Time.time  - meanLongitude));
        //meanAnomaly/n - Time.time  =- meanLongitude
        //meanAnomaly = (double)(n * (Time.time - meanLongitude));
        //timerino = meanAnomaly / n;
        //meanAnomaly = (double)(n * (timerino - meanLongitude));

        /*
        
        double E1 = meanAnomaly;   //initial guess
        double difference = 1f;
        for (int i = 0; difference > accuracyTolerance && i < maxIterations; i++)
        {
            double E0 = E1;
            double F = meanAnomaly - E0 + eccentricity_o * Mathd.Sin(E0);
            double DF = (-1f) + eccentricity_o * Mathd.Cos(E0);
            E1 = E0 - F / DF;
            difference = Mathd.Abs(E1 - E0);
        }
        double EccentricAnomaly = E1;

         //EccentricAnomaly = ecentricAnomaly;

        double trueAnomaly = 2 * Mathd.Atan(trueAnomalyConstant * Mathd.Tan(EccentricAnomaly / 2));*/
        double trueAnomaly = truanomaly;
        double EccentricAnomaly = ecentricAnomaly;
        double distance = semiMajorAxis * (1 - eccentricity_o * Mathd.Cos(EccentricAnomaly));

        double cosAOPPlusTA = Mathd.Cos(argumentOfPeriapsis + trueAnomaly);
        double sinAOPPlusTA = Mathd.Sin(argumentOfPeriapsis + trueAnomaly);

        double x = distance * ((cosLOAN * cosAOPPlusTA) - (sinLOAN * sinAOPPlusTA * cosI));
        double z = distance * ((sinLOAN * cosAOPPlusTA) + (cosLOAN * sinAOPPlusTA * cosI));      //Switching z and y to be aligned with xz not xy
        double y = distance * (sinI * sinAOPPlusTA);
        last_pos = my_pos;
        my_pos = new Vector3d(x, y, z);
        transform.position = new Vector3((float)x, (float)y, (float)z) / 1000000;

    }
    // Update is called once per frame
    public bool stop = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            escala_temporal = escala_temporal * 10d;
        }
        if (Input.GetKeyDown(KeyCode.O) && escala_temporal > 1)
        {
            escala_temporal = escala_temporal / 10d;
        }
        tiempo_pasado += Time.deltaTime * escala_temporal;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            calculateOrbit(false);
            stop = true;
        }
        if (stop) return;
        meanAnomaly = (double)(n * (tiempo_pasado - meanLongitude));
        Vector3d v_vector2 = (my_pos - last_pos).normalized * Mathd.Sqrt(U / semiMajorAxis); //velocity vector
        //Debug.Log(v_vector2);
        double E1 = meanAnomaly;   //initial guess
        double difference = 1f;
        for (int i = 0; difference > accuracyTolerance && i < maxIterations; i++)
        {
            double E0 = E1;
            double F = meanAnomaly - E0 + eccentricity_o * Mathd.Sin(E0);
            double DF = (-1f) + eccentricity_o * Mathd.Cos(E0);
            E1 = E0 - F / DF;
            difference = Mathd.Abs(E1 - E0);
        }
        double EccentricAnomaly = E1;

        double trueAnomaly = 2 * Mathd.Atan(trueAnomalyConstant * Mathd.Tan(EccentricAnomaly / 2));
        double distance = semiMajorAxis * (1 - eccentricity_o * Mathd.Cos(EccentricAnomaly));

        double cosAOPPlusTA = Mathd.Cos(argumentOfPeriapsis + trueAnomaly);
        double sinAOPPlusTA = Mathd.Sin(argumentOfPeriapsis + trueAnomaly);

        double x = distance * ((cosLOAN * cosAOPPlusTA) - (sinLOAN * sinAOPPlusTA * cosI));
        double z = distance * ((sinLOAN * cosAOPPlusTA) + (cosLOAN * sinAOPPlusTA * cosI));      //Switching z and y to be aligned with xz not xy
        double y = distance * (sinI * sinAOPPlusTA);
        last_pos = my_pos;
         my_pos = new Vector3d(x, y, z);
        transform.position = new Vector3((float)x, (float)y, (float)z)/1000000;
    }
}
