using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Condition/Frost")]
public class FrostCondition : Condition
{
    [SerializeField] FrostConditionHandler handler;

    public override ConditionHandler GetCondition()
    {
        return new FrostConditionHandler(handler);
    }
}

[Serializable]
public class FrostConditionHandler : ConditionHandler
{
    [SerializeField] float timeInSec;
    [SerializeField] StatChangeValue statChangeValue;
    private float currentTime;

    public FrostConditionHandler(FrostConditionHandler handler)
    {
        timeInSec = handler.timeInSec;
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
        currentTime = timeInSec;
    }

    IEnumerator BleedingCondition(StatManager statManager)
    {
        StackCondition(statManager);
        statManager.ChangeStat(statChangeValue.type, -statChangeValue.value);
        while (currentTime > 0) 
        {
            yield return new WaitForSeconds(timer);
            InvokeOnTimerUpdate(currentTime);
            currentTime -= timer;
        }

        statManager.ChangeStat(statChangeValue.type, statChangeValue.value);
        statManager.RemoveCondition(type);
        InvokeOnConditionEnd();
    }
}