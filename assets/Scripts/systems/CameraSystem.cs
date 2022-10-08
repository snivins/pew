 
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(PlanetariumSystem))]
public partial class CameraSystem : SystemBase
{
    protected override void OnUpdate()
    {
         float m_MoveSpeed = 10.0f;
         float m_Turbo = 10000.0f;
        float inputVertical, inputHorizontal;
        bool leftShiftBoost, leftShift, fire1;
        float delta = Time.DeltaTime;
        quaternion rotation = GameObject.Find("Camera").GetComponent<Transform>().rotation;
        leftShiftBoost = false;
        fire1 = false;
        if (Input.GetMouseButton(1))
        {
            leftShiftBoost = true;
        }
        leftShift = Input.GetKey(KeyCode.LeftShift);
        fire1 = Input.GetAxis("Fire1") > 0.0f;

        //inputChangeSpeed = Input.GetAxis(kSpeedAxis);

        inputVertical = Input.GetAxis("Vertical");
        inputHorizontal = Input.GetAxis("Horizontal");
        FixedString64Bytes nombre_planeta = GameObject.Find("Camera").GetComponent <CustomCamerino>().planet;
        Vector3d planet_speed = Vector3d.zero;
        Entities.ForEach((in Planeta planet,in PlanetarianOrbit po) =>
        {
            if (po.nombre == nombre_planeta)
                planet_speed = planet.simulated_speed;
        }).WithoutBurst().Run();

         
        Entities.ForEach((ref CameraPos camera, ref Rotation rot) => {

            rot.Value = rotation;

            // inputRotateAxisX += (Input.GetAxis(kRightStickX) * m_LookSpeedController * k_MouseSensitivityMultiplier);
            //inputRotateAxisY += (Input.GetAxis(kRightStickY) * m_LookSpeedController * k_MouseSensitivityMultiplier);



            bool moved =  inputVertical != 0.0f || inputHorizontal != 0.0f ;
            if (moved)
            {
                float moveSpeed = delta * m_MoveSpeed;
                if (fire1 || leftShiftBoost && leftShift)
                    moveSpeed *= m_Turbo;
                camera.sim_pos += Vector3d.Tove3(math.mul(rot.Value,Vector3.forward) ) * moveSpeed * inputVertical;
                camera.sim_pos += Vector3d.Tove3(math.mul(rot.Value, Vector3.right)) * moveSpeed * inputHorizontal;
            }
            if (nombre_planeta != "")
            {
                camera.sim_pos += planet_speed;
            }


        }).Schedule();
    }
}
