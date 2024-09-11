using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Weapon/Recipe")]
public class WeaponRecipe : ScriptableObject
{
    [SerializeField] WeaponObject weaponObject;

    [SerializeField] public List<MaterialValue> materialsNeeded;

    public WeaponItemWithExtraData GetWeaponItem(List<BaseMaterialItem> usedMaaterials)
    {
        WeaponItemWithExtraData returnVal = new WeaponItemWithExtraData();
        returnVal.weaponName = weaponObject.weaponName;
        returnVal.statChangeValues = new List<StatChangeValue>();


        bool isOnList = false;

        foreach (BaseMaterialItem item in usedMaaterials)
        {
            foreach (StatChangeValue statChange in returnVal.statChangeValues)
            {
                if (statChange.type == item.StatChangeValue.type)
                {
                    statChange.value += item.StatChangeValue.value;
                    isOnList = true;
                    break;
                }
            }

            if (!isOnList)
            {
                returnVal.statChangeValues.Add(item.StatChangeValue.DeepClone());
            }
            isOnList = false;
        }

        return returnVal;
    }

}


[Serializable]
public class MaterialValue
{
    public MaterialType type;
    public int amount;
}