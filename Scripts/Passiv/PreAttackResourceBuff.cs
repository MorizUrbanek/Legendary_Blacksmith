using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Passivs/PreDamageResourceBuff")]
public class PreAttackResourceBuff : Passiv
{
    [SerializeField] float buffInPercent;
    [SerializeField] StatType resourceType;
    [SerializeField] float resourceAmount;

    public override DamagePackage DoPassiv(StatManager statManager, DamagePackage damagePackage)
    {
        if (statManager.CanUseResource(resourceType, resourceAmount))
            damagePackage.bonusDamage += (damagePackage.damage * buffInPercent) / 100;

        return damagePackage;
    }

    public override PassivType GetPassivType()
    {
        return PassivType.PreDamage;
    }
}
