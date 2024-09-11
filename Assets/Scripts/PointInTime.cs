using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointInTime
{
    public Vector3 position, velocity, angularV;
    public Quaternion rotation;

    public PointInTime(Vector3 _position, Vector3 _velocity, Vector3 _angularV, Quaternion rot)
    {
        position = _position;
        velocity = _velocity;
        angularV = _angularV;
        rotation = rot;
    }
}
