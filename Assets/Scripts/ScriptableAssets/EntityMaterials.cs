/***************************************************************
*file: OverheatManager.cs
*author: Nathan Rinon
*class: CS 4700 ? Game Development
*assignment: program 1
*date last modified: 4/25/2026
*
*purpose: This script serves as a data container PlayerAttributes. It also serves as a function to store material during runtime instead as a game object
* in order to reduce memory usage.
****************************************************************/

using UnityEngine;

[CreateAssetMenu(fileName = "EntityMaterials", menuName = "Scriptable Objects/EntityMaterials")]
public class EntityMaterials : ScriptableObject
{
    public Material defaultSprite2D;
    public Material hitEffectSprite2D;
}
