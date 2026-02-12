using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public static EnemyStats Instance;

    public int level = 1;
    public int health;
    public int damage;
    public float speed;

    public int baseHealth = 6;
    public int baseDamage = 1;
    public float baseSpeed = 2f;
    public int baseXPQuantity = 4;
    public int experienceDrop = 5;
    public int goldDrop = 1;



    void Awake()
    {
        Instance = this;


        Init();
    }
    public void Init()
    {
        health = (int) Mathf.Floor(baseHealth + (level - 1) * 1.5f );
        damage = (int) Mathf.Floor(baseDamage + (level - 1) * 1.25f);
        speed = baseSpeed + (level - 1) * 1.2f;
        //transform.localScale  =  new Vector3((1+ (level-1) * 1.2f), (1 + (level - 1) * 1.2f), 1);
    }
    public void Init(int level)
    {
        health = (int) Mathf.Floor(baseHealth + (level - 1) * 1.5f );
        damage = (int) Mathf.Floor(baseDamage + (level - 1) * 1.25f);
        speed = baseSpeed + (level - 1) * 1.2f;
        //transform.localScale  =  new Vector3((1+ (level-1) * 1.2f), (1 + (level - 1) * 1.2f), 1);
    }
}
