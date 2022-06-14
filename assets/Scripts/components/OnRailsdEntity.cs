using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class OnRailsdEntity : MonoBehaviour
{
    struct Constants    //Put somewhere else
    {
        public const double G = 6.67f;
    }

    struct Math         //Put somewhere else
    {
        public const double TAU = 6.28318530718f;
    }
    private LineRenderer ln;
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
    [SerializeField] double accuracyTolerance = 1e-6f;
    [SerializeField] int maxIterations = 5;           //usually converges after 3-5 iterations.

    //Numbers which only change if orbit or mass changes
    [HideInInspector] [SerializeField] double mu;
    [HideInInspector] [SerializeField] double n, cosLOAN, sinLOAN, sinI, cosI, trueAnomalyConstant;
    Vector3 ref_pos;
    List<Vector3> orbitalPoints_Rel = new List<Vector3>();
    List<double> anomalies_Rel = new List<double>();
    void Awake() => CalculateSemiConstants();
    void Start()
    {
        ln = GetComponent<LineRenderer>(); 
        CalculateSemiConstants();
        GetOrbits();
        ref_pos =referenceBody.transform.position;
    }
    public double F(double E, double e, double M)  //Function f(x) = 0
    {
        return (M - E + e * Mathd.Sin(E));
    }
    public double DF(double E, double e)      //Derivative of the function
    {
        return (-1f) + e * Mathd.Cos(E);
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
    double modulo;
    void Update()
    {
        //CalculateSemiConstants();

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

        transform.position = new Vector3((float)x, (float)y, (float)z) + referenceBody.transform.position;

        if (!controller.map_mode) { return;  } 
         modulo = meanAnomaly % Math.TAU;
        if (modulo < 0d)
        {
            modulo = Math.TAU + modulo; 
        }
        if (ln != null)
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

}
