using System;
using System.Collections.Generic;
using ConsoleRoguelike.Behaviors;
using ConsoleRoguelike.Mechanics;

namespace ConsoleRoguelike.Entities
{
    public class CharacterEntity : MovingEntity
    {
        private List<Behavior> behaviors;
        private bool hostile;
        private InventoryManager _inventoryManager;
        private int perceptionRange;
        private StatusManager _statusManager;
        private List<Entity> visibleThisTurn;

        public CharacterEntity(Game mGame, int mX, int mY, int mZ, string mName, bool mHostile)
            : base(mGame, mX, mY, mZ, mName)
        {
            _statusManager = new StatusManager();
            _inventoryManager = new InventoryManager();
            behaviors = new List<Behavior>();
            visibleThisTurn = new List<Entity>();
            perceptionRange = 20;
            hostile = mHostile;

            Game.LogAdd("#" + UID + ": new 'CharacterEntity' named '" + Name + "' - (" + X + ";" + Y + ")", true);
        }

        public StatusManager StatusManager
        {
            get { return _statusManager; }
            set { _statusManager = value; }
        }

        public InventoryManager InventoryManager
        {
            get { return _inventoryManager; }
            set { _inventoryManager = value; }
        }

        public List<Behavior> Behaviors
        {
            get { return behaviors; }
            set { behaviors = value; }
        }

        public List<Entity> VisibleThisTurn
        {
            get { return visibleThisTurn; }
            set { visibleThisTurn = value; }
        }

        public int PerceptionRange
        {
            get { return perceptionRange; }
            set { perceptionRange = value; }
        }

        public bool Hostile
        {
            get { return hostile; }
            set { hostile = value; }
        }

        public void AttackMove(Direction direction)
        {
            int x = 0;
            int y = 0;

            if (direction == Direction.NORTH)
            {
                x = 0;
                y = -1;
            }
            else if (direction == Direction.EAST)
            {
                x = 1;
                y = 0;
            }
            else if (direction == Direction.SOUTH)
            {
                x = 0;
                y = 1;
            }
            else if (direction == Direction.WEST)
            {
                x = -1;
                y = 0;
            }
            else if (direction == Direction.NORTHEAST)
            {
                x = 1;
                y = -1;
            }
            else if (direction == Direction.SOUTHEAST)
            {
                x = 1;
                y = 1;
            }
            else if (direction == Direction.NORTHWEST)
            {
                x = -1;
                y = -1;
            }
            else if (direction == Direction.SOUTHWEST)
            {
                x = -1;
                y = 1;
            }

            if (Game.GameTiles[X + x, Y + y, 0].Entity is CharacterEntity)
            {
                var temp = (CharacterEntity) Game.GameTiles[X + x, Y + y, 0].Entity;

                if (temp.Hostile == true)
                {
                    Combat(temp);
                }
                else
                {
                    SafeMoveTo(direction);
                }
            }
            else
            {
                SafeMoveTo(direction);
            }
        }

        public void Combat(CharacterEntity entity)
        {
            entity.StatusManager.HP = entity.StatusManager.HP - (StatusManager.Attack - entity.StatusManager.Defence);
            StatusManager.HP = StatusManager.HP - (entity.StatusManager.Attack - StatusManager.Defence);

            if (entity.StatusManager.HP <= 0)
            {
                entity.Game.GameTiles[entity.X, entity.Y, entity.Z].Entity = new Nothing(entity.Game, entity.X, entity.Y,
                                                                                         entity.Z);
            }
        }

        public void Command_Grab()
        {
            if (Game.GameTiles[X, Y, 1].Entity is ContainerEntity)
            {
                var tempEntity = (ContainerEntity) Game.GameTiles[X, Y, 1].Entity;

                foreach (Item tempItem in tempEntity.Content)
                {
                    InventoryManager.Items.Add(tempItem);
                }

                Game.GameTiles[X, Y, 1].Entity = new Nothing(Game, X, Y, 1);
            }
        }

        public void Command_Drop()
        {
            if (InventoryManager.Items.Count > 0)
            {
                if (Game.GameTiles[X, Y, 1].Entity is ContainerEntity)
                {
                    var tempEntity = (ContainerEntity) Game.GameTiles[X, Y, 1].Entity;

                    for (int i = 0; i < InventoryManager.Items.Count; i++)
                    {
                        tempEntity.Content.Add(InventoryManager.Items[i]);
                    }

                    InventoryManager.Items.Clear();
                }

                if (Game.GameTiles[X, Y, 1].Entity is Nothing)
                {
                    var tempEntity = new ContainerEntity(Game, X, Y, 1, "Dropped items");

                    for (int i = 0; i < InventoryManager.Items.Count; i++)
                    {
                        tempEntity.Content.Add(InventoryManager.Items[i]);
                    }

                    InventoryManager.Items.Clear();
                    Game.GameTiles[X, Y, 1].Entity = tempEntity;
                }
            }
        }

        public string DisplayVisibleEntities()
        {
            string result = "";

            result += Name + " sees:";
            result += Environment.NewLine;

            foreach (Entity tempEntity in visibleThisTurn)
            {
                if (tempEntity.Z < 2)
                {
                    result += "> " + tempEntity.Name + " (" + tempEntity.X + ";" + tempEntity.Y + ";" + tempEntity.Z +
                              ")";
                    result += Environment.NewLine;
                }
            }

            return result;
        }

        public override void NextTurn()
        {
            visibleThisTurn.Clear();
            foreach (Behavior tempBehavior in behaviors)
            {
                if (tempBehavior.Active == true && tempBehavior.CalledThisTurn == false)
                {
                    tempBehavior.Think();
                }
            }

            base.NextTurn();
        }
    }
}