using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamagePopUpAnimation : MonoBehaviour
{
    public AnimationCurve opacityCurve;
    public AnimationCurve scaleCurve;
    public AnimationCurve heightCurve;

    private TextMeshProUGUI tmp;
    private float timer;


    private void Awake()
    {
        tmp = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        tmp.color = new Color(tmp.color.r, tmp.color.g, tmp.color.b, opacityCurve.Evaluate(timer));
        transform.localScale = Vector3.one * scaleCurve.Evaluate(timer);
        transform.position = transform.position + new Vector3(0, heightCurve.Evaluate(timer) * Time.deltaTime, 0);
        timer += Time.deltaTime;
    }
}
