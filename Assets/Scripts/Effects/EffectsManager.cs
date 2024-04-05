using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsManager
{
    public List<Effect> AllEffects = new();

    Dictionary<Guid, Effect> appliedEffects = new();

    public EffectsManager()
    {
        AllEffects.Add(Effect.CreateEffect<ValuedEffect>("��������", "����������� ���� ��������� �� %ATK%%")
            .AddValueChange(Stats.StatType.ATK, 10));
        AllEffects.Add(Effect.CreateEffect<ValuedEffect>("������", "��������� ����������� ������������ ��������� �� %CD%%")
            .AddValueChange(Stats.StatType.CD, 10));
        AllEffects.Add(Effect.CreateEffect<ValuedEffect>("�������������", "����������� ������������ �������� ��������� �� %HP%%")
            .AddValueChange(Stats.StatType.HP, 10));
        AllEffects.Add(Effect.CreateEffect<ValuedEffect>("��������", "����������� �������� ������������ ��������� �� %SPD%%")
            .AddValueChange(Stats.StatType.SPD, 2));
    }

    public void ApplyEffect(Effect effect)
    {
        effect.Apply();
        appliedEffects.Add(effect.Id, effect);
    }

    public void ClearEffects()
    {
        foreach (Effect effect in appliedEffects.Values)
        {
            effect.Cancel();
        }
        appliedEffects.Clear();
    }

    public int Count()
    {
        return appliedEffects.Count;
    }
}
