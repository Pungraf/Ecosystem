using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexTile : MonoBehaviour
{
    public enum TileType
    {
        ground, water
    }

    public TileType tileType;
    public HexCoordinates coordinates;
    public Text text;
    public Material material;

    private void Awake()
    {
        material = GetComponent<Renderer>().material;
    }

    [SerializeField]
    public HexTile[] neighbors;
    
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
        text.text = distance == int.MaxValue ? "" : distance.ToString();
    }
    
    public HexTile GetNeighbor (HexDirection direction) {
        return neighbors[(int)direction];
    }
    
    public void SetNeighbor (HexDirection direction, HexTile cell) {
        neighbors[(int)direction] = cell;
        cell.neighbors[(int)direction.Opposite()] = this;
    }
}
