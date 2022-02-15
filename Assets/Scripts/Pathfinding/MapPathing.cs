using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Code from https://www.youtube.com/watch?v=alU04hvz6L4

public class MapPathing
{
    private MapGrid<MapPathNode> grid;
    private List<MapPathNode> openList;
    private List<MapPathNode> closedList;

    public MapPathing(Tilemap tilemap)
    {
        grid = new MapGrid<MapPathNode>(tilemap, 10f, (MapGrid<MapPathNode> g, int x, int y, int m) => new MapPathNode(g, x, y, true, 1));
    }

    public List<Vector2Int> GetMovementRange(int startX, int startY, int movement)
    {
        List<Vector2Int> results = new List<Vector2Int>();

        Vector3Int cube = OffsetToCube(new Vector2Int(startX, startY));
        /*for (int q = -movement; q <= movement; q++)
        {
            for (int r = Mathf.Max(-movement, -q - movement); r <= Mathf.Min(movement, -q + movement); r++)
            {
                int s = -q - r + 1;
                Vector3Int mov = new Vector3Int(r, q, s);

                Vector3Int newCube = cube + mov;
                Vector2Int result = CubeToOffset(newCube);

                if (IsValid(result))
                {
                    results.Add(result);
                }                
            }
        }

        for (int q = -movement; q <= movement; q++)
        {
            for (int r = -movement; r <= movement; r++)
            {
                for (int s = -movement; s <= movement; s++)
                {
                    if (q+r+s == 0)
                    {                        
                        Vector3Int mov = new Vector3Int(r, q, s);

                        Vector3Int newCube = CubeAdd(cube, mov);
                        Debug.Log(newCube);

                        Vector2Int result = CubeToOffset(newCube);

                        if (IsValid(result))
                        {
                            results.Add(result);
                        }
                    }
                }
            }
        }*/

        // Super inefficient method because I'm too mad to get this to work.
        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                results.Add(new Vector2Int(x, y));
            }
        }

        List<Vector2Int> tiles = new List<Vector2Int>();
        foreach (Vector2Int result in results)
        {
            int endX = result.x;
            int endY = result.y;
            List<MapPathNode> path = FindPath(startX, startY, endX, endY);
            if (path[path.Count - 1].gCost <= movement)
            {
                tiles.Add(new Vector2Int(endX, endY));
            }                
        }        

        return tiles;
    }

    public List<MapPathNode> FindPath(int startX, int startY, int endX, int endY)
    {
        //if (GameManager.Instance.GetMapManager().GetUnitAtPosition(new Vector2Int(endX, endY)) != null) { return null; }

        MapPathNode startNode = grid.GetValue(startX, startY);
        MapPathNode endNode = grid.GetValue(endX, endY);        

        openList = new List<MapPathNode> { startNode };
        closedList = new List<MapPathNode>();

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                MapPathNode pathNode = grid.GetValue(x, y);
                pathNode.gCost = int.MaxValue;
                pathNode.CalculateFCost();
                pathNode.cameFromNode = null;
            }
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        while (openList.Count > 0)
        {
            MapPathNode currentNode = GetLowestFCostNode(openList);
            if (currentNode == endNode)
            {
                // Reached final node
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (MapPathNode neighbourNode in GetNeighbourList(currentNode))
            {
                if (closedList.Contains(neighbourNode)) continue;
                if (!neighbourNode.isWalkable)
                {
                    closedList.Add(neighbourNode);
                }

                int tentativeGCost = currentNode.gCost + currentNode.movementCost;//CalculateDistanceCost(currentNode, neighbourNode);
                if (tentativeGCost < neighbourNode.gCost)
                {
                    neighbourNode.cameFromNode = currentNode;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                    neighbourNode.CalculateFCost();
                    //if (maxMoves >= 0 && neighbourNode.gCost > maxMoves) continue;
                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }

        // Out of nodes on the open list
        return null;
    }


    public int GetDistance(int startX, int startY, int endX, int endY)
    {
        List<MapPathNode> path = FindPath(startX, startY, endX, endY);
        MapPathNode node = path[path.Count - 1];
        return node.fCost;
    }
    private List<MapPathNode> GetNeighbourList(MapPathNode currentNode)
    {
        List<MapPathNode> neighbourList = new List<MapPathNode>();
        Vector2Int[] evenNeighbours =
            {
            new Vector2Int(1, 0),   new Vector2Int(0, 1),    new Vector2Int(-1, 1),
            new Vector2Int(-1, 0),  new Vector2Int(-1, -1),   new Vector2Int(0, -1)
        };

        Vector2Int[] oddNeighbours = 
            {
            new Vector2Int(1, 0),   new Vector2Int(1, 1),  new Vector2Int(0, 1),
            new Vector2Int(-1, 0),  new Vector2Int(0, -1),   new Vector2Int(1, -1)
        };

        bool IsValid(MapPathNode node, Vector2Int direction)
        {
            if (node.x + direction.x < 0 || node.x + direction.x >= grid.GetWidth()) return false;
            if (node.y + direction.y < 0 || node.y + direction.y >= grid.GetHeight()) return false;
            return true;
        }

        if (currentNode.y % 2 == 0)
        {
            foreach (Vector2Int vector in evenNeighbours)
            {
                if (IsValid(currentNode, vector)) {
                    neighbourList.Add(grid.GetValue(currentNode.x + vector.x, currentNode.y + vector.y));
                }
            }
        }
        else
        {
            foreach (Vector2Int vector in oddNeighbours)
            {
                if (IsValid(currentNode, vector))
                {
                    neighbourList.Add(grid.GetValue(currentNode.x + vector.x, currentNode.y + vector.y));
                }
            }
        }

        return neighbourList;
    }

    private List<MapPathNode> CalculatePath(MapPathNode endNode)
    {
        List<MapPathNode> path = new List<MapPathNode>();
        path.Add(endNode);
        MapPathNode currentNode = endNode;
        while (currentNode.cameFromNode != null)
        {
            path.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;
        }
        path.Reverse();

        return path;
    }

    Vector3Int OffsetToCube(Vector2Int offset)
    {
        Vector3Int AxialToCube(Vector2Int axial)
        {
            int y = axial.y;
            int x = axial.x;
            int z = -axial.y - axial.x;

            Vector3Int cube = new Vector3Int(x, y, z);
            return cube;
        }
        Vector2Int OffsetToAxial(Vector2Int offset)
        {
            int y = offset.y - (offset.x - (offset.x & 1)) / 2;
            int x = offset.x;
            return new Vector2Int(x, y);
        }

        Vector2Int axial = OffsetToAxial(offset);
        Vector3Int cube = AxialToCube(axial);
        return cube;
    }

    Vector2Int CubeToOffset(Vector3Int cube)
    {
        Vector2Int CubeToAxial(Vector3Int cube)
        {
            int y = cube.y;
            int x = cube.x;
            Vector2Int Axial = new Vector2Int(x, y);
            return Axial;
        }

        Vector2Int AxialToOffset(Vector2Int axial)
        {
            int col = axial.y + (axial.x - (axial.x & 1)) / 2;
            int row = axial.x;

            Vector2Int offset = new Vector2Int(row, col);
            return offset;
        }

        Vector2Int axial = CubeToAxial(cube);
        Vector2Int offset = AxialToOffset(axial);
        return offset;
    }

    private Vector3Int CubeAdd(Vector3Int a, Vector3Int b)
    {
        Vector3Int add = new Vector3Int(a.x + b.x, a.y + b.y, a.z + b.z);
        return add;
    } 

    private int CalculateDistanceCost(MapPathNode a, MapPathNode b)
    {        
        Vector3Int CubeSubtract(Vector3Int a, Vector3Int b)
        {
            return new Vector3Int(a.y - b.y, a.x - b.x, a.z - b.z);
        }
        int CubeDistance(Vector3Int a, Vector3Int b)
        {
            Vector3Int vec = CubeSubtract(a, b);
            return (Mathf.Abs(vec.y) + Mathf.Abs(vec.x) + Mathf.Abs(vec.z)) / 2;
        }

        Vector3Int ac = OffsetToCube(new Vector2Int(a.x, a.y));
        Vector3Int bc = OffsetToCube(new Vector2Int(b.x, b.y));
        return CubeDistance(ac, bc);
    }

    private MapPathNode GetLowestFCostNode(List<MapPathNode> pathNodeList)
    {
        MapPathNode lowestFCostNode = pathNodeList[0];
        for (int i = 1; i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = pathNodeList[i];
            }
        }

        return lowestFCostNode;
    }

}