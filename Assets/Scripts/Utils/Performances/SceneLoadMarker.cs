using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voodoo.SceneLoader;

public class SceneLoadMarker : MonoBehaviour
{
    private void Awake()
    {
        SceneLoaderAnalytics.OnSceneLoaded(gameObject.scene);
    }
}
