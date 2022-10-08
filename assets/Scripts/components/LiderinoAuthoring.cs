using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
[AddComponentMenu("Custom Authoring/LiderinoAuthoring")]
public class LiderinoAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    // Add fields to your component here. Remember that:
    //
    // * The purpose of this class is to store data for authoring purposes - it is not for use while the game is
    //   running.
    // 
    // * Traditional Unity serialization rules apply: fields must be public or marked with [SerializeField], and
    //   must be one of the supported types.
    //
    // For example,
    //    public float scale;

    public GameObject liderino;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        // Call methods on 'dstManager' to create runtime components on 'entity' here. Remember that:
        //
        // * You can add more than one component to the entity. It's also OK to not add any at all.
        //
        // * If you want to create more than one entity from the data in this class, use the 'conversionSystem'
        //   to do it, instead of adding entities through 'dstManager' directly.
        //
        // For example,
        //   dstManager.AddComponentData(entity, new Unity.Transforms.Scale { Value = scale });

        SimulatedSpace fc = liderino.GetComponent<SimulatedSpace>();
        if (fc == null)
        {
            liderino.AddComponent<SimulatedSpace>();
        }
        fc.ent = entity;
        Vector3d jup_pos = new Vector3d(713270036657.713d, -78371630843.4211d, 182511629788.131d);
        //Vector3d my_pos = new Vector3d(713270036657.713d, -78371630843.4211d, 182411629788.131d);
        Vector3d earth_pos = new Vector3d(-69693292.5867402d, 16710563.4486748d, 128458381.981214d);
        Vector3d my_pos = new Vector3d(-69693292.5867402d, 16710563.4486748d, 128352881.981214d);
         my_pos = new Vector3d(-69693292.5867402d, 16710563.4486748d, 128451602.981214d);
        double G = 6.6743e-11;
        double jup_mass = 1.898e27;
        double earth_mass = 5.972e24;
        Vector3d diferencia = (earth_pos * 1000d) - (my_pos * 1000d);
        //double init_vel = Mathd.Sqrt((G * jup_mass) / ((jup_pos - my_pos).magnitude));
        double init_vel = Mathd.Sqrt((G * earth_mass) / ((diferencia).magnitude)) ; 

        dstManager.AddComponentData<Lider>(entity, new Lider
        {
            simulated_pos = my_pos,
            speeeeeed = new Vector3d(init_vel/1000d, 0d, 0d),//m/s to km/s
             meanAnomaly =0.1d,
            real_sim = true,
        });
        dstManager.AddComponentData<PlanetarianOrbit>(entity, new PlanetarianOrbit
        {
            mu = 0,
            eccentricity = 0,
            n = 0,
            cosLOAN = 0,
            sinLOAN = 0,
            sinI = 0,
            cosI = 0,
            trueAnomalyConstant = 0,
            semiMajorAxis = 0d,//*1000d,
            argumentOfPeriapsis = 0,
            masa = 1,
            influence_distance = 0
        });

        //calculo la orbita/*
        /*
        Vector3d v = new Vector3d(init_vel , 0d, 0d);//lo calculo en metros asi q no hace falta /1000 velocity vector
        double u = G * earth_mass;
        double fuerza = (Mathd.Pow(v.magnitude, 2) / 2d) - (u / diferencia.magnitude);
        double smeiaxis = u / (2 * -fuerza);
        double h = (diferencia.magnitude * v.magnitude);
        double eccentricity = Mathd.Sqrt(1 + ((2 * fuerza * Mathd.Pow(h, 2)) / Mathd.Pow(u, 2))); // h o esto sta mal
        // -math.pow(u, 2)/(2 * math.abs(fuerza) ) = h*h
        //double h =math.sqrt(-math.pow(u, 2) / (2 * (fuerza))); calculo h sabiendo q eccentricity es 0  
        if (eccentricity < math.EPSILON) eccentricity = 0;
        double apoapsis = smeiaxis * (1 + eccentricity);
        double periapsis = smeiaxis * (1 - eccentricity);
        Vector3d momentum = Vector3d.Cross(diferencia, v);
        double inclinacion = Mathd.Acos(momentum.y/ momentum.magnitude);
        Vector3d n = Vector3d.Cross(Vector3d.up, momentum);
        double longitud_an = 0;
        if (n != Vector3d.zero){
            longitud_an = Mathd.Acos(n.x / n.magnitude);
        }
        Vector3d eccentricity_vector = (Vector3d.Cross(v, momentum)/u)-diferencia/diferencia.magnitude; 
        Vector3d eccentricity_vector2 = ((Mathd.Pow(v.magnitude, 2)-u/diferencia.magnitude)*diferencia -Vector3d.Cross(momentum,v))/ u;
        Vector3d eccentricity_vector3 = ((Mathd.Pow(v.magnitude, 2) - u / diferencia.magnitude) * diferencia - (momentum.magnitude * v)) / u;/*
        double arg_of_p  = 0;
        if (longitud_an == 0){
            arg_of_p = (double)Mathf.Atan2((float)eccentricity_vector.z,(float)eccentricity_vector.x);
        } else {
            arg_of_p = Mathd.Acos((Vector3d.Cross(n, eccentricity_vector)/(n.magnitude * eccentricity_vector.magnitude)).magnitude);
        }
        if (n.z < 0){
            longitud_an = Mathd.TAU - longitud_an;
        }
        if (n.y < 0){
            arg_of_p = Mathd.TAU - arg_of_p;
        }
        Debug.Log("v= " + v + "u= " + u + ",  fuerza= " + fuerza + " , semimajor axis= " + smeiaxis + " , distancia= " + diferencia.magnitude + ", h= " + h);
        Debug.Log(", eccentricity = " + eccentricity + ", apoapsis = " + apoapsis + ", periapsis = " + periapsis + ", inclinacion = " + (inclinacion * 180) / (Mathd.PI) + ", momentun = " + momentum);
        Debug.Log(", n = " + n + ", longitud_an = " + longitud_an + ", longitud_an euler = " + (longitud_an * 180) / (Mathd.PI));
        Debug.Log("eccentricity_vector= " + eccentricity_vector + ",  arg_of_p= " + arg_of_p + " , arg_of_p euler= " + (arg_of_p * 180) / (Mathd.PI)  );
        //Vector3d my_pos = new Vector3d(71327002, -7837163.09, 8051163.3);*/
        //todo en metros
        Vector3d v_vector = new Vector3d(init_vel, 0d, 0d); //velocity vector
        Vector3d r_vector = diferencia;//position vector, la posicion relativa del satelite con respecto al planeta
        double U = G * earth_mass;
        Vector3d h_vector = Vector3d.Cross(r_vector,v_vector);//angular momentum 
        Vector3d n_vector = Vector3d.Cross(Vector3d.up, h_vector);
        Vector3d e_vector = (((Mathd.Pow(v_vector.magnitude, 2) / U) - (1 / r_vector.magnitude)) * r_vector) - ((Mathd.Dot(r_vector, v_vector) * v_vector) / U);
        Vector3d e_vector2 = (Vector3d.Cross(v_vector, h_vector) / U) - (r_vector / r_vector.magnitude);
        Vector3d e_vector3 = (Mathd.Pow(v_vector.magnitude, 2)*r_vector/U)- ((Mathd.Dot(r_vector,v_vector) * v_vector) / U)-(r_vector / r_vector.magnitude);
        double eccentricity = e_vector.magnitude;
        double specific_me = (Mathd.Pow(v_vector.magnitude, 2) / 2d) - (U / r_vector.magnitude);
        double semimayoraxis = 0;
        double apoapsis = 0;
        double periapsis = 0;
        string tipo = "circular";
        if (eccentricity > 0) tipo = "eliptica";
        if (eccentricity < 1)
        {
            semimayoraxis = -U / (2 * specific_me);
            apoapsis = semimayoraxis * (1 + eccentricity);
            periapsis = semimayoraxis * (1 - eccentricity);
        } else
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
        double inclinacion = math.acos(h_vector.y / h_vector.magnitude);
        double loa = math.acos(n_vector.x / n_vector.magnitude);//longitud of ascending node
        double aop = math.acos(Mathd.Dot(n_vector, e_vector) / (n_vector.magnitude * e_vector.magnitude)); //argument of periapsis
        if (inclinacion == 0)
        {

            loa = 0;//longitud of ascending node
            aop = 0; //argument of periapsis
        }
        double lop = loa - aop; //longitud of periapsis
        //double ma = math.acos(Mathd.Dot(n_vector, r_vector) / (n_vector.magnitude * r_vector.magnitude));
        double ma = math.acos( r_vector.x / r_vector.magnitude);
        Debug.Log("n= " + n_vector + "v= " + v_vector + " r= " + r_vector + " h= " + h_vector + " e= " + e_vector + " e12= " + e_vector2 + " e13= " + e_vector3);
        Debug.Log(" ecentricity= " + eccentricity + " specific_me= " + specific_me + " semimayoraxis= " + semimayoraxis + " apoapsis= " + apoapsis + " periapsis= " + periapsis);
        Debug.Log(" tipo de orbita= " + tipo + " inclinacion= " + inclinacion + " longitud of ascending node= " + loa + " argument of periapsis= " + aop + " longitud of periapsis= " + lop);
        Debug.Log(" ma= " + ma );
    }
}
