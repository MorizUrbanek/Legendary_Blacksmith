using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Condition/Stat Buff")]
public class BuffCondition : Condition
{
    [SerializeField] BuffConditionHandler handler;

    public override ConditionHandler GetCondition()
    {
        return new BuffConditionHandler(handler);
    }
}

[Serializable]
public class BuffConditionHandler : ConditionHandler
{
    [SerializeField] float timeInSec;
    [SerializeField] StatChangeValue statChangeValue;
    private float currentTime;

    public BuffConditionHandler(BuffConditionHandler handler)
    {
        timeInSec = handler.timeInSec;
        type = handler.type;
        popUpColor = handler.popUpColor;
        statChangeValue = handler.statChangeValue;
        sprite = handler.sprite;
    }

    public override IEnumerator GetCondition(StatManager statManager)
    {
        return BuffCondition(statManager);
    }

    public override void PopCondition(StatManager statManager)
    {
        StackCondition(statManager);
    }

    public override void StackCondition(StatManager statManager)
    {
        currentTime = timeInSec;
    }

    IEnumerator BuffCondition(StatManager statManager)
    {
        StackCondition(statManager);
        statManager.ChangeStat(statChangeValue.type, statChangeValue.value);
        while (currentTime > 0)
        {
            yield return new WaitForSeconds(timer);
            InvokeOnTimerUpdate(currentTime);
            currentTime -= timer;
        }

        statManager.ChangeStat(statChangeValue.type, -statChangeValue.value);
        statManager.RemoveCondition(type);
        InvokeOnConditionEnd();
    }
}