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

/// <summary>
/// Handles the A* path finding logic.
/// </summary>
public class PathFinder
{
    private Dictionary<Vector2Int, OverlayTile> searchableTiles;

    /// <summary>
    /// For each tile: check its neighbors, and calculate their GHF
    /// </summary>
    /// <param name="startingTile">The beginning node</param>
    /// <param name="endingTile">The final node</param>
    /// <returns></returns>
    public List<OverlayTile> FindPath(OverlayTile startingTile, OverlayTile endingTile, List<OverlayTile> inRangeTiles)
    {
        searchableTiles = new Dictionary<Vector2Int, OverlayTile>();

        // the tiles we want to check in the next iteration of the loop
        List<OverlayTile> openTileList = new List<OverlayTile>();
        // The already checked tiles
        HashSet<OverlayTile> closedTileList = new HashSet<OverlayTile>();
        //
        Func<OverlayTile, bool> endChecker = (tile) => tile == endingTile;

        Action<OverlayTile> setTileProcessed = (tile) =>
        {
            openTileList.Remove(tile);
            closedTileList.Add(tile);
        };

        #region preprocessing

        if (inRangeTiles.Count > 0)
        {
            foreach (var item in inRangeTiles)
            {
                searchableTiles.Add(item.grid2DLocation, MapManager.Instance.map[item.grid2DLocation]);
            }
        }
        else
        {
            searchableTiles = MapManager.Instance.map;
        }

        #endregion preprocessing

        // -- START
        openTileList.Add(startingTile);

        #region processing

        while (openTileList.Count > 0)
        {
            // Order the tiles with FCost and take the first
            OverlayTile currentOverlayTile = openTileList.OrderBy(x => x.FCost).First();

            // remove from openList, add to closedList
            setTileProcessed.Invoke(currentOverlayTile);

            if (endChecker.Invoke(currentOverlayTile))
            {
                return GetTheReorderedList(startingTile, endingTile);
            }

            // Can jump ?
            Func<OverlayTile, bool> canJump = (tile) => (Mathf.Abs(currentOverlayTile.transform.position.z - tile.transform.position.z) > 1);
            Func<OverlayTile, bool> checkAvailability = (tile) => ((tile.isBlocked) || (closedTileList.Contains(tile)) || (canJump.Invoke(tile)));

            foreach (var tile in GetNeightbourOverlayTiles(currentOverlayTile))
            {
                if (checkAvailability.Invoke(tile))
                {
                    continue;
                }
                tile.GCost = GetManhattanDistance(startingTile, tile);
                tile.HCost = GetManhattanDistance(endingTile, tile);

                tile.previous = currentOverlayTile;

                if (!openTileList.Contains(tile))
                {
                    openTileList.Add(tile);
                }
            }
        }
        return new List<OverlayTile>();

        #endregion processing
    }


    /// <summary>
    /// Redo the path in reverse to ... put it right side up.
    /// </summary>
    /// <param name="startingNode"></param>
    /// <param name="endingNode"></param>
    /// <returns></returns>
    private List<OverlayTile> GetTheReorderedList(OverlayTile startingNode, OverlayTile endingNode)
    {
        // Start from the end and then check if we are back to the beginning
        List<OverlayTile> finishedList = new List<OverlayTile>();
        OverlayTile currentTile = endingNode;
        while (currentTile != startingNode)
        {
            // Add the current tile and engage the previous one in the system
            finishedList.Add(currentTile);
            currentTile = currentTile.previous;
        }
        // Finally reverse the list
        finishedList.Reverse();
        return finishedList;
    }

    /// <summary>
    /// Get the Manhattan distance between the starting node and its neighbors.
    /// (x - x') + (y - y')
    /// </summary>
    /// <param name="startingNode"></param>
    /// <param name="overlayTile"></param>
    /// <param name="neighbor"></param>
    /// <returns></returns>
    private int GetManhattanDistance(OverlayTile startingNode, OverlayTile neighbor)
    {
        return Mathf.Abs(startingNode.gridLocation.x - neighbor.gridLocation.x) + Mathf.Abs(startingNode.gridLocation.y - neighbor.gridLocation.y);
    }

    private List<OverlayTile> GetNeightbourOverlayTiles(OverlayTile currentOverlayTile)
    {
        if (currentOverlayTile == null)
        {
            Debug.LogError("CurrentOverLay tile is null");
        }

        var map = MapManager.Instance.map;

        List<OverlayTile> neighbours = new List<OverlayTile>();

        //right
        Vector2Int locationToCheck = new Vector2Int(
            currentOverlayTile.gridLocation.x + 1,
            currentOverlayTile.gridLocation.y
        );

        if (searchableTiles.ContainsKey(locationToCheck))
        {
            neighbours.Add(searchableTiles[locationToCheck]);
        }

        //left
        locationToCheck = new Vector2Int(
            currentOverlayTile.gridLocation.x - 1,
            currentOverlayTile.gridLocation.y
        );

        if (searchableTiles.ContainsKey(locationToCheck))
        {
            neighbours.Add(searchableTiles[locationToCheck]);
        }

        //top
        locationToCheck = new Vector2Int(
            currentOverlayTile.gridLocation.x,
            currentOverlayTile.gridLocation.y + 1
        );

        if (searchableTiles.ContainsKey(locationToCheck))
        {
            neighbours.Add(searchableTiles[locationToCheck]);
        }

        //bottom
        locationToCheck = new Vector2Int(
            currentOverlayTile.gridLocation.x,
            currentOverlayTile.gridLocation.y - 1
        );

        if (searchableTiles.ContainsKey(locationToCheck))
        {
            neighbours.Add(searchableTiles[locationToCheck]);
        }

        return neighbours;
    }
}