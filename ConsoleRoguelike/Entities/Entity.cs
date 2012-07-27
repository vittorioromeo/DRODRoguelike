#region
using System.Collections.Generic;
using DRODRoguelike.Lib;

#endregion

namespace DRODRoguelike.Entities
{
    public class Entity
    {
        static Entity()
        {
            LastUID = 0;
        }

        public Entity(Game mGame, string mName)
        {
            Game = mGame;
            EntityManager = Game.EntityManager;
            Name = mName;
            UID = LastUID;
            LastUID++;
            Alive = true;
        }

        public bool UpdatedThisTurn { get; set; }
        public bool Alive { get; set; }
        public static int LastUID { get; set; }
        public Game Game { get; set; }
        public EntityManager EntityManager { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public int StartX { get; set; }
        public int StartY { get; set; }
        public int StartZ { get; set; }
        public int UID { get; private set; }
        public string Name { get; set; }

        public void Swap(int x, int y, int z)
        {
            Entity startEntity = this;
            Entity destinationEntity = Game.EntityManager[x, y, z];

            int startX = X;
            int startY = Y;
            int startZ = Z;
            int endX = x;
            int endY = y;
            int endZ = z;

            startEntity.X = endX;
            startEntity.Y = endY;
            startEntity.Z = endZ;
            destinationEntity.X = startX;
            destinationEntity.Y = startY;
            destinationEntity.Z = startZ;

            Game.EntityManager[startX, startY, startZ] = destinationEntity;
            Game.EntityManager[endX, endY, endZ] = startEntity;
        }

        public void SwapWith(Entity destinationEntity)
        {
            Entity startEntity = this;

            int startX = X;
            int startY = Y;
            int startZ = Z;
            int endX = destinationEntity.X;
            int endY = destinationEntity.Y;
            int endZ = destinationEntity.Z;

            startEntity.X = endX;
            startEntity.Y = endY;
            startEntity.Z = endZ;
            destinationEntity.X = startX;
            destinationEntity.Y = startY;
            destinationEntity.Z = startZ;

            Game.EntityManager[startX, startY, startZ] = destinationEntity;
            Game.EntityManager[endX, endY, endZ] = startEntity;
        }

        public void SwapTowards(Helper.Direction direction)
        {
            List<int> directionToInt = Helper.DirectionToInt(direction);
            Swap(X + directionToInt[0], Y + directionToInt[1], Z);
        }

        public bool IsOnStartXY()
        {
            return X == StartX && Y == StartY;
        }

        public List<Entity> GetAdjacentEntities(int z)
        {
            List<Entity> result = new List<Entity> ();

            for (int iY = -1; iY < 2; iY++)
            {
                for (int iX = -1; iX < 2; iX++)
                {
                    if (iX != 0 || iY != 0)
                    {
                        result.Add(Game.EntityManager[X + iX, Y + iY, z]);
                    }
                }
            }

            return result;
        }

        public AdjacentList GetAdjacentList(int z)
        {
            AdjacentList result = new AdjacentList
                                      {
                                          North = Game.EntityManager[X, Y - 1, z],
                                          Northeast = Game.EntityManager[X + 1, Y - 1, z],
                                          Northwest = Game.EntityManager[X - 1, Y - 1, z],
                                          South = Game.EntityManager[X, Y + 1, z],
                                          Southeast = Game.EntityManager[X + 1, Y + 1, z],
                                          Southwest = Game.EntityManager[X - 1, Y + 1, z],
                                          West = Game.EntityManager[X - 1, Y, z],
                                          East = Game.EntityManager[X + 1, Y, z]
                                      };

            return result;
        }

        public List<int> GetRelativeCoordinates(int x, int y)
        {
            List<int> result = new List<int> {X - x, Y - y};
            return result;
        }

        public virtual void NextTurn()
        {
        }

        public virtual void Destroy()
        {
            Alive = false;
            Game.EntityManager[X, Y, Z] = new EntityNull(Game);
        }

        public void Destroy(Entity e)
        {
            Alive = false;
            Game.EntityManager[X, Y, Z] = e;
        }
    }
}