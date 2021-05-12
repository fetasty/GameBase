using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Common
{
    public static void Exit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
