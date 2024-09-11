using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Passivs/Life Steal")]
public class LifeSteal : Passiv
{
    [SerializeField] float lifeStealInPercent;

    public override DamagePackage DoPassiv(StatManager statManager, DamagePackage damagePackage)
    {
        if(damagePackage.damage != 0)
        {
            float lifeSteal = (damagePackage.damage * lifeStealInPercent) / 100;
            statManager.Heal(lifeSteal);
        }
        return damagePackage;
    }

    public override PassivType GetPassivType()
    {
        return PassivType.PostDamage;
    }
}
