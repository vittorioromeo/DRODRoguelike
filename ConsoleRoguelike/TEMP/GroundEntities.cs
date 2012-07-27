namespace ConsoleRoguelike.Entities
{
    public class Floor : Entity
    {
        public Floor(Game mGame, int mX, int mY, int mZ)
            : base(mGame, mX, mY, mZ, "Floor", false)
        {
        }
    }

    public class Pit : Entity
    {
        public Pit(Game mGame, int mX, int mY, int mZ)
            : base(mGame, mX, mY, mZ, "Pit", true)
        {
        }
    }

    public class Wall : Entity
    {
        public Wall(Game mGame, int mX, int mY, int mZ)
            : base(mGame, mX, mY, mZ, "Wall", true)
        {
        }
    }
}