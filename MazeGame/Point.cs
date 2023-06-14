using System.Numerics;

namespace MazeGame
{
    public struct Point
    {
        public const uint SizeInBytes = 8;
        
        public Vector2 Coordinates { get; set; }

        public Point(float x, float y)
        {
            Coordinates = new Vector2(x, y);
        }

        public Point(Vector2 coordinates)
        {
            Coordinates = coordinates;
        }
    }
}