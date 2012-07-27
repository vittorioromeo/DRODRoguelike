#region
using SFML.Graphics;

#endregion

namespace DRODRoguelike
{
    public class Tileset
    {
        public Tileset(Image image, int tileSize)
        {
            Image = image;
            TileSize = tileSize;
        }

        public Image Image { get; set; }
        public int TileSize { get; set; }

        public Sprite GetSprite(int x, int y)
        {
            Sprite result = new Sprite(Image)
                                {
                                    SubRect = new IntRect(x * TileSize, y * TileSize, TileSize, TileSize)
                                };

            return result;
        }

        public IntRect GetSubRect(int x, int y)
        {
            return new IntRect(x * TileSize, y * TileSize, TileSize, TileSize);
        }
    }
}