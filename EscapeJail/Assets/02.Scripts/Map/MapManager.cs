﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MapManager : MonoBehaviour
{
    [SerializeField]
    private Image maskImage;
    float alphaValue = 255f;

    public static MapManager Instance;
    private List<MapModule> moduleList = new List<MapModule>();
    private List<GameObject> objectList = new List<GameObject>();
    private float mapMakeCount = 0;
    void Awake()
    {
        if (Instance == null)
            Instance = this;

        LoadObject();
    }

    private void LoadObject()
    {
        if (objectList == null) return;
        GameObject[] objects = Resources.LoadAll<GameObject>("Prefabs/Articles/");
        
        if (objects != null)
        {
            if (objects.Length != 0)
            {
                for(int i = 0; i < objects.Length; i++)
                {
                    objectList.Add(objects[i]);
                }
            }
        }
    }

    public GameObject GetRandomObject()
    {
        if (objectList == null) return null;
        if (objectList.Count == 0) return null;
        return objectList[(Random.Range(0, objectList.Count))];
    }

    void Start()
    {
        StartCoroutine(MapPositioningRoutine());
    }

    //맵이 아직 생성중일때
    public void ResetMakeCount()
    {
        mapMakeCount = 0f;   
    }

    IEnumerator MapPositioningRoutine()
    {
        while (true)
        {
            mapMakeCount += Time.deltaTime;
     
            if (mapMakeCount > 3.0f)
            {
                Debug.Log("Positioning Complete");
                break;
            }
            yield return null;
        }
        
        

        PositioningComplete();
        CreateObjects();
    }

    private void CreateObjects()
    {
        if (moduleList == null) return;
        for(int i =0; i < moduleList.Count; i++)
        {
            moduleList[i].MakeObjects();
        }
    }


    public void AddToModuleList(MapModule module)
    {
        if (moduleList == null) return;
        if (module == null) return;

        moduleList.Add(module);
    }

    public void ModuleListClear()
    {
        if (moduleList == null) return;

        for (int i = 0; i < moduleList.Count; i++)
        {
            if (moduleList[i] != null) ;
            Destroy(moduleList[i].gameObject);
        }

        moduleList.Clear();
    }

    private void PositioningComplete()
    {
        if (moduleList == null) return;
        for (int i = 0; i < moduleList.Count; i++)
        {
            if (moduleList[i] != null)
                moduleList[i].PositioningComplete();
        }

    }


}