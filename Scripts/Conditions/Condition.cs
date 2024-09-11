using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Condition : ScriptableObject
{
    public ConditionType ConditionType;

    public abstract ConditionHandler GetCondition();
}

public abstract class ConditionHandler
{
    [SerializeField] protected ConditionType type;
    [SerializeField] protected Color popUpColor;
    public Sprite sprite;
    protected float timer = 0.1f;

    public event Action<float> OnTimerUpdate;
    public event Action<int> OnStackUpdate;
    public event Action OnConditionEnd;

    public abstract IEnumerator GetCondition(StatManager statManager);

    public abstract void StackCondition(StatManager statManager);

    public abstract void PopCondition(StatManager statManager);

    protected void InvokeOnTimerUpdate(float time) => OnTimerUpdate?.Invoke(time);
    protected void InvokeOnStackUpdate(int stack) => OnStackUpdate?.Invoke(stack);
    protected void InvokeOnConditionEnd() => OnConditionEnd?.Invoke();
}

[Serializable]
public class StatChangeValue
{
    public StatType type;
    public float value;
}
