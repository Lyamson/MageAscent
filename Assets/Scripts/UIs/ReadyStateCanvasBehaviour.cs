using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReadyStateCanvasBehaviour : MonoBehaviour
{
    [SerializeField] Toggle readyStateToggle;
    private static Color32 ReadyColor = new(37, 168, 72, 255);
    private static Color32 NotReadyColor = new(194, 38, 17, 255);

    public void OnReadyToggleValueChanged()
    {
        ColorBlock cb = readyStateToggle.colors;
        if (readyStateToggle.isOn)
        {
            cb.normalColor = ReadyColor;
            cb.highlightedColor = ReadyColor;
        }
        else
        {
            cb.normalColor = NotReadyColor;
            cb.highlightedColor = NotReadyColor;
        }
        readyStateToggle.colors = cb;
        GameManager.Singleton.SetReadyState(readyStateToggle.isOn);
    }
}
