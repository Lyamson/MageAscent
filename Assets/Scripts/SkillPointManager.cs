using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillPointManager : MonoBehaviour
{
    [SerializeField] GameObject effectToChoosePrefab;

    public void OnSkillPoint_ButtonClicked()
    {
        Debug.Log("Clicked");
        if (GameManager.Singleton == null)
        {
            Debug.LogError("No GameManager.Singleton for SkillPointManager");
            return;
        }
        gameObject.SetActive(false);
        GameManager.Singleton.chooseEffectScreenBehaviour.Open();
        List<Effect> effectsToChoose = GameManager.Singleton.player.GetEffectsToChoose();
        foreach (Effect effect in effectsToChoose)
        {
            Debug.Log("Effect to choose: " + effect.Title);
            GameManager.Singleton.chooseEffectScreenBehaviour.AddEffectToChoose(effect);
        }
    }
}
