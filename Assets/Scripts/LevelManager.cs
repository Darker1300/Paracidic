using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    //// For converting to Singleton
    //public static LevelManager Instance;
    //void Awake()
    //{
    //    if (Instance == null)
    //    {
    //        DontDestroyOnLoad(gameObject);
    //        Instance = this;
    //    }
    //    else if (Instance != this)
    //    {
    //        Destroy(gameObject);
    //    }
    //}

    private static int mainMenuID = 0;

    public static void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public static void LoadNextScene()
    {
        int id = SceneManager.GetActiveScene().buildIndex + 1;
        if (id >= SceneManager.sceneCountInBuildSettings) id = mainMenuID;
        SceneManager.LoadScene(id);
    }

    public static void LoadPreviousScene()
    {
        int id = SceneManager.GetActiveScene().buildIndex - 1;
        if (id <= mainMenuID) id = mainMenuID;
        SceneManager.LoadScene(id);
    }
}
