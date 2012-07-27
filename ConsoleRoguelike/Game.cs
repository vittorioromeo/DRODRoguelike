#region
using System;
using System.Collections.Generic;

using DRODRoguelike.Entities;
using DRODRoguelike.Lib;
using DRODRoguelike.Terrains;

#endregion

namespace DRODRoguelike
{
    public class Game
    {
        public Game(SFMLGame mSFMLGame, int mSizeX, int mSizeY)
        {
            TerrainManager.Game = this;
            SFMLGame = mSFMLGame;
            EntityManager = new EntityManager(mSizeX, mSizeY);
            SizeX = mSizeX;
            SizeY = mSizeY;
            Turn = 0;
            Reset = false;

            NewGame (); // commented for debug purposes
        }

        public SFMLGame SFMLGame { get; set; }
        public EntityManager EntityManager { get; set; }
        public int SizeX { get; set; }
        public int SizeY { get; set; }
        public int Turn { get; set; }
        public EntityPlayer Player { get; set; }
        public EntitySword Sword { get; set; }
        public int EnemyCountStart { get; set; }
        public int EnemyCount { get; set; }
        public int Greckles { get; set; }
        public int Score { get; set; }
        public bool Clear { get; set; }
        public bool Reset { get; set; }
        public bool BrainYellow { get; set; }

        public void NewGame()
        {
            TerrainManager.Difficulty = 325;
            TerrainManager.TerrainNumber = 0;
            Greckles = 0;
            Score = 0;
            Clear = false;
            Reset = true;
        }

        public void ResetInventory()
        {
            Player.Inventory.Items[0] = new ItemPickAxe(this, Player.Inventory);
            Player.Inventory.Items[1] = new ItemMimicPotion(this, Player.Inventory);
            Player.Inventory.Items[2] = new ItemThrowingKnife(this, Player.Inventory);
        }

        public void Initialize()
        {
            Turn = 0;
            EnemyCount = 0;
            EnemyCountStart = 0;
            Clear = false;
            BrainYellow = false;
            InitializeClearEntities ();
            InitializeGameTiles ();
            SFMLGame.ResetBackground ();
            InitializeMusic ();
        }

        private void InitializeClearEntities()
        {
            SFMLGame.Particles.Clear ();

            for (int iZ = 0; iZ < 3; iZ++)
            {
                for (int iY = 0; iY < EntityManager.SizeY; iY++)
                {
                    for (int iX = 0; iX < EntityManager.SizeX; iX++)
                    {
                        EntityManager[iX, iY, iZ] = new EntityNull(this);
                    }
                }
            }
        }

        private void InitializeGameTiles()
        {
            List<Tile> orbsToSet = new List<Tile>();

            Inventory lastInventory = new Inventory(this);
            if (Reset == false)
            {
                lastInventory = Player.Inventory;
            }
            Terrain terrain = TerrainManager.NewTerrain(SizeX, SizeY);

            terrain.GetRandomTile(Tile.TileType.TileFloor).Type = Tile.TileType.TileStart;
            terrain.GetRandomTile(Tile.TileType.TileFloor).Type = Tile.TileType.TileEnd;

            int playerX = 0;
            int playerY = 0;

            foreach (Tile tempTile in terrain.Tiles)
            {
                switch (tempTile.Type)
                {
                    case Tile.TileType.TileFloor:
                        EntityManager[tempTile.X, tempTile.Y, 2] = new EntityFloor(this);
                        break;
                    case Tile.TileType.TileTrapdoor:
                        EntityManager[tempTile.X, tempTile.Y, 2] = new EntityTrapdoor(this);
                        break;
                    case Tile.TileType.TilePit:
                        EntityManager[tempTile.X, tempTile.Y, 2] = new EntityPit(this);
                        break;
                    case Tile.TileType.TileWall:
                        EntityManager[tempTile.X, tempTile.Y, 2] = new EntityWall(this);
                        break;
                    case Tile.TileType.TileBrokenwall:
                        EntityManager[tempTile.X, tempTile.Y, 2] = new EntityBrokenWall(this);
                        break;
                    case Tile.TileType.TileShop:
                        EntityManager[tempTile.X, tempTile.Y, 2] = new EntityShop(this);
                        break;
                    case Tile.TileType.TileRoach:
                        EntityManager[tempTile.X, tempTile.Y, 2] = new EntityFloor(this);
                        EntityManager[tempTile.X, tempTile.Y, 0] = new EntityRoach(this);
                        break;
                    case Tile.TileType.TileRoachqueen:
                        EntityManager[tempTile.X, tempTile.Y, 2] = new EntityFloor(this);
                        if (tempTile.Flags.Contains("spawns roach queens"))
                        {
                            EntityManager[tempTile.X, tempTile.Y, 0] = new EntityRoachQueen(this,
                                ESpawnable.EsRoachQueen);
                        }
                        else
                        {
                            EntityManager[tempTile.X, tempTile.Y, 0] = new EntityRoachQueen(this);
                        }
                        break;
                    case Tile.TileType.TileEvileye:
                        EntityManager[tempTile.X, tempTile.Y, 2] = new EntityFloor(this);
                        EntityManager[tempTile.X, tempTile.Y, 0] = new EntityEvilEye(this);
                        break;
                    case Tile.TileType.TileEvileyesentry:
                        EntityManager[tempTile.X, tempTile.Y, 2] = new EntityFloor(this);
                        EntityManager[tempTile.X, tempTile.Y, 0] = new EntityEvilEyeSentry(this);
                        break;
                    case Tile.TileType.TileGelbaby:
                        EntityManager[tempTile.X, tempTile.Y, 2] = new EntityFloor(this);
                        EntityManager[tempTile.X, tempTile.Y, 0] = new EntityGelBaby(this);
                        break;
                    case Tile.TileType.TileWraithwing:
                        EntityManager[tempTile.X, tempTile.Y, 2] = new EntityFloor(this);
                        EntityManager[tempTile.X, tempTile.Y, 0] = new EntityWraithwing(this);
                        break;
                    case Tile.TileType.TileZombie:
                        EntityManager[tempTile.X, tempTile.Y, 2] = new EntityFloor(this);
                        EntityManager[tempTile.X, tempTile.Y, 0] = new EntityZombie(this);
                        break;
                    case Tile.TileType.TileBrainYellow:
                        EntityManager[tempTile.X, tempTile.Y, 2] = new EntityFloor(this);
                        EntityManager[tempTile.X, tempTile.Y, 0] = new EntityBrainYellow(this);
                        break;
                    case Tile.TileType.TileStart:
                        EntityManager[tempTile.X, tempTile.Y, 2] = new EntityFloor(this);
                        playerX = tempTile.X;
                        playerY = tempTile.Y;
                        break;
                    case Tile.TileType.TileEnd:
                        EntityManager[tempTile.X, tempTile.Y, 2] = new EntityExit(this);
                        break;
                    case Tile.TileType.TileClosedDoor:
                        EntityManager[tempTile.X, tempTile.Y, 2] = new EntityClosedDoor(this);
                        break;
                    case Tile.TileType.TileOrb:
                        EntityManager[tempTile.X, tempTile.Y, 2] = new EntityPressurePlate(this);
                        orbsToSet.Add(tempTile);
                        break;
                }

                if (tempTile.Flags.Contains("orthosquare"))
                    EntityManager[tempTile.X, tempTile.Y, 1] = new EntityOrthoSquare(this);
            }

            EntityManager[playerX, playerY, 0] = new EntityPlayer(this) {PreviousX = playerX, PreviousY = playerY};
            

            for (int iY = -3; iY < 4; iY++)
            {
                for (int iX = -3; iX < 4; iX++)
                {
                    if (EntityManager.IsOutOfBoundaries(playerX + iX, playerY + iY)) continue;
                    if ((EntityManager[playerX + iX, playerY + iY, 0] is EntityPlayer) ||
                        (EntityManager[playerX + iX, playerY + iY, 0] is EntitySword)) continue;
                    EntityManager[playerX + iX, playerY + iY, 0] = new EntityNull(this);
                    if (((iX != -3 && iX != 3) && iY != -3) && iY != 3) continue;
                    if (EntityManager[playerX + iX, playerY + iY, 2] is EntityFloor)
                    {
                        EntityManager[playerX + iX, playerY + iY, 2] = new EntityBrokenWall(this);
                    }
                }
            }

            Player = (EntityPlayer) EntityManager[playerX, playerY, 0];
            List<int> dir = Helper.DirectionToInt(Player.Direction);
            EntitySword s = new EntitySword(this, Player);
            EntityManager[Player.X + dir[0], Player.Y + dir[1], Player.Z] = s;
            Player.Sword = (EntitySword)EntityManager[Player.X + dir[0], Player.Y + dir[1], Player.Z];
            Player.Name = Helper.INIParser.GetSetting("Game", "PlayerName");

            for (int iY = 0; iY < EntityManager.SizeY; iY++)
            {
                for (int iX = 0; iX < EntityManager.SizeX; iX++)
                {
                    if (EntityManager[iX, iY, 0] is IEiKillable && !(EntityManager[iX, iY, 0] is IEiIgnoreEnemyCount))
                        EnemyCountStart++;

                    if (!(EntityManager[iX, iY, 2] is EntityExit)) continue;
                    List<Entity> adjacents = EntityManager[iX, iY, 2].GetAdjacentEntities(2);

                    foreach (Entity e in adjacents)
                    {
                        if(!(e is IEiWalkableUpon) && !(e is IEiDoor))
                        EntityManager[e.X, e.Y, 2] = new EntityFloor(this);
                    }
                }
            }

            EnemyCount = EnemyCountStart;

            if (Reset)
            {
                ResetInventory();
                Reset = false;
            }
            else
            {
                Player.Inventory = lastInventory;
            }

            foreach (Tile t in orbsToSet)
            {
                EntityPressurePlate pressurePlate = (EntityPressurePlate) EntityManager[t.X, t.Y, 2];
                IEiDoor ecd = (IEiDoor)EntityManager[t.OrbFlagX, t.OrbFlagY, 2];
                pressurePlate.DoorX = ecd.X;
                pressurePlate.DoorY = ecd.Y;
            }

            SFMLGame.SpawnParticles(Player.X * SFMLGame.TileSize + 7, Player.Y * SFMLGame.TileSize + 7,
                Resources.ParticleGel, 60);
            SFMLGame.SpawnParticles(Player.X * SFMLGame.TileSize + 7, Player.Y * SFMLGame.TileSize + 7,
            Resources.ParticleBomb, 60);

        }

// ReSharper disable MemberCanBeMadeStatic.Local
        private void InitializeMusic()
// ReSharper restore MemberCanBeMadeStatic.Local
        {
            if (!Resources.Music || TerrainManager.TerrainNumber == 0 || TerrainManager.TerrainNumber % 5 != 0) return;
            Resources.CurrentMusic.Stop();
            Resources.InitializeMusic();
        }

        public void NextTurn()
        {
            NextTurnGameTiles ();

            if (EnemyCount > 0)
            {
                Turn++;
            }

            if (EnemyCount != 0) return;
            Log.AddEntry("--- Room cleared! ---");
            Clear = true;
            EnemyCount = -1;
            if (Resources.Sounds)
            {
                Resources.GetSoundLaugh().Play();
            }
        }

        private void NextTurnGameTiles()
        {
            foreach (Entity e in EntityManager.Entities)
            {
                e.UpdatedThisTurn = false;
            }

        if (Player.Alive)
            {
                Player.ResetPreviousXY();
                Player.NextTurn ();
                Player.UpdatedThisTurn = true;
                Player.Sword.NextTurn ();
                Player.Sword.UpdatedThisTurn = true;
            }

            List<EntityMimic> mimics = new List<EntityMimic> ();
            List<EntityPressurePlate> plates = new List<EntityPressurePlate>();

            bool lastBrainYellow = BrainYellow;

            BrainYellow = false;

            for (int iY = 0; iY < EntityManager.SizeY; iY++)
            {
                for (int iX = 0; iX < EntityManager.SizeX; iX++)
                {
                    if (EntityManager[iX, iY, 0] is EntityBrainYellow)
                        BrainYellow = true;

                    if (EntityManager[iX, iY, 0] is EntityMimic)
                        mimics.Add((EntityMimic)EntityManager[iX, iY, 0]);

                    if (EntityManager[iX, iY, 2] is EntityPressurePlate)
                        plates.Add((EntityPressurePlate)EntityManager[iX, iY, 2]);
                }
            }

            if (BrainYellow == false && lastBrainYellow)
            {
                Log.AddEntry("All Yellow Brains killed.");

                if (Resources.Sounds)
                {
                    Resources.SoundNoBrains.Play ();
                }
            }

            foreach (EntityMimic mimic in mimics)
            {              
                if (mimic.UpdatedThisTurn == false)
                {
                    mimic.NextTurn ();
                    mimic.UpdatedThisTurn = true;
                }

                if (mimic.Sword == null)
                {
                    List<int> dir = Helper.DirectionToInt(Player.Direction);
                    EntitySword s = new EntitySword(this, mimic);
                    EntityManager[mimic.X + dir[0], mimic.Y + dir[1], mimic.Z] = s;
                    mimic.Sword = (EntitySword)EntityManager[mimic.X + dir[0], mimic.Y + dir[1], mimic.Z];
                }

                if (mimic.Sword.UpdatedThisTurn) continue;
                mimic.Sword.NextTurn ();
                mimic.Sword.UpdatedThisTurn = true;
            }

            for (int iZ = 2; iZ >= 0; iZ--)
            {
                for (int iY = 0; iY < EntityManager.SizeY; iY++)
                {
                    for (int iX = 0; iX < EntityManager.SizeX; iX++)
                    {
                        Entity temp = EntityManager[iX, iY, iZ];

                        if(!(temp is EntityPressurePlate))
                        {
                            if (temp is EntityExit && Clear)
                            {
                                SFMLGame.SpawnParticles(iX * SFMLGame.TileSize + 7, iY * SFMLGame.TileSize + 7,
                                    Resources.ParticleGel, 25);
                            }

                            if (temp is IEiKillable && !(temp is IEiIgnoreEnemyCount) && EnemyCount < 4)
                            {
                                SFMLGame.SpawnParticles(iX * SFMLGame.TileSize + 7, iY * SFMLGame.TileSize + 7,
                                    Resources.ParticleGel, 25 / EnemyCount);
                            }

                            if (temp.UpdatedThisTurn || !temp.Alive) continue;
                            temp.UpdatedThisTurn = true;
                            temp.NextTurn();
                        }
                    }
                }
            }

            foreach (EntityPressurePlate p in plates)
            {
                p.UpdatedThisTurn = true;
                p.NextTurn();
            }
        }

        public void PlayerHit()
        {
            bool shielded = false;
            int shieldIndex = 0;

            foreach (Item i in Player.Inventory.Items)
            {
                if (i is ItemShield)
                {
                    shielded = true;
                    shieldIndex = Player.Inventory.Items.IndexOf(i);
                    break;
                }
            }

            if (shielded == false)
            {
                SFMLGame.SpawnParticles((Player.X * SFMLGame.TileSize) + 7, (Player.Y * SFMLGame.TileSize) + 7,
                    Resources.ParticleBlood, 350);
                Log.AddEntry(Player.Name + " was killed! -- Restart by pressing R.");
                if (Resources.Sounds)
                {
                    Resources.SoundDeath.Play ();
                }

                Player.Sword.Destroy();
                Player.BaseDestroy();
            }
            else
            {
                Log.AddEntry(Player.Name + " was hit! The shield was destroyed!");
                Player.Inventory.Items[shieldIndex].Use ();
            }
        }

        public void SendInput(string input)
        {
            switch (input)
            {
                case "wait":
                    NextTurn ();
                    break;
                case "b":
                    if (Player.Inventory.Items[0].Usable)
                    {
                        Player.Inventory.Items[0].Use ();
                        NextTurn ();
                    }
                    break;
                case "n":
                    if (Player.Inventory.Items[1].Usable)
                    {
                        Player.Inventory.Items[1].Use ();
                        NextTurn ();
                    }
                    break;
                case "m":
                    if (Player.Inventory.Items[2].Usable)
                    {
                        Player.Inventory.Items[2].Use ();
                        NextTurn ();
                    }
                    break;
                case "up":
                case "down":
                case "left":
                case "right":
                case "nw":
                case "ne":
                case "sw":
                case "se":
                case "cw":
                case "ccw":
                    if (Player.Alive)
                    {
                        Player.Input(input);
                    }
                    NextTurn ();
                    break;
            }
        }
    }
}