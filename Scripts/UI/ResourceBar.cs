using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceBar : MonoBehaviour
{
    [SerializeField] StatType type;
    [SerializeField] Slider slider;
    [SerializeField] StatManager statManager;

    private void Start()
    {
        SetMaxValue(statManager.GetStatBase(type));
    }

    private void OnEnable()
    {
        statManager.SubOrUnSubToCurrentChange(type, SetCurrentValue, true);
    }

    private void OnDisable()
    {
        statManager.SubOrUnSubToCurrentChange(type, SetCurrentValue, false);
    }

    private void SetMaxValue(float value)
    {
        slider.maxValue = value;
        SetCurrentValue(value);
    }

    private void SetCurrentValue(float value) 
    {
        if(value > slider.maxValue) SetMaxValue(value);
        slider.value = value;
    }
}
