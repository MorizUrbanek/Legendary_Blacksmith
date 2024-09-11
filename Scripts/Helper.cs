using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public enum ActionType { SingleStrike, Combo }

public enum SlashType { NormalRight, NormalLeft, HeavyRight, HeavyLeft, OverHead, OverHeadGround, Zone, Ground, SpinRight, SpinLeft }

public enum PassivType { PreDamage, PostDamage, Defensiv }

public enum AttackType { BasicAttack, SpecialAttack}

public enum ConditionType { Poison, Frost, Lightning, Bleeding, AttackBuff, Adrenalin }

public enum StatType { HealthPoint, Stamina, Attack, Defense, CritRate, CritDamage, MoveSpeed, AttackSpeed, conditionMultiplier, healFactor }

public enum MaterialType { Ore, Wood, BeastCore }

public enum WeaponName { Staff, Sword }

public static class Helpers
{
    private static Matrix4x4 isoMatirx = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));

    public static Vector3 ToIso(this Vector3 input) => isoMatirx.MultiplyPoint3x4(input);

    public static T DeepClone<T>(this T obj)
    {
        using (var ms = new MemoryStream())
        {
            var formatter = new BinaryFormatter();
            formatter.Serialize(ms, obj);
            ms.Position = 0;

            return (T)formatter.Deserialize(ms);
        }
    }

    
}

public interface IHitable
{
    public DamagePackage GetHit(DamagePackage damagePackage);

    public void TakeDamage(float damage, bool isCrit, Color color);

    public void Heal(float amount);
    
    public void RemoveCondition(ConditionType conditionType);
}

public interface ISaveable
{
    object OnSave();

    void OnLoad(object state);
}


