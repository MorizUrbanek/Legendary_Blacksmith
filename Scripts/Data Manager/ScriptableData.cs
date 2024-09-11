using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableData : MonoBehaviour
{
    public static ScriptableData instance;

    public List<WeaponObject> scriptableWeapons = new List<WeaponObject>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
