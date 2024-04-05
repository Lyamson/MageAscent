using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickSkillsBehaviour : MonoBehaviour
{
    [SerializeField] Image[] backgroundPanels = new Image[4];
    [SerializeField] Image[] spellsImages = new Image[4];

    public void SetSpell(Spell.SpellType spellType, byte slot)
    {
        if (slot < 5 && slot > 0)
        {
            if (spellType == Spell.SpellType.NONE)
            {
                spellsImages[slot - 1].sprite = null;
                spellsImages[slot - 1].color = new(1, 1, 1, 0);
            }
            else
            {
                spellsImages[slot - 1].sprite = PrefabContainer.Singleton.GetSprite(spellType);
                spellsImages[slot - 1].color = new(1, 1, 1, 1);
            }
        }
    }

    public void SetActiveSlot(byte slot)
    {
        for (byte i = 1; i < 5; i++)
        {
            backgroundPanels[i - 1].color = i == slot ? new(1, 0.9208561f, 0.2311321f, 1) : new(0.6415094f, 0.5658597f, 0.5658597f, 1);
        }
    }

    public void SetCooldown(byte slot, float currentCooldown)
    {
        if (slot < 5 && slot > 0)
        {
            spellsImages[slot - 1].fillAmount = 1 - currentCooldown;
        }
    }
}