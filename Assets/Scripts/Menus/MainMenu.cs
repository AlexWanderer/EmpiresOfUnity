﻿using UnityEngine;

public class MainMenu : MonoBehaviour
{

    public GameObject Blackscreen;
    public GameObject Logo;
    public GameObject Controls;
    public GameObject Credits;
    
    private GUIStyle style = new GUIStyle();
    public Texture2D background;
    private float Timer = 0;
    private bool showGUI = false;


    void Start()
    {
        style.normal.background = background;
        style.fontSize = 20;
        style.alignment = TextAnchor.MiddleCenter;
    }

    void Update()
    {
        this.Timer += Time.deltaTime;
        if (this.Timer >= 5f)
        {
            showGUI = true;
            Blackscreen.renderer.enabled = false;
            Logo.renderer.enabled = false;
        }
    }

    void OnGUI()
    {
        if (showGUI)
        {
            Vector2 size = new Vector2(1920, 1080);
            float left = Screen.width / size.x * 1300f;
            float top = Screen.height / size.y * 300f;
            float topPlus = Screen.width / size.x * 80f;
            float width = Screen.width / size.x * 400f;
            float height = Screen.width / size.x * 60f;

            if (GUI.Button(new Rect(left, top + topPlus * 1, width, height), "Start", style))
            {
                Application.LoadLevel("InGame");
            }
            if (GUI.Button(new Rect(left, top + topPlus * 2, width, height), "Controls", style))
            {
                Credits.renderer.enabled = false;
                if (Controls.renderer.isVisible)
                    Controls.renderer.enabled = false;
                else
                    Controls.renderer.enabled = true;
            }
            if (GUI.Button(new Rect(left, top + topPlus * 3, width, height), "Credits", style))
            {
                Controls.renderer.enabled = false;
                if (Credits.renderer.isVisible)
                    Credits.renderer.enabled = false;
                else
                    Credits.renderer.enabled = true;
            }
            /*if (GUI.Button(new Rect(left, top + topPlus * 4, width, height), "Settings"))
            {
                Application.LoadLevel("Settings");
            }*/
            if (GUI.Button(new Rect(left, top + topPlus * 4, width, height), "Exit", style))
            {
                Application.Quit();
            }
        }
    }

}
