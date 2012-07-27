namespace DRODRoguelike.Terrains
{
    public class Tile
    {
        #region TileType enum
        public enum TileType
        {
            TileFloor,
            TileWall,
            TilePit,
            TileTreasure,
            TileStart,
            TileEnd,
            TileRoach,
            TileRoachqueen,
            TileEvileye,
            TileGelbaby,
            TileBrokenwall,
            TileTrapdoor,
            TileWraithwing,
            TileShop,
            TileEvileyesentry,
            TileZombie,
            TileBrainYellow,
            TileClosedDoor,
            TileOrb
        } ;
        #endregion
        private Terrain _terrain;
        private TileType _type;

        public Tile(int mX, int mY, Terrain mTerrain, TileType mType)
        {
            X = mX;
            Y = mY;
            _terrain = mTerrain;
            _type = mType;
            Flags = "";
        }

        public string Flags { get; set; }
        public int OrbFlagX { get; set; }
        public int OrbFlagY { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public Terrain Terrain
        {
            get { return _terrain; }
            set { _terrain = value; }
        }

        public TileType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public int GetCountAdjacent(TileType mType)
        {
            int count = 0;

            if (_terrain.Tiles[X + 1, Y - 1]._type == mType)
                count++;
            if (_terrain.Tiles[X + 1, Y + 0]._type == mType)
                count++;
            if (_terrain.Tiles[X + 1, Y + 1]._type == mType)
                count++;
            if (_terrain.Tiles[X + 0, Y + 1]._type == mType)
                count++;
            if (_terrain.Tiles[X - 1, Y + 1]._type == mType)
                count++;
            if (_terrain.Tiles[X - 1, Y + 0]._type == mType)
                count++;
            if (_terrain.Tiles[X - 1, Y - 1]._type == mType)
                count++;
            if (_terrain.Tiles[X + 0, Y - 1]._type == mType)
                count++;

            return count;
        }
    }
}