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
    public int waterTileSmooth = 4;

    public Color waterTileColor;
    public Color groundTileColor;

    public HexTile tilePrefab;
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
        
        //smoothing water terrain

        for (int i = 0; i < 4; i++)
        {
            foreach (HexTile tile in tiles)
            {
                int waterNeighbour = 0;
                foreach (HexTile tileNeighbor in tile.neighbors)
                {
                    if (tileNeighbor != null && tileNeighbor.tileType == HexTile.TileType.water)
                    {
                        waterNeighbour++;
                    }
                }

                if (waterNeighbour > waterTileSmooth)
                {
                    tile.tileType = HexTile.TileType.water;
                    tile.material.color = waterTileColor;
                }

                if (waterNeighbour == 0)
                {
                    tile.tileType = HexTile.TileType.ground;
                    tile.material.color = groundTileColor;
                }
            }
        }
    }


    void CreateCell (int x, int z, int i) {
        Vector3 position;
        position.x = (x + z * 0.5f - z/2) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);

        HexTile tile;
        
        if (fillMap[x, z] == 0)
        {
            tile = tiles[i] = Instantiate<HexTile>(tilePrefab);
            tile.tileType = HexTile.TileType.water;
            tile.material.color = waterTileColor;
            tile.transform.SetParent(transform, false);
            tile.transform.localPosition = position;
            tile.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        }
        else
        {
            tile = tiles[i] = Instantiate<HexTile>(tilePrefab);
            tile.tileType = HexTile.TileType.ground;
            tile.material.color = groundTileColor;
            tile.transform.SetParent(transform, false);
            tile.transform.localPosition = position;
            tile.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        }
        
        if (x > 0) {
            tile.SetNeighbor(HexDirection.W, tiles[i - 1]);
        }
        if (z > 0) {
            if ((z & 1) == 0) {
                tile.SetNeighbor(HexDirection.SE, tiles[i - width]);
                if (x > 0) {
                    tile.SetNeighbor(HexDirection.SW, tiles[i - width - 1]);
                }
            }
            else {
                tile.SetNeighbor(HexDirection.SW, tiles[i - width]);
                if (x < width - 1) {
                    tile.SetNeighbor(HexDirection.SE, tiles[i - width + 1]);
                }
            }
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
    
    public void FindDistancesTo (HexTile tile) {
        StopAllCoroutines();
        StartCoroutine(Search(tile));
    }

    IEnumerator Search (HexTile tile) {
        for (int i = 0; i < tiles.Length; i++) {
            tiles[i].Distance = int.MaxValue;
        }
        
        WaitForSeconds delay = new WaitForSeconds(1 / 60f);
        Queue<HexTile> frontier = new Queue<HexTile>();
        tile.Distance = 0;
        frontier.Enqueue(tile);
        
        while (frontier.Count > 0) {
            yield return delay;
            HexTile current = frontier.Dequeue();
            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
                HexTile neighbor = current.GetNeighbor(d);
                if (neighbor == null || neighbor.Distance != int.MaxValue) {
                    continue;
                }
                if (neighbor.tileType == HexTile.TileType.water) {
                    continue;
                }
                neighbor.Distance = current.Distance + 1;
                frontier.Enqueue(neighbor);
            }
        }
    }
}
