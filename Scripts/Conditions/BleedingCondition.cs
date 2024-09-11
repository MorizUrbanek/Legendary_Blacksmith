using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Condition/Bleeding Condition")]
public class BleedingCondition : Condition
{
    [SerializeField] BleedingConditionHandler handler;

    public override ConditionHandler GetCondition()
    {
        return new BleedingConditionHandler(handler);
    }
}

[Serializable]
public class BleedingConditionHandler : ConditionHandler
{
    [SerializeField] float damagePerTick;
    [SerializeField] int ticks;
    [SerializeField] float timePerTick;
    [SerializeField] StatChangeValue statChangeValue;
    private int currentTick;

    public BleedingConditionHandler(BleedingConditionHandler handler)
    {
        damagePerTick = handler.damagePerTick;
        ticks = handler.ticks;
        timePerTick = handler.timePerTick;
        type = handler.type;
        popUpColor = handler.popUpColor;
        statChangeValue = handler.statChangeValue;
        sprite = handler.sprite;
    }

    public override IEnumerator GetCondition(StatManager statManager)
    {
        return BleedingCondition(statManager);
    }

    public override void PopCondition(StatManager statManager) 
    {
        StackCondition(statManager);
    }


    public override void StackCondition(StatManager statManager)
    {
        currentTick = 0;
    }

    IEnumerator BleedingCondition(StatManager statManager)
    {
        float tickTimer = timePerTick;
        int maxTicks = Mathf.CeilToInt(ticks * (timePerTick / timer));
        StackCondition(statManager);
        statManager.ChangeStat(statChangeValue.type, -statChangeValue.value);

        for (; currentTick <= maxTicks; currentTick++)
        {
            yield return new WaitForSeconds(timer);
            InvokeOnTimerUpdate((maxTicks - currentTick) * timer);
            tickTimer -= timer;
            if (tickTimer <= 0)
            {
                statManager.TakeDamage(damagePerTick, false, popUpColor);
                tickTimer = timePerTick;
            }
        }

        statManager.ChangeStat(statChangeValue.type, statChangeValue.value);
        statManager.RemoveCondition(type);
        InvokeOnConditionEnd();
    }
}