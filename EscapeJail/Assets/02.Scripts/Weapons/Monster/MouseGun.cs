﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace weapon
{
    public class MouseGun : Weapon
    {
        private float reBoundValue = 5f;
        public MouseGun()
        {
            weapontype = WeaponType.MouseGun;
            bulletSpeed = 6.5f;
            weaponScale = Vector3.one * 1.6f;
        }
        public override void FireBullet(Vector3 firePos, Vector3 fireDirection)
        {


            Bullet bullet = ObjectManager.Instance.bulletPool.GetItem();
            if (bullet != null)
            {          
                Vector3 PlayerPos = GamePlayerManager.Instance.player.transform.position;
                Vector3 fireDIr = PlayerPos - firePos;
                fireDIr = Quaternion.Euler(0f, 0f, Random.Range(-reBoundValue, reBoundValue)) * fireDIr;
                bullet.Initialize(firePos, fireDIr.normalized, bulletSpeed, BulletType.EnemyBullet,0.5f);
                bullet.InitializeImage("white", false);
                bullet.SetEffectName("revolver");
             

            }

            PlayFireAnim();
            SoundManager.Instance.PlaySoundEffect("pistol5");


        }
    }
}