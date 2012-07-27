#region
using System.Collections.Generic;
using SFML.Graphics;

#endregion

namespace DRODRoguelike
{
    public class TextArea
    {
        public TextArea()
        {
            Rows = new List<Text> ();
        }

        public List<Text> Rows { get; set; }
    }
}