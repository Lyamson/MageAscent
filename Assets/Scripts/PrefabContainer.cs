using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabContainer : MonoBehaviour
{
    public static PrefabContainer Singleton;

    [SerializeField] Sprite ballSpellSprite;
    [SerializeField] GameObject friendlyBallSpellPrefab;
    [SerializeField] GameObject enemyBallSpellPrefab;

    [SerializeField] Sprite fireballSpellSprite;
    [SerializeField] GameObject friendlyFireballSpellPrefab;

    [SerializeField] Sprite iceSpellSprite;
    [SerializeField] GameObject friendlyIceSpellPrefab;

    [SerializeField] GameObject enemySlimePrefab;

    private void Start()
    {
        if (Singleton == null)
        {
            Singleton = this;
            DontDestroyOnLoad(Singleton);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public GameObject GetPrefab(Enemy.EnemyType enemyType)
    {
        switch (enemyType)
        {
            case Enemy.EnemyType.SLIME:
                return enemySlimePrefab;
            default:
                Debug.LogError($"No such enemy type ({enemyType})");
                return null;
        }
    }

    public GameObject GetPrefab(Spell.SpellType spellType, bool friendlyPrefab)
    {
        switch (spellType)
        {
            case Spell.SpellType.NONE:
                return friendlyPrefab ? friendlyBallSpellPrefab : enemyBallSpellPrefab;
            case Spell.SpellType.BALL:
                return friendlyPrefab ? friendlyBallSpellPrefab : enemyBallSpellPrefab;
            case Spell.SpellType.FIREBALL:
                return friendlyFireballSpellPrefab;
            case Spell.SpellType.ICE:
                return friendlyIceSpellPrefab;
            default:
                return friendlyPrefab ? friendlyBallSpellPrefab : enemyBallSpellPrefab;
        }
    }

    public Sprite GetSprite(Spell.SpellType spellType)
    {
        switch (spellType)
        {
            case Spell.SpellType.NONE:
                return ballSpellSprite;
            case Spell.SpellType.BALL:
                return ballSpellSprite;
            case Spell.SpellType.FIREBALL:
                return fireballSpellSprite;
            case Spell.SpellType.ICE:
                return iceSpellSprite;
            default:
                return ballSpellSprite;
        }
    }
}
