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

        public static Keyboard.Key StringToKey(string s)
        {
            switch (s)
            {
                case "ESCAPE":
					return Keyboard.Key.Escape;
                case "A":
					return Keyboard.Key.A;
                case "B":
                    return Keyboard.Key.B;
                case "C":
                    return Keyboard.Key.C;
                case "D":
                    return Keyboard.Key.D;
                case "E":
                    return Keyboard.Key.E;
                case "F":
                    return Keyboard.Key.F;
                case "G":
                    return Keyboard.Key.G;
                case "H":
                    return Keyboard.Key.H;
                case "I":
                    return Keyboard.Key.I;
                case "L":
                    return Keyboard.Key.L;
                case "M":
                    return Keyboard.Key.M;
                case "N":
                    return Keyboard.Key.N;
                case "O":
                    return Keyboard.Key.O;
                case "P":
                    return Keyboard.Key.P;
                case "Q":
                    return Keyboard.Key.Q;
                case "R":
                    return Keyboard.Key.R;
                case "S":
                    return Keyboard.Key.S;
                case "T":
                    return Keyboard.Key.T;
                case "U":
                    return Keyboard.Key.U;
                case "V":
                    return Keyboard.Key.V;
                case "Z":
                    return Keyboard.Key.Z;
                case "W":
                    return Keyboard.Key.W;
                case "K":
                    return Keyboard.Key.K;
                case "J":
                    return Keyboard.Key.J;
                case "Y":
                    return Keyboard.Key.Y;
                case "UP":
                    return Keyboard.Key.Up;
                case "DOWN":
                    return Keyboard.Key.Down;
                case "LEFT":
                    return Keyboard.Key.Left;
                case "RIGHT":
                    return Keyboard.Key.Right;
                case "LSHIFT":
                    return Keyboard.Key.LShift;
                case "RSHIFT":
                    return Keyboard.Key.RShift;
                case "ADD":
                    return Keyboard.Key.Add;
                case "PAGEUP":
                    return Keyboard.Key.PageUp;
                case "PAGEDOWN":
                    return Keyboard.Key.PageDown;
                case "NUMPAD0":
                    return Keyboard.Key.Numpad0;
                case "NUMPAD1":
                    return Keyboard.Key.Numpad1;
                case "NUMPAD2":
                    return Keyboard.Key.Numpad2;
                case "NUMPAD3":
                    return Keyboard.Key.Numpad3;
                case "NUMPAD4":
                    return Keyboard.Key.Numpad4;
                case "NUMPAD5":
                    return Keyboard.Key.Numpad5;
                case "NUMPAD6":
                    return Keyboard.Key.Numpad6;
                case "NUMPAD7":
                    return Keyboard.Key.Numpad7;
                case "NUMPAD8":
                    return Keyboard.Key.Numpad8;
                case "NUMPAD9":
                    return Keyboard.Key.Numpad9;
                case "SPACE":
                    return Keyboard.Key.Space;
                case "COMMA":
                    return Keyboard.Key.Comma;
                case "TILDE":
                    return Keyboard.Key.Tilde;
                case "DASH":
                    return Keyboard.Key.Dash;
                case "PERIOD":
                    return Keyboard.Key.Period;
                case "LCONTROL":
                    return Keyboard.Key.LControl;
                case "RCONTROL":
                    return Keyboard.Key.LControl;
            }

            return Keyboard.Key.X;
        }
    }
}