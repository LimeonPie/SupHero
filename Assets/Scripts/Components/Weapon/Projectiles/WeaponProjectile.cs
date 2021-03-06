﻿using UnityEngine;
using SupHero.Components.Character;
using SupHero.Model;

namespace SupHero.Components.Weapon {
    public class WeaponProjectile : BaseProjectile {
        
        public WeaponController gun;
        public GameObject hitEffect;

        public override void Start() {
            base.Start();
            
        }

        // If trigger
        /*void OnTriggerEnter(Collider other) {
            GameObject target = other.gameObject;
            Debug.Log("Trigger: Hit " + target);
            if (target.CompareTag(Tags.Player)) {
                gun.dealDamageTo(target.GetComponent<PlayerController>());
            }
            if (target.CompareTag(Tags.Cover)) {
                target.GetComponent<CoverController>().takeDamage(gun.weapon.damage);
            }
            if (target.CompareTag(Tags.Shield)) {
                gun.dealDamageTo(target.GetComponent<PlayerController>());
            }
            Stop();
        }*/

        // If collider
        void OnCollisionEnter(Collision collision) {
            GameObject target = collision.gameObject;
            Debug.Log("Collision: Hit " + target);
            if (target.CompareTag(Tags.Player)) {
                gun.dealDamageTo(target.GetComponent<PlayerController>());
            }
            if (target.CompareTag(Tags.Cover)) {
                target.GetComponent<BaseDestructable>().takeDamage(gun.weapon.damage);
            }
            if (target.CompareTag(Tags.Shield)) {
                gun.dealDamageTo(target.GetComponent<Shield>().player);
            }
            makeHit(collision.contacts[0]);
            Stop();
        }

        protected virtual void makeHit(ContactPoint contact) {
            Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, contact.normal);
            Vector3 pos = contact.point;
            Instantiate(hitEffect, pos, rot);
        }

        public override void Update() {
            base.Update();
            if (launched && distanceTraveled >= (2 * gun.weapon.range)) {
                Stop();
            }
        }

        public void Launch(Vector3 start, Vector3 direction) {
            speed = gun.weapon.projectile.speed;
            base.Launch(start, direction, speed);
        }

        public override void Stop() {
            base.Stop();
            if (gun != null) gun.returnProjectile(this);
            else Destroy(gameObject);
        }
    }
}
