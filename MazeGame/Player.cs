using System;
using System.Numerics;
using GlmSharp;
using Veldrid;

namespace MazeGame
{
    public class Player
    {
        public const int Size = Maze.BlockSize / 2;
        public const float Speed = 20f;
        
        private readonly Rectangle _rectangle;
        private readonly Maze _maze;
        private readonly Vector2 _startingPosition;
        
        public Vector2 Position { get; private set; }

        public Player(Renderer renderer, Maze maze, Vector2 position)
        {
            Position = position;
            _startingPosition = position;
            _rectangle = new Rectangle(Size, Size, new RgbaFloat(1, 0, 1, 1))
            {
                Model = mat4.Translate(Position.X, Position.Y, 0)
            };
            _maze = maze;
            renderer.RegisterRenderable(_rectangle);
        }

        public void Render(Renderer renderer)
        {
            renderer.Render(_rectangle);
        }

        public void Update(float deltaTime)
        {
            var newX = Position.X;
            var newY = Position.Y;

            if (KeyTracker.IsPressed(Key.Space))
            {
                Position = _startingPosition;
            }
            else
            {
                if (KeyTracker.IsPressed(Key.W) | KeyTracker.IsPressed(Key.Up))
                {
                    newY += Speed * deltaTime;
                }
                if (KeyTracker.IsPressed(Key.S) | KeyTracker.IsPressed(Key.Down))
                {
                    newY -= Speed * deltaTime;
                }
                if (KeyTracker.IsPressed(Key.D) | KeyTracker.IsPressed(Key.Right))
                {
                    newX += Speed * deltaTime;
                }
                if (KeyTracker.IsPressed(Key.A) | KeyTracker.IsPressed(Key.Left))
                {
                    newX -= Speed * deltaTime;
                }
                
                if (_maze.CheckCollision(new Collider(newX, Position.Y, Size, Size)))
                {
                    newX = Position.X;
                }
                if (_maze.CheckCollision(new Collider(Position.X, newY, Size, Size)))
                {
                    newY = Position.Y;
                }
                
                newX = Math.Clamp(newX, 0, _maze.Width * Maze.BlockSize - Size);
                newY = Math.Clamp(newY, 0, _maze.Height * Maze.BlockSize - Size);
                Position = new Vector2(newX, newY);
            }

            _rectangle.Model = mat4.Translate(Position.X, Position.Y, 0);
        }
    }
}