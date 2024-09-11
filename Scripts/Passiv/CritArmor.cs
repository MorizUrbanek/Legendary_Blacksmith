using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Passivs/Crit Armor")]
public class CritArmor : Passiv
{
    public override DamagePackage DoPassiv(StatManager statManager, DamagePackage damagePackage)
    {
        damagePackage.critRate = 0;
        return damagePackage;
    }

    public override PassivType GetPassivType()
    {
        return PassivType.Defensiv;
    }
}
