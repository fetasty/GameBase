using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseConst
{
    // Message
    public const string Msg_SceneLoaded = nameof(Msg_SceneLoaded);      // Scene
    public const string Msg_SceneUnloaded = nameof(Msg_SceneUnloaded);  // Scene
    public const string Msg_SceneLoadProgress = nameof(Msg_SceneLoadProgress); // float (load progress)
    public const string Msg_KeyDown = nameof(Msg_KeyDown);              // KeyCode
    public const string Msg_KeyUp = nameof(Msg_KeyUp);                  // KeyCode
    public const string Msg_MouseDown = nameof(Msg_MouseDown);          // int (0, 1, 2)
    public const string Msg_MouseUp = nameof(Msg_MouseUp);              // int (0, 1, 2)
}
