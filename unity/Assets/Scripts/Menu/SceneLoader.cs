﻿using UnityEngine;
using System.Collections;

public class SceneLoader : MonoBehaviour
{ 
    public void LoadScene(string a_nextScene)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(a_nextScene);
    }
}
