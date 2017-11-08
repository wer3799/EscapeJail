﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ItemInfoBar : MonoBehaviour
{
    [SerializeField]
    private Image itemIcon;
    [SerializeField]
    private Text itemText;

    private Action clickEvent;

    public static ItemInfoBar Instance;

    private ItemBase nowItem = null;

    float moveSpeed = 1f;

    [SerializeField]
    private Transform showPosit;

    [SerializeField]
    private Transform hidePosit;

    private void Awake()
    {
        Instance = this;
    }
                                  //item 인자가 null -> 무기상자가 들어옴
    public void SetItemBar(ItemBase item, Action clickFunc)
    {
        iTween.MoveTo(this.gameObject, showPosit.position, moveSpeed);

        //무기상자가 들어옴
        if (item == null)
        {
            string path = "Sprites/Icons/WeaponBox";
            Sprite sprite = Resources.Load<Sprite>(path);

            if (itemIcon != null)
                itemIcon.sprite = sprite;

            //아이템 텍스트 변경
            if (itemText != null)
                itemText.text = "WeaponBox \n What's in it?";
            clickEvent = clickFunc;
            return;
        }

        //중복세팅 방지
        if (nowItem == item) return;

        nowItem = item;
        clickEvent = clickFunc;

        //아이템 아이콘 변경
        switch (nowItem.itemType)
        {
            case ItemType.Weapon:
                {
                    string path = string.Format("Sprites/Icons/{0}", nowItem.weapontype.ToString());
                    Sprite sprite = Resources.Load<Sprite>(path);

                    if (itemIcon != null)
                        itemIcon.sprite = sprite;

                    //아이템 텍스트 변경
                    if (itemText != null)
                        itemText.text = nowItem.weapontype.ToString();
                }
                break;
            default:
                {              
                    string path = string.Format("Sprites/Icons/{0}", nowItem.itemName);
                    Sprite sprite = Resources.Load<Sprite>(path);

                    if (itemIcon != null)
                        itemIcon.sprite = sprite;

                    //아이템 텍스트 변경
                    if (itemText != null)
                        itemText.text = nowItem.itemName;
                }
                break;
        }

      


    }

    public void ResetItemBar()
    {
        if (clickEvent == null) return;

        iTween.MoveTo(this.gameObject, hidePosit.position, moveSpeed);


        nowItem = null;
        clickEvent = null;

        if (itemIcon != null)
        {
            string path = "Sprites/Icons/Blank";
            Sprite sprite = Resources.Load<Sprite>(path);
            itemIcon.sprite = sprite;

        }

        if (itemText != null)
            itemText.text = string.Empty;



    }

    public void GetItemButtonClick()
    {
        if (clickEvent != null)
            clickEvent(); 
    }


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}