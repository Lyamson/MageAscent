using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectCardBehaviour : MonoBehaviour
{
    [SerializeField] private Text titleText;
    [SerializeField] private Text descriptionText;
    [SerializeField] private Button button;

    private Effect effect;
    public Effect Effect
    {
        get => effect;
        set
        {
            if (effect != value)
            {
                effect = value;
                titleText.text = effect.Title;
                descriptionText.text = effect.Description;
            }
        }
    }

    private void Start()
    {
        button.onClick.AddListener(ChooseEffect);
    }

    public void ChooseEffect()
    {
        if (Effect == null)
        {
            Debug.LogError("No effect chosen for card");
            return;
        }
        GameManager.Singleton.chooseEffectScreenBehaviour.Close();
        if (GameManager.Singleton.player.skillsManager.RemoveSkillPoints(effect.PointsCost))
        {
            Debug.Log("Chosen effect: " + effect.Title);
            Debug.Log("Stat before: " + GameManager.Singleton.player.GetAttackDamage());
            GameManager.Singleton.effectsManager.ApplyEffect(effect);
            Debug.Log("Stat after: " + GameManager.Singleton.player.GetAttackDamage());
        }
        if (GameManager.Singleton.player.HasSkillPoints())
        {
            GameManager.Singleton.ShowSkillPointButton();
        }
    }
}