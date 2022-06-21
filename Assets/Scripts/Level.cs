using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public Transform member_prefab;
    public Transform enemy_prefab;
    public int number_of_members;
    public int number_of_enemies;
    public List<Member> members;
    public List<Enemy> Enemies;
    public float bounds;
    public float spawnradius;
    // Start is called before the first frame update
    void Start()
    {
        members = new List<Member>();
        Enemies = new List<Enemy>();
        Spawn(member_prefab, number_of_members);
        members.AddRange(FindObjectsOfType<Member>());
        Spawn(enemy_prefab, number_of_enemies);
        Enemies.AddRange(FindObjectsOfType<Enemy>());
    }

    void Spawn(Transform prefab,int count)
    {
        for(int i = 0; i < count; i++)
        {
            Instantiate(prefab, new Vector3(Random.Range(-spawnradius,spawnradius),
                Random.Range(-spawnradius, spawnradius), 0f), Quaternion.identity);
        }
    }

    public List<Member> GetNeighbours(Member member,float radius)
    {
        List<Member> neighboursfound = new List<Member>();
        foreach(var Othermember in members)
        {
            if (Othermember == member)
                {continue;}
            if (Vector3.Distance(member.position, Othermember.position) <= radius)
            {
                neighboursfound.Add(Othermember);
            }
        }
        return neighboursfound;
    }
    
    public List<Enemy> GetEnemies(Member member,float radius) {
        List<Enemy> enemiesfound = new List<Enemy>();
        foreach (var i in Enemies){
            if (Vector3.Distance(member.position, i.position) <= radius){
                enemiesfound.Add(i);
            }
        }
        return enemiesfound;
    }
}
