using System;
using System.Collections;
using UnityEngine;


[CreateAssetMenu(menuName = "Condition/Poison")]
public class PoisonCondition : Condition
{
    [SerializeField] PoisonConditionHandler handler;

    public override ConditionHandler GetCondition()
    {
        return new PoisonConditionHandler(handler);
    }
}

[Serializable]
public class PoisonConditionHandler : ConditionHandler
{
    [SerializeField] float damagePerTick;
    [SerializeField] int ticks;
    [SerializeField] float timePerTick;
    private int currentTick;
    private int stacks;
    [SerializeField] float damagePerStack;
    private int maxTicks;



    public PoisonConditionHandler(PoisonConditionHandler handler)
    {
        damagePerTick = handler.damagePerTick;
        ticks = handler.ticks;
        timePerTick = handler.timePerTick;
        type = handler.type;
        stacks = 0;
        damagePerStack = handler.damagePerStack;
        popUpColor = handler.popUpColor;
        sprite = handler.sprite;
    }

    public override IEnumerator GetCondition(StatManager statManager)
    {
        return PoisonCondition(statManager);
    }

    public override void PopCondition(StatManager statManager)
    {
        statManager.TakeDamage(stacks * damagePerStack, false ,popUpColor);
        currentTick = maxTicks;
    }

    public override void StackCondition(StatManager statManager)
    {
        stacks++;
        currentTick = 0;
        InvokeOnStackUpdate(stacks);
    }

    IEnumerator PoisonCondition(StatManager statManager)
    {
        float tickTimer = timePerTick;
        maxTicks = Mathf.CeilToInt(ticks * (timePerTick / timer));
        StackCondition(statManager);

        for (; currentTick <= maxTicks; currentTick++)
        {
            yield return new WaitForSeconds(timer);
            InvokeOnTimerUpdate((maxTicks - currentTick) * timer);
            tickTimer -= timer;
            if(tickTimer <= 0)
            {
                statManager.TakeDamage(damagePerTick, false, popUpColor);
                tickTimer = timePerTick;
            }
        }

        statManager.RemoveCondition(type);
        InvokeOnConditionEnd();
    }
}
