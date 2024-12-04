using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [Header("Default")]
    public Texture2D defaultCursorTexture;
    public Vector2 defaultCursorHotspot = Vector2.zero;

    [Header("Hover")]
    public Texture2D hoverCursorTexture;
    public Vector2 hoverCursorHotspot = Vector2.zero;


    public CursorMode cursorMode = CursorMode.Auto;

    public static CursorManager Instance;
    void Awake()
    {
        Instance = this;
    }
    public void SetCursorHover()
    {
        Cursor.SetCursor(hoverCursorTexture, hoverCursorHotspot, cursorMode);
    }

    public void SetCursorDefault()
    {
        Cursor.SetCursor(defaultCursorTexture, defaultCursorHotspot, cursorMode);
    }


}
