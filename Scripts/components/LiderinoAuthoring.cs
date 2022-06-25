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
         Vector3d pos_after = new Vector3d(-69700915.7231168d,16711332.2981334d,128459727.603711d);
         Debug.Log(my_pos -  pos_after);
         Debug.Log(my_pos);
        double G = 6.6743e-11;
        double jup_mass = 1.898e27;
        double earth_mass = 5.972e24;
        Vector3d diferencia = (earth_pos * 1000d) - (my_pos * 1000d);
        //double init_vel = Mathd.Sqrt((G * jup_mass) / ((jup_pos - my_pos).magnitude));
        double init_vel = Mathd.Sqrt((G * earth_mass) / ((diferencia).magnitude)) ; 

        dstManager.AddComponentData<Lider>(entity, new Lider
        {
            simulated_pos = my_pos,
            speeeeeed = new Vector3d(init_vel/1000d, 1d, 0d),//m/s to km/s
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

        //calculo la orbita
        Vector3d v = new Vector3d(init_vel , 1000d, 0d);//lo calculo en metros asi q no hace falta /1000
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
        //Vector3d eccentricity_vector = ((Mathd.Pow(v.magnitude, 2)-u/diferencia.magnitude)*diferencia -Vector3d.Cross(Vector3d.Cross(diferencia,v),v))/ u;
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
        /*if (h < 0){
            arg_of_p = Mathd.TAU - arg_of_p;
        }*/
        Debug.Log("v= " + v + "u= " + u + ",  fuerza= " + fuerza + " , semimajor axis= " + smeiaxis + " , distancia= " + diferencia.magnitude + ", h= " + h);
        Debug.Log(", eccentricity = " + eccentricity + ", apoapsis = " + apoapsis + ", periapsis = " + periapsis + ", inclinacion = " + (inclinacion * 180) / (Mathd.PI) + ", momentun = " + momentum);
        Debug.Log(", n = " + n + ", longitud_an = " + longitud_an + ", longitud_an euler = " + (longitud_an * 180) / (Mathd.PI));
        Debug.Log("eccentricity_vector= " + eccentricity_vector + ",  arg_of_p= " + arg_of_p + " , arg_of_p euler= " + (arg_of_p * 180) / (Mathd.PI)  );
        //Vector3d my_pos = new Vector3d(71327002, -7837163.09, 8051163.3);
    }
}
