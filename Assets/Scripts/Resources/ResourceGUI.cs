﻿using UnityEngine;
using System.Collections;

public class ResourceGUI : MonoBehaviour
{

    public Texture2D icon_wood;
    public Texture2D icon_stone;
    public Texture2D icon_gold;

	/* Reflection */
	void Start () {
        UpdateHandler.OnUpdate += DoUpdate;
	}

    void DoUpdate()
    {
    }

    void OnGUI()
    {
        GUI.Box(new Rect(12, 12, 70, 30), new GUIContent(" " + ResourceManager.GetResourceCount(ResourceManager.Resource.WOOD), icon_wood));
        GUI.Box(new Rect(88, 12, 70, 30), new GUIContent(" " + ResourceManager.GetResourceCount(ResourceManager.Resource.STONE), icon_stone));
        GUI.Box(new Rect(164, 12, 70, 30), new GUIContent(" " + ResourceManager.GetResourceCount(ResourceManager.Resource.GOLD), icon_gold));
    }

    /* Methods */
}
