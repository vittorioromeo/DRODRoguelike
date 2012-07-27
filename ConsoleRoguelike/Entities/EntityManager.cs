#region
using System.Collections.Generic;

#endregion

namespace DRODRoguelike.Entities
{
    public class EntityManager
    {
        public EntityManager(int sizeX, int sizeY)
        {
            SizeX = sizeX;
            SizeY = sizeY;
            SizeZ = 3;
            Entities = new Entity[sizeX,sizeY,3];
        }

        public Entity[,,] Entities { get; set; }

        public int SizeX { get; private set; }
        public int SizeY { get; private set; }
        public int SizeZ { get; private set; }

        public Entity this[int indexX, int indexY, int indexZ]
        {
            get
            {
                if (IsOutOfBoundaries(indexX, indexY))
                {
                    indexX = 1;
                    indexY = 1;
                }
                return Entities[indexX, indexY, indexZ];
            }
            set
            {
                Entity temp = value;
                temp.X = indexX;
                temp.Y = indexY;
                temp.Z = indexZ;
                Entities[indexX, indexY, indexZ] = value;
            }
        }

        public Entity GetRandomEntity(int z)
        {
            int tempX = Helper.Random.Next(0, SizeX);
            int tempY = Helper.Random.Next(0, SizeY);
            Entity result = this[tempX, tempY, z];

            return result;
        }

        public List<Entity> Get2DZ(int z)
        {
            List<Entity> result = new List<Entity> ();

            for (int iY = 0; iY < SizeY; iY++)
            {
                for (int iX = 0; iX < SizeX; iX++)
                {
                    result.Add(this[iX, iY, z]);
                }
            }

            return result;
        }

        public bool OutOfBoundaries(int x, int y, int z)
        {
            bool bx = (x < SizeX && x > -1);
            bool by = (y < SizeY && y > -1);
            bool bz = (z < SizeZ && z > -1);

            return bx && by && bz;
        }

        public bool IsOutOfBoundaries(int x, int y)
        {
            if (x > SizeX - 1)
                return true;
            if (x < 0)
                return true;
            if (y > SizeY - 1)
                return true;
            return y < 0;
        }
    }
}