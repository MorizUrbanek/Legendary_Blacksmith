using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon Actions/Sinlge Strike")]
[Serializable]
public class WeaponAction : ScriptableObject
{
    public ActionType actionType;
    public List<ActionAttribute> actionAttributes;
}

[Serializable]
public class ActionAttribute
{
    public string actionName;
    public SlashType slashType;
    public bool canRotate;
}

