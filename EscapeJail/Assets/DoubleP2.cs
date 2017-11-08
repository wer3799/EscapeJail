﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace weapon
{
    public class DoubleP2 : Weapon
    {
        //리볼버 반동
        private float reBoundValue = 10f;

        public DoubleP2()
        {
            weapontype = WeaponType.DoubleP2;
            bulletSpeed = 10f;
            fireDelay = 0.03f;
            SetAmmo(999);
            needBulletToFire = 1;
            weaponScale = Vector3.one * 1.7f;





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
                bullet.Initialize(firePos + fireDir * 0.6f, fireDir, bulletSpeed, BulletType.PlayerBullet, 0.3f, 1, 0.5f);
                bullet.InitializeImage("white", false);
                bullet.SetEffectName("revolver");
                bullet.SetBloom(true, Color.black);


            }

        }
    }
}