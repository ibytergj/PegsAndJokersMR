using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardFactory : MonoBehaviour
{
    [SerializeField]
    GameManager gamemanger;

    public int numBoards;

    // Default BordSettings baked in the game
    public BoardSettings twoPlayerDefaultSettings;
    public BoardSettings fourPlayerDefaultSettings;
    public BoardSettings sixPlayerDefaultSettings;
    public BoardSettings eightPlayerDefaultSettings;

    [SerializeField]
    GameObject FourPlayerBoardModel;

    [SerializeField]
    GameObject SixPlayerBoardModel;
    
    [SerializeField]
    GameObject EightPlayerBoardModel;


    [SerializeField]
    Transform gameParent;

    [SerializeField]
    Transform tableTopParent;



    // Start is called before the first frame update
    void Start()
    {
        gamemanger = GameObject.Find("GameManager").GetComponent<GameManager>();
        CreateBoard(numBoards);
    }

    public void CreateBoard(int numPlayers, BoardSettings customSettings = null)
        {
            switch (numPlayers)
            {

                case 2:
                    Debug.Log("2 players selected");
                    CreateTwoPlayerBoard(customSettings);
                break;

                case 4:
                    Debug.Log("4 players selected");
                    CreateFourPlayerBoard(customSettings);
                    break;
                case 6:
                    Debug.Log("6 players selected");
                    CreateSixPlayerBoard(customSettings);
                    break;
                case 8:
                    Debug.Log("8 players selected");
                    CreateEightPlayerBoard(customSettings);
                break;
                default:
                    Debug.Log("Invalid number of players selected");
                    break;

            }

        gameParent.SetParent(tableTopParent, true);
        }

    private void CreateTwoPlayerBoard(BoardSettings twoPlayerSettings = null)
    {
        if (twoPlayerSettings != null)
        {
            // Setup using custom player data
            BoardSettings newFourPlayerSettings = new BoardSettings();
            newFourPlayerSettings.name = "2PlayerGameBoardData";
            newFourPlayerSettings.numPlayers = 4;
            
            // Set colors
            newFourPlayerSettings.boardColors[0] = twoPlayerSettings.boardColors[0];
            newFourPlayerSettings.boardColors[1] = fourPlayerDefaultSettings.boardColors[1];
            newFourPlayerSettings.boardColors[2] = twoPlayerSettings.boardColors[1];
            newFourPlayerSettings.boardColors[3] = fourPlayerDefaultSettings.boardColors[3];
            // Check the remaining colors were not used by the first two players

            // Set Postions
            newFourPlayerSettings.boardAnchorPoints[0] = twoPlayerSettings.boardAnchorPoints[0];
            newFourPlayerSettings.boardAnchorPoints[1] = fourPlayerDefaultSettings.boardAnchorPoints[1];
            newFourPlayerSettings.boardAnchorPoints[2] = twoPlayerSettings.boardAnchorPoints[1];
            newFourPlayerSettings.boardAnchorPoints[3] = fourPlayerDefaultSettings.boardAnchorPoints[3];

            // Set Rotations
            newFourPlayerSettings.boardRatationAngles[0] = twoPlayerSettings.boardRatationAngles[0];
            newFourPlayerSettings.boardRatationAngles[1] = fourPlayerDefaultSettings.boardRatationAngles[1];
            newFourPlayerSettings.boardRatationAngles[2] = twoPlayerSettings.boardRatationAngles[1];
            newFourPlayerSettings.boardRatationAngles[3] = fourPlayerDefaultSettings.boardRatationAngles[3];

            // Set Table Sizes
            newFourPlayerSettings.gameBaseSize = twoPlayerSettings.gameBaseSize;
            newFourPlayerSettings.gameTableTopSize = twoPlayerSettings.gameTableTopSize;

            // Create a 4 player board
            CreateFourPlayerBoard(newFourPlayerSettings);
        }
        else
        {
            // Setup using default player data
            BoardSettings newFourPlayerSettings = new BoardSettings();
            newFourPlayerSettings.name = "2PlayerGameBoardData";
            newFourPlayerSettings.numPlayers = 4;
            newFourPlayerSettings.boardColors[0] = twoPlayerDefaultSettings.boardColors[0];
            newFourPlayerSettings.boardColors[1] = fourPlayerDefaultSettings.boardColors[1];
            newFourPlayerSettings.boardColors[2] = twoPlayerDefaultSettings.boardColors[1];
            newFourPlayerSettings.boardColors[3] = fourPlayerDefaultSettings.boardColors[3];

            // Set Postions
            newFourPlayerSettings.boardAnchorPoints[0] = twoPlayerDefaultSettings.boardAnchorPoints[0];
            newFourPlayerSettings.boardAnchorPoints[1] = fourPlayerDefaultSettings.boardAnchorPoints[1];
            newFourPlayerSettings.boardAnchorPoints[2] = twoPlayerDefaultSettings.boardAnchorPoints[1];
            newFourPlayerSettings.boardAnchorPoints[3] = fourPlayerDefaultSettings.boardAnchorPoints[3];

            // Set Rotations
            newFourPlayerSettings.boardRatationAngles[0] = twoPlayerDefaultSettings.boardRatationAngles[0];
            newFourPlayerSettings.boardRatationAngles[1] = fourPlayerDefaultSettings.boardRatationAngles[1];
            newFourPlayerSettings.boardRatationAngles[2] = twoPlayerDefaultSettings.boardRatationAngles[1];
            newFourPlayerSettings.boardRatationAngles[3] = fourPlayerDefaultSettings.boardRatationAngles[3];

            // Set Table Sizes
            newFourPlayerSettings.gameBaseSize = twoPlayerDefaultSettings.gameBaseSize;
            newFourPlayerSettings.gameTableTopSize = twoPlayerDefaultSettings.gameTableTopSize;

            CreateFourPlayerBoard(newFourPlayerSettings);
        }
    }
    private void CreateFourPlayerBoard(BoardSettings fourPlayerSettings = null)
    {
        if (fourPlayerSettings != null)
        {
            // Setup using 4 player data
            for (int i=0; i < fourPlayerSettings.numPlayers;i++)
            {
                GameObject board = Instantiate(FourPlayerBoardModel, fourPlayerSettings.boardAnchorPoints[i], fourPlayerSettings.boardRatationAngles[i], gameParent);
                MeshRenderer mesh = board.GetComponentInChildren<MeshRenderer>();
                mesh.material.color = fourPlayerSettings.boardColors[i];

                // Set Table Sizes Custom
                gamemanger.gameBase.transform.localScale = new Vector3(fourPlayerSettings.gameBaseSize, gamemanger.gameBase.transform.localScale.y, fourPlayerSettings.gameBaseSize);
                gamemanger.gameTableTop.transform.localScale = new Vector3(fourPlayerSettings.gameTableTopSize, gamemanger.gameBase.transform.localScale.y, fourPlayerSettings.gameTableTopSize);
            }
        }
        else
        {
            // Setup using 4 player data
            for (int i = 0; i < fourPlayerDefaultSettings.numPlayers; i++)
            {
                GameObject board = Instantiate(FourPlayerBoardModel, fourPlayerDefaultSettings.boardAnchorPoints[i], fourPlayerDefaultSettings.boardRatationAngles[i], gameParent);
                MeshRenderer mesh = board.GetComponentInChildren<MeshRenderer>();
                mesh.material.color = fourPlayerDefaultSettings.boardColors[i];
                
                // Set Table Sizes Default
                gamemanger.gameBase.transform.localScale = new Vector3(fourPlayerDefaultSettings.gameBaseSize, gamemanger.gameBase.transform.localScale.y, fourPlayerDefaultSettings.gameBaseSize);
                gamemanger.gameTableTop.transform.localScale = new Vector3(fourPlayerDefaultSettings.gameTableTopSize, gamemanger.gameBase.transform.localScale.y, fourPlayerDefaultSettings.gameTableTopSize);
            }
        }


    }

    private void CreateSixPlayerBoard(BoardSettings sixPlayerSettings = null)
    {
        if (sixPlayerSettings != null)
        {
            // Setup using 6 player data
            for (int i = 0; i < sixPlayerSettings.numPlayers; i++)
            {
                GameObject board = Instantiate(SixPlayerBoardModel, sixPlayerSettings.boardAnchorPoints[i], sixPlayerSettings.boardRatationAngles[i], gameParent);
                MeshRenderer mesh = board.GetComponentInChildren<MeshRenderer>();
                mesh.material.color = sixPlayerSettings.boardColors[i];
                
                // Set Table Sizes Custom
                gamemanger.gameBase.transform.localScale = new Vector3(sixPlayerSettings.gameBaseSize, gamemanger.gameBase.transform.localScale.y, sixPlayerSettings.gameBaseSize);
                gamemanger.gameTableTop.transform.localScale = new Vector3(sixPlayerSettings.gameTableTopSize, gamemanger.gameBase.transform.localScale.y, sixPlayerSettings.gameTableTopSize);
            }
        }
        else
        {
            // Setup using 6 player data
            for (int i = 0; i < sixPlayerDefaultSettings.numPlayers; i++)
            {
                GameObject board = Instantiate(SixPlayerBoardModel, sixPlayerDefaultSettings.boardAnchorPoints[i], sixPlayerDefaultSettings.boardRatationAngles[i], gameParent);
                MeshRenderer mesh = board.GetComponentInChildren<MeshRenderer>();
                mesh.material.color = sixPlayerDefaultSettings.boardColors[i];

                // Set Table Sizes Default
                gamemanger.gameBase.transform.localScale = new Vector3(sixPlayerDefaultSettings.gameBaseSize, gamemanger.gameBase.transform.localScale.y, sixPlayerDefaultSettings.gameBaseSize);
                gamemanger.gameTableTop.transform.localScale = new Vector3(sixPlayerDefaultSettings.gameTableTopSize, gamemanger.gameBase.transform.localScale.y, sixPlayerDefaultSettings.gameTableTopSize);
            }
        }
    }

    private void CreateEightPlayerBoard(BoardSettings eightPlayerSettings = null)
    {
        if (eightPlayerSettings != null)
        {
            // Setup using 8 player data
            for (int i = 0; i < eightPlayerSettings.numPlayers; i++)
            {
                GameObject board = Instantiate(EightPlayerBoardModel, eightPlayerSettings.boardAnchorPoints[i], eightPlayerSettings.boardRatationAngles[i], gameParent);
                MeshRenderer mesh = board.GetComponentInChildren<MeshRenderer>();
                mesh.material.color = eightPlayerSettings.boardColors[i];

                // Set Table Sizes Custom
                gamemanger.gameBase.transform.localScale = new Vector3(eightPlayerSettings.gameBaseSize, gamemanger.gameBase.transform.localScale.y, eightPlayerSettings.gameBaseSize);
                gamemanger.gameTableTop.transform.localScale = new Vector3(eightPlayerSettings.gameTableTopSize, gamemanger.gameBase.transform.localScale.y, eightPlayerSettings.gameTableTopSize);
            }
        }
        else
        {
            // Setup using 8 player data
            for (int i = 0; i < eightPlayerDefaultSettings.numPlayers; i++)
            {
                GameObject board = Instantiate(EightPlayerBoardModel, eightPlayerDefaultSettings.boardAnchorPoints[i], eightPlayerDefaultSettings.boardRatationAngles[i], gameParent);
                MeshRenderer mesh = board.GetComponentInChildren<MeshRenderer>();
                mesh.material.color = eightPlayerDefaultSettings.boardColors[i];

                // Set Table Sizes Default
                gamemanger.gameBase.transform.localScale = new Vector3(eightPlayerDefaultSettings.gameBaseSize, gamemanger.gameBase.transform.localScale.y, eightPlayerDefaultSettings.gameBaseSize);
                gamemanger.gameTableTop.transform.localScale = new Vector3(eightPlayerDefaultSettings.gameTableTopSize, gamemanger.gameBase.transform.localScale.y, eightPlayerDefaultSettings.gameTableTopSize);
            }
        }
    }

}
