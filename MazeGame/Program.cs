using System;
using System.Numerics;
using GlmSharp;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace MazeGame
{
    class Program
    {
        public const int MazeWidth = 50;
        public const int MazeHeight = 50;
        public const int WindowSize = 540;
        
        private static Sdl2Window _window;
        private static Maze _maze;
        private static GraphicsDevice _graphicsDevice;
        private static Renderer _renderer;
        private static Player _player;

        static void Main(string[] args)
        {
            _maze = new MazeGenerator(MazeWidth, MazeHeight).GenerateMaze();
            
            _window = CreateWindow();
            _graphicsDevice = InitializeGraphicsDevice(_window);
            _renderer = new Renderer(_graphicsDevice, _window.Bounds, RgbaFloat.Blue);
            _player = new Player(_renderer, _maze, new Vector2(Maze.BlockSize / 4f, Maze.BlockSize / 4f));

            _maze.GenerateRectangles(_renderer);
            
            _window.Resized += () => _renderer.WindowResized(_window.Bounds);
            _window.MouseMove += _ => _window.CursorVisible = true;
            _window.KeyDown += keyEvent => KeyTracker.KeyDown(keyEvent.Key);
            _window.KeyUp += keyEvent => KeyTracker.KeyUp(keyEvent.Key);
            
            var loopStart = DateTime.Now;
            var lastFrameTime = (DateTime.Now - loopStart).TotalSeconds;
            while (_window.Exists)
            {
                var currentFrameTime = (DateTime.Now - loopStart).TotalSeconds;
                var deltaTime = (float) (currentFrameTime - lastFrameTime);

                _window.PumpEvents();

                UpdateWindow();
                _player.Update(deltaTime);

                var playerPosition = _player.Position;
                var scaleFactor = (_window.Width + _window.Height) / 200f;
                var view = mat4.Translate(
                               _window.Width / 2f - (playerPosition.X + Player.Size / 2f) * scaleFactor,
                               _window.Height / 2f - (playerPosition.Y + Player.Size / 2f) * scaleFactor,
                               -100) * mat4.Scale(scaleFactor);

                _renderer.BeginRender(view);
                _maze.Render(_renderer);
                _player.Render(_renderer);
                _renderer.EndRender();

                KeyTracker.Update();
                lastFrameTime = currentFrameTime;
            }

            _renderer.Dispose();
            _graphicsDevice.Dispose();
        }

        private static Sdl2Window CreateWindow()
        {
            var windowCreateInfo = new WindowCreateInfo
            {
                X = 100,
                Y = 100,
                WindowWidth = WindowSize,
                WindowHeight = WindowSize,
                WindowTitle = "Maze"
            };

            var window = VeldridStartup.CreateWindow(ref windowCreateInfo);
            window.CursorVisible = false;
            return window;
        }

        private static void UpdateWindow()
        {
            if (KeyTracker.IsPressed(Key.Escape))
            {
                _window.Close();
            }

            if (KeyTracker.IsJustPressed(Key.F11))
            {
                _window.WindowState = _window.WindowState != WindowState.BorderlessFullScreen
                    ? WindowState.BorderlessFullScreen
                    : WindowState.Normal;
            }

            if (KeyTracker.KeyPressed)
            {
                _window.CursorVisible = false;
            }
        }

        private static GraphicsDevice InitializeGraphicsDevice(Sdl2Window window)
        {
            var options = new GraphicsDeviceOptions
            {
                PreferStandardClipSpaceYDirection = true,
                PreferDepthRangeZeroToOne = true,
                SyncToVerticalBlank = true
            };
            return VeldridStartup.CreateGraphicsDevice(window, options, GraphicsBackend.OpenGL);
        }
    }
}