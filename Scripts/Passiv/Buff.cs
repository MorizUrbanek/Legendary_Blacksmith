using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Passivs/Buff")]
public class Buff : Passiv
{
    [SerializeField] Condition condition;
    [SerializeField] AttackType attackType;

    public override DamagePackage DoPassiv(StatManager statManager, DamagePackage damagePackage)
    {
        if (damagePackage.attackType == attackType)
            statManager.HandleConditions(condition,false);

        return damagePackage;
    }

    public override PassivType GetPassivType()
    {
        return PassivType.PostDamage;
    }
}
