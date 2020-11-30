using System;
using System.Collections.Generic;
using System.Text;

namespace BattleTanksCommon.Network.Entities
{
    [Flags]
    public enum Key : byte
    {
        None = 0,
        Forward = 1,
        Backward = 1 << 1,
        Left = 1 << 2,
        Right = 1 << 3,
        Fire = 1 << 4,
        NoRotation = 1 << 5
    }

    public static class KeyHelper
    {
        public static bool IsForward(this Key key) => (key & Key.Forward) == Key.Forward;
        public static bool IsBackward(this Key key) => (key & Key.Backward) == Key.Backward;
        public static bool IsLeft(this Key key) => (key & Key.Left) == Key.Left;
        public static bool IsRight(this Key key) => (key & Key.Right) == Key.Right;
        public static bool IsFire(this Key key) => (key & Key.Fire) == Key.Fire;
        public static bool IsNoRotation(this Key key) => (key & Key.NoRotation) == Key.NoRotation;
    }
}
