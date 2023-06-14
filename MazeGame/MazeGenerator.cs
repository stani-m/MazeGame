using System;
using System.Collections.Generic;

namespace MazeGame
{
    public class MazeGenerator
    {
        private readonly int _width;
        private readonly int _height;
        private readonly Random _random;

        public MazeGenerator(int width, int height)
        {
            _width = width;
            _height = height;
            _random = new Random();
        }

        public Maze GenerateMaze()
        {
            var maze = new ushort[_width, _height];
            maze[0, 0] = 1;
            var possiblePathLocations = new List<(int, int)>();
            var lastX = 0;
            var lastY = 0;
            while (true)
            {
                possiblePathLocations.Clear();
                for (int x = 0; x < _width; x++)
                {
                    for (int y = 0; y < _height; y++)
                    {
                        if (GetNumberOfNeighborPaths(x, y) == 1)
                        {
                            possiblePathLocations.Add((x, y));
                        }
                    }
                }

                if (possiblePathLocations.Count == 0)
                {
                    break;
                }
                else
                {
                    var (x, y) = possiblePathLocations[_random.Next(possiblePathLocations.Count)];
                    maze[x, y] = 1;
                    lastX = x;
                    lastY = y;
                }
            }

            return new Maze(maze, (0, 0), (lastX, lastY));

            int GetNumberOfNeighborPaths(int x, int y)
            {
                var n = 0;
                if (maze[x, y] == 0)
                {
                    if (x + 1 < _width && maze[x + 1, y] == 1)
                    {
                        n++;
                    }

                    if (x - 1 >= 0 && maze[x - 1, y] == 1)
                    {
                        n++;
                    }

                    if (y + 1 < _height && maze[x, y + 1] == 1)
                    {
                        n++;
                    }

                    if (y - 1 >= 0 && maze[x, y - 1] == 1)
                    {
                        n++;
                    }
                }

                return n;
            }
        }
    }
}