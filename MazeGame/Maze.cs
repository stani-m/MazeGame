using System.Collections.Generic;
using System.Linq;
using GlmSharp;
using Veldrid;

namespace MazeGame
{
    public class Maze
    {
        public const int BlockSize = 10;
        
        public ushort[,] Map { get; }
        public (int x, int y) Start { get; }
        public (int x, int y) End { get; }

        public int Width { get; }
        public int Height { get; }

        private List<Rectangle> _rectangles;

        public Maze(ushort[,] map, (int x, int y) start, (int x, int y) end)
        {
            Map = map;
            Start = start;
            End = end;
            Width = Map.GetLength(0);
            Height = Map.GetLength(1);
            _rectangles = new List<Rectangle>();
        }

        public void GenerateRectangles(Renderer renderer)
        {
            var rectangles = new Rectangle[Width, Height];
            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    if (Map[x, y] == 1)
                    {
                        var rectangle = new Rectangle(BlockSize, BlockSize, RgbaFloat.Cyan)
                        {
                            Model = mat4.Translate(BlockSize * x, BlockSize * y, 0)
                        };
                        renderer.RegisterRenderable(rectangle);
                        rectangles[x, y] = rectangle;
                        _rectangles.Add(rectangle);
                    }
                }
            }
            
            rectangles[Start.x, Start.y].Color = RgbaFloat.Pink;
            rectangles[End.x, End.y].Color = RgbaFloat.Pink;
        }

        public void Render(Renderer renderer)
        {
            foreach (var rectangle in _rectangles)
            {
                renderer.Render(rectangle);
            }
        }
    }
}