﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Last4 : MonsterBase
{
    private enum Last4State
    {
        Normal,
        Avoid,
        Transform
    }

    private Last4State scientistState = Last4State.Normal;

    private Turret myTurret = null;


    //이게 다차면 타워 생성됨
    private int buildCountMax=3;
    private int buildCount=0;
    private bool nowBuildTurret = false;
    

    //애니메이션에 연결
    public void BuildCountUp()
    {
        buildCount++;
        if (buildCount >= buildCountMax)
        {
           BuildEnd();
        }
    }

    public void BuildStart()
    {
        nowBuildTurret = true;

        if (animator != null)
            animator.SetTrigger("BuildTurretStart");
    }

    public void BuildEnd()
    {
        nowBuildTurret = false;

        //터렛 생성
        Turret buildTurret = ObjectManager.Instance.turretPool.GetItem();
        if (buildTurret != null)
        {
            buildTurret.Initialize(this.transform.position,BulletType.EnemyBullet, 10);
            myTurret = buildTurret;
        }
        //터렛 생성
        buildCount = 0;
        if (animator != null)
            animator.SetTrigger("BuildTurretEnd");
    }

    // Use this for initialization
    private new void Start()
    {
        base.Start();
        SetUpMonsterAttribute();
    }

    public new void SetUpMonsterAttribute()
    {
        monsterName = MonsterName.Last4;
        SetHp(10);
        nearestAcessDistance = 5f;
     
    }
    public override void ResetMonster()
    {
        base.ResetMonster();
        nowBuildTurret = false;
        StartCoroutine(AvoidRoutine());
        myTurret = null;

    }


    private IEnumerator AvoidRoutine()
    {
        BuildStart();

        float moveDirectionChangeTime = 1.5f;
        float count = 0f;
        Vector3 moveDir = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)) * Vector3.right;

        while (true)
        {
            //재건축
            if (myTurret != null&& nowBuildTurret==false)
            {
                if (myTurret.gameObject.activeSelf == false)
                {
                    BuildStart();
                }
            }

            this.moveDir = moveDir;

            if (rb != null && nowBuildTurret == false)
            {
                rb.velocity = moveDir.normalized * moveSpeed;
                SetAnimation(MonsterState.Walk);
                FlipCharacterByMoveDir();
            }
            else if (nowBuildTurret == true)
                rb.velocity = Vector3.zero;           
     

            count += Time.deltaTime;
            if(count>= moveDirectionChangeTime)
            {
                count = 0f;

                //다음 랜덤 방향 세팅
                if (nowBuildTurret == false)
                    moveDir = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)) * Vector3.right;
            }

            yield return null;
        }

    }



}