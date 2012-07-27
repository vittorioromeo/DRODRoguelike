#region
using System.Collections.Generic;

#endregion

namespace DRODRoguelike.Terrains
{
    public class Room
    {
        private readonly int _endX;
        private readonly int _endY;
        private readonly int _startX;
        private readonly int _startY;
        private Terrain _terrain;
        private List<Tile> _tiles;

        public Room(int mStartX, int mStartY, int mEndX, int mEndY, Terrain mTerrain)
        {
            _startX = mStartX;
            _startY = mStartY;
            _endX = mEndX;
            _endY = mEndY;
            _terrain = mTerrain;
            SizeX = _endX - _startX;
            SizeY = _endY - _startY;
            _tiles = new List<Tile> ();

            for (int iY = _startY; iY < _endY; iY++)
            {
                for (int iX = _startX; iX < _endX; iX++)
                {
                    _tiles.Add(_terrain.Tiles[iX, iY]);
                }
            }
        }

        public int SizeX { get; set; }

        public int SizeY { get; set; }

        public Terrain Terrain
        {
            get { return _terrain; }
            set { _terrain = value; }
        }

        public List<Tile> Tiles
        {
            get { return _tiles; }
            set { _tiles = value; }
        }
    }
}