using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStuff : MonoBehaviour
{
    public int POPoints = 0;
    public int powerpointsO = 0;
    public int poLevel = 1;
    public int PTPoints = 0;
    public int powerpointsT = 0;
    public int ptLevel = 1;

    public void OnGUI()
    {
        // Score for the players
        GUI.Label(new Rect(10, 10, 100, 100), "Player One Score: " + POPoints);
        GUI.Label(new Rect(750, 10, 100, 100), "Player Two Score: " + PTPoints);

        GUI.Label(new Rect(10, 100, 100, 100), "Level: " + poLevel);
        GUI.Label(new Rect(750, 100, 100, 100), "Level: " + ptLevel);
    }
}
