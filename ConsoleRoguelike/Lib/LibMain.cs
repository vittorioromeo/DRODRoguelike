#region
using System;
using System.Collections.Generic;
using DRODRoguelike.Entities;

#endregion

namespace DRODRoguelike.Lib
{
    //TO ADD A NEW ELEMENT:
    //Create the element
    //IF SPAWNABLE -- Add it to ESpawnable enum
    //IF SPAWNABLE -- Go to Data and add it to the GetNewEntity method
    //Go to Tile and add it to that enum
    //Go to Game in Initialize_GameTiles and add it
    //Go to TerrainManager and add it to spawn logic (entityStart / entityPercent / ...)
    //Add a special room for it if you want to
    //Go to SFMLGame and add rendering code

    public static class LibHelper
    {
        public static bool IsSafe(Game game, int startX, int startY, int x, int y, bool ignoreSword, bool ignorePits,
                                  bool sounds = false)
        {
            bool firstCheck = false;

            if ((game.EntityManager[startX + x, startY + y, 2] is IEiWalkableUpon) ||
                (ignorePits && game.EntityManager[startX + x, startY + y, 2] is EntityPit))
            {
                if (!(game.EntityManager[startX + x, startY + y, 1] is EntityZombieTrail))
                {
                    if (game.EntityManager[startX + x, startY + y, 1] is IEiWalkableUpon &&
                        !(game.EntityManager[startX, startY, 1] is EntityOrthoSquare))
                    {
                        firstCheck = true;
                    }
                    else if (game.EntityManager[startX + x, startY + y, 1] is EntityOrthoSquare ||
                             game.EntityManager[startX, startY, 1] is EntityOrthoSquare)
                    {
                        List<int> intdir = new List<int> { x, y };

                        Helper.Direction direction = Helper.IntToDirection(intdir);

                        if (direction == Helper.Direction.North ||
                            direction == Helper.Direction.South ||
                            direction == Helper.Direction.West ||
                            direction == Helper.Direction.East)
                        {
                            firstCheck = true;
                        }
                    }
                }
            }

            if (Resources.Sounds && sounds && game.EntityManager[startX + x, startY + y, 2] is EntityPit)
            {
                Resources.SoundPitBump.Play();
            }
            if (Resources.Sounds && sounds && game.EntityManager[startX + x, startY + y, 2] is EntityWall)
            {
                Resources.SoundWallBump.Play();
            }

            if (firstCheck)
            {
                if (game.EntityManager[startX + x, startY + y, 0] is IEiWalkableUpon)
                {
                    return true;
                }
                if (ignoreSword)
                {
                    if (game.EntityManager[startX, startY, 0] is IEiCanWieldSword)
                    {
                        IEiCanWieldSword temp = (IEiCanWieldSword)game.EntityManager[startX, startY, 0];
                        if (game.EntityManager[startX + x, startY + y, 0] is EntitySword && game.EntityManager[startX + x, startY + y, 0] == temp.Sword)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public static bool IsSafeKillPlayer(Game game, int startX, int startY, int x, int y, bool ignoreSword,
                                            bool ignorePits)
        {
            bool firstCheck = false;

            if ((game.EntityManager[startX + x, startY + y, 2] is IEiWalkableUpon) ||
                (ignorePits && game.EntityManager[startX + x, startY + y, 2] is EntityPit))
            {
                if (game.EntityManager[startX + x, startY + y, 1] is IEiWalkableUpon &&
                    !(game.EntityManager[startX, startY, 1] is EntityOrthoSquare))
                {
                    firstCheck = true;
                }
                else if (game.EntityManager[startX + x, startY + y, 1] is EntityOrthoSquare ||
                         game.EntityManager[startX, startY, 1] is EntityOrthoSquare)
                {
                    List<int> intdir = new List<int> { x, y };

                    Helper.Direction direction = Helper.IntToDirection(intdir);

                    if (direction == Helper.Direction.North ||
                        direction == Helper.Direction.South ||
                        direction == Helper.Direction.West ||
                        direction == Helper.Direction.East)
                    {
                        firstCheck = true;
                    }
                }
            }

            if (firstCheck)
            {
                if (game.EntityManager[startX + x, startY + y, 0] is EntityPlayer ||
                    (game.EntityManager[startX + x, startY + y, 0] is EntitySword &&
                     ignoreSword))
                {
                    return true;
                }
            }

            return false;
        }


        public static List<int> DirectMovement(Game game, Entity startEntity, Entity targetEntity,
                                               bool ignoreSword = false, bool ignorePits = false,
                                               bool tryPlayerHit = true)
        {
            int nextX = 0;
            int nextY = 0;

            if (targetEntity.Y > startEntity.Y)
            {
                nextY = 1;
            }
            else if (targetEntity.Y < startEntity.Y)
            {
                nextY = -1;
            }

            if (targetEntity.X > startEntity.X)
            {
                nextX = 1;
            }
            else if (targetEntity.X < startEntity.X)
            {
                nextX = -1;
            }

            List<int> nextMove = new List<int> { nextX, nextY };

            if (tryPlayerHit && startEntity.X + nextMove[0] == targetEntity.X &&
                startEntity.Y + nextMove[1] == targetEntity.Y)
            {
                if (IsSafeKillPlayer(game, startEntity.X, startEntity.Y, nextMove[0], nextMove[1], ignoreSword,
                    ignorePits))
                {
                    game.Player.Destroy();
                }
            }

            if (IsSafe(game, startEntity.X, startEntity.Y, nextMove[0], nextMove[1], ignoreSword, ignorePits))
            {
                startEntity.Swap(startEntity.X + nextMove[0], startEntity.Y + nextMove[1], startEntity.Z);
            }

            return nextMove;
        }
        public static List<int> BeelineNormalMovement(Game game, Entity startEntity, Entity targetEntity,
                                                      bool ignoreSword = false, bool ignorePits = false,
                                                      bool tryPlayerHit = true, bool inverse = false)
        {
            int nextX = 0;
            int nextY = 0;

            if (targetEntity.Y > startEntity.Y)
            {
                nextY = 1;

                if (inverse)
                    nextY = -1;
            }
            else if (targetEntity.Y < startEntity.Y)
            {
                nextY = -1;

                if (inverse)
                    nextY = 1;
            }

            if (targetEntity.X > startEntity.X)
            {
                nextX = 1;

                if (inverse)
                    nextX = -1;
            }
            else if (targetEntity.X < startEntity.X)
            {
                nextX = -1;

                if (inverse)
                    nextX = 1;
            }

            List<int> nextMove = new List<int> { nextX, nextY };

            if (tryPlayerHit && startEntity.X + nextMove[0] == targetEntity.X &&
                startEntity.Y + nextMove[1] == targetEntity.Y)
            {
                if (IsSafeKillPlayer(game, startEntity.X, startEntity.Y, nextMove[0], nextMove[1], ignoreSword,
                    ignorePits))
                {
                    game.Player.Destroy();
                }
            }

            if (IsSafe(game, startEntity.X, startEntity.Y, nextMove[0], nextMove[1], ignoreSword, ignorePits))
            {
                startEntity.Swap(startEntity.X + nextMove[0], startEntity.Y + nextMove[1], startEntity.Z);
            }
            else
            {
                if (IsSafe(game, startEntity.X, startEntity.Y, 0, nextMove[1], ignoreSword, ignorePits))
                {
                    startEntity.Swap(startEntity.X, startEntity.Y + nextMove[1], startEntity.Z);
                }
                else if (IsSafe(game, startEntity.X, startEntity.Y, nextMove[0], 0, ignoreSword, ignorePits))
                {
                    startEntity.Swap(startEntity.X + nextMove[0], startEntity.Y, startEntity.Z);
                }
            }

            return nextMove;
        }
        public static List<int> BeelineSmartMovement(Game game, Entity startEntity, Entity targetEntity,
                                                      bool ignoreSword = false, bool ignorePits = false,
                                                      bool tryPlayerHit = true, bool inverse = false)
        {
            int nextX = 0;
            int nextY = 0;

            if (targetEntity.Y > startEntity.Y)
            {
                nextY = 1;

                if (inverse)
                    nextY = -1;
            }
            else if (targetEntity.Y < startEntity.Y)
            {
                nextY = -1;

                if (inverse)
                    nextY = 1;
            }

            if (targetEntity.X > startEntity.X)
            {
                nextX = 1;

                if (inverse)
                    nextX = -1;
            }
            else if (targetEntity.X < startEntity.X)
            {
                nextX = -1;

                if (inverse)
                    nextX = 1;
            }

            List<int> nextMove = new List<int> { nextX, nextY };

            if (tryPlayerHit && startEntity.X + nextMove[0] == targetEntity.X &&
                startEntity.Y + nextMove[1] == targetEntity.Y)
            {
                if (IsSafeKillPlayer(game, startEntity.X, startEntity.Y, nextMove[0], nextMove[1], ignoreSword,
                    ignorePits))
                {
                    game.Player.Destroy();
                }
            }

            if (IsSafe(game, startEntity.X, startEntity.Y, nextMove[0], nextMove[1], ignoreSword, ignorePits))
            {
                startEntity.Swap(startEntity.X + nextMove[0], startEntity.Y + nextMove[1], startEntity.Z);
            }
            else
            {
                int distanceX = Math.Abs(startEntity.X - targetEntity.X);
                int distanceY = Math.Abs(startEntity.Y - targetEntity.Y);

                int biggestDistance = distanceY;

                if (distanceX > distanceY)
                    biggestDistance = distanceX;

                if (biggestDistance == distanceX)
                {
                    if (IsSafe(game, startEntity.X, startEntity.Y, nextMove[0], 0, ignoreSword, ignorePits))
                    {
                        startEntity.Swap(startEntity.X + nextMove[0], startEntity.Y, startEntity.Z);
                    }
                    else if (IsSafe(game, startEntity.X, startEntity.Y, 0, nextMove[1], ignoreSword, ignorePits))
                    {
                        startEntity.Swap(startEntity.X, startEntity.Y + nextMove[1], startEntity.Z);
                    }
                }
                else if (biggestDistance == distanceY)
                {
                    if (IsSafe(game, startEntity.X, startEntity.Y, 0, nextMove[1], ignoreSword, ignorePits))
                    {
                        startEntity.Swap(startEntity.X, startEntity.Y + nextMove[1], startEntity.Z);
                    }
                    else if (IsSafe(game, startEntity.X, startEntity.Y, nextMove[0], 0, ignoreSword, ignorePits))
                    {
                        startEntity.Swap(startEntity.X + nextMove[0], startEntity.Y, startEntity.Z);
                    }
                }

            }

            return nextMove;
        }
        public static List<int> FlexibleNormalMovement(Game game, Entity startEntity, Entity targetEntity,
                                                     bool ignoreSword = false, bool ignorePits = false,
                                                     bool tryPlayerHit = true, bool inverse = false)
        {
            int nextX = 0;
            int nextY = 0;

            if (targetEntity.Y > startEntity.Y)
            {
                nextY = 1;

                if (inverse)
                    nextY = -1;
            }
            else if (targetEntity.Y < startEntity.Y)
            {
                nextY = -1;

                if (inverse)
                    nextY = 1;
            }

            if (targetEntity.X > startEntity.X)
            {
                nextX = 1;

                if (inverse)
                    nextX = -1;
            }
            else if (targetEntity.X < startEntity.X)
            {
                nextX = -1;

                if (inverse)
                    nextX = 1;
            }

            List<int> nextMove = new List<int> { nextX, nextY };

            if (tryPlayerHit && startEntity.X + nextMove[0] == targetEntity.X &&
                startEntity.Y + nextMove[1] == targetEntity.Y)
            {
                if (IsSafeKillPlayer(game, startEntity.X, startEntity.Y, nextMove[0], nextMove[1], ignoreSword,
                    ignorePits))
                {
                    game.Player.Destroy();
                }
            }

            if (IsSafe(game, startEntity.X, startEntity.Y, nextMove[0], nextMove[1], ignoreSword, ignorePits))
            {
                startEntity.Swap(startEntity.X + nextMove[0], startEntity.Y + nextMove[1], startEntity.Z);
            }
            else
            {
                if (((nextMove[0] == 1 || nextMove[0] == -1) && nextMove[1] == 0) ||
                    ((nextMove[1] == 1 || nextMove[1] == -1) && nextMove[0] == 0))
                {
                    if ((nextMove[0] == 1 || nextMove[0] == -1) && nextMove[1] == 0)
                    {
                        if (IsSafe(game, startEntity.X, startEntity.Y, nextMove[0], -1, ignoreSword, ignorePits))
                        {
                            startEntity.Swap(startEntity.X + nextMove[0], startEntity.Y + -1, startEntity.Z);
                            nextMove[1] = -1;
                        }
                        else if (IsSafe(game, startEntity.X, startEntity.Y, nextMove[0], 1, ignoreSword, ignorePits))
                        {
                            startEntity.Swap(startEntity.X + nextMove[0], startEntity.Y + 1, startEntity.Z);
                            nextMove[1] = 1;
                        }
                    }
                    if ((nextMove[1] == 1 || nextMove[1] == -1) && nextMove[0] == 0)
                    {
                        if (IsSafe(game, startEntity.X, startEntity.Y, -1, nextMove[1], ignoreSword, ignorePits))
                        {
                            startEntity.Swap(startEntity.X + -1, startEntity.Y + nextMove[1], startEntity.Z);
                            nextMove[0] = -1;
                        }
                        else if (IsSafe(game, startEntity.X, startEntity.Y, 1, nextMove[1], ignoreSword, ignorePits))
                        {
                            startEntity.Swap(startEntity.X + 1, startEntity.Y + nextMove[1], startEntity.Z);
                            nextMove[0] = 1;
                        }
                    }
                 }
                else
                {
                    int distanceX = Math.Abs(startEntity.X - targetEntity.X);
                    int distanceY = Math.Abs(startEntity.Y - targetEntity.Y);

                    int biggestDistance = distanceY;

                    if (distanceX > distanceY)
                        biggestDistance = distanceX;

                    if (biggestDistance == distanceX)
                    {
                        if (IsSafe(game, startEntity.X, startEntity.Y, nextMove[0], 0, ignoreSword, ignorePits))
                        {
                            startEntity.Swap(startEntity.X + nextMove[0], startEntity.Y, startEntity.Z);
                        }
                        else if (IsSafe(game, startEntity.X, startEntity.Y, 0, nextMove[1], ignoreSword, ignorePits))
                        {
                            startEntity.Swap(startEntity.X, startEntity.Y + nextMove[1], startEntity.Z);
                        }
                    }
                    else if (biggestDistance == distanceY)
                    {
                        if (IsSafe(game, startEntity.X, startEntity.Y, 0, nextMove[1], ignoreSword, ignorePits))
                        {
                            startEntity.Swap(startEntity.X, startEntity.Y + nextMove[1], startEntity.Z);
                        }
                        else if (IsSafe(game, startEntity.X, startEntity.Y, nextMove[0], 0, ignoreSword, ignorePits))
                        {
                            startEntity.Swap(startEntity.X + nextMove[0], startEntity.Y, startEntity.Z);
                        }
                    }
                }
            }

            return nextMove;
        }
    }

    public enum ESpawnable
    {
        EsRoach,
        EsRoachQueen,
        EsEvilEye,
        EsGelBaby
    }

    public interface IEiCanTriggerPlates
    {
        int X { get; set; }
        int Y { get; set; }
        bool Alive { get; set; }
    }

    public interface IEiWallTiling
    {
    }

    public interface IEiKillable
    {
        int X { get; set; }
        int Y { get; set; }
        void Destroy();
    }

    public interface IEiWalkableUpon
    {
    }

    public interface IEiEvilEyeGazeAllowed
    {
    }

    public interface IEiDoor
    {
        int X { get; set; }
        int Y { get; set; }
    }

    public interface IEiSpawnAllowed
    {
        int X { get; set; }
        int Y { get; set; }
    }

    public interface IEiCanWieldSword
    {
        Helper.Direction Direction { get; set; }
        int X { get; set; }
        int Y { get; set; }
        int Z { get; set; }
        EntitySword Sword { get; set; }
        string Name { get; set; }
    }

    public interface IEiCanDropTrapdoor
    {
        int X { get; set; }
        int Y { get; set; }
    }

    public interface IEiIgnoreEnemyCount
    {
    }

    public class EntityClosedDoor : Entity, IEiDoor
    {
        public EntityClosedDoor(Game mGame)
            : base(mGame, "Closed Door")
        {
        }
    }

    public class EntityOpenDoor : Entity, IEiDoor, IEiWalkableUpon, IEiEvilEyeGazeAllowed
    {
        public EntityOpenDoor(Game mGame)
            : base(mGame, "Open Door")
        {
        }
    }

    public class EntityPressurePlate : Entity, IEiWalkableUpon, IEiEvilEyeGazeAllowed, IEiSpawnAllowed
    {
        public int DoorX { get; set; }
        public int DoorY { get; set; }

        public bool Pressed { get; set; }

        public IEiCanTriggerPlates OnMe { get; set; }

        public EntityPressurePlate(Game mGame)
            : base(mGame, "Pressure Plate")
        {
            OnMe = null;
        }

        public void Activate()
        {
            if (Game.EntityManager[DoorX, DoorY, 2] is IEiDoor)
            {
                IEiDoor door = (IEiDoor)Game.EntityManager[DoorX, DoorY, 2];

                if (door is EntityOpenDoor)
                {
                    EntityClosedDoor newDoor = new EntityClosedDoor(Game);
                    Entity tempDoor = (Entity)door;
                    tempDoor.Destroy(newDoor);
                }
                else if (door is EntityClosedDoor)
                {
                    EntityOpenDoor newDoor = new EntityOpenDoor(Game);
                    Entity tempDoor = (Entity)door;
                    tempDoor.Destroy(newDoor);
                }
            }          
        }

        public override void NextTurn()
        {
            base.NextTurn();

            List<IEiCanTriggerPlates> canTrigger = new List<IEiCanTriggerPlates>();

            for (int iY = 0; iY < EntityManager.SizeY; iY++)
            {
                for (int iX = 0; iX < EntityManager.SizeX; iX++)
                {
                    if (Game.EntityManager[iX, iY, 0] is IEiCanTriggerPlates)
                    {
                        canTrigger.Add((IEiCanTriggerPlates)Game.EntityManager[iX, iY, 0]);
                    }
                }
            }

            foreach (IEiCanTriggerPlates t in canTrigger)
            {
                if (t.X == X && t.Y == Y)
                {
                    if (Pressed == false)
                    {
                        Activate();
                        if (Resources.Sounds)
                        {
                            Resources.SoundOrbHit.Play();
                        }
                    }

                    OnMe = t;

                    Pressed = true;
                    break;
                }
            }


            if (OnMe != null)
            {
                if ((OnMe.X != X || OnMe.Y != Y) || OnMe.Alive == false)
                {
                    Pressed = false;
                    OnMe = null;
                }
            }      
        }
    
    }

    public class EntityNull : Entity, IEiWalkableUpon, IEiEvilEyeGazeAllowed, IEiSpawnAllowed
    {
        public EntityNull(Game mGame)
            : base(mGame, "Null")
        {
        }
    }

    public class EntityFloor : Entity, IEiWalkableUpon, IEiEvilEyeGazeAllowed, IEiSpawnAllowed
    {
        public EntityFloor(Game mGame)
            : base(mGame, "Floor")
        {
        }
    }

    public class EntityPit : Entity, IEiEvilEyeGazeAllowed
    {
        public EntityPit(Game mGame)
            : base(mGame, "Pit")
        {
        }
    }

    public class EntityWall : Entity, IEiWallTiling
    {
        public EntityWall(Game mGame)
            : base(mGame, "Wall")
        {
        }
    }

    public class EntityBrokenWall : Entity, IEiWallTiling
    {
        public EntityBrokenWall(Game mGame)
            : base(mGame, "Broken Wall")
        {
        }

        public override void NextTurn()
        {
            base.NextTurn();

                    if (EntityManager[X, Y, 0] is EntitySword)
                    {
                        Destroy(new EntityFloor(Game));
                        Game.SFMLGame.SpawnParticles(
                            X * Game.SFMLGame.TileSize + 7,
                            Y * Game.SFMLGame.TileSize + 7,
                            Resources.ParticleDebris, 65);

                        if (Resources.Sounds)
                        {
                            Resources.SoundBrokenWall.Play();
                        }
                    }      
        }
    }

    public class EntityTrapdoor : Entity, IEiWalkableUpon, IEiEvilEyeGazeAllowed
    {
        private bool _steppedOn;
        private IEiCanDropTrapdoor _onMe;

        public EntityTrapdoor(Game mGame)
            : base(mGame, "Trapdoor")
        {
        }

        public override void NextTurn()
        {
            base.NextTurn();

            List<IEiCanDropTrapdoor> canDrop = new List<IEiCanDropTrapdoor>();

            for (int iY = 0; iY < EntityManager.SizeY; iY++)
            {
                for (int iX = 0; iX < EntityManager.SizeX; iX++)
                {
                    if (Game.EntityManager[iX, iY, 0] is IEiCanDropTrapdoor)
                    {
                        canDrop.Add((IEiCanDropTrapdoor)Game.EntityManager[iX, iY, 0]);
                    }
                }
            }

            foreach (IEiCanDropTrapdoor c in canDrop)
            {
                if (c.X == X && c.Y == Y)
                {
                    _steppedOn = true;
                    _onMe = c;
                }
            }


            if (!_steppedOn || (_onMe.X == X && _onMe.Y == Y)) return;
            Destroy(new EntityPit(Game));
            if (Resources.Sounds)
            {
                Resources.SoundTrapdoor.Play();
            }
        }
    }

    public class EntityShop : Entity, IEiWalkableUpon, IEiEvilEyeGazeAllowed, IEiSpawnAllowed
    {
        private bool _empty = true;
        private bool _wasPlayerHere;

        public EntityShop(Game mGame)
            : base(mGame, "Shop")
        {
            Items = new List<Item>();
            Index = 0;
        }

        public List<Item> Items { get; set; }
        public int Index { get; set; }
        public int MaxIndex { get; set; }

        public void GenerateItems()
        {
            for (int n = 0; n < 4; n++)
            {
                Item item = new ItemNull(Game, Game.Player.Inventory);
                Items.Add(item);
                int i = Helper.Random.Next(0, 7);

                switch (i)
                {
                    case 0:
                        item = new ItemHandBomb(Game, Game.Player.Inventory);
                        break;
                    case 1:
                        item = new ItemPickAxe(Game, Game.Player.Inventory);
                        break;
                    case 2:
                        item = new ItemTrapdoor(Game, Game.Player.Inventory);
                        break;
                    case 3:
                        item = new ItemShield(Game, Game.Player.Inventory);
                        break;
                    case 4:
                        item = new ItemPrism(Game, Game.Player.Inventory);
                        break;
                    case 5:
                        item = new ItemThrowingKnife(Game, Game.Player.Inventory);
                        break;
                    case 6:
                        item = new ItemMimicPotion(Game, Game.Player.Inventory);
                        break;
                }

                Items[n] = item;
            }
        }

        public override void NextTurn()
        {
            base.NextTurn();

            MaxIndex = Items.Count - 1;

            if (_empty)
            {
                GenerateItems();
                _empty = false;
            }

            if (_wasPlayerHere)
            {
                if (Index < MaxIndex)
                {
                    Index++;
                }
                else
                {
                    Index = 0;
                }
            }

            if (Game.Player.X == X && Game.Player.Y == Y)
            {
                Log.AddEntry("----------");

                foreach (Item item in Items)
                {
                    if (!(item is ItemNull))
                    {
                        if (Index == Items.IndexOf(item))
                        {
                            Log.AddEntry("---> " + item.Price + " greckles: buy " + item.Name + ". <----");
                        }
                        else
                        {
                            Log.AddEntry(item.Price + " greckles: buy " + item.Name + ".");
                        }
                    }
                }
                Log.AddEntry("- press wait/swing sword to browse - press free slot key to buy -");
                Log.AddEntry("-- Shop: --");

                _wasPlayerHere = true;
            }
            else
            {
                _wasPlayerHere = false;
            }
        }
    }

    public class EntityExit : Entity, IEiWalkableUpon, IEiEvilEyeGazeAllowed, IEiSpawnAllowed
    {
        public EntityExit(Game mGame)
            : base(mGame, "Exit")
        {
        }

        public override void NextTurn()
        {
            base.NextTurn();

            if (Game.Player.X != X || Game.Player.Y != Y) return;
            if (Game.Clear)
            {
                int turnCount = Game.EnemyCountStart * 45;
                int scoreToAdd = ((Game.EnemyCountStart * 25) + ((turnCount - Game.Turn) * 6)) / 10;
                int grecklesToAdd = scoreToAdd / 10;

                Log.AddEntry("Clear room bonus: (" + Game.EnemyCountStart + " * 15 + (" + turnCount + " - " +
                             Game.Turn + ") * 6) / 10 = " + scoreToAdd + ".");
                Log.AddEntry("Clear room greckles: " + grecklesToAdd);
                Game.Score += scoreToAdd;
                Game.Greckles += grecklesToAdd;
            }
            else
            {
                int scoreToAdd = (Game.EnemyCountStart * 5) / 10;
                int grecklesToAdd = scoreToAdd / 10;

                Log.AddEntry("Non-clear room bonus: (" + Game.EnemyCountStart + " * 5) / 10 = " + scoreToAdd + ".");
                Log.AddEntry("Non-clear room greckles: " + grecklesToAdd);
                Game.Score += scoreToAdd;
                Game.Greckles += grecklesToAdd;
            }

            Game.Initialize();
            Game.SFMLGame.ResetBackground();
        }
    }

    public class EntityOrthoSquare : Entity, IEiSpawnAllowed, IEiEvilEyeGazeAllowed
    {
        public EntityOrthoSquare(Game mGame)
            : base(mGame, "Orthogonal Square")
        {
        }
    }

    public class EntityZombieTrail : Entity, IEiEvilEyeGazeAllowed
    {
        public int Life { get; set; }

        public EntityZombieTrail(Game mGame)
            : base(mGame, "Zombie Trail")
        {
            Life = 7;
        }

        public override void NextTurn()
        {
            base.NextTurn();

            if (Life > 0)
            {
                Life--;
            }
            else
            {
                Destroy();
            }
        }
    }
}