#region
using System.Collections.Generic;

#endregion

namespace DRODRoguelike.Terrains
{
    public class Terrain
    {
        private int _sizeX;
        private int _sizeY;
        private Tile[,] _tiles;

        public Terrain(int mSizeX, int mSizeY)
        {
            _sizeX = mSizeX;
            _sizeY = mSizeY;
            _tiles = new Tile[_sizeX,_sizeY];
            Rooms = new List<Room> ();
        }

        public int SizeX
        {
            get { return _sizeX; }
            set { _sizeX = value; }
        }

        public int SizeY
        {
            get { return _sizeY; }
            set { _sizeY = value; }
        }

        public Tile[,] Tiles
        {
            get { return _tiles; }
            set { _tiles = value; }
        }

        public List<Room> Rooms { get; set; }

        public Tile GetRandomTile()
        {
            return
                _tiles[Helper.Random.Next(0, _tiles.GetLength(0) - 1), Helper.Random.Next(0, _tiles.GetLength(1) - 1)];
        }

        public Tile GetRandomTile(Tile.TileType type)
        {
            Tile temp =
                _tiles[Helper.Random.Next(0, _tiles.GetLength(0) - 1), Helper.Random.Next(0, _tiles.GetLength(1) - 1)];

            while (temp.Type != type)
            {
                temp =
                    _tiles[
                        Helper.Random.Next(0, _tiles.GetLength(0) - 1), Helper.Random.Next(0, _tiles.GetLength(1) - 1)];
            }

            return temp;
        }
    }
}