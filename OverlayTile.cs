//------------------------------------------------------------------------------
// Written by
//
// |)  .     _   |\ _  _|      |
// |)|`|(|L|(/_  |/(/_(_|<(||`(|
//
// ~ 2022
//------------------------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;
using static ArrowTranslator;

/// <summary>
///  Node of A*.
/// </summary>
public class OverlayTile : MonoBehaviour
{
    /// <summary>
    /// G-Cost : The distance from the starting node
    /// </summary>
    public int GCost;

    /// <summary>
    /// H-Cost : Distance from end node.
    /// </summary>
    public int HCost;

    /// <summary>
    /// F is G + H
    /// </summary>
    public int FCost
    { get { return GCost + HCost; } }

    /// <summary>
    /// TODO: Fill
    /// </summary>
    public bool isBlocked;

    /// <summary>
    /// Previous tile in the process.
    /// </summary>
    public OverlayTile previous;

    /// <summary>
    /// Grid localion of the tile.
    /// </summary>
    public Vector3Int gridLocation;

    /// <summary>
    /// Grid 2D location of the tile.
    /// </summary>
    public Vector2Int grid2DLocation
    { get { return new Vector2Int(gridLocation.x, gridLocation.y); } }

    public List<Sprite> arrows;

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HideTile();
        }
    }

    /// <summary>
    /// Show the tile.
    /// </summary>
    public void ShowTile()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
    }

    /// <summary>
    /// Hide the tile.
    /// </summary>
    public void HideTile()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
    }

    /// <summary>
    /// Set the arrow sprite in the right direction on the top of a tile.
    /// </summary>
    /// <param name="direction"></param>
    public void SetSprite(ArrowDirection direction)
    {
        
        // Default
        if (direction == ArrowDirection.None)
        {
            GetComponentsInChildren<SpriteRenderer>()[1].color = new Color(1, 1, 1, 0);
        }
        // We should set a value
        
        else
        {
            // Set visible
            GetComponentsInChildren<SpriteRenderer>()[1].color = new Color(1, 1, 1, 1);
            // Set direction
            GetComponentsInChildren<SpriteRenderer>()[1].sprite = arrows[(int)direction];
            // Set sorting order
            GetComponentsInChildren<SpriteRenderer>()[1].sortingOrder = GetComponent<SpriteRenderer>().sortingOrder;
        }
        
    }
}