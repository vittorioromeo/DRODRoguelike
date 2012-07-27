#region
using System;
using System.Collections.Generic;

using DRODRoguelike.Entities;

#endregion

namespace DRODRoguelike.Lib
{
    public class EntityPlayer : Entity, IEiKillable, IEiEvilEyeGazeAllowed, IEiCanWieldSword, IEiCanDropTrapdoor, IEiIgnoreEnemyCount, IEiCanTriggerPlates
    {
        private int _dir;

        public int PreviousX { get; set; }
        public int PreviousY { get; set; }

        public EntityPlayer(Game mGame)
            : base(mGame, "Player")
        {
            Inventory = new Inventory(Game);
        }

        public Helper.Direction Direction { get; set; }
        public EntitySword Sword { get; set; }
        public Inventory Inventory { get; set; }
        public object Next { get; set; }

        public void ResetPreviousXY()
        {
            PreviousX = X;
            PreviousY = Y;
        }

        public void Input(string input)
        {
            switch (input)
            {
                case "up":
                    Next = Helper.Direction.North;
                    break;
                case "down":
                    Next = Helper.Direction.South;
                    break;
                case "left":
                    Next = Helper.Direction.West;
                    break;
                case "right":
                    Next = Helper.Direction.East;
                    break;
                case "nw":
                    Next = Helper.Direction.Northwest;
                    break;
                case "ne":
                    Next = Helper.Direction.Northeast;
                    break;
                case "sw":
                    Next = Helper.Direction.Southwest;
                    break;
                case "se":
                    Next = Helper.Direction.Southeast;
                    break;
                case "cw":
                    Next = "cw";
                    break;
                case "ccw":
                    Next = "ccw";
                    break;
                default:
                    Next = null;
                    break;
            }
        }

        public override void NextTurn()
        {
            base.NextTurn ();

            if (Next != null)
            {
                if (Next is Helper.Direction)
                {
                    List<int> nextdir = Helper.DirectionToInt((Helper.Direction) Next);

                    if (LibHelper.IsSafe(Game, X, Y, nextdir[0], nextdir[1], true, false, true))
                    {
                        if (Resources.Sounds)
                        {
                            Resources.GetSoundStep().Play();
                        }
                    }

                    LibHelper.DirectMovement(Game, this, Game.EntityManager[X + nextdir[0], Y + nextdir[1], 0], true,
                            false, false);

                }

                if (Next is string)
                {
                    switch ((string)Next)
                    {
                        case "cw":
                            if (_dir < 7)
                            {
                                _dir++;
                            }
                            else
                            {
                                _dir = 0;
                            }
                            break;
                        case "ccw":
                            if (_dir > 0)
                            {
                                _dir--;
                            }
                            else
                            {
                                _dir = 7;
                            }
                            break;
                    }

                    if (Resources.Sounds)
                    {
                        Resources.SoundSwing.Play();
                    }
                }

                Next = null;
            }

            Direction = (Helper.Direction) Enum.Parse(typeof (Helper.Direction), _dir.ToString ());
        }

        public void BaseDestroy()
        {
            base.Destroy();
        }
        public override void Destroy()
        {
            Game.PlayerHit ();
        }
    }

    public class EntitySword : Entity, IEiEvilEyeGazeAllowed
    {
        public EntitySword(Game mGame, IEiCanWieldSword parent)
            : base(mGame, "Sword")
        {
            Parent = parent;
        }

        public IEiCanWieldSword Parent { get; set; }
        public Helper.Direction Direction { get; set; }

        public override void NextTurn()
        {
            base.NextTurn ();
            Direction = Parent.Direction;
            List<int> dir = Helper.DirectionToInt(Direction);
            if (Game.EntityManager[Parent.X + dir[0], Parent.Y + dir[1], Parent.Z] is IEiKillable)
            {
                if (!(Game.EntityManager[Parent.X + dir[0], Parent.Y + dir[1], Parent.Z] is EntityGelBaby))
                {
                    Game.SFMLGame.SpawnParticles(
                        (Parent.X + dir[0]) * Game.SFMLGame.TileSize + 7,
                        (Parent.Y + dir[1]) * Game.SFMLGame.TileSize + 7,
                        Resources.ParticleBlood, 65);
                }
                if (Game.EntityManager[Parent.X + dir[0], Parent.Y + dir[1], Parent.Z] is EntityGelBaby)
                {
                    Game.SFMLGame.SpawnParticles(
                        (Parent.X + dir[0]) * Game.SFMLGame.TileSize + 7,
                        (Parent.Y + dir[1]) * Game.SFMLGame.TileSize + 7,
                        Resources.ParticleGel, 65);
                }

                if (!(Game.EntityManager[Parent.X + dir[0], Parent.Y + dir[1], Parent.Z] is IEiIgnoreEnemyCount))
                    Game.EnemyCount--;

                Log.AddEntry(Parent.Name + " killed a(n) " +
                             Game.EntityManager[Parent.X + dir[0], Parent.Y + dir[1], Parent.Z].Name +
                             ". [ " + (Parent.X + dir[0]) + " ; " + (Parent.Y + dir[1]) + " ]");
                Game.EntityManager[Parent.X + dir[0], Parent.Y + dir[1], Parent.Z].Destroy ();

                if (Resources.Sounds)
                {
                    Resources.GetSoundKill ().Play ();
                }
            }
            if(Game.EntityManager[Parent.X + dir[0], Parent.Y + dir[1], 1] is EntityPressurePlate)
            {
                EntityPressurePlate pressurePlate = (EntityPressurePlate) Game.EntityManager[Parent.X + dir[0], Parent.Y + dir[1], 1];
                pressurePlate.Activate();
                if (Resources.Sounds)
                {
                    Resources.SoundOrbHit.Play();
                }
            }
            Swap(Parent.X + dir[0], Parent.Y + dir[1], Parent.Z);
        }
    }

    public class EntityEgg : Entity, IEiKillable, IEiEvilEyeGazeAllowed, IEiCanTriggerPlates
    {
        private readonly int _hatchTurn;
        private int _startTurn;

        public EntityEgg(Game mGame)
            : base(mGame, "Egg")
        {
            _startTurn = Game.Turn;
            _hatchTurn = Game.Turn + 4;
            Spawn = ESpawnable.EsRoach;
        }

        public EntityEgg(Game mGame, ESpawnable spawn)
            : base(mGame, "Egg")
        {
            _startTurn = Game.Turn;
            _hatchTurn = Game.Turn + 4;
            Spawn = spawn;
        }

        public int State { get; set; }
        public ESpawnable Spawn { get; set; }

        public override void NextTurn()
        {
            base.NextTurn ();

            if (_startTurn == _hatchTurn)
            {
                Destroy(Helper.GetNewEntity(Game, Spawn));
            }

            _startTurn++;
            State++;
        }
    }

    public class EntityRoach : Entity, IEiKillable, IEiEvilEyeGazeAllowed, IEiCanTriggerPlates
    {
        public EntityRoach(Game mGame)
            : base(mGame, "Roach")
        {
            LastDirection = Helper.GetRandomDirection ();
        }

        public Helper.Direction LastDirection { get; set; }

        public override void NextTurn()
        {
            base.NextTurn ();

            List<int> dir = Game.BrainYellow ? LibHelper.FlexibleNormalMovement(Game, this, Game.Player) : LibHelper.BeelineNormalMovement(Game, this, Game.Player);

            LastDirection = Helper.IntToDirection(dir);    
        }
    }

    public class EntityRoachQueen : Entity, IEiKillable, IEiEvilEyeGazeAllowed, IEiCanTriggerPlates
    {
        public EntityRoachQueen(Game mGame)
            : base(mGame, "Roach Queen")
        {
            LastDirection = Helper.GetRandomDirection ();
            Spawn = ESpawnable.EsRoach;
        }

        public EntityRoachQueen(Game mGame, ESpawnable spawn)
            : base(mGame, "Roach Queen <special>")
        {
            LastDirection = Helper.GetRandomDirection ();
            Spawn = spawn;
        }

        public Helper.Direction LastDirection { get; set; }
        public ESpawnable Spawn { get; set; }

        public override void NextTurn()
        {
            base.NextTurn ();

            if (Game.Turn % 30 == 0 && Game.Turn != 0)
            {
                if (Log.Entries[Log.Entries.Count - 1] != "Roach Queens spawned eggs!")
                {
                    Log.AddEntry("Roach Queens spawned eggs!");
                }

                List<Entity> adjacentEntities0 = GetAdjacentEntities(0);

                foreach (Entity e in adjacentEntities0)
                {
                    if (e is IEiSpawnAllowed)
                    {
                        if (Game.EntityManager[e.X, e.Y, 1] is IEiSpawnAllowed)
                        {
                            if (Game.EntityManager[e.X, e.Y, 2] is IEiSpawnAllowed)
                            {
                                Game.EntityManager[e.X, e.Y, 0] = new EntityEgg(Game, Spawn);
                                Game.EnemyCount++;
                            }
                        }
                    }
                }
            }

            List<int> dir = Game.BrainYellow ? LibHelper.FlexibleNormalMovement(Game, this, Game.Player, false, false, true, true) : LibHelper.BeelineNormalMovement(Game, this, Game.Player, false, false, true, true);

            LastDirection = Helper.IntToDirection(dir);
        }
    }

    public class EntityEvilEye : Entity, IEiKillable, IEiEvilEyeGazeAllowed, IEiCanTriggerPlates
    {
        public EntityEvilEye(Game mGame)
            : base(mGame, "Evil Eye")
        {
            Direction = Helper.GetRandomDirection ();
            Awake = false;
        }

        public Helper.Direction Direction { get; set; }
        public bool Awake { get; set; }

        public bool IsGazeSafe(int x, int y)
        {
            if (EntityManager[x, y, 2] is IEiEvilEyeGazeAllowed)
            {
                if (EntityManager[x, y, 1] is IEiEvilEyeGazeAllowed)
                {
                    if (EntityManager[x, y, 0] is IEiEvilEyeGazeAllowed)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void CheckForPlayer(Helper.Direction direction)
        {
            List<int> dir = Helper.DirectionToInt(direction);

            int currentX = X + dir[0];
            int currentY = Y + dir[1];

            while (IsGazeSafe(currentX, currentY))
            {
                if (Game.Player.X == currentX && Game.Player.Y == currentY)
                {
                    Log.AddEntry(Game.Player.Name + " was spotted by an Evil Eye! [ " + X + " ; " + Y + " ]");

                    Awake = true;

                    if (Resources.Sounds)
                    {
                        Resources.SoundEvilEye.Play ();
                    }
                    break;
                }
                if (Game.EntityManager.IsOutOfBoundaries(currentX + dir[0], currentY + dir[1]))
                {
                    break;
                }
                currentX += dir[0];
                currentY += dir[1];
            }
        }

        public override void NextTurn()
        {
            base.NextTurn ();

            if (Awake == false)
            {
                CheckForPlayer(Direction);
            }

            if (!Awake) return;
            List<int> dir = Game.BrainYellow ? LibHelper.FlexibleNormalMovement(Game, this, Game.Player) : LibHelper.BeelineNormalMovement(Game, this, Game.Player);
            Direction = Helper.IntToDirection(dir);
        }
    }

    public class EntityGelBaby : Entity, IEiKillable, IEiEvilEyeGazeAllowed, IEiCanTriggerPlates
    {
        public EntityGelBaby(Game mGame)
            : base(mGame, "Gel Baby")
        {
            LastDirection = Helper.GetRandomDirection ();
        }

        public Helper.Direction LastDirection { get; set; }

        public bool IsSafe(int x, int y)
        {
            if (EntityManager[X + x, Y + y, 2] is IEiWalkableUpon)
            {
                if (EntityManager[X + x, Y + y, 1] is EntityNull)
                {
                    if (EntityManager[X + x, Y + y, 0] is EntityNull)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public override void NextTurn()
        {
            base.NextTurn ();

            List<int> dir = Game.BrainYellow ? LibHelper.FlexibleNormalMovement(Game, this, Game.Player) : LibHelper.DirectMovement(Game, this, Game.Player);
            LastDirection = Helper.IntToDirection(dir);
        }
    }

    public class EntityWraithwing : Entity, IEiKillable, IEiEvilEyeGazeAllowed
    {
        public EntityWraithwing(Game mGame)
            : base(mGame, "Wraithwing")
        {
            LastDirection = Helper.GetRandomDirection ();
        }

        public Helper.Direction LastDirection { get; set; }
        public int DistanceMax { get; set; }

        public int CalculateDistance(int x, int y)
        {
            int distanceX = Math.Abs(X - x);
            int distanceY = Math.Abs(Y - y);

            int result = distanceX;

            if (distanceY > distanceX)
            {
                result = distanceY;
            }

            return result;
        }

        public override void NextTurn()
        {
            base.NextTurn ();

            string nextMove = "";

            DistanceMax = CalculateDistance(Game.Player.X, Game.Player.Y);

            if (DistanceMax > 5)
            {
                nextMove = "attack";
            }
            else if (DistanceMax <= 5)
            {
                if (DistanceMax == 5)
                {
                    nextMove = "stop";
                }

                if (DistanceMax < 5)
                {
                    nextMove = "flee";
                }

                List<EntityWraithwing> w = new List<EntityWraithwing> ();

                for (int iY = 0; iY < Game.EntityManager.SizeY; iY++)
                {
                    for (int iX = 0; iX < Game.EntityManager.SizeX; iX++)
                    {
                        if (Game.EntityManager[iX, iY, 0] is EntityWraithwing)
                        {
                            w.Add((EntityWraithwing) Game.EntityManager[iX, iY, 0]);
                        }
                    }
                }

                foreach (EntityWraithwing ww in w)
                {
                    int d2 = ww.CalculateDistance(Game.Player.X, Game.Player.Y);
                    int d3 = ww.CalculateDistance(X, Y);

                    if (Math.Abs(DistanceMax - d2) <= 2 && d3 >= 3)
                    {
                        nextMove = "attack";
                        break;
                    }
                }
            }

            List<int> dir;

            switch (nextMove)
            {
                case "attack":
                    
                    dir = Game.BrainYellow ? LibHelper.FlexibleNormalMovement(Game, this, Game.Player, false, true) : LibHelper.BeelineNormalMovement(Game, this, Game.Player, false, true);

                    LastDirection = Helper.IntToDirection(dir);
                    break;
                case "flee":
                    dir = Game.BrainYellow ? LibHelper.FlexibleNormalMovement(Game, this, Game.Player, false, true, true, true) : LibHelper.BeelineNormalMovement(Game, this, Game.Player, false, true, true, true);

                    LastDirection = Helper.IntToDirection(dir);
                    break;
            }
        }
    }

    public class EntityEvilEyeSentry : Entity, IEiKillable, IEiEvilEyeGazeAllowed, IEiCanTriggerPlates
    {
        public EntityEvilEyeSentry(Game mGame)
            : base(mGame, "Evil Eye Sentry")
        {
            Direction = Helper.GetRandomDirection ();
        }

        public Helper.Direction Direction { get; set; }

        public bool IsGazeSafe(int x, int y)
        {
            if (EntityManager[x, y, 2] is IEiEvilEyeGazeAllowed)
            {
                if (EntityManager[x, y, 1] is IEiEvilEyeGazeAllowed)
                {
                    if (EntityManager[x, y, 0] is IEiEvilEyeGazeAllowed)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void CheckForPlayer(Helper.Direction direction)
        {
            List<int> dir = Helper.DirectionToInt(direction);

            int currentX = X + dir[0];
            int currentY = Y + dir[1];

            while (IsGazeSafe(currentX, currentY))
            {
                if (Game.Player.X == currentX && Game.Player.Y == currentY)
                {
                    Log.AddEntry(Game.Player.Name + " was spotted by an Evil Eye Sentry! [ " + X + " ; " + Y + " ]");

                    if (Game.EntityManager[X + dir[0], Y + dir[1], 0] is EntityNull)
                    {
                        EntityEvilEye temp = new EntityEvilEye(Game) {Awake = true, UpdatedThisTurn = true};
                        Game.EntityManager[X + dir[0], Y + dir[1], 0] = temp;
                        Game.EnemyCount++;
                        if (Resources.Sounds)
                        {
                            Resources.SoundEvilEye.Play ();
                        }
                    }

                    break;
                }
                if (Game.EntityManager.IsOutOfBoundaries(currentX + dir[0], currentY + dir[1]))
                {
                    break;
                }
                currentX += dir[0];
                currentY += dir[1];
            }
        }

        public override void NextTurn()
        {
            base.NextTurn ();

            List<int> dir = Helper.DirectionToInt(Direction);
            dir[0] = -dir[0];
            dir[1] = -dir[1];
            Helper.Direction inverse = Helper.IntToDirection(dir);

            CheckForPlayer(Direction);
            CheckForPlayer(inverse);
        }
    }

    public class EntityZombie : Entity, IEiKillable, IEiEvilEyeGazeAllowed, IEiCanTriggerPlates
    {
        public EntityZombie(Game mGame)
            : base(mGame, "Zombie")
        {
            LastDirection = Helper.GetRandomDirection();
        }

        public Helper.Direction LastDirection { get; set; }

        public int CalculateDistance(int x, int y)
        {
            int distanceX = Math.Abs(X - x);
            int distanceY = Math.Abs(Y - y);

            int result = distanceX;

            if (distanceY > distanceX)
            {
                result = distanceY;
            }

            return result;
        }

        public override void NextTurn()
        {
            base.NextTurn();

            int lastX = X;
            int lastY = Y;

            List<int> dir = LibHelper.FlexibleNormalMovement(Game, this, Game.Player, false, false, true, CalculateDistance(Game.Player.X, Game.Player.Y) > 8);
            LastDirection = Helper.IntToDirection(dir);

            EntityManager[lastX, lastY, 1] = new EntityZombieTrail(Game);
        }
    }

    public class EntityBrainYellow : Entity, IEiKillable, IEiEvilEyeGazeAllowed, IEiCanTriggerPlates
    {
        public EntityBrainYellow(Game mGame)
            : base(mGame, "Yellow Brain")
        {
        }
    }

    public class EntityMimic : Entity, IEiKillable, IEiEvilEyeGazeAllowed, IEiCanWieldSword, IEiCanDropTrapdoor, IEiIgnoreEnemyCount, IEiCanTriggerPlates
    {
        public Helper.Direction Direction { get; set; }
        public EntitySword Sword { get; set; }

        public EntityMimic(Game mGame)
            : base(mGame, "Mimic")
        {
            Sword = null;
        }

        public override void NextTurn()
        {
            base.NextTurn();

            Direction = Game.Player.Direction;

            int nextX = Game.Player.X - Game.Player.PreviousX;
            int nextY = Game.Player.Y - Game.Player.PreviousY;

            LibHelper.BeelineNormalMovement(Game, this, Game.EntityManager[X + nextX, Y + nextY, 0], true, false, false );
        }

        public override void Destroy()
        {
           base.Destroy ();
           Sword.Destroy ();
        }
    }
}