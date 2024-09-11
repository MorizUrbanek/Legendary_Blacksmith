using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class BaseMaterialItem : ScriptableObject
{
    public StatChangeValue StatChangeValue;

    public abstract MaterialType GetMaterialType();
}
