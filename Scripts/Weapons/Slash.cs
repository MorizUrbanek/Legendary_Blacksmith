using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Slash")]
public class Slash : ScriptableObject
{
    public GameObject slashVFX;
    public SlashType type;
    public List<SlashInteraction> interactions;
}

[Serializable]
public class SlashInteraction
{
    public AttackType attackType;
    public float time;
    public float damage;
    public float hitRange;
    public float hitAngle;
    public bool isBox;
    public float boxRange;
    public bool popCondition;
}
