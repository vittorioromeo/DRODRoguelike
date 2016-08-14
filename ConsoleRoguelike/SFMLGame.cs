#region
using System;
using System.Collections.Generic;
using System.Linq;
using DRODRoguelike.Lib;
using DRODRoguelike.Terrains;
using SFML.Graphics;
using SFML.Window;
using SFML.System;
#endregion

namespace DRODRoguelike
{
    public class RenderCell
    {
        public RenderCell(int x, int y, int z, int rx, int ry)
        {
            X = x;
            Y = y;
            Z = z;
            RenderX = rx;
            RenderY = ry;
            Sprite = new Sprite { Position = new Vector2f(RenderX, RenderY) };
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public int RenderX { get; set; }
        public int RenderY { get; set; }
        public Sprite Sprite { get; set; }
    }

    public class Particle
    {
        public Particle(int rx, int ry)
        {
            RenderX = rx;
            RenderY = ry;
            Sprite = new Sprite { Position = new Vector2f(RenderX, RenderY) };
            Life = 36 + Helper.Random.Next(0, 30);
            Angle = Helper.Random.Next(0, 360);
            Speed = Helper.Random.Next(10, 150) / 100f;
            Sprite.Scale = new Vector2f(Helper.Random.Next(23, 63) / 100f, Helper.Random.Next(23, 63) / 100f);
            Sprite.Color = new Color(255, 255, 255, Convert.ToByte(Helper.Random.Next(90, 225)));
        }

        public float RenderX { get; set; }
        public float RenderY { get; set; }
        public Sprite Sprite { get; set; }
        public float Angle { get; set; }
        public float Speed { get; set; }
        public int Life { get; set; }
    }

    public class SFMLGame
    {
        private Keyboard.Key _kcEast = Keyboard.Key.D;
        private Keyboard.Key _kcExit = Keyboard.Key.Escape;
        private Keyboard.Key _kcItem1 = Keyboard.Key.B;
        private Keyboard.Key _kcItem2 = Keyboard.Key.N;
        private Keyboard.Key _kcItem3 = Keyboard.Key.M;
        private Keyboard.Key _kcNorth = Keyboard.Key.W;
        private Keyboard.Key _kcNortheast = Keyboard.Key.E;
        private Keyboard.Key _kcNorthwest = Keyboard.Key.Q;
        private Keyboard.Key _kcRestart = Keyboard.Key.R;
        private Keyboard.Key _kcSkip = Keyboard.Key.T;
        private Keyboard.Key _kcSouth = Keyboard.Key.S;
        private Keyboard.Key _kcSoutheast = Keyboard.Key.C;
        private Keyboard.Key _kcSouthwest = Keyboard.Key.Z;
        private Keyboard.Key _kcSwingCcw = Keyboard.Key.Left;
        private Keyboard.Key _kcSwingCw = Keyboard.Key.Right;
        private Keyboard.Key _kcWait = Keyboard.Key.LShift;
        private Keyboard.Key _kcWest = Keyboard.Key.A;
        private decimal count;

        public SFMLGame()
        {
            RenderWindow = Helper.INIParser.GetSetting("Rendering", "Windowed") == "1"
                               ? new RenderWindow(new VideoMode(1022, 900), "DROD Roguelike", Styles.Default)
                               : new RenderWindow(new VideoMode(1022, 900), "DROD Roguelike", Styles.Fullscreen);

			RenderWindow.SetVerticalSyncEnabled(true);
			RenderWindow.Size = new Vector2u(Convert.ToUInt32(Helper.INIParser.GetSetting("Rendering", "Width")),
                Convert.ToUInt32(Helper.INIParser.GetSetting("Rendering", "Height")));
            RenderWindow.Display();

            Initialize();
        }

        public List<RenderCell> RenderCells { get; set; }
        public List<Particle> Particles { get; set; }
        public RenderWindow RenderWindow { get; set; }

        public bool SimpleFloor { get; set; }
        public bool SimpleTrapdoor { get; set; }
        public bool SimpleBrokenWall { get; set; }

        public int TilesX { get; set; }
        public int TilesY { get; set; }
        public int TileSize { get; set; }

        public int InputDelay { get; set; }
        public int InputDelayMax { get; set; }
		public bool BlockNextInput {get;set;}
        public Keyboard.Key LastKeyPress { get; set; }

        public Tileset CurrentTileset { get; set; }

        public bool Running { get; set; }
        public Game Game { get; set; }

        public Text TextInfoLevel { get; set; }
        public Text TextInfoTurn { get; set; }
        public Text TextInfoEnemies { get; set; }
        public Text TextInfoGreckles { get; set; }
        public Text TextInfoScore { get; set; }
        public TextArea TextLog { get; set; }
        public List<Text> TextInventory { get; set; }
        public List<Sprite> SpriteInventory { get; set; }
        public Sprite HUDBackground { get; set; }
        public Sprite TitleBar { get; set; }

        public void Initialize()
        {
            InitializeVariables();
            InitializeHUD();
            InitializeRenderCells();
            Game.Initialize();
            Run();
        }

        private void InitializeVariables()
        {
            SimpleFloor = Helper.INIParser.GetSetting("Rendering", "SimpleFloor") != "0";
            SimpleTrapdoor = Helper.INIParser.GetSetting("Rendering", "SimpleTrapdoor") != "0";
            SimpleBrokenWall = Helper.INIParser.GetSetting("Rendering", "SimpleBrokenWall") != "0";

            _kcExit = Helper.StringToKey(Helper.INIParser.GetSetting("Input", "kcExit"));
            _kcNorth = Helper.StringToKey(Helper.INIParser.GetSetting("Input", "kcNorth"));
            _kcSouth = Helper.StringToKey(Helper.INIParser.GetSetting("Input", "kcSouth"));
            _kcWest = Helper.StringToKey(Helper.INIParser.GetSetting("Input", "kcWest"));
            _kcEast = Helper.StringToKey(Helper.INIParser.GetSetting("Input", "kcEast"));
            _kcNorthwest = Helper.StringToKey(Helper.INIParser.GetSetting("Input", "kcNorthwest"));
            _kcSouthwest = Helper.StringToKey(Helper.INIParser.GetSetting("Input", "kcSouthwest"));
            _kcNortheast = Helper.StringToKey(Helper.INIParser.GetSetting("Input", "kcNortheast"));
            _kcSoutheast = Helper.StringToKey(Helper.INIParser.GetSetting("Input", "kcSoutheast"));
            _kcItem1 = Helper.StringToKey(Helper.INIParser.GetSetting("Input", "kcItem1"));
            _kcItem2 = Helper.StringToKey(Helper.INIParser.GetSetting("Input", "kcItem2"));
            _kcItem3 = Helper.StringToKey(Helper.INIParser.GetSetting("Input", "kcItem3"));
            _kcRestart = Helper.StringToKey(Helper.INIParser.GetSetting("Input", "kcRestart"));
            _kcSkip = Helper.StringToKey(Helper.INIParser.GetSetting("Input", "kcSkip"));
            _kcSwingCw = Helper.StringToKey(Helper.INIParser.GetSetting("Input", "kcSwingCw"));
            _kcSwingCcw = Helper.StringToKey(Helper.INIParser.GetSetting("Input", "kcSwingCcw"));
            _kcWait = Helper.StringToKey(Helper.INIParser.GetSetting("Input", "kcWait"));

            InputDelayMax = int.Parse(Helper.INIParser.GetSetting("Game", "InputDelay"));
            TilesX = int.Parse(Helper.INIParser.GetSetting("Game", "TilesX"));
            TilesY = int.Parse(Helper.INIParser.GetSetting("Game", "TilesY"));


            RenderCells = new List<RenderCell>();
            Particles = new List<Particle>();
            CurrentTileset = Resources.GetRandomTileset();
            TileSize = 14;
            InputDelay = 0;
            LastKeyPress = Keyboard.Key.Escape;
            TextInventory = new List<Text>();
            SpriteInventory = new List<Sprite>();
            Game = new Game(this, TilesX, TilesY) { SFMLGame = this };
            Running = true;
        }

		private Texture idtexture(Texture x){return x; }

        private void InitializeHUD()
        {
            TextInfoLevel = new Text("", Resources.TomsNewRoman)
                           {
				Position = new Vector2f(43,  (TilesY * TileSize) + 10),
                               CharacterSize = 19,
                               Color = Color.Black
                           };

            TextInfoTurn = new Text("", Resources.TomsNewRoman)
            {
				Position = new Vector2f(143 + 10,  (TilesY * TileSize) + 10),
				CharacterSize = 19,
                Color = Color.Black
            };

            TextInfoEnemies = new Text("", Resources.TomsNewRoman)
            {
				Position = new Vector2f(243 + 20,  (TilesY * TileSize) + 10),
				CharacterSize = 19,
                Color = Color.Black
            };

            TextInfoGreckles = new Text("", Resources.TomsNewRoman)
            {
				Position = new Vector2f(373 + 25,  (TilesY * TileSize) + 10),
				CharacterSize = 19,
                Color = Color.Black
            };

            TextInfoScore = new Text("", Resources.TomsNewRoman)
            {
				Position = new Vector2f(513 + 25,  (TilesY * TileSize) + 10),
				CharacterSize = 19,
                Color = Color.Black
            };

            TextLog = new TextArea();
            for (int i = 0; i < 8; i++)
            {
                TextLog.Rows.Add(new Text("", Resources.Epilog));
				TextLog.Rows[i].Position = new Vector2f(8, (TilesY * TileSize) + 40 +  (TileSize * i));
				TextLog.Rows[i].CharacterSize = 15;
            }

            Text temp0 = new Text("", Resources.TomsNewRoman)
                             {
                                 Position = new Vector2f(726, (TilesY * TileSize) + 18),
				CharacterSize = 13
                             };
            TextInventory.Add(temp0);

            Text temp1 = new Text("", Resources.TomsNewRoman)
                             {
                                 Position = new Vector2f(824, (TilesY * TileSize) + 18),
				CharacterSize = 13
                             };
            TextInventory.Add(temp1);

            Text temp2 = new Text("", Resources.TomsNewRoman)
                             {
                                 Position = new Vector2f(922, (TilesY * TileSize) + 18),
				CharacterSize = 13
                             };
            TextInventory.Add(temp2);

            Sprite s1 = new Sprite
                            {
                                Position = new Vector2f(723 + 8, (TilesY * TileSize) + 62),
                                Scale = new Vector2f(0.85f, 0.85f)
                            };
            Sprite s2 = new Sprite
                            {
                                Position = new Vector2f(821 + 8, (TilesY * TileSize) + 62),
                                Scale = new Vector2f(0.85f, 0.85f)
                            };
            Sprite s3 = new Sprite
                            {
                                Position = new Vector2f(919 + 8, (TilesY * TileSize) + 62),
                                Scale = new Vector2f(0.85f, 0.85f)
                            };

            SpriteInventory.Add(s1);
            SpriteInventory.Add(s2);
            SpriteInventory.Add(s3);

			HUDBackground = new Sprite(idtexture(Resources.HUDBackground)) { Position = new Vector2f(0, TilesY * TileSize) };
			TitleBar = new Sprite(idtexture(Resources.TitleBar)) {Position = new Vector2f(15,55)};
        }

        private void InitializeRenderCells()
        {
            RenderCells.Clear();

            for (int iZ = 2; iZ > -1; iZ--)
            {
                for (int iY = 0; iY < TilesY; iY++)
                {
                    for (int iX = 0; iX < TilesX; iX++)
                    {
                        RenderCell temp = new RenderCell(iX, iY, iZ, TileSize * iX, TileSize * iY);
						temp.Sprite = new Sprite(CurrentTileset.Image)
                                          {
                                              Position = new Vector2f(temp.RenderX, temp.RenderY)
                                          };
                        RenderCells.Add(temp);
                    }
                }
            }
        }

        public void ResetBackground()
        {
            CurrentTileset = Resources.GetRandomTileset();
            InitializeRenderCells();
        }

        public void Run()
        {
            while (Running)
            {
                RenderWindow.Clear(Color.Black);

                RunInput();
                count = 0;
                RunDrawRenderCells();
                RunDrawParticles();
                RunHUD();

                RenderWindow.Display();

                //
            }
        }

        public void RunInput()
        {
			var kcToCheck = new List<Keyboard.Key>
                                          {
                                              _kcExit,
                                              _kcNorth,
                                              _kcSouth,
                                              _kcWest,
                                              _kcEast,
                                              _kcNortheast,
                                              _kcNorthwest,
                                              _kcSoutheast,
                                              _kcSouthwest,
                                              _kcItem1,
                                              _kcItem2,
                                              _kcItem3,
                                              _kcRestart,
                                              _kcSkip,
                                              _kcSwingCw,
                                              _kcSwingCcw,
                                              _kcWait
                                          };

			RenderWindow.DispatchEvents();

			foreach (Keyboard.Key kc in kcToCheck.Where(Keyboard.IsKeyPressed))
            {
                if (kc == _kcExit)
                {
                    Environment.Exit(0);
                }
                if (kc == _kcRestart)
                {
                    Game.NewGame();
                    Game.Initialize();
                    return;
                }
                if (kc == _kcSkip)
                {
                    Game.Initialize();
                    return;
                }

                int temp = InputDelayMax;

                if (InputDelay != 0) continue;
                if (kc == _kcNorth)
                {
                    Game.SendInput("up");
                }
                else if (kc == _kcSouth)
                {
                    Game.SendInput("down");
                }
                else if (kc == _kcWest)
                {
                    Game.SendInput("left");
                }
                else if (kc == _kcEast)
                {
                    Game.SendInput("right");
                }
                else if (kc == _kcNorthwest)
                {
                    Game.SendInput("nw");
                }
                else if (kc == _kcNortheast)
                {
                    Game.SendInput("ne");
                }
                else if (kc == _kcSouthwest)
                {
                    Game.SendInput("sw");
                }
                else if (kc == _kcSoutheast)
                {
                    Game.SendInput("se");
                }
                else if (kc == _kcItem1)
                {
                    Game.SendInput("b");
                }
                else if (kc == _kcItem2)
                {
                    Game.SendInput("n");
                }
                else if (kc == _kcItem3)
                {
                    Game.SendInput("m");
                }
                else if (kc == _kcSwingCw)
                {
                    Game.SendInput("cw");
                    temp = temp * 2;
                }
                else if (kc == _kcSwingCcw)
                {
                    Game.SendInput("ccw");
                    temp = temp * 2;
                }
                else if (kc == _kcWait)
                {
                    Game.SendInput("wait");
                }

				if (!BlockNextInput) {
					InputDelay = temp;
				} else {
					InputDelay = 18;
					BlockNextInput = false;
				}
					LastKeyPress = kc;
            }

            if (InputDelay > 0)
                InputDelay--;

			if (!(Keyboard.IsKeyPressed (LastKeyPress))) {
				InputDelay = 0;
				BlockNextInput = true;
			}
        }

        public void RunDrawRenderCells()
        {
            foreach (RenderCell r in RenderCells)
            {
				r.Sprite.TextureRect = new IntRect(0, 0, 0, 0);

                int tileX = 4;
                int tileY = 0;

                if (Game.EntityManager[r.X, r.Y, r.Z] is EntityNull)
                    continue;
                #region "Draw Floor"
                
                if (Game.EntityManager[r.X, r.Y, r.Z] is EntityFloor)
                {
                    tileX = 11;
                    tileY = 7;

                    if (SimpleFloor)
                    {
						r.Sprite.Texture =Resources.GeneralTilesImage;
                        tileX = 7;
                        tileY = 11;
                    }

                    if (r.Y % 2 == 0)
                    {
                        if (r.X % 2 == 0)
                        {
                            if (Game.EntityManager[r.X, r.Y - 1, r.Z] is IEiWallTiling)
                            {
                                r.Sprite.TextureRect = CurrentTileset.GetSubRect(tileX + 1, tileY + 1);
                            }
                            else
                            {
								r.Sprite.TextureRect = CurrentTileset.GetSubRect(tileX, tileY);
                            }
                        }
                        else
                        {
                            if (Game.EntityManager[r.X, r.Y - 1, r.Z] is IEiWallTiling)
                            {
								r.Sprite.TextureRect = CurrentTileset.GetSubRect(tileX + 2, tileY + 1);
                            }
                            else
                            {
								r.Sprite.TextureRect = CurrentTileset.GetSubRect(tileX + 1, tileY);
                            }
                        }
                    }
                    else
                    {
                        if (r.X % 2 == 0)
                        {
                            if (Game.EntityManager[r.X, r.Y - 1, r.Z] is IEiWallTiling)
                            {
								r.Sprite.TextureRect = CurrentTileset.GetSubRect(tileX + 2, tileY + 1);
                            }
                            else
                            {
								r.Sprite.TextureRect = CurrentTileset.GetSubRect(tileX + 1, tileY);
                            }
                        }
                        else
                        {
                            if (Game.EntityManager[r.X, r.Y - 1, r.Z] is IEiWallTiling)
                            {
								r.Sprite.TextureRect = CurrentTileset.GetSubRect(tileX + 1, tileY + 1);
                            }
                            else
                            {
								r.Sprite.TextureRect = CurrentTileset.GetSubRect(tileX, tileY);
                            }
                        }
                    }
                }
                #endregion
                #region "Draw Player"
                else if (Game.EntityManager[r.X, r.Y, r.Z] is EntityPlayer)
                {
                    EntityPlayer player = (EntityPlayer)Game.EntityManager[r.X, r.Y, r.Z];
					r.Sprite.Texture =  Resources.GeneralTilesImage;

                    switch (player.Direction)
                    {
                        case Helper.Direction.North:
						r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(0, 2);
                            break;
                        case Helper.Direction.Northeast:
						r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(1, 2);
                            break;
                        case Helper.Direction.East:
						r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(2, 2);
                            break;
                        case Helper.Direction.Southeast:
						r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(3, 2);
                            break;
                        case Helper.Direction.South:
						r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(4, 2);
                            break;
                        case Helper.Direction.Southwest:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(5, 2);
                            break;
                        case Helper.Direction.West:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(6, 2);
                            break;
                        case Helper.Direction.Northwest:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(7, 2);
                            break;
                    }
                }
                #endregion
                #region "Draw Sword"
                else if (Game.EntityManager[r.X, r.Y, r.Z] is EntitySword)
                {
                    EntitySword sword = (EntitySword)Game.EntityManager[r.X, r.Y, r.Z];
					r.Sprite.Texture = idtexture(Resources.GeneralTilesImage);

                    int ytile = 3;

                    if (sword.Parent is EntityMimic)
                        ytile = 14;

                    switch (sword.Direction)
                    {
                        case Helper.Direction.North:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(0, ytile);
                            break;
                        case Helper.Direction.Northeast:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(1, ytile);
                            break;
                        case Helper.Direction.East:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(2, ytile);
                            break;
                        case Helper.Direction.Southeast:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(3, ytile);
                            break;
                        case Helper.Direction.South:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(4, ytile);
                            break;
                        case Helper.Direction.Southwest:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(5, ytile);
                            break;
                        case Helper.Direction.West:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(6, ytile);
                            break;
                        case Helper.Direction.Northwest:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(7, ytile);
                            break;
                    }
                }
                #endregion
                #region "Draw Roach"
                else if (Game.EntityManager[r.X, r.Y, r.Z] is EntityRoach)
                {
                    EntityRoach roach = (EntityRoach)Game.EntityManager[r.X, r.Y, r.Z];
					r.Sprite.Texture = idtexture(Resources.GeneralTilesImage);

                    switch (roach.LastDirection)
                    {
                        case Helper.Direction.North:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(0, 6);
                            break;
                        case Helper.Direction.Northeast:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(1, 6);
                            break;
                        case Helper.Direction.East:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(2, 6);
                            break;
                        case Helper.Direction.Southeast:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(3, 6);
                            break;
                        case Helper.Direction.South:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(4, 6);
                            break;
                        case Helper.Direction.Southwest:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(5, 6);
                            break;
                        case Helper.Direction.West:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(6, 6);
                            break;
                        case Helper.Direction.Northwest:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(7, 6);
                            break;
                    }
                }
                #endregion
                #region "Draw Roach Queen"
                else if (Game.EntityManager[r.X, r.Y, r.Z] is EntityRoachQueen)
                {
                    EntityRoachQueen roachqueen = (EntityRoachQueen)Game.EntityManager[r.X, r.Y, r.Z];
					r.Sprite.Texture = idtexture(Resources.GeneralTilesImage);

                    int ytile = 8;

                    if (roachqueen.Spawn != ESpawnable.EsRoach)
                    {
                        ytile = 7;
                    }

                    switch (roachqueen.LastDirection)
                    {
                        case Helper.Direction.North:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(0, ytile);
                            break;
                        case Helper.Direction.Northeast:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(1, ytile);
                            break;
                        case Helper.Direction.East:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(2, ytile);
                            break;
                        case Helper.Direction.Southeast:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(3, ytile);
                            break;
                        case Helper.Direction.South:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(4, ytile);
                            break;
                        case Helper.Direction.Southwest:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(5, ytile);
                            break;
                        case Helper.Direction.West:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(6, ytile);
                            break;
                        case Helper.Direction.Northwest:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(7, ytile);
                            break;
                    }
                }
                #endregion
                #region "Draw Roach Egg"
                else if (Game.EntityManager[r.X, r.Y, r.Z] is EntityEgg)
                {
                    EntityEgg roachegg = (EntityEgg)Game.EntityManager[r.X, r.Y, r.Z];
					r.Sprite.Texture = idtexture(Resources.GeneralTilesImage);

                    switch (roachegg.State)
                    {
                        case 0:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(2, 11);
                            break;
                        case 1:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(2, 11);
                            break;
                        case 2:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(1, 11);
                            break;
                        default:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(0, 11);
                            break;
                    }
                }
                #endregion
                #region "Draw Evil Eye"
                else if (Game.EntityManager[r.X, r.Y, r.Z] is EntityEvilEye)
                {
                    EntityEvilEye evileye = (EntityEvilEye)Game.EntityManager[r.X, r.Y, r.Z];
					r.Sprite.Texture = idtexture(Resources.GeneralTilesImage);
                    int ytile = 4;

                    if (evileye.Awake)
                        ytile = 5;

                    switch (evileye.Direction)
                    {
                        case Helper.Direction.North:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(8, ytile);
                            break;
                        case Helper.Direction.Northeast:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(9, ytile);
                            break;
                        case Helper.Direction.East:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(10, ytile);
                            break;
                        case Helper.Direction.Southeast:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(11, ytile);
                            break;
                        case Helper.Direction.South:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(12, ytile);
                            break;
                        case Helper.Direction.Southwest:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(13, ytile);
                            break;
                        case Helper.Direction.West:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(14, ytile);
                            break;
                        case Helper.Direction.Northwest:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(15, ytile);
                            break;
                    }
                }
                #endregion
                #region "Draw Gel Baby"
                else if (Game.EntityManager[r.X, r.Y, r.Z] is EntityGelBaby)
                {
					r.Sprite.Texture = idtexture(Resources.GeneralTilesImage);
                    r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(12, 1);
                }
                #endregion
                #region "Draw Wall"
                else if (Game.EntityManager[r.X, r.Y, r.Z] is EntityWall)
                {
                    if (!(Game.EntityManager[r.X, r.Y + 1, r.Z] is IEiWallTiling))
                    {
                        r.Sprite.TextureRect = CurrentTileset.GetSubRect(9, 6);
                    }
                    else
                    {
                        r.Sprite.TextureRect = CurrentTileset.GetSubRect(8, 6);
                    }
                }
                #endregion
                #region "Draw Broken Wall"
                else if (Game.EntityManager[r.X, r.Y, r.Z] is EntityBrokenWall)
                {
                    tileX = 10;
                    tileY = 6;

                    if (SimpleBrokenWall)
                    {
						r.Sprite.Texture = idtexture(Resources.GeneralTilesImage);
                        tileX = 10;
                        tileY = 11;
                    }

                    if (!(Game.EntityManager[r.X, r.Y + 1, 2] is IEiWallTiling))
                    {
                        r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(tileX+1,tileY);
                    }
                    else
                    {
                        r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(tileX, tileY);
                    }
                }
                #endregion
                #region "Draw Orthogonal Square"
                else if (Game.EntityManager[r.X, r.Y, r.Z] is EntityOrthoSquare)
                {
					r.Sprite.Texture = idtexture(Resources.GeneralTilesImage);
                    r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(11, 2);
                }
                #endregion
                #region "Draw Trapdoor"
                else if (Game.EntityManager[r.X, r.Y, r.Z] is EntityTrapdoor)
                {
                    if (SimpleTrapdoor)
                    {
						r.Sprite.Texture = idtexture(Resources.GeneralTilesImage);
                        tileX = 9;
                        tileY = 11;
                    }

                    r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(tileX, tileY);
                }
                #endregion
                #region "Draw Pit"
                else if (Game.EntityManager[r.X, r.Y, r.Z] is EntityPit)
                {
					r.Sprite.Texture = CurrentTileset.Image;

                    if (Game.EntityManager.OutOfBoundaries(r.X, r.Y - 1, r.Z) &&
                        !(Game.EntityManager[r.X, r.Y - 1, r.Z] is EntityPit))
                    {
                        r.Sprite.TextureRect = CurrentTileset.GetSubRect(12, 10);
                    }
                    else if (Game.EntityManager.OutOfBoundaries(r.X, r.Y - 2, r.Z) &&
                             !(Game.EntityManager[r.X, r.Y - 2, r.Z] is EntityPit) &&
                             Game.EntityManager[r.X, r.Y - 1, r.Z] is EntityPit
                        )
                    {
                        r.Sprite.TextureRect = CurrentTileset.GetSubRect(12, 11);
                    }
                    else
                    {
                        r.Sprite.TextureRect = CurrentTileset.GetSubRect(12, 12);
                    }
                }
                #endregion
                #region "Draw Wraithwing"
                else if (Game.EntityManager[r.X, r.Y, r.Z] is EntityWraithwing)
                {
                    EntityWraithwing wraithwing = (EntityWraithwing)Game.EntityManager[r.X, r.Y, r.Z];
					r.Sprite.Texture = idtexture(Resources.GeneralTilesImage);

                    switch (wraithwing.LastDirection)
                    {
                        case Helper.Direction.North:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(0, 13);
                            break;
                        case Helper.Direction.Northeast:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(1, 13);
                            break;
                        case Helper.Direction.East:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(2, 13);
                            break;
                        case Helper.Direction.Southeast:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(3, 13);
                            break;
                        case Helper.Direction.South:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(4, 13);
                            break;
                        case Helper.Direction.Southwest:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(5, 13);
                            break;
                        case Helper.Direction.West:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(6, 13);
                            break;
                        case Helper.Direction.Northwest:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(7, 13);
                            break;
                    }
                }
                #endregion
                #region "Draw Shop"
                else if (Game.EntityManager[r.X, r.Y, r.Z] is EntityShop)
                {
					r.Sprite.Texture = idtexture(Resources.GeneralTilesImage);
                    r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(8, 7);
                }
                #endregion
                #region "Draw Exit"
                else if (Game.EntityManager[r.X, r.Y, r.Z] is EntityExit)
                {
					r.Sprite.Texture = idtexture(Resources.GeneralTilesImage);
                    r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(10, 2);
                }
                #endregion
                #region "Draw Evil Eye Sentry"
                else if (Game.EntityManager[r.X, r.Y, r.Z] is EntityEvilEyeSentry)
                {
                    EntityEvilEyeSentry evileyesentry = (EntityEvilEyeSentry)Game.EntityManager[r.X, r.Y, r.Z];
					r.Sprite.Texture = idtexture(Resources.GeneralTilesImage);
                    const int ytile = 6;

                    switch (evileyesentry.Direction)
                    {
                        case Helper.Direction.North:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(8, ytile);
                            break;
                        case Helper.Direction.Northeast:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(9, ytile);
                            break;
                        case Helper.Direction.East:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(10, ytile);
                            break;
                        case Helper.Direction.Southeast:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(11, ytile);
                            break;
                        case Helper.Direction.South:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(12, ytile);
                            break;
                        case Helper.Direction.Southwest:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(13, ytile);
                            break;
                        case Helper.Direction.West:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(14, ytile);
                            break;
                        case Helper.Direction.Northwest:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(15, ytile);
                            break;
                    }
                }
                #endregion
                #region "Draw Zombie"
                else if (Game.EntityManager[r.X, r.Y, r.Z] is EntityZombie)
                {
                    EntityZombie zombie = (EntityZombie)Game.EntityManager[r.X, r.Y, r.Z];
					r.Sprite.Texture = idtexture(Resources.GeneralTilesImage);

                    switch (zombie.LastDirection)
                    {
                        case Helper.Direction.North:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(8, 9);
                            break;
                        case Helper.Direction.Northeast:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(9, 9);
                            break;
                        case Helper.Direction.East:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(10, 9);
                            break;
                        case Helper.Direction.Southeast:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(11, 9);
                            break;
                        case Helper.Direction.South:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(12, 9);
                            break;
                        case Helper.Direction.Southwest:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(13, 9);
                            break;
                        case Helper.Direction.West:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(14, 9);
                            break;
                        case Helper.Direction.Northwest:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(15, 9);
                            break;
                    }
                }
                #endregion
                #region "Draw Zombie Trail"
                else if (Game.EntityManager[r.X, r.Y, r.Z] is EntityZombieTrail)
                {
                    EntityZombieTrail zombietrail = (EntityZombieTrail)Game.EntityManager[r.X, r.Y, r.Z];
					r.Sprite.Texture = idtexture(Resources.GeneralTilesImage);

                    switch (zombietrail.Life)
                    {
                        case 0:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(9, 8);
                            break;
                        default:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(8, 8);
                            break;
                    }
                }
                #endregion
                #region "Draw Brain Yellow"
                else if (Game.EntityManager[r.X, r.Y, r.Z] is EntityBrainYellow)
                {
					r.Sprite.Texture = idtexture(Resources.GeneralTilesImage);
                    r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(7, 12);
                }
                #endregion
                #region "Draw Mimic"
                else if (Game.EntityManager[r.X, r.Y, r.Z] is EntityMimic)
                {
                    EntityMimic mimic = (EntityMimic)Game.EntityManager[r.X, r.Y, r.Z];
					r.Sprite.Texture = idtexture(Resources.GeneralTilesImage);

                    switch (mimic.Direction)
                    {
                        case Helper.Direction.North:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(0, 15);
                            break;
                        case Helper.Direction.Northeast:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(1, 15);
                            break;
                        case Helper.Direction.East:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(2, 15);
                            break;
                        case Helper.Direction.Southeast:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(3, 15);
                            break;
                        case Helper.Direction.South:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(4, 15);
                            break;
                        case Helper.Direction.Southwest:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(5, 15);
                            break;
                        case Helper.Direction.West:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(6, 15);
                            break;
                        case Helper.Direction.Northwest:
                            r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(7, 15);
                            break;
                    }
                }
                #endregion
                #region "Draw Doors and Orbs"
                else if (Game.EntityManager[r.X, r.Y, r.Z] is EntityClosedDoor)
                {
					r.Sprite.Texture = idtexture(Resources.GeneralTilesImage);
                    r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(15, 16);
                }
                else if (Game.EntityManager[r.X, r.Y, r.Z] is EntityOpenDoor)
                {
					r.Sprite.Texture = idtexture(Resources.GeneralTilesImage);
                    r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(9, 2);
                }
                else if (Game.EntityManager[r.X, r.Y, r.Z] is EntityPressurePlate)
                {
					r.Sprite.Texture = idtexture(Resources.GeneralTilesImage);
                    EntityPressurePlate epp = (EntityPressurePlate)Game.EntityManager[r.X, r.Y, r.Z];

                    r.Sprite.TextureRect = Resources.GeneralTiles.GetSubRect(epp.Pressed ? 10 : 9, 3);
                }
                #endregion
                RenderWindow.Draw(r.Sprite);
                count++;
            }
        }

        public void RunDrawParticles()
        {
            for (int n = 0; n < Particles.Count; n++)
            {
                Particles[n].RenderX += Particles[n].Speed * (float)Math.Cos(Helper.ARadians(Particles[n].Angle));
                Particles[n].RenderY += Particles[n].Speed * (float)Math.Sin(Helper.ARadians(Particles[n].Angle));
                Particles[n].Sprite.Position = new Vector2f(Particles[n].RenderX, Particles[n].RenderY);
                RenderWindow.Draw(Particles[n].Sprite);

                Particles[n].Life--;

                if (Particles[n].Life < 1)
                {
                    Particles.RemoveAt(n);
                }
            }
        }

        public void RunHUD()
        {
            RenderWindow.Draw(HUDBackground);

            TextInfoLevel.DisplayedString = "Room: " + TerrainManager.TerrainNumber;
			TextInfoTurn.DisplayedString = "Turn: " + Game.Turn;
            TextInfoEnemies.DisplayedString = "Enemies: " + Game.EnemyCount;
            TextInfoGreckles.DisplayedString = "Greckles: " + Game.Greckles;
            TextInfoScore.DisplayedString = "Score: " + Game.Score;

            for (int i = 0; i < 7; i++)
            {
                TextLog.Rows[i].Style = i == 0 ? Text.Styles.Bold : Text.Styles.Regular;

                TextLog.Rows[i].DisplayedString = Log.Entries[Log.Entries.Count - (i + 1)];
                RenderWindow.Draw(TextLog.Rows[i]);
            }

            RenderWindow.Draw(TextInfoLevel);
            RenderWindow.Draw(TextInfoTurn);
            RenderWindow.Draw(TextInfoEnemies);
            RenderWindow.Draw(TextInfoGreckles);
            RenderWindow.Draw(TextInfoScore);

            foreach (Text t in TextInventory)
            {
                t.DisplayedString = "[" + Game.Player.Inventory.Items[TextInventory.IndexOf(t)].Uses + "]" +
                                    Game.Player.Inventory.Items[TextInventory.IndexOf(t)].Name;
                RenderWindow.Draw(t);
            }

            foreach (Sprite s in SpriteInventory)
            {
				s.Texture = Game.Player.Inventory.Items [SpriteInventory.IndexOf (s)].Image;
                RenderWindow.Draw(s);
            }


            if (TitleBar.Color.A > 2)
            {
                TitleBar.Color = new Color(TitleBar.Color.R, TitleBar.Color.G, TitleBar.Color.B,
                    Convert.ToByte(TitleBar.Color.A - Convert.ToByte(2)));
            }
            RenderWindow.Draw(TitleBar);
        }

        public void SpawnParticles(int x, int y, Texture image, int count)
        {
            for (int i = 0; i < count; i++)
            {
				Particle p = new Particle(x - 10 + Helper.Random.Next(0, 10), y - 10 + Helper.Random.Next(0, 10)) { Sprite = { Texture = image } };
                Particles.Add(p);
            }
        }
    }
}