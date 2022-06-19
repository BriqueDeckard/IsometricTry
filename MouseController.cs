//------------------------------------------------------------------------------
// Written by
//
// |)  .     _   |\ _  _|      |
// |)|`|(|L|(/_  |/(/_(_|<(||`(|
//
// ~ 2022
//------------------------------------------------------------------------------
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ArrowTranslator;

/// <summary>
/// Controller for mouse.
/// </summary>
public class MouseController : MonoBehaviour
{
    /// <summary>
    /// Cursor for the mouse.
    /// </summary>
    public GameObject cursor;

    /// <summary>
    /// Speed of the character.
    /// </summary>
    public float speed;

    /// <summary>
    /// The character to instantiate.
    /// </summary>
    public GameObject characterPrefab;

    /// <summary>
    ///  The character "template".
    /// </summary>
    private CharacterInfo character;

    /// <summary>
    /// The A* pathfinder.
    /// </summary>
    private PathFinder pathFinder;

    /// <summary>
    /// The range finder.
    /// </summary>
    private RangeFinder rangeFinder;

    /// <summary>
    /// The arrow translator.
    /// </summary>
    private ArrowTranslator arrowTranslator;

    /// <summary>
    /// Tiles in the path.
    /// </summary>
    private List<OverlayTile> path;

    /// <summary>
    /// Tiles in the range.
    /// </summary>
    private List<OverlayTile> inRangeTiles;

    /// <summary>
    /// Does the character is moving ?
    /// </summary>
    private bool isMoving = false;

    /// <summary>
    /// Step to move.
    /// </summary>
    public float step { get => speed * Time.deltaTime; }

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    private void Start()
    {
        pathFinder = new PathFinder();
        rangeFinder = new RangeFinder();
        arrowTranslator = new ArrowTranslator();

        path = new List<OverlayTile>();
        isMoving = false;
        inRangeTiles = new List<OverlayTile>();
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    private void LateUpdate()
    {
        RaycastHit2D? hit = GetFocusedOnTile();

        // If there is a click.
        if (hit.HasValue)
        {
            // Get the collider at the click point, and get the associated OverlayTile.
            OverlayTile tile = hit.Value.collider.gameObject.GetComponent<OverlayTile>();

            // Set the cursor position and sorting order
            cursor.transform.position = tile.transform.position;
            cursor.gameObject.GetComponent<SpriteRenderer>().sortingOrder = tile.transform.GetComponent<SpriteRenderer>().sortingOrder;

            // If the current tile is in the range and the character is not moving.
            if (inRangeTiles.Contains(tile) && !isMoving)
            {
                path = pathFinder.FindPath(character.standingOnTile, tile, inRangeTiles);

                foreach (var item in inRangeTiles)
                {
                    MapManager.Instance.map[item.grid2DLocation].SetSprite(ArrowDirection.None);
                    //item.SetArrowSprite(ArrowDirection.None);
                }

                for (int i = 0; i < path.Count; i++)
                {
                    // i > 0 == if it is not the first tile -> return the previous tile, else return the current tile
                    var previousTile = i > 0 ? path[i - 1] : character.standingOnTile;
                    // i > path.Count -1  == if we are at the penultimate, take the last
                    var futureTile = i < path.Count - 1 ? path[i + 1] : null;

                    var arrowDir = arrowTranslator.TranslateDirection(previousTile, path[i], futureTile);
                    path[i].SetSprite(arrowDir);
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                tile.ShowTile();
                // Character instantiation.
                if (character == null)
                {
                    // Instantiate the prefab and get the character.
                    character = Instantiate(characterPrefab).GetComponent<CharacterInfo>();
                    PositionCharacterOnTile(tile);
                    GetInRangeTiles();
                }
                else
                {
                    isMoving = true;
                    tile.gameObject.GetComponent<OverlayTile>().HideTile();
                }
            }
        }
        // If the path is marked and the character is not moving.
        if (path.Count > 0 && isMoving)
        {
            MoveAlongPath();
        }
    }

    /// <summary>
    /// Move the character along the path
    /// </summary>
    private void MoveAlongPath()
    {
        var targetZIndex = path[0].transform.position.z;

        character.transform.position = Vector2.MoveTowards(character.transform.position, path[0].transform.position, step);
        character.transform.position = new Vector3(character.transform.position.x, character.transform.position.y, targetZIndex);

        // If the character has arrived at the destination, it is placed on the tile.
        if (Vector2.Distance(character.transform.position, path[0].transform.position) < 0.00001f)
        {
            PositionCharacterOnTile(path[0]);
            path.RemoveAt(0);
        }
        // If the path is covered, we look for the tiles in range.
        if (path.Count == 0)
        {
            GetInRangeTiles();
            isMoving = false;
        }
    }

    /// <summary>
    /// Position the character on the tile.
    /// </summary>
    /// <param name="tile"></param>
    private void PositionCharacterOnTile(OverlayTile tile)
    {
        // get the coordinates
        float x = tile.transform.position.x;
        float y = tile.transform.position.y - 0.0001f;
        float z = tile.transform.position.z;
        // move the character to the tile.
        character.transform.position = new Vector3(x, y, z);
        character.GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder;
        character.standingOnTile = tile;
    }

    /// <summary>
    /// Check if there are colliders around the mouse click position.
    /// </summary>
    /// <returns></returns>
    public RaycastHit2D? GetFocusedOnTile()
    {
        // Get the mouse position
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePosition2D = new Vector2(mousePosition.x, mousePosition.y);

        // Casts a ray against Colliders in the Scene, returning all Colliders that contact.
        // with it.
        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePosition2D, Vector2.zero);

        // If there is more than 0 colliders, return the first.
        if (hits.Length > 0)
        {
            return hits.OrderByDescending(i => i.collider.transform.position.z).First();
        }

        return null;
    }

    private void GetInRangeTiles()
    {
        inRangeTiles = rangeFinder.GetTilesInRange(new Vector2Int(character.standingOnTile.gridLocation.x, character.standingOnTile.gridLocation.y), 3);

        foreach (var item in inRangeTiles)
        {
            item.ShowTile();
        }
    }
}