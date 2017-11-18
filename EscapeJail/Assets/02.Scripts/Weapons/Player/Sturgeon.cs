﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace weapon
{
    public class Sturgeon : Weapon
    {
        private float reBoundValue = 40f;

        public Sturgeon()
        {
            weapontype = WeaponType.Sturgeon;
            bulletSpeed = 10f;
            fireDelay = 0.08f;
            SetAmmo(999);
            needBulletToFire = 1;
            damage = 1;
        }

        public override void FireBullet(Vector3 firePos, Vector3 fireDirection)
        {
            if (canFire() == false) return;

            useBullet();
            FireDelayOn();
            PlayFireAnim();

            Bullet bullet = ObjectManager.Instance.bulletPool.GetItem();
            if (bullet != null)
            {

                Vector3 fireDir = fireDirection;
                fireDir.Normalize();
                fireDir = Quaternion.Euler(0f, 0f, Random.Range(-reBoundValue, reBoundValue)) * fireDir;
                bullet.Initialize(firePos + fireDir * 0.3f, fireDir, bulletSpeed, BulletType.PlayerBullet, 1f, damage, 0.6f);
                bullet.InitializeImage("SturgeonBullet", false);
                bullet.SetEffectName("revolver");
                bullet.SetBloom(false);


            }


        }
    }
}