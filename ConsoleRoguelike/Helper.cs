#region
using System;
using System.Collections.Generic;
using DRODRoguelike.Entities;
using DRODRoguelike.Lib;
using DRODRoguelike.Terrains;
using SFML.Window;

#endregion

namespace DRODRoguelike
{
    public static class Helper
    {
        #region Direction enum
        public enum Direction
        {
            North = 0,
            East = 2,
            South = 4,
            West = 6,
            Northeast = 1,
            Southeast = 3,
            Northwest = 7,
            Southwest = 5
        } ;
        #endregion
        public static Random Random { get; set; }
        public static INIParser INIParser { get; set; }

        public static void Initialize()
        {
            Random = new Random ();
            INIParser = new INIParser(Environment.CurrentDirectory + @"/Data/Config.ini");
        }

        public static float ARadians(float angle)
        {
            return angle / 57.32f;
        }

        // Transforms an angle to radians
        public static float ADegrees(float angle)
        {
            return angle * 57.32f;
        }

        // Transforms an angle to degrees
        public static float ANormalize(float angle)
        {
            if (angle > 360)
            {
                angle = angle - (360 * Convert.ToInt32((angle / 360)));
            }

            if (angle < 0)
            {
                angle = angle + (360 * Convert.ToInt32((angle / 360)));
            }

            return angle;
        }

        // Trasforms any angle to be in 0-360 range
        public static Direction GetRandomDirection()
        {
            int x = Random.Next(-1, 2);
            int y = Random.Next(-1, 2);

            List<int> l = new List<int> {x, y};

            return IntToDirection(l);
        }

        public static List<int> DirectionToInt(Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    return new List<int> {0, -1};
                case Direction.East:
                    return new List<int> {1, 0};
                case Direction.South:
                    return new List<int> {0, 1};
                case Direction.West:
                    return new List<int> {-1, 0};
                case Direction.Northeast:
                    return new List<int> {1, -1};
                case Direction.Southeast:
                    return new List<int> {1, 1};
                case Direction.Northwest:
                    return new List<int> {-1, -1};
                case Direction.Southwest:
                    return new List<int> {-1, 1};
            }

            return null;
        }

        public static Direction IntToDirection(List<int> num)
        {
            if (num[0] == 0 && num[1] == -1) return Direction.North;
            if (num[0] == 1 && num[1] == 0) return Direction.East;
            if (num[0] == 0 && num[1] == 1) return Direction.South;
            if (num[0] == -1 && num[1] == 0) return Direction.West;
            if (num[0] == 1 && num[1] == -1) return Direction.Northeast;
            if (num[0] == 1 && num[1] == 1) return Direction.Southeast;
            if (num[0] == -1 && num[1] == -1) return Direction.Northwest;
            if (num[0] == -1 && num[1] == 1) return Direction.Southwest;

            return Direction.North;
        }

        public static void SwapInt(ref int a, ref int b)
        {
            int c = a;
            a = b;
            b = c;
        }

        public static List<Point> LinePoints(Point startPoint, Point endPoint)
        {
            var result = new List<Point> ();

            int startX = startPoint.X;
            int startY = startPoint.Y;
            int endX = endPoint.X;
            int endY = endPoint.Y;

            bool steep = Math.Abs(endY - startY) > Math.Abs(endX - startX);

            if (steep)
            {
                SwapInt(ref startX, ref startY);
                SwapInt(ref endX, ref endY);
            }

            if (startX > endX)
            {
                SwapInt(ref startX, ref endX);
                SwapInt(ref startY, ref endY);
            }

            int deltaX = endX - startX;
            int deltaY = Math.Abs(endY - startY);

            int error = deltaX / 2;

            int iY = startY;
            int iYStep;

            if (startY < endY)
            {
                iYStep = 1;
            }
            else
            {
                iYStep = -1;
            }

            for (int iX = startX; iX < endX; iX++)
            {
                result.Add(steep ? new Point(iY, iX) : new Point(iX, iY));

                error = error - deltaY;

                if (error >= 0) continue;
                iY = iY + iYStep;
                error = error + deltaX;
            }

            return result;
        }

        public static int Distance(int x1, int y1, int x2, int y2)
        {
            return Convert.ToInt32(Math.Sqrt(((x2 - x1) * (x2 - x1)) + ((y2 - y1) * (y2 - y1))));
        }

        public static Entity GetNewEntity(Game game, ESpawnable es)
        {
            switch (es)
            {
                case ESpawnable.EsRoach:
                    return new EntityRoach(game);
                case ESpawnable.EsRoachQueen:
                    return new EntityRoachQueen(game);
                case ESpawnable.EsEvilEye:
                    return new EntityEvilEye(game);
                case ESpawnable.EsGelBaby:
                    return new EntityGelBaby(game);
            }

            return new EntityNull(game);
        }

        public static KeyCode StringToKeyCode(string s)
        {
            switch (s)
            {
                case "ESCAPE":
                    return KeyCode.Escape;
                case "A":
                    return KeyCode.A;
                case "B":
                    return KeyCode.B;
                case "C":
                    return KeyCode.C;
                case "D":
                    return KeyCode.D;
                case "E":
                    return KeyCode.E;
                case "F":
                    return KeyCode.F;
                case "G":
                    return KeyCode.G;
                case "H":
                    return KeyCode.H;
                case "I":
                    return KeyCode.I;
                case "L":
                    return KeyCode.L;
                case "M":
                    return KeyCode.M;
                case "N":
                    return KeyCode.N;
                case "O":
                    return KeyCode.O;
                case "P":
                    return KeyCode.P;
                case "Q":
                    return KeyCode.Q;
                case "R":
                    return KeyCode.R;
                case "S":
                    return KeyCode.S;
                case "T":
                    return KeyCode.T;
                case "U":
                    return KeyCode.U;
                case "V":
                    return KeyCode.V;
                case "Z":
                    return KeyCode.Z;
                case "W":
                    return KeyCode.W;
                case "K":
                    return KeyCode.K;
                case "J":
                    return KeyCode.J;
                case "Y":
                    return KeyCode.Y;
                case "UP":
                    return KeyCode.Up;
                case "DOWN":
                    return KeyCode.Down;
                case "LEFT":
                    return KeyCode.Left;
                case "RIGHT":
                    return KeyCode.Right;
                case "LSHIFT":
                    return KeyCode.LShift;
                case "RSHIFT":
                    return KeyCode.RShift;
                case "ADD":
                    return KeyCode.Add;
                case "PAGEUP":
                    return KeyCode.PageUp;
                case "PAGEDOWN":
                    return KeyCode.PageDown;
                case "NUMPAD0":
                    return KeyCode.Numpad0;
                case "NUMPAD1":
                    return KeyCode.Numpad1;
                case "NUMPAD2":
                    return KeyCode.Numpad2;
                case "NUMPAD3":
                    return KeyCode.Numpad3;
                case "NUMPAD4":
                    return KeyCode.Numpad4;
                case "NUMPAD5":
                    return KeyCode.Numpad5;
                case "NUMPAD6":
                    return KeyCode.Numpad6;
                case "NUMPAD7":
                    return KeyCode.Numpad7;
                case "NUMPAD8":
                    return KeyCode.Numpad8;
                case "NUMPAD9":
                    return KeyCode.Numpad9;
                case "SPACE":
                    return KeyCode.Space;
                case "COMMA":
                    return KeyCode.Comma;
                case "TILDE":
                    return KeyCode.Tilde;
                case "DASH":
                    return KeyCode.Dash;
                case "PERIOD":
                    return KeyCode.Period;
                case "LCONTROL":
                    return KeyCode.LControl;
                case "RCONTROL":
                    return KeyCode.LControl;
            }

            return KeyCode.X;
        }
    }
}