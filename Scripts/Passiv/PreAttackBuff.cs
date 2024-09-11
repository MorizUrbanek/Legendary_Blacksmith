using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Passivs/PreDamageBuff")]
public class PreAttackBuff : Passiv
{
    [SerializeField] float buffInPercent;
    [SerializeField] AttackType attackType;

    public override DamagePackage DoPassiv(StatManager statManager, DamagePackage damagePackage)
    {
        if(damagePackage.attackType == attackType)
            damagePackage.bonusDamage += (damagePackage.damage * buffInPercent) / 100;

        return damagePackage;
    }

    public override PassivType GetPassivType()
    {
        return PassivType.PreDamage;
    }
}
