#region
using System.Collections.Generic;
using System.Linq;
using DRODRoguelike.Lib;
using SFML.Graphics;

#endregion

namespace DRODRoguelike.Entities
{
    public class ItemNull : Item
    {
        public ItemNull(Game game, Inventory inventory)
            : base(game, inventory, "Nothing")
        {
            Price = 0;
            Usable = true;
            Image = Resources.IconNull;
        }

        public override void Use()
        {
            if (!(Game.EntityManager[Game.Player.X, Game.Player.Y, 2] is EntityShop)) return;
            EntityShop entities = (EntityShop) Game.EntityManager[Game.Player.X, Game.Player.Y, 2];

            if ((entities.Items[entities.Index] is ItemNull)) return;
            if (Game.Greckles < entities.Items[entities.Index].Price) return;
            Game.Greckles -= entities.Items[entities.Index].Price;
            Inventory.Items[Inventory.Items.IndexOf(this)] = entities.Items[entities.Index];
            entities.Items[entities.Index] = new ItemNull(Game, Inventory);
            entities.MaxIndex--;
            entities.Index = 0;
        }
    }

    public class ItemHandBomb : Item
    {
        public ItemHandBomb(Game game, Inventory inventory)
            : base(game, inventory, "Handbomb")
        {
            Price = 500;
            Usable = true;
            Uses = 1;
            Image = Resources.IconHandBomb;
        }

        public override void Use()
        {
            List<Entity> ents = Game.EntityManager[Game.Player.X, Game.Player.Y, 0].GetAdjacentEntities(0);

            base.Use ();

            Game.SFMLGame.SpawnParticles(Game.Player.X * Game.SFMLGame.TileSize + 7,
                Game.Player.Y * Game.SFMLGame.TileSize + 7, Resources.ParticleBomb, 75);
            Game.SFMLGame.SpawnParticles(Game.Player.X * Game.SFMLGame.TileSize + 7,
                Game.Player.Y * Game.SFMLGame.TileSize + 7, Resources.ParticleDebris, 50);

            foreach (IEiKillable e in ents.OfType<IEiKillable> ())
            {
                Game.SFMLGame.SpawnParticles(e.X * Game.SFMLGame.TileSize + 7, e.Y * Game.SFMLGame.TileSize + 7,
                    Resources.ParticleBlood, 25);
                e.Destroy ();

                if(!(e is IEiIgnoreEnemyCount))
                Game.EnemyCount--;
            }
        }
    }

    public class ItemPickAxe : Item
    {
        public ItemPickAxe(Game game, Inventory inventory)
            : base(game, inventory, "Pickaxe")
        {
            Price = 250;
            Usable = true;
            Uses = 3;
            Image = Resources.IconPickAxe;
        }

        public override void Use()
        {
            List<int> direction = Helper.DirectionToInt(Game.Player.Direction);

            

            if (!(Game.EntityManager[Game.Player.X + direction[0], Game.Player.Y + direction[1], 2] is EntityWall))
                return;
            base.Use();

            Game.SFMLGame.SpawnParticles((Game.Player.X + direction[0]) * Game.SFMLGame.TileSize + 7,
                (Game.Player.Y + direction[1]) * Game.SFMLGame.TileSize + 7,
                Resources.ParticleDebris, 50);
            Game.EntityManager[Game.Player.X + direction[0], Game.Player.Y + direction[1], 2].Destroy(
                new EntityBrokenWall(Game));
            Uses--;
        }
    }

    public class ItemTrapdoor : Item
    {
        public ItemTrapdoor(Game game, Inventory inventory)
            : base(game, inventory, "Trapdoor")
        {
            Price = 250;
            Usable = true;
            Uses = 3;
            Image = Resources.IconTrapdoor;
        }

        public override void Use()
        {
            List<int> direction = Helper.DirectionToInt(Game.Player.Direction);

            

            if (!(Game.EntityManager[Game.Player.X + direction[0], Game.Player.Y + direction[1], 2] is EntityPit) &&
                !(Game.EntityManager[Game.Player.X + direction[0], Game.Player.Y + direction[1], 2] is EntityFloor))
                return;

            base.Use();

            Game.SFMLGame.SpawnParticles((Game.Player.X + direction[0]) * Game.SFMLGame.TileSize + 7,
                (Game.Player.Y + direction[1]) * Game.SFMLGame.TileSize + 7,
                Resources.ParticleBomb, 50);
            Game.EntityManager[Game.Player.X + direction[0], Game.Player.Y + direction[1], 2].Destroy(
                new EntityTrapdoor(Game));
            Uses--;
        }
    }

    public class ItemShield : Item
    {
        public ItemShield(Game game, Inventory inventory)
            : base(game, inventory, "Shield")
        {
            Price = 900;
            Usable = false;
            Image = Resources.IconShield;
            Uses = 1;
        }

        public override void Use()
        {
            base.Use ();

            Game.SFMLGame.SpawnParticles((Game.Player.X) * Game.SFMLGame.TileSize + 7,
                (Game.Player.Y) * Game.SFMLGame.TileSize + 7, Resources.ParticleBomb, 50);
            Game.SFMLGame.SpawnParticles((Game.Player.X) * Game.SFMLGame.TileSize + 7,
                (Game.Player.Y) * Game.SFMLGame.TileSize + 7, Resources.ParticleDebris, 50);
        }
    }

    public class ItemPrism : Item
    {
        public ItemPrism(Game game, Inventory inventory)
            : base(game, inventory, "Prism")
        {
            Price = 900;
            Usable = true;
            Uses = 1;
            Image = Resources.IconPrism;
        }

        public bool IsGazeSafe(int x, int y)
        {
            if (Game.EntityManager[x, y, 2] is IEiEvilEyeGazeAllowed)
            {
                if (Game.EntityManager[x, y, 1] is IEiEvilEyeGazeAllowed)
                {
                    if (Game.EntityManager[x, y, 0] is IEiEvilEyeGazeAllowed)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void Attack(Helper.Direction direction)
        {
            List<int> dir = Helper.DirectionToInt(direction);

            int currentX = Game.Player.X + dir[0];
            int currentY = Game.Player.Y + dir[1];

            while (IsGazeSafe(currentX, currentY))
            {
                if (Game.EntityManager[currentX, currentY, 0] is IEiKillable)
                {
                    Entity e = Game.EntityManager[currentX, currentY, 0];
                    Game.SFMLGame.SpawnParticles(e.X * Game.SFMLGame.TileSize + 7, e.Y * Game.SFMLGame.TileSize + 7,
                        Resources.ParticleBlood, 25);
                    e.Destroy ();
                    if (!(e is IEiIgnoreEnemyCount))
                    Game.EnemyCount--;
                }
                else
                {
                    if (Game.EntityManager.IsOutOfBoundaries(currentX + dir[0], currentY + dir[1]))
                    {
                        break;
                    }
                }

                currentX += dir[0];
                currentY += dir[1];
            }
        }

        public override void Use()
        {
            base.Use ();

            Game.SFMLGame.SpawnParticles(Game.Player.X * Game.SFMLGame.TileSize + 7,
                Game.Player.Y * Game.SFMLGame.TileSize + 7, Resources.ParticleBomb, 75);
            Attack(Game.Player.Direction);
        }
    }

    public class ItemThrowingKnife : Item
    {
        public ItemThrowingKnife(Game game, Inventory inventory)
            : base(game, inventory, "T. Knife")
        {
            Price = 120;
            Usable = true;
            Uses = 2;
            Image = Resources.IconThrowingKnife;
        }

        public bool IsGazeSafe(int x, int y)
        {
            if (Game.EntityManager[x, y, 2] is IEiEvilEyeGazeAllowed)
            {
                if (Game.EntityManager[x, y, 1] is IEiEvilEyeGazeAllowed)
                {
                    if (Game.EntityManager[x, y, 0] is IEiEvilEyeGazeAllowed)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void Attack(Helper.Direction direction)
        {
            int left = 1;

            List<int> dir = Helper.DirectionToInt(direction);

            int currentX = Game.Player.X + dir[0];
            int currentY = Game.Player.Y + dir[1];

            while (IsGazeSafe(currentX, currentY))
            {
                if (Game.EntityManager[currentX, currentY, 0] is IEiKillable)
                {
                    Entity e = Game.EntityManager[currentX, currentY, 0];
                    Game.SFMLGame.SpawnParticles(e.X * Game.SFMLGame.TileSize + 7, e.Y * Game.SFMLGame.TileSize + 7,
                        Resources.ParticleBlood, 25);
                    e.Destroy ();
                    if (!(e is IEiIgnoreEnemyCount))
                    Game.EnemyCount--;
                    if (left == 0)
                    {
                        break;
                    }
                    left--;
                }
                if (Game.EntityManager.IsOutOfBoundaries(currentX + dir[0], currentY + dir[1]))
                {
                    break;
                }

                currentX += dir[0];
                currentY += dir[1];
            }
        }

        public override void Use()
        {
            base.Use ();

            Game.SFMLGame.SpawnParticles(Game.Player.X * Game.SFMLGame.TileSize + 7,
                Game.Player.Y * Game.SFMLGame.TileSize + 7, Resources.ParticleBomb, 75);
            Attack(Game.Player.Direction);

            Uses--;
        }
    }

    public class ItemMimicPotion : Item
    {
        public ItemMimicPotion(Game game, Inventory inventory)
            : base(game, inventory, "Mimic p.")
        {
            Price = 420;
            Usable = true;
            Uses = 1;
            Image = Resources.IconMimicPotion;
        }

        public override void Use()
        {
            List<int> dir = Helper.DirectionToInt(Game.Player.Direction);

            if (!(Game.EntityManager[Game.Player.X + dir[0] + dir[0], Game.Player.Y + dir[1] + dir[1], 2] is IEiWalkableUpon))
                return;
            if (!(Game.EntityManager[Game.Player.X + dir[0] + dir[0], Game.Player.Y + dir[1] + dir[1], 1] is IEiWalkableUpon ||
                Game.EntityManager[Game.Player.X + dir[0] + dir[0], Game.Player.Y + dir[1] + dir[1], 1] is EntityOrthoSquare))
                return;
            if (!(Game.EntityManager[Game.Player.X + dir[0] + dir[0], Game.Player.Y + dir[1] + dir[1], 0] is EntityNull))
                return;

            base.Use();

            Game.SFMLGame.SpawnParticles(Game.Player.X * Game.SFMLGame.TileSize + 7,
                Game.Player.Y * Game.SFMLGame.TileSize + 7, Resources.ParticleBomb, 75);

            if (Game.EntityManager[Game.Player.X + dir[0] + dir[0] + dir[0], Game.Player.Y + dir[1] + dir[1] + dir[1], 0] is IEiKillable &&
                !(Game.EntityManager[Game.Player.X + dir[0] + dir[0] + dir[0], Game.Player.Y + dir[1] + dir[1] + dir[1], 0] is IEiIgnoreEnemyCount))
            {
                Game.EnemyCount--;
            }

            EntityMimic temp = new EntityMimic(Game);
            Game.Player.PreviousX = Game.Player.X;
            Game.Player.PreviousY = Game.Player.Y;
            Game.EntityManager[Game.Player.X + dir[0] + dir[0], Game.Player.Y + dir[1] + dir[1], 0] = temp;
                
        }
    }

    public class Item
    {
        public Item(Game game, Inventory inventory, string name)
        {
            Inventory = inventory;
            Game = game;
            Name = name;
        }

        public Game Game { get; set; }
        public Inventory Inventory { get; set; }
        public string Name { get; set; }
        public int Uses { get; set; }
        public int Price { get; set; }
        public bool Usable { get; set; }
        public Texture Image { get; set; }

        public virtual void Use()
        {
            if (Resources.Sounds)
            {
                Resources.SoundMimic.Play ();
            }

            if (Uses == 1)
            {
                Inventory.Items[Inventory.Items.IndexOf(this)] = new ItemNull(Game, Inventory);
            }
        }
    }

    public class Inventory
    {
        public Inventory(Game game)
        {
            Items = new List<Item> {new ItemNull(game, this), new ItemNull(game, this), new ItemNull(game, this)};

            Game = game;
        }

        public List<Item> Items { get; set; }
        public Game Game { get; set; }
    }
}