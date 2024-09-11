using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Condition/Lighning")]
public class LightningCondition : Condition
{
    [SerializeField] LighningConditionHandler handler;

    public override ConditionHandler GetCondition()
    {
        return new LighningConditionHandler(handler);
    }
}

[Serializable]
public class LighningConditionHandler : ConditionHandler
{
    [SerializeField] float timeInSec;
    [SerializeField] float damagePerAttack;
    [SerializeField] float popDamage;
    [SerializeField] int stacksTopop;
    [SerializeField] StatChangeValue statChangeValue;
    private float currentTime;

    public LighningConditionHandler(LighningConditionHandler handler)
    {
        timeInSec = handler.timeInSec;
        damagePerAttack = handler.damagePerAttack;
        popDamage = handler.popDamage;
        stacksTopop = handler.stacksTopop;
        type = handler.type;
        popUpColor = handler.popUpColor;
        statChangeValue = handler.statChangeValue;
        sprite = handler.sprite;
    }

    public override IEnumerator GetCondition(StatManager statManager)
    {
        return LighningCondition(statManager);
    }

    public override void PopCondition(StatManager statManager) 
    {
        StackCondition(statManager);
    }

    public override void StackCondition(StatManager statManager)
    {
        stacksTopop--;
        if (stacksTopop == 0)
        {
            statManager.TakeDamage(popDamage, false, popUpColor);
            currentTime = 0;
        }
        else
        {
            statManager.TakeDamage(damagePerAttack, false, popUpColor);
            currentTime = timeInSec;
        }
        InvokeOnStackUpdate(stacksTopop);
    }

    IEnumerator LighningCondition(StatManager statManager)
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
