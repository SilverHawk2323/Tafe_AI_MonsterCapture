using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSource : MonoBehaviour
{

    public enum Type
    {
        Instant,
        OverTime,
    }
    [SerializeField] private float damage;
    [SerializeField] private Type type;

    public float GetDamage()
    {
        switch (type)
        {
            case Type.Instant:
                return damage;
                
            case Type.OverTime:
                return damage * Time.deltaTime;
                
            default:
                return damage;
                
        }
        
    }
}
