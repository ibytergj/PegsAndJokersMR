using UnityEngine;

public class IntroManager : MonoBehaviour
{

    // The purpose of this class is to manage the intro scene
    // This class will be responsible for:
    // - Displaying the intro scene
    // - Handling user input to skip the intro scene
    // - Allow player to select default or custom RPM avatar
    // - Transitioning to the main lobby scene


    [SerializeField]
    private GameObject introCanvas;

    //[SerializeField]
    //private SceneLoader loader;

    [SerializeField]
    private GameObject selectAvatarButton;

    // Scene object to load the customize avatar scene
    [SerializeField]
    private GameObject customizeAvatarScene;

    // Scriptable object to hold the player data
    [SerializeField]
    private PlayerData playerData;

    [SerializeField]
    private GameObject defaultAvatar;

    [SerializeField]

    private GameObject customAvatar;

    [SerializeField]

    private GameObject mainLobbyScene;





    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
