using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Condition/Stack Stat Buff")]
public class StackingBuffCondition : Condition
{
    [SerializeField] StackBuffConditionHandler handler;

    public override ConditionHandler GetCondition()
    {
        return new StackBuffConditionHandler(handler);
    }
}

[Serializable]
public class StackBuffConditionHandler : ConditionHandler
{
    [SerializeField] float timeInSec;
    [SerializeField] List<StatChangeValue> statChangeValues;
    [SerializeField] int maxStacks;
    private float currentTime;
    private int currentStacks = 0;

    public StackBuffConditionHandler(StackBuffConditionHandler handler)
    {
        timeInSec = handler.timeInSec;
        type = handler.type;
        popUpColor = handler.popUpColor;
        statChangeValues = handler.statChangeValues;
        maxStacks = handler.maxStacks;
        sprite = handler.sprite;
    }

    public override IEnumerator GetCondition(StatManager statManager)
    {
        return StackBuffCondition(statManager);
    }

    public override void PopCondition(StatManager statManager)
    {
        StackCondition(statManager);
    }

    public override void StackCondition(StatManager statManager)
    {
        if (currentStacks < maxStacks)
        {
            foreach (StatChangeValue statChangeValue in statChangeValues)
            {
                statManager.ChangeStat(statChangeValue.type, statChangeValue.value);
            }
            currentStacks++;
            InvokeOnStackUpdate(currentStacks);
        }
        currentTime = timeInSec;
    }

    IEnumerator StackBuffCondition(StatManager statManager)
    {
        StackCondition(statManager);
        
        while (currentTime > 0)
        {
            yield return new WaitForSeconds(timer);
            InvokeOnTimerUpdate(currentTime);
            currentTime -= timer;
        }

        foreach (StatChangeValue statChangeValue in statChangeValues)
        {
            statManager.ChangeStat(statChangeValue.type, -statChangeValue.value * currentStacks);
        }
        statManager.RemoveCondition(type);
        InvokeOnConditionEnd();
    }
}
