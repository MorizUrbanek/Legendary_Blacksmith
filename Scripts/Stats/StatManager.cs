using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class StatManager : MonoBehaviour, IHitable
{
    [Header("Stats")]
    [SerializeField] ResourceStat healthPoint;
    [SerializeField] ResourceStat stamina;

    [SerializeField] Stat attack;
    [SerializeField] Stat defense;
    [SerializeField] Stat critRate;
    [SerializeField] Stat critDam;
    [SerializeField] Stat moveSpeed;
    [SerializeField] Stat attackSpeed;
    [SerializeField] Stat conditionMultiplier;
    [SerializeField] Stat healFactor;

    [Header("Hitable")]
    public Dictionary<ConditionType, ConditionHandler> conditionTimer = new Dictionary<ConditionType, ConditionHandler>();
    public event Action<float, Color, bool> OnDamageTaken;

    public event Action<ConditionHandler> OnCreateConditionIcon;

    void Awake()
    {
        healthPoint.SetCurrentToBase();
        stamina.SetCurrentToBase();

        attack.SetCurrentToBase();
        defense.SetCurrentToBase();
        critRate.SetCurrentToBase();
        critDam.SetCurrentToBase();
        moveSpeed.SetCurrentToBase();
        attackSpeed.SetCurrentToBase();
        conditionMultiplier.SetCurrentToBase();
        healFactor.SetCurrentToBase();
    }


    #region Stats
    public float ChangeStat(StatType type, float value)
    {
        if (GetStat(type, out Stat stat))
            return stat.ChangeCurrentValue(value);
        else return 1;
    }

    public float GetCurrentStat(StatType type)
    {
        if (GetStat(type, out Stat stat))
            return stat.GetCurrent();
        else return 1;
    }

    public float ChangeStatBase(StatType type, float value)
    {
        if (GetResourceStat(type, out ResourceStat stat))
            return stat.ChangeCurrentValue(value);
        else return 0;
    }

    public float GetStatBase(StatType type)
    {
        if (GetResourceStat(type, out ResourceStat stat))
            return stat.GetCurrent();
        else return 0;
    }

    public void SubOrUnSubToCurrentChange(StatType type, Action<float> onCurrentChange, bool isSub)
    {
        if (GetResourceStat(type, out ResourceStat stat))
            stat.SubOrUnSubToCurrentChange(onCurrentChange, isSub);
    }

    public void SubOrUnSubToBaseChange(StatType type, Action<float> onCurrentChange, bool isSub)
    {
        if (GetResourceStat(type, out ResourceStat stat))
            stat.SubOrUnSubToBaseChange(onCurrentChange, isSub);
    }

    public bool CanUseResource(StatType type, float amountToUse)
    {
        if (GetResourceStat(type, out ResourceStat stat))
        {
            if (stat.GetCurrent() > amountToUse)
            {
                stat.ChangeCurrentValue(-amountToUse);
                return true;
            }
        }
        return false;
    }

    private bool GetStat(StatType type, out Stat stat)
    {
        stat = null;
        switch (type)
        {
            case StatType.HealthPoint:
                stat = healthPoint;
                break;
            case StatType.Stamina:
                stat = stamina;
                break;
            case StatType.Attack:
                stat = attack;
                break;
            case StatType.Defense:
                stat = defense;
                break;
            case StatType.CritRate:
                stat = critRate;
                break;
            case StatType.CritDamage:
                stat = critDam;
                break;
            case StatType.MoveSpeed:
                stat = moveSpeed;
                break;
            case StatType.AttackSpeed:
                stat = attackSpeed;
                break;
            case StatType.conditionMultiplier:
                stat = conditionMultiplier;
                break;
            case StatType.healFactor:
                stat = healFactor;
                break;
        }
        if (stat == null) return false;
        else return true;
    }

    private bool GetResourceStat(StatType type, out ResourceStat stat)
    {
        stat = null;
        switch (type)
        {
            case StatType.HealthPoint:
                stat = healthPoint;
                break;
            case StatType.Stamina:
                stat = stamina;
                break;
        }
        if (stat == null) return false;
        else return true;
    }
    #endregion

    #region Hitable
    public virtual DamagePackage GetHit(DamagePackage damagePackage)
    {
        DamagePackage returnPackage = new DamagePackage(damagePackage);
        returnPackage.damage += returnPackage.bonusDamage;
        returnPackage.damage -= returnPackage.damage * defense.GetCurrent() / 100;
        returnPackage.isCrit = returnPackage.critRate > UnityEngine.Random.Range(1, 101);
        if (returnPackage.isCrit) returnPackage.damage += returnPackage.damage * returnPackage.critDamage / 100;
        TakeDamage(returnPackage.damage, returnPackage.isCrit, Color.black);
        HandleConditions(returnPackage.condition, returnPackage.popCondition);
        return returnPackage;
    }

    public virtual void TakeDamage(float damage, bool isCrit, Color color)
    {
        //if(damage > 0)
        healthPoint.ChangeCurrentValue(-damage);
        OnDamageTaken?.Invoke(damage, color, isCrit);
    }

    public virtual void Heal(float amount)
    {
        amount = amount * healFactor.GetCurrent();
        healthPoint.ChangeCurrentValue(amount);
    }

    public void RemoveCondition(ConditionType conditionType)
    {
        conditionTimer.Remove(conditionType);
    }

    public void HandleConditions(Condition condition, bool popConditon)
    {
        if(condition == null) return;
        if (conditionTimer.TryGetValue(condition.ConditionType, out ConditionHandler conditionHandler))
        {
            if (popConditon)
                conditionHandler.PopCondition(this);
            else
                conditionHandler.StackCondition(this);
        }
        else
        {
            var temp = condition.GetCondition();
            conditionTimer.Add(condition.ConditionType, temp);
            StartCoroutine(temp.GetCondition(this));
            OnCreateConditionIcon?.Invoke(temp);
        }
    }

    #endregion
}

[Serializable]
public class Stat
{
    [SerializeField] protected float baseValue;
    [SerializeField] float currentValue;
    [SerializeField] float currentMin;
    [SerializeField] float currentMax;

    public virtual void SetCurrentToBase()
    {
        currentValue = Mathf.Clamp(baseValue, currentMin, currentMax);
    }

    public float GetCurrent()
    {
        return Mathf.Clamp(currentValue, currentMin, currentMax);
    }

    public virtual float ChangeCurrentValue(float value)
    {
        currentValue += value;
        return GetCurrent();
    }
}


[Serializable]
public class ResourceStat : Stat
{
    [SerializeField] float baseMin;
    [SerializeField] float baseMax;

    public event Action<float> OnCurrentChange;
    public event Action<float> OnBaseChange;

    public override void SetCurrentToBase()
    {
        base.SetCurrentToBase();
        OnCurrentChange?.Invoke(GetCurrent());
    }

    public override float ChangeCurrentValue(float value)
    {
        base.ChangeCurrentValue(value);
        OnCurrentChange?.Invoke(GetCurrent());
        return GetCurrent();
    }

    public void ChangeBaseValue(float value)
    {
        baseValue += value;
        OnBaseChange?.Invoke(GetBase());
    }

    public float GetBase()
    {
        return Mathf.Clamp(baseValue, baseMin, baseMax);
    }

    public void SubOrUnSubToCurrentChange(Action<float> onCurrentChange,bool isSub)
    {
        if (isSub)
            OnCurrentChange += onCurrentChange;
        else
            OnCurrentChange -= onCurrentChange;
    }

    public void SubOrUnSubToBaseChange(Action<float> onBaseChange, bool isSub)
    {
        if (isSub)
            OnBaseChange += onBaseChange;
        else
            OnBaseChange -= onBaseChange;
    }
}
