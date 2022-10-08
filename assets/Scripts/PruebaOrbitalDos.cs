using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Mathematics;
public class PruebaOrbitalDos : MonoBehaviour
{
    public Transform caja;
    public Transform esfera;
    public Transform capsula;
    // Start is called before the first frame update
    Vector3d v_vector = Vector3d.zero;
    [SerializeField]
    double velocidad_mag = 0;
    [SerializeField]
    double posicion_mag = 0;
    Vector3d r_vector = new Vector3d(0d, 0d, -6779d);
    Vector3d r_vectorX = new Vector3d(6779d,0d, 0d);

    Vector3d simv_vector = Vector3d.zero;
    Vector3d simr_vector = Vector3d.zero;

    [SerializeField]
    Vector3 velocidad_v = Vector3.zero;
    [SerializeField]
    Vector3 posicion_v = Vector3.zero;

    [SerializeField]
    double tiempo = 0;
    [SerializeField]
    [Range(1, 10000)]
    int escala_temporal = 1;


    double G = 6.6743e-11;
    double earth_mass = 5.972e24;
    double U = 0;

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
    double cosLOAN2 = 0;
    double sinLOAN2 = 0;
    double cosI2 = 0;
    double sinI2 = 0;
    double n = 0;
    double truanomaly = 0;
    double eccenomaly = 0;
    double meannomaly = 0;

    double accuracyTolerance = 1e-7d;

    public bool x_origin = false;
    public bool prueba = true;
    void Start()
    {
        U = G * earth_mass;
        double init_vel = Mathd.Sqrt((U) / (r_vector.magnitudems));
        // init_vel--;
         //init_vel++;
        if (x_origin)
        {
            v_vector = new Vector3d(0d, 0d, init_vel / 1000d);
            transform.position = (r_vectorX / 1000d).ToFloat();

        } else {

            v_vector = new Vector3d(init_vel / 1000d, 0d, 0d);
            transform.position = (r_vector / 1000d).ToFloat();
        }//velocity vector

        serializarSpeedAndPos();
    }
    void serializarSpeedAndPos()
    {
        velocidad_v = v_vector.ToFloat(); 
        posicion_v = r_vector.ToFloat();
        velocidad_mag = v_vector.magnitude;
        posicion_mag = r_vector.magnitude;
    }
    // Update is called once per frame
    void Update()
    {
        tiempo += Time.deltaTime * escala_temporal;
        
        if (x_origin)
        {

            double forceMagnitude = (U) / Mathd.Pow(r_vectorX.magnitudems, 2);
            Vector3d fuerza_g = -r_vectorX.normalized * forceMagnitude;
            fuerza_g = fuerza_g / 1000d;// lo paso a km/s^2
            v_vector += fuerza_g * Time.deltaTime * escala_temporal;
            r_vectorX += v_vector * Time.deltaTime * escala_temporal;

            serializarSpeedAndPos();

            simr_vector = -r_vectorX * 1000d;

        } else
        {

            double forceMagnitude = (U) / Mathd.Pow(r_vector.magnitudems, 2);
            Vector3d fuerza_g = -r_vector.normalized * forceMagnitude;
            fuerza_g = fuerza_g / 1000d;// lo paso a km/s^2
            v_vector += fuerza_g * Time.deltaTime * escala_temporal;
            r_vector += v_vector * Time.deltaTime * escala_temporal;

            serializarSpeedAndPos();

            simr_vector = -r_vector * 1000d;
        }

        simv_vector = v_vector * 1000d;
            Vector3d h_vector = Vector3d.Cross(simr_vector, simv_vector);// angular momentum 
            double Eoe = (Mathd.Pow(simv_vector.magnitude, 2) / 2d) - (U / simr_vector.magnitude);//specific orbital energy
            Vector3d n_vector = Vector3d.Cross(Vector3d.up, h_vector);
            Vector3d e_vector = (((Mathd.Pow(simv_vector.magnitude, 2) / U) - (1 / simr_vector.magnitude)) * simr_vector) - ((Mathd.Dot(simr_vector, simv_vector) * simv_vector) / U);
            double semimayoraxis = -U / (2 * Eoe);
            double eccentricity = e_vector.magnitude;
        if (prueba)
        {
            eccentricity_o = eccentricity;
            semiMajorAxis = semimayoraxis;
        }
            double semiminoraxis = semimayoraxis * Mathd.Sqrt(1- Mathd.Pow(eccentricity,2));
            double A0 = semimayoraxis * semiminoraxis * Mathd.PI;

            double inclinacion = math.acos(h_vector.y / h_vector.magnitude);
            if (eccentricity < Mathd.Epsilon)
            {
                eccentricity = 0;
            }

            double loa = 0;
            double aop = Mathd.Atan2(e_vector.z, e_vector.x);
            if (inclinacion != 0)
            {
                loa = math.acos(n_vector.x / n_vector.magnitude);
                if (n_vector.y < 0)
                {
                    loa = Mathd.TAU - loa;
                }
                aop = math.acos(Mathd.Dot(n_vector, e_vector) / (n_vector.magnitude * e_vector.magnitude));
            }
            if (h_vector.y < 0)
            {
                aop = Mathd.TAU - aop;

            }
            double lop = loa + aop;
        if (inclinacion < Mathf.Epsilon && eccentricity < Mathf.Epsilon) //orbita circular equatorial
        {
            truanomaly = math.acos(simr_vector.x / simr_vector.magnitude);//aqui es true longitud
            if (simv_vector.x < 0)
            {
                truanomaly = Mathd.TAU - truanomaly;
            }
        } else if (eccentricity < Mathf.Epsilon)//orbital circular no equatorial (con inclinacion)
        {
            truanomaly = math.acos(Mathd.Dot(n_vector, simr_vector) / (n_vector.magnitude * simr_vector.magnitude));//aqui es argumento de latitud
            if (Mathd.Dot(n_vector, simv_vector) > 0)
            {
                truanomaly = Mathd.TAU - truanomaly;
            }
        } else //orbita eliptica
        {
            truanomaly = math.acos(Mathd.Dot(e_vector, simr_vector) / (e_vector.magnitude * simr_vector.magnitude));
            if (Mathd.Dot(simr_vector, simv_vector) < 0)
            {
                truanomaly = Mathd.TAU - truanomaly;
            }
        }
        if (prueba)
        {
            argumentOfPeriapsis = aop;
            trueAnomalyConstant = Mathd.Sqrt((1 + eccentricity) / (1 - eccentricity));
        }
        double tAC = Mathd.Sqrt((1 + eccentricity) / (1 - eccentricity));
        TruetoMean(eccentricity, tAC);





        n = Mathd.Sqrt(U / Mathd.Pow(semimayoraxis, 3)); //mean notion
        //Debug.Log(meannomaly/n);

        //Debug.Log("h " + h_vector + " inclinacion " + inclinacion + " e " + e_vector + " em " + eccentricity + " semimayoraxis " + semimayoraxis);
        //Debug.Log("n " + n_vector + " loa " + loa + " lop " + lop + " aop " + aop);

        //double L = Math.IEEERemainder(Math.IEEERemainder(meanLongitude[1] * tCenturiesFromJ2000, 360) + meanLongitude[0], 360);

        /*double meanAnomaly = tiempo - lop;
            double w = lop - meanAnomaly;
            double E = meanAnomaly;
            int contador = 0;
            while (true)
            {
                double dE = (E - eccentricity * Mathd.Sin(E) - meanAnomaly) / (1 - eccentricity * Mathd.Cos(E));
                E -= dE;
                contador++;
                if (Mathd.Abs(dE) < 1e-6 || contador > maxIterations)
                    break;
            }*/
        double E = eccenomaly; 
        double w = lop - meannomaly;
        double P = semimayoraxis * (Mathd.Cos(E) - eccentricity);
            double Q = semimayoraxis * Mathd.Sin(E) * Mathd.Sqrt(1 - Mathd.Pow(eccentricity, 2d));

            double x = Mathd.Cos(w) * P - Mathd.Sin(w) * Q;
            double z = Mathd.Sin(w) * P + Mathd.Cos(w) * Q;
            // rotate by inclination
            double y = Mathd.Sin(inclinacion) * x;
            x = Mathd.Cos(inclinacion * x);
            // rotate by longitude of ascending node
            double xTemp = x;
            x = Mathd.Cos(loa) * xTemp - Mathd.Sin(loa) * z;
            z = Mathd.Sin(loa) * xTemp + Mathd.Cos(loa) * z;
            simr_vector = -new Vector3d(x, y, z);
        if (x_origin)
        {

            simv_vector = (simr_vector + (r_vectorX * 1000d)).normalized * Mathd.Sqrt(U / (semimayoraxis * 1000d));
            //r_vector = simr_vector / 1000d;
            caja.position = (simr_vector / 1000000d).ToFloat();
            transform.position = (r_vectorX / 1000d).ToFloat();
        }
        else
        {

            simv_vector = (simr_vector + (r_vector * 1000d)).normalized * Mathd.Sqrt(U / (semimayoraxis * 1000d));
            //r_vector = simr_vector / 1000d;
            caja.position = (simr_vector / 1000000d).ToFloat();
            transform.position = (r_vector / 1000d).ToFloat();
        }

        cosI = math.cos(inclinacion);
        sinI = math.sin(inclinacion);
        cosLOAN = math.cos(loa);
        sinLOAN = math.sin(loa);
        if (prueba)
        {

            cosI2 = math.cos(inclinacion);
            sinI2 = math.sin(inclinacion);
            cosLOAN2 = math.cos(loa);
            sinLOAN2 = math.sin(loa);
            prueba = false;
        }
        
        double distance = semimayoraxis * (1 - eccentricity * Mathd.Cos(eccenomaly));

        double cosAOPPlusTA = Mathd.Cos(aop + truanomaly);
        double sinAOPPlusTA = Mathd.Sin(aop + truanomaly);

        double x1 = distance * ((cosLOAN * cosAOPPlusTA) - (sinLOAN * sinAOPPlusTA * cosI));
        double z2 = distance * ((sinLOAN * cosAOPPlusTA) + (cosLOAN * sinAOPPlusTA * cosI));      //Switching z and y to be aligned with xz not xy
        double y3 = distance * (sinI * sinAOPPlusTA);

        esfera.position = (-new Vector3d(x, y, z) / 1000000d).ToFloat();
        capsula_orbit();
    }

    void capsula_orbit()
    {
        meanAnomaly = (double)(n * (tiempo - meanLongitude));
        double periapsis = (1 - eccentricity_o) * semiMajorAxis;
        double vel_periapsis = (U * (1 + eccentricity_o)) / (semiMajorAxis * (1 - eccentricity_o));
        Debug.Log(meanAnomaly + " and  " + meannomaly);
        double E1 = meanAnomaly;   //initial guess
        double difference = 1f;
        for (int i = 0; difference > accuracyTolerance && i < 6; i++)
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

        double x = distance * ((cosLOAN2 * cosAOPPlusTA) - (sinLOAN2 * sinAOPPlusTA * cosI2));
        double z = distance * ((sinLOAN2 * cosAOPPlusTA) + (cosLOAN2 * sinAOPPlusTA * cosI2));      //Switching z and y to be aligned with xz not xy
        double y = distance * (sinI2 * sinAOPPlusTA);

        capsula.position = new Vector3((float)x, (float)y, (float)z) / -1000000f;
    }
    void TruetoMean(double eccentricity,double tac)
    {

        eccenomaly = math.atan(math.tan(truanomaly / 2) / tac) * 2;
        
         meannomaly = eccenomaly - eccentricity * math.sin(eccenomaly);
        //Debug.Log("trye " + truanomaly + "ecc " + eccenomaly + " mean " + meannomaly);
    }
}
