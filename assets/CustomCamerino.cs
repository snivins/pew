using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class CustomCamerino : MonoBehaviour
{

    const float k_MouseSensitivityMultiplier = 0.01f;

    /// <summary>
    /// Rotation speed when using a controller.
    /// </summary>
    public float m_LookSpeedController = 120f;
    /// <summary>
    /// Rotation speed when using the mouse.
    /// </summary>
    public float m_LookSpeedMouse = 4.0f;
    /// <summary>
    /// Movement speed.
    /// </summary>
    public float m_MoveSpeed = 10.0f;
    /// <summary>
    /// Value added to the speed when incrementing.
    /// </summary>
    public float m_MoveSpeedIncrement = 2.5f;
    /// <summary>
    /// Scale factor of the turbo mode.
    /// </summary>
    public float m_Turbo = 10.0f;
    public Vector3d sim_pos = new Vector3d(-69693292.5867402d, 16710563.4486748d, 128451602.981214d); //Vector3d.zero;
    public Vector3d offset = Vector3d.zero;

    EntityManager em;
    Entity camera;
    public string planet;
    // Start is called before the first frame update
    void Start()
    {
        em = World.DefaultGameObjectInjectionWorld.EntityManager;
        camera = em.CreateEntity();

        em.AddComponent(camera, typeof(CameraPos));
        em.SetComponentData(camera, new CameraPos
        {
            sim_pos = new Vector3d(-69693292.5867402d, 16710563.4486748d, 128451602.981214d),//*1000d,
         }
        );
    }

    float inputRotateAxisX, inputRotateAxisY;
    float inputChangeSpeed;
    float inputVertical, inputHorizontal, inputYAxis;
    bool leftShiftBoost, leftShift, fire1;

    void UpdateInputs()
    {
        inputRotateAxisX = 0.0f;
        inputRotateAxisY = 0.0f;
        if (Input.GetMouseButton(1))
        {
            inputRotateAxisX = Input.GetAxis("Mouse X") * m_LookSpeedMouse;
            inputRotateAxisY = Input.GetAxis("Mouse Y") * m_LookSpeedMouse;
        }
    }
    // Update is called once per frame
    void Update()
    {
        //transform.position = em.GetComponentData<R>(camera).sim_pos.ToFloat();
        
         UpdateInputs();
        /*
            if (inputChangeSpeed != 0.0f)
            {
                m_MoveSpeed += inputChangeSpeed * m_MoveSpeedIncrement;
                if (m_MoveSpeed < m_MoveSpeedIncrement) m_MoveSpeed = m_MoveSpeedIncrement;
            }
        */
            bool moved = inputRotateAxisX != 0.0f || inputRotateAxisY != 0.0f ;
            if (moved)
            {
                float rotationX = transform.localEulerAngles.x;
                float newRotationY = transform.localEulerAngles.y + inputRotateAxisX;

                // Weird clamping code due to weird Euler angle mapping...
                float newRotationX = (rotationX - inputRotateAxisY);
                if (rotationX <= 90.0f && newRotationX >= 0.0f)
                    newRotationX = Mathf.Clamp(newRotationX, 0.0f, 90.0f);
                if (rotationX >= 270.0f)
                    newRotationX = Mathf.Clamp(newRotationX, 270.0f, 360.0f);

                transform.localRotation = Quaternion.Euler(newRotationX, newRotationY, transform.localEulerAngles.z);

            }
       /* if (planet != null)
        {
            sim_pos += planet.sped;
        }*/
    }
}
