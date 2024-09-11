using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Weapon : MonoBehaviour
{
    public List<Slash> Slashes;
    public Color slashColor;
    public bool canChangeBladeColor;
    public List<Passiv> passivs;
    public float weaponDamage;

    [SerializeField] public WeaponAction attack1;
    [SerializeField] public WeaponAction attack2;
    [SerializeField] public WeaponAction specialAttack;
}

