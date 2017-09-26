﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapModuleGenerator
{
    private MapManager mapManager;

    private Transform moduleParent;

    private GameObject normalTile;
    private GameObject moduleObject;

    //타일 사이즈
    private float widthDistance = 0.64f;
    private float heightDistance = 0.64f;   

    private int doorSize = 4;


    //특성별 타일 저장
    private List<Tile> normalTileList;
    private List<Tile> wallTileList;
    private List<Tile> doorTileList;
    private List<Tile> bossMakableList;

    //타일 이미지 풀
    private List<Sprite> tileSpriteList;
    private Sprite wallSprite;
    private Sprite doorSprite;

    //모듈 리스트
    private List<MapModuleBase> moduleList;


    //벽만들때 필요
    //현재 모듈들의 모서리 벽부분(보스룸 생성에 정보가 필요함)
    public List<Tile> everyWallList;

    public MapModuleGenerator(Transform moduleParent, MapManager mapManager)
    {
        normalTile = Resources.Load<GameObject>("Prefabs/Maps/MapObjects/tile") ;
        moduleObject = Resources.Load<GameObject>("Prefabs/Maps/MapObjects/MapModule");
        this.moduleParent = moduleParent;
        this.mapManager = mapManager;

        Initialize();
    }

    private void ResetLists()
    {
        //새로 할당해줘야 중복안됨
        normalTileList = new List<Tile>();
        wallTileList = new List<Tile>();
        doorTileList = new List<Tile>();
    }

    private void Initialize()
    {     
        tileSpriteList = new List<Sprite>();
        everyWallList = new List<Tile>();
        bossMakableList = new List<Tile>();
        moduleList = new List<MapModuleBase>();


        LoadTileSprites();

    }

    public void MakeWall(Transform parent = null)
    {
        if (everyWallList == null) return;


        //X최대 최소
        everyWallList.Sort((a, b) =>
        {
            return (a.transform.position.x.CompareTo(b.transform.position.x));
        });

        float minX = everyWallList[0].transform.position.x;
        float maxX = everyWallList[everyWallList.Count - 1].transform.position.x;


        //Y최대 최소
        everyWallList.Sort((a, b) =>
        {
            return (a.transform.position.y.CompareTo(b.transform.position.y));
        });

        float minY = everyWallList[0].transform.position.y;
        float maxY = everyWallList[everyWallList.Count - 1].transform.position.y;

        float width = maxX - minX;
        float height = maxY - minY;

        float offSetX = minX + width / 2f;
        float offSetY = minY + height / 2f;

        int widthNum = (int)(width / widthDistance) + 8;
        int heightNum = (int)(height / heightDistance) + 8;


        for (int x = 0; x < widthNum; x++)
        {
            for (int y = 0; y < heightNum; y++)
            {
                if (y == 0 || y == heightNum - 1 || x == 0 || x == widthNum - 1)
                {
                    Vector3 posit = new Vector3((float)(-widthNum / 2 + x) * widthDistance + offSetX,
                                          (float)(-heightNum / 2 + y) * heightDistance + offSetY,
                                          0f);
                    //일반벽   
                    Tile tile;
                    tile = MakeTile(TileType.Wall, posit, x, y, parent);
                    if (wallSprite != null)
                        tile.SetSprite(wallSprite);
                    SetTileColor(tile, Color.white);

                    //위쪽문
                    if (x >= widthNum / 2 && y == heightNum - 1)
                    {
                        bossMakableList.Add(tile);
                    }
                    ////아래쪽문
                    //else if (x >= widthNum / 2  / 2 && x < widthNum / 2  / 2 && y == 0)
                    //{

                    //}
                    ////왼쪽문
                    //else if (y >= heightNum / 2 / 2 && y < heightNum / 2  / 2 && x == 0)
                    //{

                    //}
                    ////오쪽문
                    //else if (y >= heightNum / 2 - doorSize / 2 && y < heightNum / 2 + doorSize / 2 && x == widthNum - 1)
                    //{

                    //}              
                }
            }
        }

    }

    public void MakeBossModule(Transform moduleParent)
    {
        if (bossMakableList == null) return;
        if (bossMakableList.Count == 0) return;

        BossModule bossModuleObject = GameOption.Instance.StageData.bossModule.GetComponent<BossModule>();
        if (bossModuleObject == null) return;



        //보스 문방향이 down
        Vector3 SpawnPosit = bossMakableList[0].transform.position + Vector3.right * 0.64f / 2f + Vector3.up * 0.64f / 2f + Vector3.up * (bossModuleObject.heightNum / 2 - 1) * 0.64f;

        BossModule module = GameObject.Instantiate(bossModuleObject, SpawnPosit, Quaternion.identity, moduleParent);

        if (moduleList != null)
            moduleList.Add(module);


    }

   


    private void LoadTileSprites()
    {
        StageData nowStageData = GameOption.Instance.StageData;
        if (nowStageData == null) return;

        //일반타일 로드
        if (tileSpriteList != null)
        {
            List<Sprite> normalTileList = nowStageData.GetNormalTileList();
            if (normalTileList != null)
            {
                for (int i = 0; i < normalTileList.Count; i++)
                {
                    tileSpriteList.Add(normalTileList[i]);
                }
            }
        }

        //벽타일 로드
        wallSprite = nowStageData.GetWallTile();

        //문타일 로드
        doorSprite = nowStageData.GetDoorTile();

    }

    private Sprite GetRandomTileSprite()
    {
        if (tileSpriteList == null) return null;
        if (tileSpriteList.Count == 0) return null;

        return tileSpriteList[Random.Range(0, tileSpriteList.Count)];
    }


    public void MakeMap(StageData stageData)
    {
        if (stageData == null) return;

        //센터
        GenerateBaseMap(10, 10, Vector3.zero, true);

        int minsize = stageData.RoomMinSize;
        int maxSize = stageData.RoomMaxSize;

        for (int i = 0; i < stageData.RoomNum; i++)
        {
            GenerateBaseMap
                (
                Random.Range(minsize, maxSize),  //x크기
                Random.Range(minsize, maxSize),  //y크기
                new Vector3((float)Random.Range(-15, 15) * 0.64f, (float)Random.Range(-15, 15) * 0.64f, 0) //초기 생성 좌표
                );

        }
    }

    private void GenerateBaseMap(int widthNum, int heightNum, Vector3 modulePosit, bool isStartModule = false)
    {
        ResetLists();

        #region Exception
        //예외처리

        //짝수로 교정
        if (widthNum % 2 != 0)
            widthNum += 1;

        if (heightNum % 2 != 0)
            heightNum += 1;

        //만들수있는 최소사이즈
        if (widthNum < 4)
            widthNum = 4;

        if (heightNum < 4)
            heightNum = 4;

        //문사이즈는 반드시 짝수
        if (doorSize % 2 != 0)
            doorSize += 1;

        //문 사이즈가 벽보다 크다
        if (doorSize >= widthNum || doorSize >= heightNum)
            doorSize -= 2;
        #endregion

        #region MakeModule
        //모듈 생성
        GameObject obj = GameObject.Instantiate(moduleObject, modulePosit, Quaternion.identity, moduleParent);
        MapModule module = null;
        if (obj != null)
            module = obj.GetComponent<MapModule>();

        if (module == null) return;
        module.Initialize(widthNum, heightNum, widthDistance, heightDistance, isStartModule, mapManager);

        if (moduleList != null)
            moduleList.Add(module);
        #endregion

        #region TileMaking
        //타일,벽 생성
        for (int x = 0; x < widthNum; x++)
        {
            for (int y = 0; y < heightNum; y++)
            {
                Vector3 posit = new Vector3((float)(-widthNum / 2 + x) * widthDistance,
                                            (float)(-heightNum / 2 + y) * heightDistance, 0f);
                posit += modulePosit;

                //벽 (네방향 테두리부분)
                if (y == 0 || y == heightNum - 1 || x == 0 || x == widthNum - 1)
                {
                    Tile tile;

                    //위쪽문
                    if (x >= widthNum / 2 - doorSize / 2 && x < widthNum / 2 + doorSize / 2 && y == heightNum - 1)
                    {
                        tile = MakeTile(TileType.Door, posit, x, y, module.transform, module);
                    }
                    //아래쪽문
                    else if (x >= widthNum / 2 - doorSize / 2 && x < widthNum / 2 + doorSize / 2 && y == 0)
                    {
                        tile = MakeTile(TileType.Door, posit, x, y, module.transform, module);
                    }
                    //왼쪽문
                    else if (y >= heightNum / 2 - doorSize / 2 && y < heightNum / 2 + doorSize / 2 && x == 0)
                    {
                        tile = MakeTile(TileType.Door, posit, x, y, module.transform, module);
                    }
                    //오쪽문
                    else if (y >= heightNum / 2 - doorSize / 2 && y < heightNum / 2 + doorSize / 2 && x == widthNum - 1)
                    {
                        tile = MakeTile(TileType.Door, posit, x, y, module.transform, module);
                    }
                    //일반벽
                    else
                    {
                        tile = MakeTile(TileType.Wall, posit, x, y, module.transform);

                        if (wallSprite != null)
                            tile.SetSprite(wallSprite);

                        //LeftTop
                        if (x == 0 && y == heightNum - 1)
                            everyWallList.Add(tile);
                        //RightTop
                        else if (x == widthNum - 1 && y == heightNum - 1)
                            everyWallList.Add(tile);
                        //LeftBottom
                        else if (x == 0 && y == 0)
                            everyWallList.Add(tile);
                        //RightBottom
                        else if (x == widthNum - 1 && y == 0)
                            everyWallList.Add(tile);


                        SetTileColor(tile, Color.white);
                    }
                }
                //노말타일
                else
                {
                    Tile tile = MakeTile(TileType.Normal, posit, x, y, module.transform);
                    SetTileColor(tile, GameOption.Instance.StageData.GetRandomTileColor());

                }


            }
        }
        #endregion

        #region LinkModule
        module.LinkTile(normalTileList, wallTileList, doorTileList);
        #endregion

    }
    private void SetTileColor(Tile tile, Color color)
    {
        if (tile == null) return;
        tile.ChangeColor(color);

    }

    private Tile MakeTile(TileType type, Vector3 posit, int x, int y, Transform parent, MapModule parentModule = null)
    {
        GameObject obj = GameObject.Instantiate(normalTile, parent);
        obj.transform.position = posit;
        if (obj == null) return null;

        Tile tile = null;
        tile = obj.GetComponent<Tile>();
        if (tile == null) return null;
        tile.Initialize(type, GetRandomTileSprite(), parentModule);
        tile.SetIndex(x, y);
        switch (type)
        {
            case TileType.Normal:
                {
                    if (normalTileList != null)
                        normalTileList.Add(tile);
                }
                break;
            case TileType.Wall:
                {
                    if (wallTileList != null)
                        wallTileList.Add(tile);
                }
                break;
            case TileType.Door:
                {
                    if (doorTileList != null)
                        doorTileList.Add(tile);
                }
                break;
        }

        return tile;


    }



}






