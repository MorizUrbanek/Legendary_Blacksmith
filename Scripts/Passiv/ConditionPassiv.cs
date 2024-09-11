using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Passivs/Condition Passiv")]
public class ConditionPassiv : Passiv
{
    [SerializeField] Condition condition;

    public override DamagePackage DoPassiv(StatManager statManager, DamagePackage damagePackage)
    {
        damagePackage.condition = condition;
        return damagePackage;
    }

    public override PassivType GetPassivType()
    {
        return PassivType.PreDamage;
    }
}
