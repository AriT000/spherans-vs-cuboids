using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class ArmorBarUI : MonoBehaviour
{
    public float Armor, MaxArmor, Width, Height;
    public Text armorText, MaxarmorText;
    [SerializeField]
    private RectTransform ArmorBar;

    public void SetMaxArmor(float maxArmor)
    {
        MaxArmor = maxArmor;
        MaxarmorText.text = MaxArmor.ToString();
    }

    public void SetArmor(float armor)
    {
        Armor = armor;
        float newWidth = (Armor / MaxArmor) * Width;
        ArmorBar.sizeDelta = new Vector2(newWidth,Height);
        armorText.text = Armor.ToString();
    }
}
