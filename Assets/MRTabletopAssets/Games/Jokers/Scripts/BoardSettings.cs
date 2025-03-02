using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BoardData", menuName = "CreateGameBoard", order = 1)]
public class BoardSettings : ScriptableObject
{
    public string objectName = "GameBoardData";
    public int numPlayers;
    public float gameTableTopSize;
    public float gameBaseSize;
    public Color [] boardColors = new Color[] { Color.white, Color.white, Color.white, Color.white };
    public Vector3[] boardAnchorPoints = new Vector3[] { new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f) };
    public Quaternion[] boardRatationAngles = new Quaternion[] { new Quaternion(0f,0f,0f,0f), new Quaternion(0f, 0f, 0f, 0f), new Quaternion(0f, 0f, 0f, 0f), new Quaternion(0f, 0f, 0f, 0f) };
}
