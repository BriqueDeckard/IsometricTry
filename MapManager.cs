//------------------------------------------------------------------------------
// Written by
//
// |)  .     _   |\ _  _|      |
// |)|`|(|L|(/_  |/(/_(_|<(||`(|
//
// ~ 2022
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Handles the map
/// </summary>
public class MapManager : MonoBehaviour
{
    /// <summary>
    ///  By this way our instance is immutable
    /// </summary>
    private static MapManager _instance;

    /// <summary>
    ///  By this way our instance is immutable
    /// </summary>
    public static MapManager Instance
    {
        get
        {
            return _instance;
        }
    }

    /// <summary>
    /// The overlay prefab
    /// </summary>
    public GameObject overlayPrefab;

    /// <summary>
    /// The overlay container
    /// </summary>
    public GameObject overlayContainer;

    public Dictionary<Vector2Int, OverlayTile> map;
    public bool ignoreBottomTiles;

    /// <summary>
    /// Called when the instance is loaded
    /// </summary>
    private void Awake()
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

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    private void Start()
    {
        var tileMaps = gameObject.transform.GetComponentsInChildren<Tilemap>().OrderByDescending(x => x.GetComponent<TilemapRenderer>().sortingOrder);
        map = new Dictionary<Vector2Int, OverlayTile>();

        foreach (var tileMap in tileMaps)
        {
            BoundsInt bounds = tileMap.cellBounds;

            for (int z = bounds.max.z; z >= bounds.min.z; z--)
            {
                for (int y = bounds.min.y; y < bounds.max.y; y++)
                {
                    for (int x = bounds.min.x; x < bounds.max.x; x++)
                    {
                        if (z == 0 && ignoreBottomTiles)
                            return;

                        if (tileMap.HasTile(new Vector3Int(x, y, z)))
                        {
                            if (!map.ContainsKey(new Vector2Int(x, y)))
                            {
                                var overlayTile = Instantiate(overlayPrefab, overlayContainer.transform);
                                // Get the center of the cell's coordinates
                                var cellWorldPosition = tileMap.GetCellCenterWorld(new Vector3Int(x, y, z));

                                // z+1 because the tile is on top
                                overlayTile.transform.position = new Vector3(cellWorldPosition.x, cellWorldPosition.y, cellWorldPosition.z + 1);

                                // Set the overlay tile sorting order.
                                overlayTile.GetComponent<SpriteRenderer>().sortingOrder = tileMap.GetComponent<TilemapRenderer>().sortingOrder;

                                overlayTile.gameObject.GetComponent<OverlayTile>().gridLocation = new Vector3Int(x, y, z); ;

                                map.Add(new Vector2Int(x, y), overlayTile.gameObject.GetComponent<OverlayTile>());
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Check the
    /// </summary>
    /// <param name="currentOverlayTile"></param>
    /// <returns></returns>
    public List<OverlayTile> GetSurroundingTiles(Vector2Int originTile)
    {
        var surroundingTiles = new List<OverlayTile>();
        /*
        Func<int, int, Vector2Int> locationToCheckProducer = (x, y) => new Vector2Int(originTile.x + x, originTile.y + y);
        Func<Vector2Int, bool> heightCheck = (tileToCheck) => (Mathf.Abs(map[tileToCheck].transform.position.z - map[originTile].transform.position.z) <= 1);
        Action<Vector2Int> checkTile = (tileToCheck) =>
        {
            if (map.ContainsKey(tileToCheck))
            {
                if (heightCheck.Invoke(tileToCheck))
                {
                    surroundingTile.Add(map[tileToCheck]);
                }
            }
        };

        // BOTTOM
        checkTile.Invoke(locationToCheckProducer.Invoke(1, 0));
        checkTile.Invoke(locationToCheckProducer.Invoke(-1, 0));
        // TOP
        checkTile.Invoke(locationToCheckProducer.Invoke(0, 1));
        checkTile.Invoke(locationToCheckProducer.Invoke(0, -1));
        */

        Vector2Int TileToCheck = new Vector2Int(originTile.x + 1, originTile.y);
        if (map.ContainsKey(TileToCheck))
        {
            if (Mathf.Abs(map[TileToCheck].transform.position.z - map[originTile].transform.position.z) <= 1)
                surroundingTiles.Add(map[TileToCheck]);
        }

        TileToCheck = new Vector2Int(originTile.x - 1, originTile.y);
        if (map.ContainsKey(TileToCheck))
        {
            if (Mathf.Abs(map[TileToCheck].transform.position.z - map[originTile].transform.position.z) <= 1)
                surroundingTiles.Add(map[TileToCheck]);
        }

        TileToCheck = new Vector2Int(originTile.x, originTile.y + 1);
        if (map.ContainsKey(TileToCheck))
        {
            if (Mathf.Abs(map[TileToCheck].transform.position.z - map[originTile].transform.position.z) <= 1)
                surroundingTiles.Add(map[TileToCheck]);
        }

        TileToCheck = new Vector2Int(originTile.x, originTile.y - 1);
        if (map.ContainsKey(TileToCheck))
        {
            if (Mathf.Abs(map[TileToCheck].transform.position.z - map[originTile].transform.position.z) <= 1)
                surroundingTiles.Add(map[TileToCheck]);
        }

        return surroundingTiles;
    }
}