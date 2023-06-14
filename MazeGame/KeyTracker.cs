using System.Collections.Generic;
using Veldrid;

namespace MazeGame
{
    public static class KeyTracker
    {
        private static readonly HashSet<Key> PressedKeys = new HashSet<Key>();
        private static readonly HashSet<Key> JustPressedKeys = new HashSet<Key>();

        public static bool KeyPressed { get; private set; }

        public static bool IsPressed(Key key)
        {
            return PressedKeys.Contains(key);
        }

        public static bool IsJustPressed(Key key)
        {
            return JustPressedKeys.Contains(key);
        }

        public static void Update()
        {
            JustPressedKeys.Clear();
            KeyPressed = false;
        }

        public static void KeyDown(Key key)
        {
            KeyPressed = true;
            if (PressedKeys.Add(key))
            {
                JustPressedKeys.Add(key);
            }
        }

        public static void KeyUp(Key key)
        {
            PressedKeys.Remove(key);
        }
    }
}