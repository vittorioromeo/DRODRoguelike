namespace ConsoleRoguelike.Entities
{
    public class MovingEntity : Entity
    {
        #region Direction enum

        

        #endregion

        public MovingEntity(Game mGame, int mX, int mY, int mZ, string mName)
            : base(mGame, mX, mY, mZ, mName, false)
        {
            Game.LogAdd("#" + UID + ": new 'MovingEntity' named '" + Name + "' - (" + X + ";" + Y + ")", true);
        }

        public void SafeMoveTo(Direction direction)
        {
            if (direction == Direction.NORTH)
            {
                SwapTo(X, Y - 1, Z, true);
            }
            else if (direction == Direction.EAST)
            {
                SwapTo(X + 1, Y, Z, true);
            }
            else if (direction == Direction.SOUTH)
            {
                SwapTo(X, Y + 1, Z, true);
            }
            else if (direction == Direction.WEST)
            {
                SwapTo(X - 1, Y, Z, true);
            }
            else if (direction == Direction.NORTHEAST)
            {
                SwapTo(X + 1, Y - 1, Z, true);
            }
            else if (direction == Direction.SOUTHEAST)
            {
                SwapTo(X + 1, Y + 1, Z, true);
            }
            else if (direction == Direction.NORTHWEST)
            {
                SwapTo(X - 1, Y - 1, Z, true);
            }
            else if (direction == Direction.SOUTHWEST)
            {
                SwapTo(X - 1, Y + 1, Z, true);
            }
        }

        public void MoveTo(Direction direction)
        {
            if (direction == Direction.NORTH)
            {
                SwapTo(X, Y - 1, Z, false);
            }
            else if (direction == Direction.EAST)
            {
                SwapTo(X + 1, Y, Z, false);
            }
            else if (direction == Direction.SOUTH)
            {
                SwapTo(X, Y + 1, Z, false);
            }
            else if (direction == Direction.WEST)
            {
                SwapTo(X - 1, Y, Z, false);
            }
            else if (direction == Direction.NORTHEAST)
            {
                SwapTo(X + 1, Y - 1, Z, false);
            }
            else if (direction == Direction.SOUTHEAST)
            {
                SwapTo(X + 1, Y + 1, Z, false);
            }
            else if (direction == Direction.NORTHWEST)
            {
                SwapTo(X - 1, Y - 1, Z, false);
            }
            else if (direction == Direction.SOUTHWEST)
            {
                SwapTo(X - 1, Y + 1, Z, false);
            }
        }

        private void SwapTo(int x, int y, int z, bool safe)
        {
            Entity startEntity = this;
            Entity destinationEntity = Game.GameTiles[x, y, z].Entity;
            Entity destinationFloorEntity = Game.GameTiles[x, y, 2].Entity;

            if ((safe == false) || (safe == true && destinationFloorEntity.Obstacle == false))
            {
                int startX = X;
                int startY = Y;
                int startZ = Z;
                int endX = x;
                int endY = y;
                int endZ = z;

                startEntity.X = endX;
                startEntity.Y = endY;
                startEntity.Z = endZ;
                destinationEntity.X = startX;
                destinationEntity.Y = startY;
                destinationEntity.Z = startZ;

                Game.GameTiles[endX, endY, endZ].Entity = startEntity;
                Game.GameTiles[startX, startY, startZ].Entity = destinationEntity;

                Game.LogAdd(
                    "#" + UID + ": 'MovingEntity' named '" + Name + "' - from(" + startX + ";" + startY + ") -> to(" +
                    endX + ";" + endY + ")", true);
            }
        }

        public bool OnStartPosition()
        {
            if (X == StartX && Y == StartY)
            {
                return true;
            }

            return false;
        }

        public override void NextTurn()
        {
            base.NextTurn();
        }
    }
}