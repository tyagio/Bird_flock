using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Member
{
    override protected void Running()
    {
        acceleration += Wander() * conf.WanderPriority;
        acceleration += Cohesion() * conf.CohesionPriority*2;
        acceleration = Vector3.ClampMagnitude(acceleration, conf.max_acceleration * 2f);
    }

    override protected void Update()
    {
        //acceleration = Wander();
        //acceleration = Vector3.ClampMagnitude(acceleration, conf.max_acceleration);
        velocity += (acceleration * Time.deltaTime);
        velocity = Vector3.ClampMagnitude(velocity, conf.max_velocity*1.15f);
        position += (velocity * Time.deltaTime);
        prev_position = position;
        WrapAround(ref position, -level.bounds, level.bounds);
        transform.position = position;
        transform.rotation = Quaternion.Euler(0f, 0f, (Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg) + 90f);
        if (prev_position != position)
        {
            my_trail.Clear();
        }
    }

}
