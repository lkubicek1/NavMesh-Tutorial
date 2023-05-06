using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class LevelGenerator : MonoBehaviour {

	public int width = 10;
	public int height = 10;
    public float scalingFactor = 1f;

    public Material wallMaterial;

	public GameObject wall;
	public GameObject player;
	public NavMeshSurface surface;

    private const int Obstacle = 0;
    private const int Path = 1;
    private const int Player = 2;
    private const int Goal = 3;

    // Use this for initialization
    void Start () {
		GenerateLevel();
		surface.BuildNavMesh();
	}
	
	// Create a grid based level
	void GenerateLevel()
	{
        Vector2Int start = GetStart(width, height);
        int[][] maze = CreateMaze(start.x, start.y, width, height);

        Debug.Log(maze);

        // Loop over the grid
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 pos = new Vector3((x - width / 2f) * scalingFactor, 1f, (y - height / 2f) * scalingFactor);
                int mazeValue = maze[y][x];

                switch (mazeValue)
                {
                    case Obstacle:
                        // Spawn a wall
                        GameObject spawnedWall = Instantiate(wall, pos, Quaternion.identity, transform);
                        spawnedWall.transform.localScale = new Vector3(scalingFactor, spawnedWall.transform.localScale.y, scalingFactor);
                        // Apply the wall material
                        MeshRenderer wallRenderer = spawnedWall.GetComponent<MeshRenderer>();
                        if (wallRenderer != null)
                        {
                            wallRenderer.material = wallMaterial;
                        }
                        break;
                    case Player:
                        // Spawn the player
                        pos.y = 2f;
                        player.transform.position = pos;
                        break;
                    default:
                        break;

                }
            }
        }
	}

    public static Vector2Int GetStart(int width, int height)
    {
        int getRandomOdd(int limit)
        {
            int num = UnityEngine.Random.Range(0, limit);
            return num % 2 == 0 ? num + 1 : num;
        }

        return new Vector2Int(getRandomOdd(width), getRandomOdd(height));
    }

    public static int[][] CreateMaze(int startX, int startY, int width, int height)
    {
        bool playerSet = false;

        void ShuffleArray<T>(IList<T> array)
        {
            int count = array.Count;
            for (int i = count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                T temp = array[i];
                array[i] = array[j];
                array[j] = temp;
            }
        }

        int[][] grid = new int[height][];
        for (int i = 0; i < height; i++)
        {
            grid[i] = new int[width];
            for (int j = 0; j < width; j++)
            {
                grid[i][j] = Obstacle;
            }
        }

        List<Vector2Int> GetNeighbors(int x, int y)
        {
            Vector2Int[] directions = {
                new Vector2Int(-1, 0),
                new Vector2Int(1, 0),
                new Vector2Int(0, -1),
                new Vector2Int(0, 1),
            };

            List<Vector2Int> neighbors = new List<Vector2Int>();
            foreach (var dir in directions)
            {
                int nx = x + dir.x * 2;
                int ny = y + dir.y * 2;

                if (nx >= -1 && nx < width && ny >= -1 && ny < height)
                {
                    neighbors.Add(new Vector2Int(nx, ny));
                }
            }

            return neighbors;
        }

        void Visit(int x, int y)
        {
            if (!playerSet)
            {
                grid[y][x] = Player;
                playerSet = true;
            }
            else
            {
                grid[y][x] = Path;
            }

            List<Vector2Int> neighbors = GetNeighbors(x, y);
            ShuffleArray(neighbors);

            foreach (var neighbor in neighbors)
            {
                int nx = neighbor.x;
                int ny = neighbor.y;

                if (ny == -1 || nx == -1 || (grid[ny] != null && grid[ny][nx] == Obstacle))
                {
                    int midX = (x + nx) / 2;
                    int midY = (y + ny) / 2;

                    if (grid[midY] != null && grid[midY][midX] == Obstacle)
                    {
                        grid[midY][midX] = Path;
                        if (ny > 0 && nx > 0)
                        {
                            Visit(nx, ny);
                        }
                    }
                }
            }
        }

        Visit(startX, startY);

        bool goalSet = false;

        // Set the first and last rows to Path
        for (int x = 0; x < width; x++)
        {
            grid[0][x] = Path;
            grid[height - 1][x] = Path;
        }

        // Set the first and last columns to Path
        for (int y = 0; y < height; y++)
        {
            grid[y][0] = Path;
            grid[y][width - 1] = Path;
        }

        for (int i = width - 1; i > width / 2; i--)
        {
            if (grid[height - 1][i] == Path)
            {
                grid[height - 1][i] = Goal;
                goalSet = true;
                break;
            }
        }

        if (!goalSet)
        {
            for (int i = height - 1; i > height / 2; i--)
            {
                if (grid[i][width - 1] == Path)
                {
                    grid[i][width - 1] = Goal;
                    break;
                }
            }
        }

        return grid;
    }
}
