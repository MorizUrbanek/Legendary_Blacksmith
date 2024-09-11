using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConditionIconHandler : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] TextMeshProUGUI stack;
    [SerializeField] TextMeshProUGUI timer;
    private ConditionHandler iconhandler;

    private void Awake()
    {
        stack.text = string.Empty;
        timer.text = string.Empty;
    }

    public void SetUpConditionIcon(ConditionHandler handler)
    {
        if (handler == null) return;
        iconhandler = handler;
        image.sprite = handler.sprite;
        handler.OnTimerUpdate += UpdateTimer;
        handler.OnStackUpdate += UpdateStack;
        handler.OnConditionEnd += DestroyIcon;
    }

    public void UpdateTimer(float currentTimer) => timer.text = string.Format("{0:F1}", currentTimer) + "s";

    public void UpdateStack(int currentStacks) => stack.text = currentStacks + "x";

    public void DestroyIcon()
    {
        iconhandler.OnTimerUpdate += UpdateTimer;
        iconhandler.OnStackUpdate += UpdateStack;
        iconhandler.OnConditionEnd += DestroyIcon;
        Destroy(gameObject);
    }
}
        
