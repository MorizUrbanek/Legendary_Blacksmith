using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon/Weapon")]
public class WeaponObject : ScriptableObject
{
    [SerializeField] public WeaponItem weaponStats;

    public WeaponName weaponName;

    public WeaponItem GetWeaponItem() { return new WeaponItem(weaponStats); }
}

[Serializable]
public class WeaponItem
{
    public GameObject weaponPrefab;
    public List<Slash> Slashes;
    public Color slashColor;
    public bool canChangeBladeColor;
    public List<StatChangeValue> statChangeValues;
    public List<Passiv> passivs;
    public float weaponDamage;

    public WeaponAction attack1;
    public WeaponAction attack2;
    public WeaponAction specialAttack;

    public WeaponItem(WeaponItem weaponItem)
    {
        weaponPrefab = weaponItem.weaponPrefab;
        Slashes = weaponItem.Slashes;
        slashColor = weaponItem.slashColor;
        canChangeBladeColor = weaponItem.canChangeBladeColor;
        statChangeValues = weaponItem.statChangeValues.DeepClone();
        passivs = weaponItem.passivs;
        weaponDamage = weaponItem.weaponDamage;
        attack1 = weaponItem.attack1;
        attack2 = weaponItem.attack2;
        specialAttack = weaponItem.specialAttack;
    }
}
