/***************************************************************
*file: AttributeManager.cs
*author: Nathan Rinon
*class: CS 4700 – Game Development
*assignment: program 1
*date last modified: 4/22/2026
*
*purpose: This script controls the attributes of player and enemy components via particle collisions.
*
****************************************************************/


using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace Assets.Scripts.Entities
{
    public class AttributesManager : MonoBehaviour
    {

        [SerializeField] private int damagePower;
        [SerializeField] private int health;
        [SerializeField] private EntityMaterials entityMaterials;
        [SerializeField] private Color damageColor;

        private void takeDamage(int damage)
        {
            health -= damage;
            //stops it to resolve conflict 
            StopCoroutine(playDamageAnimation(entityMaterials));
            StartCoroutine(playDamageAnimation(entityMaterials));
            if (health < 0)
            {
                Die();
            }
        }


        //purpose: plays couroutine to play damage vfx
        IEnumerator playDamageAnimation(EntityMaterials entityMaterials)
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            Material flashMaterial = entityMaterials.hitEffectSprite2D;
            Material defaultSprite = entityMaterials.defaultSprite2D;
            float flashLength = flashMaterial.GetFloat("_flashLength");
            spriteRenderer.material = flashMaterial;
            yield return new WaitForSeconds(flashLength);
            spriteRenderer.material = defaultSprite;
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