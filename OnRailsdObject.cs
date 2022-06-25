using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Transforms;
using Unity.Entities;
using Unity.Mathematics;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class OnRailsdObject : MonoBehaviour
{
    struct Constants    //Put somewhere else
    {
        public const double G = 6.67f;
        public const double G_real = 6.6743e-11;
        public const double G_km = 6.6743e-17;
    }

    struct Math         //Put somewhere else
    {
        public const double TAU = 6.28318530718f;
    }
    private LineRenderer ln;
    private Entity plantity;
    //Orbital Keplerian Parameters
    [SerializeField] double semiMajorAxis = 20f;        //a - size
    [SerializeField] [Range(0f, 0.99f)]         double eccentricity;             //e - shape
    [SerializeField] [Range(0f, (float)Math.TAU)]      double inclination = 0f;         //i - tilt
    [SerializeField] [Range(0f, (float)Math.TAU)]      double longitudeOfAcendingNode;  //n - swivel
    [SerializeField] [Range(0f, (float)Math.TAU)]      double argumentOfPeriapsis;      //w - position
    [SerializeField] double meanLongitude;             //L - offset
    [SerializeField] OnRailsReferenceBody referenceBody;
    [SerializeField] double meanAnomaly;
    [Space]
    //Settings
    [SerializeField] double accuracyTolerance = 1e-12d;
    [SerializeField] int maxIterations = 5;           //usually converges after 3-5 iterations.
    public double masa_planeta;
    //Numbers which only change if orbit or mass changes
    [HideInInspector] [SerializeField] double mu;
    [HideInInspector] [SerializeField] double n, cosLOAN, sinLOAN, sinI, cosI, trueAnomalyConstant;
    Vector3 ref_pos;
    List<Vector3> orbitalPoints_Rel = new List<Vector3>();
    List<double> anomalies_Rel = new List<double>();
    
    public double influence_distance;
    EntityManager em;
    private void OnValidate() => orbitalPoints.Clear();
    void Awake() => CalculateSimuConstants();
    public bool mostrar_distancia = false;
    void Start()
    {
        
           ln = GetComponent<LineRenderer>();
        CalculateKmConstants();
        GetOrbits();
        ref_pos =referenceBody.transform.position;

        meanAnomaly = (double)(n * (Time.time - meanLongitude));

        double E1 = meanAnomaly;   //initial guess
        double difference = 1f;
        for (int i = 0; difference > accuracyTolerance && i < maxIterations; i++)
        {
            double E0 = E1;
            E1 = E0 - F(E0, eccentricity, meanAnomaly) / DF(E0, eccentricity);
            difference = Mathd.Abs(E1 - E0);
        }
        double EccentricAnomaly = E1;

        double trueAnomaly = 2 * Mathd.Atan(trueAnomalyConstant * Mathd.Tan(EccentricAnomaly / 2));
        double distance = semiMajorAxis * (1 - eccentricity * Mathd.Cos(EccentricAnomaly));

        double cosAOPPlusTA = Mathd.Cos(argumentOfPeriapsis + trueAnomaly);
        double sinAOPPlusTA = Mathd.Sin(argumentOfPeriapsis + trueAnomaly);

        double x = distance * ((cosLOAN * cosAOPPlusTA) - (sinLOAN * sinAOPPlusTA * cosI));
        double z = distance * ((sinLOAN * cosAOPPlusTA) + (cosLOAN * sinAOPPlusTA * cosI));      //Switching z and y to be aligned with xz not xy
        double y = distance * (sinI * sinAOPPlusTA);

        Vector3d simu_posit = new Vector3d(x, y, z) + new Vector3d(ref_pos.x, ref_pos.y, ref_pos.z);

         em = World.DefaultGameObjectInjectionWorld.EntityManager;
        Entity pplantity = em.CreateEntity();

        em.AddComponent(pplantity, typeof(Planeta));
        em.SetComponentData(pplantity,  new Planeta 
            {
            simulated_pos = simu_posit * 100000d,//*1000d,
            simulated_speed = Vector3d.zero,
            meanAnomaly = meanAnomaly
        }
        );
        em.AddComponent(pplantity, typeof(PlanetarianOrbit));
        /*mu;
    public double eccentricity;
    public double n, cosLOAN, sinLOAN, sinI, cosI, trueAnomalyConstant;
    public double semiMajorAxis;
    public double argumentOfPeriapsis;
    public double masa;*/
        em.SetComponentData(pplantity, new PlanetarianOrbit
        {
            mu = mu,
            eccentricity = eccentricity,
            n = n,
            cosLOAN = cosLOAN,
            sinLOAN = sinLOAN,
            sinI = sinI,
            cosI = cosI,
            trueAnomalyConstant = trueAnomalyConstant,
            semiMajorAxis = semiMajorAxis * 100000d,//*1000d,
            argumentOfPeriapsis = argumentOfPeriapsis,
            masa = masa_planeta,
            influence_distance = influence_distance
        }
        );
        plantity = pplantity;
        CalculateSemiConstants();
        //em.Instantiate(pplantity);
    }
    public double F(double E, double e, double M)  //Function f(x) = 0
    {
        return (M - E + e * Mathd.Sin(E));
    }
    public double DF(double E, double e)      //Derivative of the function
    {
        return (-1d) + e * Mathd.Cos(E);
    }
    public void CalculateSemiConstants()    //Numbers that only need to be calculated once if the orbit doesn't change.
    {
        mu = Constants.G * referenceBody.mass;
        n = Mathd.Sqrt(mu / Mathd.Pow(semiMajorAxis, 3));
        trueAnomalyConstant = Mathd.Sqrt((1 + eccentricity) / (1 - eccentricity));
        cosLOAN = Mathd.Cos(longitudeOfAcendingNode);
        sinLOAN = Mathd.Sin(longitudeOfAcendingNode);
        cosI = Mathd.Cos(inclination);
        sinI = Mathd.Sin(inclination);
    }
    public void CalculateSimuConstants()    //simulated numbers for the entities
    {
        mu = Constants.G_real * 1.989 * 10e30;
        n = Mathd.Sqrt(mu / Mathd.Pow(semiMajorAxis * 100000 * 1000, 3));// lo multiplico x la escala 100000 y lo paso a m/s 1000
        trueAnomalyConstant = Mathd.Sqrt((1 + eccentricity) / (1 - eccentricity));
        cosLOAN = Mathd.Cos(longitudeOfAcendingNode);
        sinLOAN = Mathd.Sin(longitudeOfAcendingNode);
        cosI = Mathd.Cos(inclination);
        sinI = Mathd.Sin(inclination);
    }
    public void CalculateKmConstants()    //simulated numbers for the entities
    {
        mu = Constants.G_real * 1.989 * 10e30;
        n = Mathd.Sqrt(mu / Mathd.Pow(semiMajorAxis * 100000 , 3));// lo multiplico x la escala 100000 y lo paso a m/s 1000
        trueAnomalyConstant = Mathd.Sqrt((1 + eccentricity) / (1 - eccentricity));
        cosLOAN = Mathd.Cos(longitudeOfAcendingNode);
        sinLOAN = Mathd.Sin(longitudeOfAcendingNode);
        cosI = Mathd.Cos(inclination);
        sinI = Mathd.Sin(inclination);
    }
    double modulo;
    void Update()
    {
        //EntityManager em = World.DefaultGameObjectInjectionWorld.EntityManager;
        //CalculateSemiConstants();
        /*meanAnomaly = (double)(n * (Time.time - meanLongitude));

        double E1 = meanAnomaly;   //initial guess
        double difference = 1f;
        for (int i = 0; difference > accuracyTolerance && i < maxIterations; i++)
        {
            double E0 = E1;
            E1 = E0 - F(E0, eccentricity, meanAnomaly) / DF(E0, eccentricity);
            difference = Mathd.Abs(E1 - E0);
        }
        double EccentricAnomaly = E1;

        double trueAnomaly = 2 * Mathd.Atan(trueAnomalyConstant * Mathd.Tan(EccentricAnomaly / 2));
        double distance = semiMajorAxis * (1 - eccentricity * Mathd.Cos(EccentricAnomaly));

        double cosAOPPlusTA = Mathd.Cos(argumentOfPeriapsis + trueAnomaly);
        double sinAOPPlusTA = Mathd.Sin(argumentOfPeriapsis + trueAnomaly);

        double x = distance * ((cosLOAN * cosAOPPlusTA) - (sinLOAN * sinAOPPlusTA * cosI));
        double z = distance * ((sinLOAN * cosAOPPlusTA) + (cosLOAN * sinAOPPlusTA * cosI));      //Switching z and y to be aligned with xz not xy
        double y = distance * (sinI * sinAOPPlusTA);

        transform.position = new Vector3((float)x, (float)y, (float)z) + referenceBody.transform.position;*/

        //Vector3 old_pos = transform.position;
        Vector3 sim_positon = (em.GetComponentData<Planeta>(plantity).simulated_pos  - referenceBody.GetComponent<SimulatedSpace>().sim_pos).ToFloat()/10f;
        transform.position = sim_positon;
        if (mostrar_distancia)
        {
            Debug.Log(sim_positon.magnitude);
        }


            modulo = em.GetComponentData<Planeta>(plantity).meanAnomaly % Math.TAU;
        if (modulo < 0d)
        {
            modulo = Math.TAU + modulo; 
        }
        if (ln != null && false)
        {
            //orbitalPoints_Rel.Clear();
            //GetOrbits();
            List<Vector3> begin_points = new List<Vector3>();
            List<Vector3> end_points = new List<Vector3>();
            bool cambio = false;

            for (int i = 0;i< orbitalPoints_Rel.Count; i++)
            {
                if ( anomalies_Rel[i] > modulo)
                {
                    if (!cambio)
                    {
                        begin_points.Add(transform.position );
                    }
                    cambio = true;
                }
                if (cambio)
                {

                    begin_points.Add(orbitalPoints_Rel[i] + referenceBody.transform.position - ref_pos);
                } else
                {
                    end_points.Add(orbitalPoints_Rel[i] + referenceBody.transform.position - ref_pos);                    
                }


            }
            end_points.Add(transform.position);
            begin_points.AddRange(end_points);
            ln.positionCount = begin_points.Count;
            ln.SetPositions(begin_points.ToArray());
            
        }
    }


    private void GetOrbits()
    {
        if (referenceBody == null) return;

        CalculateSemiConstants();
        Vector3 pos = referenceBody.transform.position;
        double orbitFraction = 1f / orbitResolution;

        for (int i = 0; i < orbitResolution + 1; i++)
        {
            double EccentricAnomaly = i * orbitFraction * Math.TAU;

            double trueAnomaly = 2 * Mathd.Atan(trueAnomalyConstant * Mathd.Tan(EccentricAnomaly / 2));
            double distance = semiMajorAxis * (1 - eccentricity * Mathd.Cos(EccentricAnomaly));

            double cosAOPPlusTA = Mathd.Cos(argumentOfPeriapsis + trueAnomaly);
            double sinAOPPlusTA = Mathd.Sin(argumentOfPeriapsis + trueAnomaly);

            double x = distance * ((cosLOAN * cosAOPPlusTA) - (sinLOAN * sinAOPPlusTA * cosI));
            double z = distance * ((sinLOAN * cosAOPPlusTA) + (cosLOAN * sinAOPPlusTA * cosI));
            double y = distance * (sinI * sinAOPPlusTA);

            double meanAnomaly = EccentricAnomaly - eccentricity * Mathd.Sin(EccentricAnomaly);
            anomalies_Rel.Add(meanAnomaly);
            orbitalPoints_Rel.Add(pos + new Vector3((float)x, (float)y, (float)z));
        }
    }


    int orbitResolution = 50;
    List<Vector3> orbitalPoints = new List<Vector3>();
#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        if (orbitalPoints.Count == 0)
        {
            if (referenceBody == null)
            {
                Debug.LogWarning($"Add a reference body to {gameObject.name}");
                return;
            }

            CalculateSemiConstants();
            Vector3 pos = referenceBody.transform.position;
            double orbitFraction = 1f / orbitResolution;

            for (int i = 0; i < orbitResolution + 1; i++)
            {
                double EccentricAnomaly = i * orbitFraction * Math.TAU;

                double trueAnomaly = 2 * Mathd.Atan(trueAnomalyConstant * Mathd.Tan(EccentricAnomaly / 2));
                double distance = semiMajorAxis * (1 - eccentricity * Mathd.Cos(EccentricAnomaly));

                double cosAOPPlusTA = Mathd.Cos(argumentOfPeriapsis + trueAnomaly);
                double sinAOPPlusTA = Mathd.Sin(argumentOfPeriapsis + trueAnomaly);

                double x = distance * ((cosLOAN * cosAOPPlusTA) - (sinLOAN * sinAOPPlusTA * cosI));
                double z = distance * ((sinLOAN * cosAOPPlusTA) + (cosLOAN * sinAOPPlusTA * cosI));
                double y = distance * (sinI * sinAOPPlusTA);

                double meanAnomaly = EccentricAnomaly - eccentricity * Mathd.Sin(EccentricAnomaly);

                orbitalPoints.Add(pos + new Vector3((float)x, (float)y, (float)z));
            }
        }

        Handles.DrawAAPolyLine(orbitalPoints.ToArray());
    }
#endif

}
