using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Member : MonoBehaviour
{
    public Vector3 position;
    public Vector3 prev_position;
    public Vector3 velocity;
    public Vector3 acceleration;
    public Level level;
    public Member_config conf;
    protected TrailRenderer my_trail;
    protected virtual void Start()
    {
        level = FindObjectOfType<Level>();
        conf = FindObjectOfType<Member_config>();
        position = transform.position;
        prev_position = position;
        velocity = new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0);
        InvokeRepeating("Running", 0f,conf.recalc_time);
        my_trail = GetComponent<TrailRenderer>();
    }

    protected virtual void Running()
    {
        acceleration = Cohesion()*conf.CohesionPriority;
        acceleration += Wander() * conf.WanderPriority;
        acceleration += Alignment() * conf.AlignmentPriority;
        acceleration += Seperation() * conf.SeperationPriority;
        acceleration += Avoidance() * conf.AvoidancePriority;
        acceleration = Vector3.ClampMagnitude(acceleration, conf.max_acceleration);
    }
    protected virtual void Update()
    {
        //acceleration = Wander();
        //acceleration = Vector3.ClampMagnitude(acceleration, conf.max_acceleration);
        velocity += (acceleration * Time.deltaTime);
        velocity = Vector3.ClampMagnitude(velocity, conf.max_velocity);
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

    protected Vector3 Wander()
    {
        float jitter = conf.WanderJitter * Time.deltaTime;
        Vector3 wandertarget = new Vector3(RandomBinomial() * jitter, RandomBinomial() * jitter, 0);
        wandertarget = wandertarget.normalized;
        wandertarget *= conf.WanderRadius;
        Vector3 target_in_local_space = wandertarget + new Vector3(0f, conf.WanderDistance, 0f);
        Vector3 target_in_world_space = transform.TransformPoint(target_in_local_space);
        target_in_world_space -= this.position;
        return target_in_world_space.normalized;
    }

    protected Vector3 Cohesion()
    {
        Vector3 cohesion_vector = new Vector3();
        int neigh_count = 0;
        var neighbours = level.GetNeighbours(this, conf.CohesionRadius);
        if (neighbours.Count == 0){ return cohesion_vector; }
        foreach(var i in neighbours){
            if (IsInFov(i.position)){
                cohesion_vector += i.position;
                neigh_count++;
            }
        }
        if (neigh_count == 0){
            return cohesion_vector;
        }
        cohesion_vector /= neigh_count;
        cohesion_vector -= this.position;
        return cohesion_vector.normalized;

    }

    protected Vector3 Alignment()
    {
        Vector3 align_vector = new Vector3();
        var neighbours = level.GetNeighbours(this, conf.AlignmentRadius);
        if (neighbours.Count == 0) { return align_vector; }
        foreach (var i in neighbours)
        {
            if (IsInFov(i.position)){
                align_vector += i.velocity;
            }
        }
        return align_vector.normalized;
    }

    protected Vector3 Seperation()
    {
        Vector3 sep_vec = new Vector3();
        var neighbours = level.GetNeighbours(this, conf.SeperationRadius);
        if (neighbours.Count == 0) { return sep_vec; }
        foreach(var i in neighbours) {
            if (IsInFov(i.position)){
                Vector3 mov_dir = this.position - i.position;
                if (mov_dir.magnitude > 0)
                {
                    sep_vec += mov_dir.normalized / mov_dir.magnitude;
                }
            }
        }
        return sep_vec.normalized;

    }

    protected Vector3 Avoidance()
    {
        Vector3 avoid_vec = new Vector3();
        var enemy_list= level.GetEnemies(this,conf.AvoidanceRadius);
        if (enemy_list.Count == 0) { return avoid_vec; }
        foreach(var i in enemy_list) {
            avoid_vec += RunAway(i.position);
        }
        return avoid_vec.normalized;
    }

    Vector3 RunAway(Vector3 target)
    {
        Vector3 needed_velocity = (position - target).normalized * conf.max_velocity;
        return needed_velocity - this.velocity;
    }
    protected void WrapAround(ref Vector3 vec, float min, float max)
    {
        vec.x = WrapAroundFloat(vec.x, min, max);
        vec.y = WrapAroundFloat(vec.y, min, max);
        vec.z = WrapAroundFloat(vec.z, min, max);
    }
    
    float WrapAroundFloat(float val, float min, float max)
    {
        if (val > max)
        {val = min;}
        else if (val < min)
        {val = max;}
        return val;
    }

    float RandomBinomial()
    {return (Random.Range(0f, 1f) - Random.Range(0f, 1f));}

    bool IsInFov(Vector3 vec)
    {
        return Vector3.Angle(this.velocity, vec - this.position) <= conf.maxfov;
    }
}
