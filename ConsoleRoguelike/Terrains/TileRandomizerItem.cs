namespace DRODRoguelike.Terrains
{
    public class TileRandomizerItem
    {
        public TileRandomizerItem(int mPercent, Tile.TileType mType)
        {
            Percent = mPercent;
            Type = mType;
        }

        public int Percent { get; set; }

        public Tile.TileType Type { get; set; }
    }
}