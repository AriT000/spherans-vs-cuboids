using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Entities
{
    public class AttributesManager : MonoBehaviour
    {

        [SerializeField] private int damagePower;
        [SerializeField] private int health;

        private void takeDamage(int damage)
        {
            health -= damage;
            if (health < 0)
            {
                Die();
            }
        }

        private void damage(GameObject gameObject)
        {
            AttributesManager target = gameObject.GetComponent<AttributesManager>();
            target.takeDamage(damagePower);

        }
        
        private int getDamagePower()
        {
            return this.damagePower;
        }    



        private int getHealth()
        {
            return this.health;
        }


        private void Die()
        {
            Destroy(gameObject);
        }

        private void OnParticleCollision(GameObject other)
        {
            AttributesManager enemyAttributes = other.GetComponentInParent<AttributesManager>();
            if (enemyAttributes == null)
            {
                enemyAttributes = other.GetComponent<AttributesManager>();
            }
            
            try
            {
                int damageReceived = enemyAttributes.getDamagePower();
                print(gameObject.name + ": " + health);
                takeDamage(damageReceived);
            }
            catch (NullReferenceException e)
            {
                print("the shooter's Attributes is null, the current object that is hitting this object" + gameObject.name + "is " + other.name);
                print(e.Message);
            }
            

        }





    }
}