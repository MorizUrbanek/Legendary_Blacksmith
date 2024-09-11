using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Passiv : ScriptableObject
{
    public abstract PassivType GetPassivType();

    public abstract DamagePackage DoPassiv(StatManager statManager, DamagePackage damagePackage);

}
