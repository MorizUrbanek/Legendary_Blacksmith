using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConditionBar : MonoBehaviour
{
    [SerializeField] StatManager statManager;
    [SerializeField] GameObject iconPrefab;

    private void OnEnable()
    {
        statManager.OnCreateConditionIcon += CreateConditionIcon;
    }

    private void OnDisable()
    {
        statManager.OnCreateConditionIcon -= CreateConditionIcon;
    }

    private void CreatePassivIcon(Sprite sprite)
    {
        
    }

    public void CreateConditionIcon(ConditionHandler condition) 
    {
        var temp = Instantiate(iconPrefab, transform);
        if (temp.TryGetComponent(out ConditionIconHandler iconHandler))
        {
            iconHandler.SetUpConditionIcon(condition);
        }
    }
}
