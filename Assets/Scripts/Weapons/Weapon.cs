/***************************************************************
*file: WeaponController.cs
*author: Nathan Rinon
*class: CS 4700 ? Game Development
*assignment: program 1
*date last modified: 4/22/2026
*
*purpose: This script controls the abstract class Weapon and shows what every weapon has in order to use the factory design pattern.
*
****************************************************************/


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