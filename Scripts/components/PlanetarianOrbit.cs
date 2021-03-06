using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public struct PlanetarianOrbit : IComponentData
{
    // Add fields to your component here. Remember that:
    //
    // * A component itself is for storing data and doesn't 'do' anything.
    //
    // * To act on the data, you will need a System.
    //
    // * Data in a component must be blittable, which means a component can
    //   only contain fields which are primitive types or other blittable
    //   structs; they cannot contain references to classes.
    //
    // * You should focus on the data structure that makes the most sense
    //   for runtime use here. Authoring Components will be used for 
    //   authoring the data in the Editor.
    public double mu;
    public double eccentricity;
    public double n, cosLOAN, sinLOAN, sinI, cosI, trueAnomalyConstant;
    public double semiMajorAxis;
    public double argumentOfPeriapsis;
    public double masa;
    public double influence_distance;
    public Vector3d e_vector;
    public Vector3d n_vector;
}
