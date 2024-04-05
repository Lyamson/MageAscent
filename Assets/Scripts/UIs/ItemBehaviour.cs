using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemBehaviour : MonoBehaviour
{
    [SerializeField] Image backgroundImage;
    [SerializeField] Text nameText;
    [SerializeField] Text priceText;
    [NonSerialized] public Spell item;
    ShopBehaviour shopBehaviour;
    bool chosen;
    public bool Chosen
    {
        get => chosen;
        set
        {
            if (value)
            {
                backgroundImage.color = new Color(0.8f, 1f, 0.5f, 1f);
            }
            else
            {
                backgroundImage.color = Color.white;
            }
        }
    }

    private void Start()
    {
        shopBehaviour = gameObject.GetComponentInParent<ShopBehaviour>();
    }

    public void SetItem<T>() where T : Spell, new()
    {
        item = gameObject.AddComponent<T>();
        nameText.text = item.Name;
        priceText.text = item.price.ToString();
    }

    public void Toggled()
    {
        shopBehaviour.ChooseItem(this);
    }
}
