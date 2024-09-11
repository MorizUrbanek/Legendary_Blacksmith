using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CraftMenuManager : MonoBehaviour
{
    [SerializeField] WeaponRecipe[] weaponRecipes;
    [SerializeField] GameObject recipelistPrefab;
    [SerializeField] Transform recipelistParent;

    private WeaponRecipe selectedRecipe;

    [SerializeField] BaseMaterialItem[] baseMaterials;
    [SerializeField] GameObject materialGridPrefab;
    [SerializeField] Transform materialGridParent;
    Dictionary<MaterialType,int> neededMaterials = new Dictionary<MaterialType, int>();

    [SerializeField] List<BaseMaterialItem> selectedBaseMaterials = new List<BaseMaterialItem>();

    [SerializeField] List<BaseMaterialItem> useableBaseMaterials = new List<BaseMaterialItem>();

    private void Start()
    {
       SetUpRecipeList();
    }

   
    private void SetUpMaterialGrid()
    {
        foreach (var neededMaterial in neededMaterials.Keys) 
        {
            var temparray = baseMaterials.Where(x => x.GetMaterialType() == neededMaterial).ToArray();
            useableBaseMaterials.AddRange(temparray);
        }

        GameObject temp;
        for (int i = 0; i < useableBaseMaterials.Count; i++)
        {
            temp = Instantiate(materialGridPrefab, materialGridParent);
            if (temp.TryGetComponent(out SelectableListObject selectableListObject))
            {
                selectableListObject.SetUp(i, useableBaseMaterials[i].name);
                selectableListObject.OnClick += OnMaterialClick;
            }
        }
    }

    private void OnMaterialClick(int index)
    {
        BaseMaterialItem temp = useableBaseMaterials[index];

        if(neededMaterials.ContainsKey(temp.GetMaterialType()))
        {
            selectedBaseMaterials.Add(temp);
            neededMaterials[temp.GetMaterialType()]--;
            if(neededMaterials[temp.GetMaterialType()] == 0)
                neededMaterials.Remove(temp.GetMaterialType());
        }

        Debug.Log(useableBaseMaterials[index].name);
    }

    private void SetUpRecipeList()
    {
        GameObject temp;
        for (int i = 0; i < weaponRecipes.Length; i++)
        {
            temp = Instantiate(recipelistPrefab, recipelistParent);
            if (temp.TryGetComponent(out SelectableListObject selectableListObject))
            {
                selectableListObject.SetUp(i, weaponRecipes[i].name);
                selectableListObject.OnClick += OnRecipeClick;
            }
        }
    }

    private void OnRecipeClick(int index)
    {
        selectedRecipe = weaponRecipes[index];
        neededMaterials.Clear();
        foreach (var materialNeeded in selectedRecipe.materialsNeeded)
        {
            neededMaterials.Add(materialNeeded.type, materialNeeded.amount);
        }
        SetUpMaterialGrid();
        Debug.Log(weaponRecipes[index].name);
    }


    //public void SetStatScrollViewToSelectedChild(RectTransform selectedChild)
    //{
    //    RectTransform contentPanel = statParent.GetComponent<RectTransform>();

    //    Vector2 viewportLocalPosition = statScrollRect.viewport.localPosition;
    //    Vector2 childLocalPosition = selectedChild.localPosition;
    //    Vector2 result = new Vector2(
    //        0 - (viewportLocalPosition.x + childLocalPosition.x),
    //        0 - (viewportLocalPosition.y + childLocalPosition.y));

    //    contentPanel.localPosition = result;
    //}
}
