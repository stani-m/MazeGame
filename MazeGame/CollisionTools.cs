namespace MazeGame
{
    public struct Collider
    {
        public float X;
        public float Y;
        public float Width;
        public float Height;

        public Collider(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public bool CollidesWith(Collider other)
        {
            // I have copied this collision detection code from here:
            // https://developer.mozilla.org/en-US/docs/Games/Techniques/2D_collision_detection
            // And modified it to work in my project
            return X < other.X + other.Width && X + Width > other.X &&
                   Y < other.Y + other.Height && Y + Height > other.Y;
        }
    }

    public static class CollisionTools
    {
        public static Collider GenerateColliderAt(this Maze _, int x, int y)
        {
            return new(Maze.BlockSize * x, Maze.BlockSize * y, Maze.BlockSize, Maze.BlockSize);
        }

        public static bool CheckCollision(this Maze maze, Collider collider)
        {
            for (int x = 0; x < maze.Width; x++)
            {
                for (int y = 0; y < maze.Height; y++)
                {
                    if (maze.Map[x, y] == 0 && maze.GenerateColliderAt(x, y).CollidesWith(collider))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}