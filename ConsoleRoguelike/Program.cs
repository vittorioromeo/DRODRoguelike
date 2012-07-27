namespace DRODRoguelike
{
    public class Program
    {
        private static void Main()
        {
            Log.Initialize ();
            Helper.Initialize ();
            Resources.Initialize ();
#pragma warning disable 168
            SFMLGame testGame = new SFMLGame ();
#pragma warning restore 168
        }
    }
}