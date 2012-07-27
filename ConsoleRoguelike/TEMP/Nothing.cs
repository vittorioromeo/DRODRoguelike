namespace ConsoleRoguelike.Entities
{
    public class Nothing : Entity
    {
        public Nothing(Game mGame, int mX, int mY, int mZ)
            : base(mGame, mX, mY, mZ, "Nothing", false)
        {
        }
    }
}