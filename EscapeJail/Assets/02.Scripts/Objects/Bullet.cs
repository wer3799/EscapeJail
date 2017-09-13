﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//레이어이름과같음
public enum BulletType
{
    PlayerBullet,
    EnemyBullet
}

public enum ExplosionType
{
    single,
    multiple
}


[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class Bullet : MonoBehaviour
{
    private int power = 0;
    private BulletType bulletType;
    private Rigidbody2D rb;
    private string effectName = "revolver";
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Sprite defaultSprite;
    private float lifeTime = 1.0f;
    private float effectsize = 0f;
    private float explosionRadius = 1f;
    private ExplosionType explosionType;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
            defaultSprite = spriteRenderer.sprite;


    }

    public void Initialize(Vector3 startPos, Vector3 moveDir, float moveSpeed, BulletType bulletType, float bulletScale = 1f, int power = 1,float lifeTime = 100f)
    {
        //위치
        this.transform.position = new Vector3(startPos.x, startPos.y, 0f);

        //이동
        if (rb != null)
            rb.velocity = moveDir.normalized * moveSpeed;

        //피아식별
        this.bulletType = bulletType;
        //레이어
        SetLayer(bulletType);
        //크기
        this.transform.localScale = Vector3.one * bulletScale;
        //파워
        this.power = power;

        //애니메이션불렛 유무
        if (animator != null)
            animator.runtimeAnimatorController = null;

        //지속시간
        if (lifeTime != 100f)
        {
            Invoke("BulletDestroy", lifeTime);
        }

        //폭발 타입
        explosionType = ExplosionType.single;
    }

    public void SetExplosion(float radius)
    {
        explosionType = ExplosionType.multiple;
        explosionRadius = radius;
    }

    
    public void SetEffectName(string effectName, float effectsize = 1f)
    {
        this.effectName = effectName;
        this.effectsize = effectsize;
    }

    public void InitializeImage(string bulletImageName, bool isAnimBullet)
    {
        if (isAnimBullet == true && animator != null)
        {
            RuntimeAnimatorController animController = ObjectManager.LoadGameObject(string.Format("Animators/Bullet/{0}", bulletImageName)) as RuntimeAnimatorController;
            if (animController != null)
            {
                animator.runtimeAnimatorController = animController;
            }
        }
        else if (isAnimBullet == false && spriteRenderer != null)
        {
            Sprite sprite = ObjectManager.LoadGameObject(string.Format("Sprites/Bullet/{0}", bulletImageName)) as Sprite;
            if (sprite != null)
                spriteRenderer.sprite = sprite;
            else if (sprite == null)
                spriteRenderer.sprite = defaultSprite;

        }


    }

    public void SetBulletLifeTime(float lifeTime)
    {
        Invoke("BulletDestroy", lifeTime);
    }

    private void SetLayer(BulletType bulletType)
    {
        this.gameObject.layer = LayerMask.NameToLayer(bulletType.ToString());
    }

 

    private void SingleTargetDamage(Collider2D collision)
    {
        //충돌여부는 layer collision matrix로 분리해놓음
        if (collision.gameObject.CompareTag("Enemy") == true || collision.gameObject.CompareTag("Player"))
        {
            CharacterInfo characterInfo = collision.gameObject.GetComponent<CharacterInfo>();

            if (characterInfo != null)
                characterInfo.GetDamage(this.power);
        }
    }
    
    private void MultiTargetDamage()
    {
        int layerMask = MyUtils.GetLayerMaskByString("Enemy");
        Collider2D[] colls = Physics2D.OverlapCircleAll(this.transform.position, explosionRadius, layerMask);
        if (colls == null) return;

        for(int i=0;i< colls.Length; i++)
        {
            CharacterInfo characterInfo = colls[i].gameObject.GetComponent<CharacterInfo>();
            if (characterInfo != null)
                characterInfo.GetDamage(power);
        }

    }

    //다른 물체와의 충돌은 layer로 막아놓음
    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (explosionType)
        {
            case ExplosionType.single:
                {
                    SingleTargetDamage(collision);
                  
                } break;
            case ExplosionType.multiple:
                {
                    MultiTargetDamage();
                } break;
        }
        //이펙트 호출
        BulletDestroy();
    }

  

    private void BulletDestroy()
    {
        ShowEffect();
        this.gameObject.SetActive(false);
    }

    private void ShowEffect()
    {
        //이펙트 호출
        ExplosionEffect effect = ObjectManager.Instance.effectPool.GetItem();
        if(effect!=null)
        effect.Initilaize(this.transform.position, effectName,0.5f, effectsize);
    }

}
