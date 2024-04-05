using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseEffectScreenBehaviour : MonoBehaviour
{
    [SerializeField] GameObject effectToChoosePrefab;
    List<GameObject> effectCards = new();

    public void AddEffectToChoose(Effect effect)
    {
        GameObject effectCard = Instantiate(effectToChoosePrefab, transform);
        effectCard.GetComponent<EffectCardBehaviour>().Effect = effect;
        effectCards.Add(effectCard);
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Close and clear this screen
    /// </summary>
    public void Close()
    {
        while (effectCards.Count > 0)
        {
            Destroy(effectCards[0]);
            effectCards.RemoveAt(0);
        }
        gameObject.SetActive(false);
    }
}
