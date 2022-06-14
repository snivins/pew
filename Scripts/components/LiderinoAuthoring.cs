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
        Vector3d earth_pos = new Vector3d(-69693292.5867402d, 16710563.4486748d, 128450881.981214d);
        Vector3d my_pos = new Vector3d(-69693292.5867402d, 16710563.4486748d, 128352881.981214d);
         my_pos = new Vector3d(-69693292.5867402d, 16710563.4486748d, 128450401.981214d);
        double G = 6.6743e-11;
        double jup_mass = 1.898e27;
        double earth_mass = 5.972e24;
        //double init_vel = Mathd.Sqrt((G * jup_mass) / ((jup_pos - my_pos).magnitude));
        double init_vel = Mathd.Sqrt((G * earth_mass) / ((earth_pos * 1000d - my_pos * 1000d).magnitude));
        Debug.Log(init_vel);
        
        dstManager.AddComponentData<Lider>(entity, new Lider
        {
            simulated_pos = my_pos,
            speeeeeed = new Vector3d(init_vel/1000d, 0d, 0d)//m/s to km/s
        });
        //Vector3d my_pos = new Vector3d(71327002, -7837163.09, 8051163.3);
    }
}
