using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class HexCell : MonoBehaviour
{
    
    [SerializeField]
    HexCell[] neighbors = new HexCell[6];
    
    public RectTransform uiRect;
    public HexCoordinates coordinates;
    public Color color;
    
    private int elevation;
    // Setter automatically update label
    public int Elevation
    {
        get => elevation;
        set
        {
            elevation = value;
            Vector3 position = transform.localPosition;
            position.y = value * HexMetrics.elevationStep;
            transform.localPosition = position;
            
            Vector3 uiPosition = uiRect.localPosition;
            uiPosition.z = elevation * -HexMetrics.elevationStep ;
            uiRect.localPosition = uiPosition;
        }
    }

    public HexCell GetNeighbor (HexDirection direction) 
    {
        return neighbors[(int)direction];
    }
    
    public void SetNeighbor (HexDirection direction, HexCell cell)
    {
        neighbors[(int)direction] = cell;
        cell.neighbors[(int)direction.Opposite()] = this;
    }
    
    public HexEdgeType GetEdgeType (HexDirection direction)
    {
        return HexMetrics.GetEdgeType(elevation, neighbors[(int)direction].elevation);
    }
    
    public HexEdgeType GetEdgeType (HexCell otherCell)
    {
        return HexMetrics.GetEdgeType(elevation, otherCell.elevation);
    }
}
