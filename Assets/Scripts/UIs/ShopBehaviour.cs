using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopBehaviour : MonoBehaviour
{
    [SerializeField] Canvas ShopMenuCanvas;
    [SerializeField] GameObject shopItemPrefab;
    [SerializeField] GameObject shopItemsListPanel;
    List<ItemBehaviour> shopItems = new();

    ItemBehaviour chosenItem;

    private void Start()
    {
        AddItem<BallSpell>();
        AddItem<FireballSpell>();
        AddItem<IceSpell>();
    }

    private void AddItem<T>() where T : Spell, new()
    {
        ItemBehaviour item = Instantiate(shopItemPrefab, shopItemsListPanel.transform).GetComponent<ItemBehaviour>();
        item.SetItem<T>();
        shopItems.Add(item);
    }

    public void OpenShop()
    {
        foreach (ItemBehaviour item in shopItems)
        {
            if (GameManager.Singleton.player.skillsManager.HasSpell(item.item.spellType))
            {
                item.gameObject.SetActive(false);
            }
            else
            {
                item.gameObject.SetActive(true);
            }
        }
        ShopMenuCanvas.gameObject.SetActive(true);
    }

    public void ChooseItem(ItemBehaviour item)
    {
        if (chosenItem != null)
            chosenItem.Chosen = false;
        chosenItem = item;
        if (item != null)
        {
            item.Chosen = true;
        }

    }

    public void Buy()
    {
        if (chosenItem == null || !GameManager.Singleton.player.TryPayPrice(chosenItem.item.price))
            return;
        GameManager.Singleton.player.skillsManager.LearnSpell(chosenItem.item.spellType);
        ChooseItem(null);
        OpenShop();
    }

    public void CloseShop()
    {
        ShopMenuCanvas.gameObject.SetActive(false);
    }

    public void ToggleShop()
    {
        if (ShopMenuCanvas.gameObject.activeSelf)
            CloseShop();
        else
            OpenShop();
    }
}