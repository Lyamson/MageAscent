using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Singleton;
    public EffectsManager effectsManager;

    [NonSerialized] public Player player;
    [SerializeField] public Text healthTextUI;
    [SerializeField] public Slider healthBarUI;
    [SerializeField] public Slider manaBarUI;
    [SerializeField] public Text goldTextUI;
    [SerializeField] public Text expTextUI;
    [SerializeField] public Toggle readyToggle;
    [SerializeField] Button skillPointButton;
    [SerializeField] Text toggleText;
    [SerializeField] QuickSkillsBehaviour quickSkillsBehaviour;
    [SerializeField] public ChooseEffectScreenBehaviour chooseEffectScreenBehaviour;
    [SerializeField] Canvas shopCanvas;


    public Dictionary<ulong, Player> players = new();
    ushort readyCount = 0;

    public ushort ReadyCount
    {
        get => readyCount;
        set
        {
            readyCount = value;
            UpdateReadyCount();
        }
    }

    public void AddPlayer(ulong id, Player player)
    {
        try
        {
            players.Add(id, player);
            UpdateReadyCount();
        }
        catch
        {
            print("ERRORROR AddPlayer");
        }
    }

    public void RemovePlayer(ulong id)
    {
        try
        {
            players.Remove(id);
            UpdateReadyCount();
        }
        catch
        {
            print("ERRORROR RemovePlayer");
        }
    }

    public void SetCooldown(byte slot, float currentCooldown)
    {
        quickSkillsBehaviour.SetCooldown(slot, currentCooldown);
    }

    private void Awake()
    {
        if (Singleton != null)
        {
            Destroy(this);
        }
        else
        {
            Singleton = this;
            skillPointButton.gameObject.SetActive(false);
            effectsManager = new();
            NetworkManager.Singleton.SceneManager.OnSceneEvent += OnSceneEvent;
            DontDestroyOnLoad(this);
        }
    }

    public void SetReadyState(bool isReady)
    {
        player.isReady.Value = isReady;
        if (readyToggle.isOn != isReady) readyToggle.isOn = isReady;
    }

    internal void UpdateReadyCount()
    {
        try
        {
            toggleText.text = $"{readyCount} / {players.Count}";
        }
        catch
        {
            print("ERRORROR UpdateReadyCount");
        }
    }

    public void SetQuickSpell(Spell.SpellType spellType, byte slot)
    {
        quickSkillsBehaviour.SetSpell(spellType, slot);
    }

    public void SetActiveSlot(byte slot)
    {
        quickSkillsBehaviour.SetActiveSlot(slot);
    }

    public void ShowSkillPointButton()
    {
        if (!chooseEffectScreenBehaviour.gameObject.activeInHierarchy)
        {
            skillPointButton.gameObject.SetActive(true);
        }
    }

    public void HideSkillPointButton()
    {
        skillPointButton.gameObject.SetActive(false);
    }

    private void OnSceneEvent(SceneEvent sceneEvent)
    {
        switch (sceneEvent.SceneEventType)
        {
            case SceneEventType.LoadComplete:
                if (SceneManager.GetActiveScene().name == "Lobby")
                {
                    shopCanvas.gameObject.SetActive(true);
                }
                else
                {
                    shopCanvas.gameObject.SetActive(false);
                }
                break;
            default:
                break;
        }
    }
}