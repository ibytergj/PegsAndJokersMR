using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PJSceneManager : MonoBehaviour
{
    private static PJSceneManager _instance;

    public static PJSceneManager Instance
    {
        get
        {
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

    }


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadAsyncScene());
    }

    IEnumerator LoadAsyncScene()
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        // Interhaptics
        // AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("PegsAndJokersIH", LoadSceneMode.Additive);

        // Oculus Intergration
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("PegsAndJokersBasic", LoadSceneMode.Additive);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
