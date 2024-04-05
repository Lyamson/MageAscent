using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : Unit
{
    [SerializeField] private GameObject gameManagerPrefab;
    [SerializeField] public Camera playerCamera;
    [SerializeField] Text readyText;
    [SerializeField] public SkillsManager skillsManager;

    private static Color32 ReadyColor = new(37, 168, 72, 255);

    private static Color32 NotReadyColor = new(194, 38, 17, 255);

    private const string ReadyTextString = "Ready";
    private const string NotReadyTextString = "Not Ready";

    private GameObject canvasInstance;
    private Camera previousCamera;

    public NetworkVariable<bool> isReady = new(writePerm: NetworkVariableWritePermission.Owner);

    private void Awake()
    {
        MyDebug.Log(this, "Player.Awake Callback");
        team = TeamLogic.Team.PLAYER;
        NetworkManager.Singleton.SceneManager.OnSceneEvent += OnSceneEvent;
    }

    public override void OnNetworkSpawn()
    {
        MyDebug.Log(this, "Player.OnNetworkSpawn Callback");
        base.OnNetworkSpawn();
        isReady.OnValueChanged += OnReadyStateChanged;
        UpdateReadyStateInfo();
        if (IsLocalPlayer)
        {
            GameObject gameManagerObject = Instantiate(gameManagerPrefab, Vector2.zero, Quaternion.identity);

            DontDestroyOnLoad(gameManagerObject);

            GameManager.Singleton.player = this;
            ((PlayerStats)stats).healthTextUI = GameManager.Singleton.healthTextUI;
            ((PlayerStats)stats).healthBarUI = GameManager.Singleton.healthBarUI;
            ((PlayerStats)stats).manaBarUI = GameManager.Singleton.manaBarUI;
            ((PlayerStats)stats).goldTextUI = GameManager.Singleton.goldTextUI;
            ((PlayerStats)stats).expTextUI = GameManager.Singleton.expTextUI;
        }
    }

    public bool TryPayPrice(int price)
    {
        if (((PlayerStats)stats).gold >= price)
        {
            ((PlayerStats)stats).ReduceCoins(price);
            return true;
        }
        return false;
    }

    public bool HasSkillPoints() => skillsManager.SkillPoints > 0;

    private void Start()
    {
        MyDebug.Log(this, "Player.Start Callback");
        if (IsLocalPlayer)
        {
            ((PlayerStats)stats).SetCoins(0);
            ((PlayerStats)stats).UpdateHealthUI();

            //previousCamera = Camera.main;
            //previousCamera.gameObject.SetActive(false);
            playerCamera.gameObject.SetActive(true);
            skillsManager.LearnSpell<FireballSpell>();
            ((PlayerStats)stats).ExpHandler += OnExpChanged;
        }
        Death += OnPlayerDeath;
        
        GameManager.Singleton.AddPlayer(OwnerClientId, this);
    }

    private void OnPlayerDeath(object sender, EventArgs e)
    {
        character.SetActive(false);
        if (!IsServer) return;
        try
        {
            foreach (Player player in GameManager.Singleton.players.Values)
            {
                if (player.IsAlive) return;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Cant check for alive players: " + ex.Message);
            return;
        }

        try
        {
            foreach (Player player in GameManager.Singleton.players.Values)
            {
                player.SetPositionClientRpc(new Vector2(0f, 0f));
            }
            NetworkManager.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
        }
        catch (Exception ex)
        {
            Debug.LogError("Cant start the game: " + ex.Message);
        }
    }

    private void OnExpChanged(object sender, EventArgs e)
    {
        PlayerStats playerStats = sender as PlayerStats;
        try
        {
            if (playerStats.Exp >= 100)
            {
                int exp = playerStats.Exp;
                playerStats.SetExp(playerStats.Exp - 100);
                for (int count = 0; count < exp / 100; count++)
                {
                    LevelUp();
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"OnExpChanged: {ex.Message}");
        }
    }

    private void LevelUp()
    {
        skillsManager.AddSkillPoint();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (!IsOwner) return;
        previousCamera?.gameObject.SetActive(true);
        Destroy(canvasInstance);
    }

    public override void OnNetworkDespawn()
    {
        MyDebug.Log(this, "OnNetworkDespawn Callback");
        base.OnNetworkDespawn();

        if (isReady.Value)
        {
            if (GameManager.Singleton.ReadyCount != ushort.MinValue)
                GameManager.Singleton.ReadyCount -= 1;
        }
        GameManager.Singleton.RemovePlayer(OwnerClientId);
    }

    public override int GetEnemyHitBoxLayerMask()
    {
        return 1 << 7;
    }

    private void OnReadyStateChanged(bool previousValue, bool newValue)
    {
        UpdateReadyStateInfo();
        if (newValue)
        {
            if (GameManager.Singleton.ReadyCount != ushort.MaxValue)
                GameManager.Singleton.ReadyCount += 1;
        }
        else
        {
            if (GameManager.Singleton.ReadyCount != ushort.MinValue)
                GameManager.Singleton.ReadyCount -= 1;
        }
        if (IsOwner)
        {
            GameManager.Singleton.SetReadyState(newValue);
        }
        if (IsServer)
        {
            if (GameManager.Singleton.players.Count == GameManager.Singleton.ReadyCount)
            {
                try
                {
                    foreach (Player player in GameManager.Singleton.players.Values)
                    {
                        player.SetPositionClientRpc(new Vector2(8f, 8f));
                    }
                    NetworkManager.SceneManager.LoadScene("World", LoadSceneMode.Single);
                }
                catch (Exception ex)
                {
                    Debug.LogError("Cant start the game: " + ex.Message);
                }
            }
        }
    }

    private void OnSceneEvent(SceneEvent sceneEvent)
    {
        MyDebug.Log(this, $"Event type: {sceneEvent.SceneEventType} ; OwnerClientId: {OwnerClientId}");
        switch (sceneEvent.SceneEventType)
        {
            case SceneEventType.LoadComplete:
                if (IsOwner)
                {
                    isReady.Value = false;
                    if (SceneManager.GetActiveScene().name == "Lobby")
                    {
                        GameManager.Singleton.effectsManager.ClearEffects();
                        Debug.Log("Effects count: " + GameManager.Singleton.effectsManager.Count());
                        ((PlayerStats)stats).health.Value = Stats.BaseMaxHealth;
                        ((PlayerStats)stats).maxHealth.Value = Stats.BaseMaxHealth;

                        ((PlayerStats)stats).SetExp(0);
                        skillsManager.Reset();
                    }
                }
                if (SceneManager.GetActiveScene().name == "Lobby")
                {
                    character.SetActive(true);
                    Revive();
                }
                UpdateReadyStateInfo();
                break;
            default:
                break;
        }
    }

    private void Revive()
    {
        IsAlive = true;
    }

    private void UpdateReadyStateInfo()
    {
        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            if (isReady.Value)
            {
                readyText.text = ReadyTextString;
                readyText.color = ReadyColor;
            }
            else
            {
                readyText.text = NotReadyTextString;
                readyText.color = NotReadyColor;
            }
            GameManager.Singleton?.readyToggle?.gameObject.SetActive(true);
            readyText.gameObject.SetActive(true);
        }
        else
        {
            GameManager.Singleton?.readyToggle?.gameObject.SetActive(false);
            readyText.gameObject.SetActive(false);
        }
    }

    public int GetAttackDamage()
    {
        return ((PlayerStats)stats).Damage;
    }

    public List<Effect> GetEffectsToChoose()
    {
        List<Effect> effects = new();
        System.Random random = new();
        for (int i = 0; i < 3; i++)
        {
            Effect effect = GameManager.Singleton.effectsManager.AllEffects[random.Next(0, GameManager.Singleton.effectsManager.AllEffects.Count)];
            effects.Add(effect.Clone());
        }

        return effects;
    }

    public void AddStatsChange(Stats.StatType key, Guid id, int value)
    {
        stats.AddStatsChange(key, id, value);
    }

    public void RemoveStatsChange(ValuedEffect effect)
    {
        stats.RemoveStatsChange(effect);
    }
}