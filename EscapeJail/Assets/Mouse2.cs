﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse2 : MonsterBase
{


    public new void SetUpMonsterAttribute()
    {
        SetHp(10);
        nearestAcessDistance = 5f;
        SetWeapon();
    }

    private void SetWeapon()
    {
        nowWeapon.SetWeapon(new MouseGun());

    }

    // Use this for initialization
    private new void Start()
    {
        base.Start();
        StartCoroutine(TempFireRoutine());
    }

    private new void Awake()
    {
        base.Awake();      
        SetUpMonsterAttribute();
    }

    // Update is called once per frame
    private void Update()
    {
        ActionCheck();
        if (isActionStart == false) return; 
        MoveToTarget();
        RotateWeapon();
   
    }

    IEnumerator TempFireRoutine()
    {
        while (true)
        {
            FireWeapon();
            yield return new WaitForSeconds(0.3f);
        }
    }

    protected void RotateWeapon()
    {
        float angle = MyUtils.GetAngle( this.transform.position, target.position);
        if (weaponPosit != null)
            weaponPosit.rotation = Quaternion.Euler(0f, 0f, angle);
              
        //flip
        if ((angle >= 0f && angle <= 90) ||
              angle >= 270f && angle <= 360)
        {
            if (nowWeapon != null)
                nowWeapon.FlipWeapon(false);
        }
        else
        {
            if (nowWeapon != null)
                nowWeapon.FlipWeapon(true);
        }

    }

    private void FireWeapon()
    {
        if (nowWeapon != null)
            nowWeapon.FireBullet(this.transform.position);
    }


}
