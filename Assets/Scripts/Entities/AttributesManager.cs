/***************************************************************
*file: AttributeManager.cs
*author: Nathan Rinon
*class: CS 4700 – Game Development
*assignment: program 1
*date last modified: 4/25/2026
*
*purpose: This script controls the attributes of player and enemy components via particle collisions.
*
****************************************************************/


using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
namespace Assets.Scripts.Entities
{
    public class AttributesManager : MonoBehaviour
    {
        
        [SerializeField] GameOverScreen GameOverScreen;
        [SerializeField] private int damagePower;
        [SerializeField] private int health;
        [SerializeField] private EntityMaterials entityMaterials;

        public int Health { get => health; set => health = value; }


        //Purpose: Damages the current game object health. If health reaches 0, game object dies.
        private void takeDamage(int damage)
        {
            health -= damage;
            //stops it to resolve conflict 
            HealthBarUI healthBarUI = GetComponent<HealthBarUI>();
            if (healthBarUI != null && gameObject.CompareTag("Player"))
            {
                healthBarUI.UpdateHealthBar(health);
            }

            
            StartCoroutine(playDamageAnimation(entityMaterials));
            StopCoroutine(playDamageAnimation(entityMaterials)); // stops to prevent material collision problems
         

            if (gameObject.CompareTag("Player") && health <= 0)
            {
                GameOverScreen.Setup();
            }
            if (!gameObject.CompareTag("Player") && health <= 0)
            {
                Die();
            }
        }


        //purpose: Plays couroutine to play damage vfx asychronously
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

        //Purpose: Takes a game object's attribute's manager and damages its health.
        private void damage(GameObject gameObject)
        {
            AttributesManager target = gameObject.GetComponent<AttributesManager>();
            target.takeDamage(damagePower);

        }
        
        private int getDamagePower()
        {
            return this.damagePower;
        }    


        //Purpose: removes game object from scene.

        private void Die()
        {
            Destroy(gameObject);
        }

        //Purpose: Upon coming across a particle, the game object gets the atributes and loses health. If the game object doesn't have the attribute manager,
        /// it checks it parents, if not, returns a nullreceptionexception.
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