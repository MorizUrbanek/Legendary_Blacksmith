using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectableListObject : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] int objectIndex;
    [SerializeField] TMP_Text nameText;
    [SerializeField] CanvasGroup canvasGroup;
    public float defaultAlpha = 0.6f;

    public event Action<int> OnClick;

    private void Awake()
    {
        canvasGroup.alpha = defaultAlpha;
    }

    public void SetUp(int objectIndex, string name)
    {
        this.objectIndex = objectIndex;
        nameText.text = name.Replace("_"," ");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnClick?.Invoke(objectIndex);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        canvasGroup.alpha = 1;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        canvasGroup.alpha = defaultAlpha;
    }

}
