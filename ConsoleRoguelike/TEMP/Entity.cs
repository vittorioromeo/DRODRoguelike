namespace ConsoleRoguelike.Entities
{
    public class Entity
    {
        private static int lastUID = 0;

        private Game game;
        private GameTile gameTile;
        private string name;
        private bool obstacle;
        private string special;
        private int startX, startY, startZ;
        private int uID;
        private int x, y, z;

        public Entity(Game mGame, int mX, int mY, int mZ, string mName, bool mObstacle)
        {
            game = mGame;
            gameTile = game.GameTiles[mX, mY, mZ];
            x = mX;
            y = mY;
            z = mZ;
            startX = mX;
            startY = mY;
            startZ = mZ;
            name = mName;
            obstacle = mObstacle;
            uID = lastUID;
            lastUID++;

            game.LogAdd("#" + UID + ": new 'Entity' named '" + name + "' - (" + X + ";" + Y + ")", true);
        }

        public Game Game
        {
            get { return game; }
            set { game = value; }
        }

        public GameTile GameTile
        {
            get { return gameTile; }
            set { gameTile = value; }
        }

        public int X
        {
            get { return x; }
            set { x = value; }
        }

        public int Y
        {
            get { return y; }
            set { y = value; }
        }

        public int Z
        {
            get { return z; }
            set { z = value; }
        }

        public int StartX
        {
            get { return startX; }
            set { startX = value; }
        }

        public int StartY
        {
            get { return startY; }
            set { startY = value; }
        }

        public int StartZ
        {
            get { return startZ; }
            set { startZ = value; }
        }

        public int UID
        {
            get { return uID; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public bool Obstacle
        {
            get { return obstacle; }
            set { obstacle = value; }
        }

        public string Special
        {
            get { return special; }
            set { special = value; }
        }

        public virtual void NextTurn()
        {
        }
    }
}