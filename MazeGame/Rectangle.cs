using Veldrid;

namespace MazeGame
{
    public class Rectangle : Renderable
    {
        
        public Rectangle(uint width, uint height, RgbaFloat color) : base(color)
        {
            VertexArray = new[]
            {
                new Point(0, 0),
                new Point(0, height),
                new Point(width, 0),
                new Point(width, height)
            };
            IndexArray = new uint[] {0, 1, 2, 3};
        }
    }
}