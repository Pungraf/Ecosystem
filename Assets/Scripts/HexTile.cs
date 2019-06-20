using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexTile : MonoBehaviour
{
    public HexCoordinates coordinates;
    public Text text;
    
    
    int distance;
    public int Distance {
        get {
            return distance;
        }
        set {
            distance = value;
            UpdateDistanceLabel();
        }
    }

    void UpdateDistanceLabel () {
        text.text = distance.ToString();
    }
}
