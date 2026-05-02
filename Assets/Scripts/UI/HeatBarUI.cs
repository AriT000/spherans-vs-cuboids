using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class HeatBarUI : MonoBehaviour
{
    private float Width, Height;

    private float MaxHeat;
    public Text heatText, maxHeatText;
    [SerializeField]
    private RectTransform heatBar;

    void Start()
    {
        Width = heatBar.sizeDelta.x;
        Height = heatBar.sizeDelta.y;
    }

    public void SetMaxHeat(float maxHeat)
    {
        MaxHeat = maxHeat;
        maxHeatText.text = maxHeat.ToString();
    }

    public void updateHeat(float heat)
    {
        float newWidth = (heat / MaxHeat) * Width;
        heatBar.sizeDelta = new Vector2(newWidth,Height);
        heatText.text = heat.ToString();
    }
}
