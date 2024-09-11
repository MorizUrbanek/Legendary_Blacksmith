using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "MaterialItem/Ore")]
public class OreMaterialItem : BaseMaterialItem
{
    public override MaterialType GetMaterialType()
    {
        return MaterialType.Ore;
    }
}
