using System.Collections.Generic;
using ConsoleRoguelike.Mechanics;

namespace ConsoleRoguelike.Entities
{
    public class ContainerEntity : Entity
    {
        private List<Item> content;

        public ContainerEntity(Game mGame, int mX, int mY, int mZ, string mName)
            : base(mGame, mX, mY, mZ, mName, false)
        {
            content = new List<Item>();
        }

        public List<Item> Content
        {
            get { return content; }
            set { content = value; }
        }

        public override void NextTurn()
        {
            base.NextTurn();
        }
    }
}