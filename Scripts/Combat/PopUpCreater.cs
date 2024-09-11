using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopUpCreater : MonoBehaviour
{
    IEnumerator hit;
    public GameObject popupPrefab;
    public Transform canvasParent;
    public StatManager statManager;
    
    private float popupSpray = 0.5f;

    private void OnEnable()
    {
        statManager.OnDamageTaken += CreatePopUp;
    }

    private void OnDisable()
    {
        statManager.OnDamageTaken -= CreatePopUp;
    }

    private void Update()
    {
        canvasParent.forward = Camera.main.transform.forward;
    }

    public void GetHit()
    {
        if (hit == null)
        {
            hit = ChangeColor();
            StartCoroutine(hit);
        }
    }

    public void CreatePopUp(float amount, Color color, bool isCrit)
    {
        GetHit();
        var popup = Instantiate(popupPrefab, canvasParent);
        var popuptext = popup.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        popuptext.transform.localPosition = new Vector3(Random.Range(-popupSpray, popupSpray), Random.Range(-popupSpray, popupSpray), Random.Range(-popupSpray, popupSpray));
        popuptext.faceColor = color;
        if (isCrit) popuptext.outlineColor = Color.yellow;
        else popuptext.outlineColor = Color.black;
        popuptext.text = Mathf.FloorToInt(amount).ToString();

        Destroy(popup, 1f);
    }

    IEnumerator ChangeColor()
    {
        if (TryGetComponent(out Renderer renderer))
        {
            var temp = renderer.material;
            var baseColor = temp.color;
            temp.color = Color.red;

            yield return new WaitForSeconds(0.05f);

            temp.color = baseColor;
            hit = null;
        }
    }
}
