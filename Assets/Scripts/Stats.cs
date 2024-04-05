using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Stats : NetworkBehaviour
{
    public enum StatType
    {
        ATK,
        CD,
        HP,
        SPD
    }
    Dictionary<Stats.StatType, Dictionary<Guid, int>> statsChange;

    public NetworkVariable<float> health = new(writePerm: NetworkVariableWritePermission.Owner);
    public const float BaseMaxHealth = 100f;
    public NetworkVariable<float> maxHealth = new(writePerm: NetworkVariableWritePermission.Owner);
    [SerializeField] int BaseDamage = 10;
    int damageIncrease = 0;
    public int Damage
    {
        get => (int)Math.Floor(BaseDamage * (1 + damageIncrease / 100f));
    }
    float baseSpeed = 6f;
    int speedIncrease = 0;
    public float Speed
    {
        get => baseSpeed * (1 + speedIncrease / 100f);
    }

    public int AllCooldownReduction = 0;

    [SerializeField] private Slider healthBarAboveHead;
    [SerializeField] private Unit unit;

    public override void OnNetworkSpawn()
    {
        MyDebug.Log(this, "Stats.OnNetworkSpawn Callback");
        health.OnValueChanged += OnHealthChanged;
        maxHealth.OnValueChanged += OnMaxHealthChanged;
        if (!IsOwner) return;
        maxHealth.Value = BaseMaxHealth;
        health.Value = maxHealth.Value;
        statsChange = new();
        foreach (StatType statType in Enum.GetValues(typeof(StatType)))
        {
            statsChange.Add(statType, new());
        }
    }

    private void OnHealthChanged(float previousValue, float newValue)
    {
        MyDebug.Log(this, "Stats.OnHealthChanged Callback; newValue: " + newValue);
        UpdateHealthBar(newValue);
        if (newValue == 0f)
        {
            unit.Die();
        }
    }

    private void OnMaxHealthChanged(float previousValue, float newValue)
    {
        MyDebug.Log(this, "Stats.OnMaxHealthChanged Callback; newValue: " + newValue);
        UpdateHealthBar(health.Value);
    }

    public Unit GetUnit()
    {
        return unit;
    }

    private void UpdateHealthBar(float value)
    {
        healthBarAboveHead.value = value / maxHealth.Value;
        if (healthBarAboveHead.value < 1f)
        {
            healthBarAboveHead.gameObject.SetActive(true);
        }
        else
        {
            healthBarAboveHead.gameObject.SetActive(false);
        }
    }

    public void AddStatsChange(StatType stat, Guid id, int value)
    {
        MyDebug.Log(this, "AddStatsChange Callback");
        foreach (var item in statsChange)
        {
            Debug.Log($"Key: {item.Key}; Value: {item.Value}");
        }
        statsChange[stat].Add(id, value);
        Debug.Log("StatChange Added");
        UpdateStat(stat, value);
    }

    void UpdateStat(StatType stat, int value)
    {
        Debug.Log($"Updating stat \"{stat}\" with value = {value}");
        switch (stat)
        {
            case StatType.ATK:
                damageIncrease += value;
                break;
            case StatType.CD:
                AllCooldownReduction += value;
                break;
            case StatType.HP:
                maxHealth.Value += BaseMaxHealth * value / 100f;
                break;
            case StatType.SPD:
                speedIncrease += value;
                break;
            default:
                throw new Exception("Unexpected stat.");
        }
    }

    internal void RemoveStatsChange(ValuedEffect effect)
    {
        foreach (var item in effect.Values)
        {
            statsChange[item.Key].Remove(effect.Id);
        }
    }
}