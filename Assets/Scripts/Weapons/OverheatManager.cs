/***************************************************************
*file: OverheatManager.cs
*author: Nathan Rinon
*class: CS 4700 ? Game Development
*assignment: program 1
*date last modified: 4/23/2026
*
*purpose: This script controls the overheat mechanic. It is attatched to the weapon object
*
****************************************************************/

using System.Threading;
using UnityEngine;

public class OverheatManager : MonoBehaviour
{
    [SerializeField] private float maxHeat;
    [SerializeField] private float heat;
    [SerializeField] private float heatDamage;
    [SerializeField] private float heatRegenerationRate;
    [SerializeField] private float refreshRate;

    public float MaxHeat { get => maxHeat; set => maxHeat = value; }
    public float Heat { get => heat; set => heat = value; }
    public float HeatDamage { get => heatDamage; set => heatDamage = value; }
    public float HeatRegenerationRate { get => heatRegenerationRate; set => heatRegenerationRate = value; }
    public float RefreshRate { get => refreshRate; set => refreshRate = value; }

    private float time;

    private bool buffer; //controls timing



    //purpose: checks to see if weapon is overheated
    public bool isOverHeat()
    {
        if(heat <=0)
        {
            print("Weapon is overheated!");
            return true;
            
        }
        return false;
    }

    //purpose: Regenerates heat based on time delta, if it reaches the refresh rate, resets the buffer
    public void regenerateHeat(float deltaTime)
    {
        
        while (time < refreshRate && buffer == false)
        {
            heat += heatRegenerationRate;

            if (heat > maxHeat)
            {
                heat = maxHeat;
            }
            buffer = true;
        }
        time += deltaTime;
        if(time > refreshRate && buffer == true)
        {
            resetTimer();
        }
    }

    //spends heat of the weapon
    public void spendHeat()
    {
        if(!isOverHeat())
        {
          heat -= heatDamage;
        }
        else
        {
            heat = 0;
        }

    }

    //purpose: rest the timer to control the timing of the regeneration rate
    private void resetTimer()
    {
        buffer = false;
        time = 0;
    }

}
