using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

public class HexaTileMap : MonoBehaviour
{
    [Range(0, 100)]
    public int fillPercent;
    
    public int width = 6;
    public int height = 6;
    
    public HexTile waterTilePrefab;
    public HexTile groundTilePrefab;
    public Text tileCoordTextPrefab;

    private Canvas gridCanvas;
    private HexTile[] tiles;
    private int[,] fillMap;
    public HexTile currentTile;
    
    void Awake ()
    {
        gridCanvas = GetComponentInChildren<Canvas>();
        GenerateMap();
    }
    
    void Update () {
        if (Input.GetMouseButtonDown(0)) {
            HandleInput();
        }
    }

    void HandleInput () {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit)) {
            TouchCell(hit.point);
        }
    }
	
    void TouchCell (Vector3 position) {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        foreach (HexTile tile in tiles)
        {
            if (tile.coordinates.Equals(coordinates))
            {
                currentTile = tile;
                break;
            }
        }

        FindDistancesTo(currentTile);
    }

    public void GenerateMap()
    {
        tiles = new HexTile[height * width];
        
        RandomMapFill();
        
        for (int i = 0; i < 5; i++)
        {
            
        }
        
        for (int z = 0, i = 0; z < height; z++) {
            for (int x = 0; x < width; x++) {
                CreateCell(x, z, i++);
            }
        }
    }

    void SmoothMap()
    {
        
    }


    void CreateCell (int x, int z, int i) {
        Vector3 position;
        position.x = (x + z * 0.5f - z/2) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);

        HexTile tile;
        
        if (fillMap[x, z] == 0)
        {
            tile = tiles[i] = Instantiate<HexTile>(waterTilePrefab);
            tile.transform.SetParent(transform, false);
            tile.transform.localPosition = position;
            tile.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        }
        else
        {
            tile = tiles[i] = Instantiate<HexTile>(groundTilePrefab);
            tile.transform.SetParent(transform, false);
            tile.transform.localPosition = position;
            tile.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        }
        
        Text label = Instantiate<Text>(tileCoordTextPrefab);
        tile.text = label;
        
        label.rectTransform.SetParent(gridCanvas.transform, false);
        label.rectTransform.anchoredPosition =
            new Vector2(position.x, position.z);
        //label.text = tile.coordinates.ToStringOnSeparateLines();
    }

    void RandomMapFill()
    {
        string seed = Time.time.ToString();
        fillMap = new int[width, height];
        
        System.Random prnd = new System.Random(seed.GetHashCode());

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                fillMap[x, y] = (prnd.Next(0, 100) < fillPercent) ? 1 : 0;
            }
        }
    }
    
    public void FindDistancesTo (HexTile cell) {
        for (int i = 0; i < tiles.Length; i++) {
            tiles[i].Distance =
                cell.coordinates.DistanceTo(tiles[i].coordinates);
        }
    }
}
