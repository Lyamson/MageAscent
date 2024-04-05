using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerStats : Stats
{
    [NonSerialized] public Text healthTextUI;
    [NonSerialized] public Slider healthBarUI;
    [NonSerialized] public Slider manaBarUI;
    [NonSerialized] public Text goldTextUI;
    [NonSerialized] public Text expTextUI;

    [NonSerialized] public int gold;
    [NonSerialized] private int exp;
    public int Exp
    {
        get => exp;
    }

    event EventHandler GoldHandler;
    public event EventHandler ExpHandler;

    private void Start()
    {
        if (!IsOwner) return;
        health.OnValueChanged += OnHealthChanged;
        maxHealth.OnValueChanged += OnMaxHealthChanged;
        GoldHandler += UpdateGoldUI;
        ExpHandler += UpdateExpUI;
    }

    public void SetCoins(int value)
    {
        gold = value;
        OnGoldChanged(this, new());
    }

    public void SetExp(int value)
    {
        exp = value;
        OnExpChanged(this, new());
    }

    public void AddExp(int value)
    {
        SetExp(exp + value);
    }

    public void AddCoins(int value)
    {
        SetCoins(gold + value);
    }

    internal void ReduceCoins(int value)
    {
        SetCoins(gold - value);
    }

    private void UpdateGoldUI(object sender, EventArgs e) 
    {
        goldTextUI.text = gold.ToString();
    }

    private void UpdateExpUI(object sender, EventArgs e)
    {
        MyDebug.Log(this, "UpdateExpUI Callback; Exp: " + exp);

        expTextUI.text = exp.ToString();

        manaBarUI.value = exp / 100f;
    }

    private void OnHealthChanged(float previousValue, float newValue)
    {
        MyDebug.Log(this, "PlayerStats.OnHealthChanged Callback; newValue: " + newValue);
        UpdateHealthUI(newValue);
    }

    private void OnMaxHealthChanged(float previousValue, float newValue)
    {
        MyDebug.Log(this, "PlayerStats.OnHealthChanged Callback; newValue: " + newValue);
        UpdateHealthUI(health.Value);
    }

    public void UpdateHealthUI()
    {
        healthTextUI.text = $"{health.Value} / {maxHealth.Value}";

        healthBarUI.value = health.Value / maxHealth.Value;
    }

    public void UpdateHealthUI(float value)
    {
        healthTextUI.text = $"{value} / {maxHealth.Value}";

        healthBarUI.value = value / maxHealth.Value;
    }

    protected virtual void OnGoldChanged(object sender, EventArgs e)
    {
        MyDebug.Log(this, "OnGoldChanged Callback");
        GoldHandler?.Invoke(sender, e);
    }

    protected virtual void OnExpChanged(object sender, EventArgs e)
    {
        MyDebug.Log(this, "OnExpChanged Callback");
        ExpHandler?.Invoke(sender, e);
    }
}