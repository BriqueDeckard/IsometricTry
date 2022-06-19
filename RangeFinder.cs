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
/// Find the tiles in the range to move.
/// </summary>
public class RangeFinder
{
    /// <summary>
    /// Find the tiles within the range for moving
    /// </summary>
    /// <param name="startingTile"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public List<OverlayTile> GetTilesInRange(Vector2Int location, int range)
    {
        var startingTile = MapManager.Instance.map[location];
        int stepCount = 0;

        List<OverlayTile> inRangeTiles = new() { startingTile };

        List<OverlayTile> tileForPreviousStep = new() { startingTile };

        // Loop in our map and find all the tiles within range
        while (stepCount < range)
        {
            List<OverlayTile> surroundingTiles = new List<OverlayTile>();             

            foreach (OverlayTile tile in tileForPreviousStep)
            {
                surroundingTiles.AddRange(MapManager.Instance.GetSurroundingTiles(new Vector2Int(tile.gridLocation.x, tile.gridLocation.y)));
            }

            inRangeTiles.AddRange(surroundingTiles);

            // Thanks to tileForPreviousStep we won't check the already checked tiles
            tileForPreviousStep = surroundingTiles.Distinct().ToList();
            stepCount++;
        }

        return inRangeTiles.Distinct().ToList();
    }
}