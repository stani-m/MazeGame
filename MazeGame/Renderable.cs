using GlmSharp;
using Veldrid;

namespace MazeGame
{
    public class Renderable
    {
        public mat4 Model { get; set; }
        public RgbaFloat Color { get; set; }
        public Point[] VertexArray { get; protected set; }
        public uint[] IndexArray { get; protected set; }

        protected Renderable(RgbaFloat color)
        {
            Color = color;
            Model = mat4.Identity;
        }
    }
}