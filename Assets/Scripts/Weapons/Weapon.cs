using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public abstract class Weapon
{
    private Color _color;

    public abstract GameObject GetWeapon();

    public abstract void SetWeaponObject(GameObject obj);

    public abstract void Fire();

    public void setColor(Color color)
    {
        _color = color;
        Debug.Log(_color);
    }

    public Color getColor()
    {
        return _color;
    }

    public void applyCrosshairColor()
    {
        SpriteRenderer sr = GetWeapon().GetComponentInParent<SpriteRenderer>();
        sr.color = _color;
    }



}