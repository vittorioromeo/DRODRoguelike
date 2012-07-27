#region
using System.Collections.Generic;
using System.Linq;

#endregion

namespace DRODRoguelike.Terrains
{
    public static class TerrainManager
    {
        #region SpecialRoom enum
        public enum SpecialRoom
        {
            SrNormal,
            SrEvileye,
            SrRoachqueen,
            SrGelbaby,
            SrTrapdoor,
            SrOrthosquare,
            SrBrokenwall,
            SrZombie
        }
        #endregion
        private const int DifficultyMax = 1600;

        private const int RoachStart = 1;
        private const int RoachQueenStart = 2;
        private const int EvilEyeStart = 3;
        private const int WraithwingStart = 5;
        private const int GelBabyStart = 7;
        private const int OrthoSquareStart = 8;
        private const int TrapDoorStart = 9;
        private const int EvilEyeSentryStart = 10;
        private const int ZombieStart = 12;
        private const int BrainYellowStart = 20;
        private const int SpecialRoomStart = 16;
        private const int RoachQueenLimit = 15;
        private const int RoomMax = 24;
        private static SpecialRoom _currentSpecial;

        private static int _pitPercent;
        private static int _roachNumber;
        private static int _roachQueenNumber;
        private static int _evilEyeNumber;
        private static int _specialRoomPercent;
        private static int _gelBabyNumber;
        private static int _wraithwingNumber;
        private static int _brokenWallPercent;
        private static int _orthoSquarePercent;
        private static int _doorPercent;
        private static int _trapDoorPercent;
        private static int _evilEyeSentryNumber;
        private static int _zombieNumber;

        private static int _brainYellowCount;
        private static int _brainYellowNumber;

        private static int _roachQueenCurrentCount;

        private static int _roomNumber;

        private static bool _shop;

        public static int Difficulty { get; set; }
        public static int TerrainNumber { get; set; }
        public static Game Game { get; set; }

        public static void ResetValues()
        {
            Difficulty = 325;
            TerrainNumber = 1;
            _currentSpecial = SpecialRoom.SrNormal;
        }

        public static Terrain NewTerrain(int sizeX, int sizeY)
        {
            Terrain result = new Terrain(sizeX, sizeY);
            ClearTerrain(result, Tile.TileType.TilePit);

            TerrainNumber++;

            _roachQueenCurrentCount = 0;
            _brainYellowCount = 0;

            CalculatePercents ();
            CalculateSpecialRoom ();
            GenerateArtificialTerrain(result);

            return result;
        }

        private static void CalculatePercents()
        {
            _pitPercent = Helper.Random.Next(0, 40);
            _brokenWallPercent = 6;
            _orthoSquarePercent = 15;
            _trapDoorPercent = 2;
            _specialRoomPercent = 26;
            _doorPercent = 6;

            _roachNumber = Difficulty / 12 + 1;
            _roachQueenNumber = Difficulty / 70 + 1;
            _evilEyeNumber = Difficulty / 20 + 1;
            _wraithwingNumber = Difficulty / 75 + 1;
            _gelBabyNumber = Difficulty / 75 + 1;
            _zombieNumber = Difficulty / 90 + 1;
            _evilEyeSentryNumber = Difficulty / 40 + 1;
            _brainYellowNumber = Difficulty / 700 + 1;

            _roomNumber = Difficulty / 70 + 3;

            if (_roomNumber > RoomMax)
                _roomNumber = RoomMax;

           
            if (Difficulty > DifficultyMax)
            {
                Difficulty++;
            }
            else
            {
                Difficulty += 14;
            }
        }

        private static void CalculateSpecialRoom()
        {
            if (TerrainNumber % 3 == 0)
            {
                _currentSpecial = SpecialRoom.SrNormal;
                _shop = true;
            }
            else
            {
                _currentSpecial = SpecialRoom.SrNormal;
                _shop = false;
                if (TerrainNumber < SpecialRoomStart)
                {
                    _currentSpecial = SpecialRoom.SrNormal;
                }
                else
                {
                    if (CalculatePercent(_specialRoomPercent))
                    {
                        int random = Helper.Random.Next(0, 7);

                        if (random == 0)
                        {
                            _currentSpecial = SpecialRoom.SrEvileye;
                            _evilEyeNumber *= 4;
                            _evilEyeSentryNumber *= 3;
                        }

                        if (random == 1)
                            {
                            _currentSpecial = SpecialRoom.SrRoachqueen;
                            }

                        if (random == 2)
                            {
                            _currentSpecial = SpecialRoom.SrGelbaby;
                                _gelBabyNumber *= 5;
                            }

                        if (random == 3)
                            {
                            _currentSpecial = SpecialRoom.SrTrapdoor;
                                _wraithwingNumber *= 3;
                            }

                        if (random == 4)
                            {
                            _currentSpecial = SpecialRoom.SrOrthosquare;
                            }

                        if (random == 5)
                            {
                            _currentSpecial = SpecialRoom.SrBrokenwall;
                            }

                        if (random == 6)
                            {
                            _currentSpecial = SpecialRoom.SrZombie;
                                _zombieNumber *= 4;
                            }
                    }
                    else
                    {
                        _currentSpecial = SpecialRoom.SrNormal;
                    }
                }
            }
        }

        private static void GenerateArtificialTerrain(Terrain terrain)
        {
            var startPoints = new List<Point> ();
            const int offset = 7;

            int roomsToCreate = _roomNumber - 3 > 0 ? Helper.Random.Next(_roomNumber - 3, _roomNumber) : _roomNumber;

            for (int i = 0; i < roomsToCreate; i++)
            {
                startPoints.Add(new Point(Helper.Random.Next(offset, terrain.SizeX - offset),
                    Helper.Random.Next(offset, terrain.SizeY - offset)));
            }

            foreach (Point tempPoint in startPoints)
            {
                int roomWidth = Helper.Random.Next(2, 6);
                int roomHeight = Helper.Random.Next(2, 6);

                var tempRoom = new Room(tempPoint.X - roomWidth, tempPoint.Y - roomHeight, tempPoint.X + roomWidth,
                    tempPoint.Y + roomHeight, terrain);
                terrain.Rooms.Add(tempRoom);

                for (int iY = -roomHeight; iY < roomHeight; iY++)
                {
                    for (int iX = -roomWidth; iX < roomWidth; iX++)
                    {
                        terrain.Tiles[tempPoint.X + iX, tempPoint.Y + iY].Type = Tile.TileType.TileFloor;
                    }
                }
            }

            for (int i = 0; i < startPoints.Count - 1; i++)
            {
                ConnectPoints(terrain, startPoints[i], startPoints[i + 1]);
            }

            CoverPits(terrain);
            Populate(terrain);
            FinalizeTerrain ();
        }

        private static void ClearTerrain(Terrain terrain, Tile.TileType tileType)
        {
            for (int iY = 0; iY < terrain.SizeY; iY++)
            {
                for (int iX = 0; iX < terrain.SizeX; iX++)
                {
                    terrain.Tiles[iX, iY] = new Tile(iX, iY, terrain, tileType);
                }
            }
        }

        private static bool CalculatePercent(int percent)
        {
            return Helper.Random.Next(0, 100) <= percent;
        }

        private static void ConnectPoints(Terrain terrain, Point a, Point b)
        {
            int tempAX = a.X;
            int tempAY = a.Y;
            int tempBX = b.X;
            int tempBY = b.Y;

            while (tempAX != tempBX)
            {
                if (tempAX > tempBX)
                {
                    tempAX--;
                }
                else if (tempAX < tempBX)
                {
                    tempAX++;
                }

                terrain.Tiles[tempAX, tempAY].Type = Tile.TileType.TileFloor;
            }

            while (tempAY != tempBY)
            {
                if (tempAY > tempBY)
                {
                    tempAY--;
                }
                else if (tempAY < tempBY)
                {
                    tempAY++;
                }

                terrain.Tiles[tempAX, tempAY].Type = Tile.TileType.TileFloor;
            }
        }

        private static void CoverPits(Terrain terrain)
        {
            foreach (Tile tempTile in terrain.Tiles)
            {
                if (terrain.Tiles[tempTile.X, tempTile.Y].Type != Tile.TileType.TileFloor) continue;
                for (int iY = -1; iY < 2; iY++)
                {
                    for (int iX = -1; iX < 2; iX++)
                    {
                        if ((iX == 0 || iY == 0) || (tempTile.X + iX - 1 <= -1) || (tempTile.Y + iY - 1 <= -1) ||
                            (tempTile.X + iX + 1 >= terrain.SizeX) || (tempTile.Y + iY + 1 >= terrain.SizeY)) continue;
                        if (terrain.Tiles[tempTile.X + iX, tempTile.Y + iY].Type == Tile.TileType.TilePit)
                        {
                            terrain.Tiles[tempTile.X + iX, tempTile.Y + iY].Type = Tile.TileType.TileWall;
                        }
                    }
                }
            }
        }

        private static void Populate(Terrain terrain)
        {
            int dX = terrain.Tiles.GetLength(0) - 1;
            int dY = terrain.Tiles.GetLength(1) - 1;

            for (int iY = 0; iY < dY; iY++)
            {
                for (int iX = 0; iX < dX; iX++)
                {
                    Tile tempTile = terrain.GetRandomTile ();

                    if (tempTile.X <= 1 || tempTile.X >= terrain.SizeX - 1 || tempTile.Y <= 1 ||
                        tempTile.Y >= terrain.SizeY - 1) continue;
                    #region "Doors"
                    if (tempTile.Type == Tile.TileType.TileFloor)
                    {
                        if (tempTile.GetCountAdjacent(Tile.TileType.TileWall) == 6)
                        {
                            if (CalculatePercent(_doorPercent))
                            {
                                terrain.Tiles[tempTile.X - 1, tempTile.Y + 1].Type = Tile.TileType.TileFloor;
                                terrain.Tiles[tempTile.X + 1, tempTile.Y - 1].Type = Tile.TileType.TileFloor;

                                int tempX = 0;
                                int tempY = 0;
                                int rnd = 0;

                                tempX = 1;
                                tempY = 1;

                                rnd = Helper.Random.Next(0, 2);

                                if (rnd == 0)
                                {
                                    tempX = -tempX;                                   
                                }


                                Tile orbTile1 = terrain.Tiles[tempTile.X - tempX, tempTile.Y + tempY];
                                orbTile1.Type = Tile.TileType.TileOrb;
                                orbTile1.OrbFlagX = tempTile.X;
                                orbTile1.OrbFlagY = tempTile.Y;
                                Tile orbTile2 = terrain.Tiles[tempTile.X + tempX, tempTile.Y - tempY];
                                orbTile2.Type = Tile.TileType.TileOrb;
                                orbTile2.OrbFlagX = tempTile.X;
                                orbTile2.OrbFlagY = tempTile.Y;

                                tempTile.Type = Tile.TileType.TileClosedDoor;
                            }
                        }
                    }
                    #endregion
                    #region "Pits"
                    switch (tempTile.Type)
                    {
                        case Tile.TileType.TileWall:
                            if (tempTile.GetCountAdjacent(Tile.TileType.TilePit) > 2)
                            {
                                if (CalculatePercent(_pitPercent))
                                {
                                    tempTile.Type = Tile.TileType.TilePit;
                                }
                            }
                            break;
                        case Tile.TileType.TileFloor:
                            if (tempTile.GetCountAdjacent(Tile.TileType.TileFloor) > 6)
                            {
                                if (CalculatePercent(_pitPercent))
                                {
                                    tempTile.Type = Tile.TileType.TilePit;
                                }
                            }
                            break;
                    }
                    #endregion
                    #region "Normal Room"
                    if (_currentSpecial == SpecialRoom.SrNormal ||
                        _currentSpecial == SpecialRoom.SrOrthosquare ||
                        _currentSpecial == SpecialRoom.SrBrokenwall)
                    {
                        if (tempTile.Type == Tile.TileType.TileFloor)
                        {
                            #region "Brain Yellow"
                            if (tempTile.Type == Tile.TileType.TileFloor)
                            {
                                if (TerrainNumber >= BrainYellowStart)
                                {
                                    if (_brainYellowCount < _brainYellowNumber)
                                    {
                                        tempTile.Type = Tile.TileType.TileBrainYellow;
                                        _brainYellowCount++;
                                    }
                                }
                            }
                            #endregion
                            #region "Evil Eye"
                            if (tempTile.Type == Tile.TileType.TileFloor)
                            {
                                if (TerrainNumber >= EvilEyeStart)
                                {
                                    if (_evilEyeNumber > 0)
                                    {
                                        tempTile.Type = Tile.TileType.TileEvileye;
                                        _evilEyeNumber--;

                                        if (TerrainNumber >= EvilEyeSentryStart &&
                                            _evilEyeSentryNumber > 0)
                                        {
                                            tempTile.Type = Tile.TileType.TileEvileyesentry;
                                            _evilEyeSentryNumber--;
                                        }
                                    }
                                }
                            }
                            #endregion
                            #region "Roach / Roach Queen / Wraithwings / Gel Baby / Zombie"
                            if (tempTile.Type == Tile.TileType.TileFloor)
                            {
                                if (TerrainNumber >= RoachStart)
                                {
                                    if (_roachNumber > 0)
                                    {
                                        tempTile.Type = Tile.TileType.TileRoach;
                                        _roachNumber--;

                                        if (TerrainNumber >= RoachQueenStart)
                                        {
                                            if (_zombieNumber > 0 &&
                                                TerrainNumber >= ZombieStart)
                                            {
                                                tempTile.Type = Tile.TileType.TileZombie;
                                                _zombieNumber--;
                                            }
                                            else if (_roachQueenNumber > 0 &&
                                                     _roachQueenCurrentCount < RoachQueenLimit)
                                            {
                                                tempTile.Type = Tile.TileType.TileRoachqueen;
                                                _roachQueenCurrentCount++;
                                                _roachQueenNumber--;
                                            }
                                            else
                                            {
                                                if (TerrainNumber >= WraithwingStart &&
                                                    _wraithwingNumber > 0)
                                                {
                                                    tempTile.Type = Tile.TileType.TileWraithwing;
                                                    _wraithwingNumber--;
                                                }
                                                else
                                                {
                                                    if (TerrainNumber >= GelBabyStart)
                                                    {
                                                        if (_gelBabyNumber > 0)
                                                        {
                                                            tempTile.Type = Tile.TileType.TileGelbaby;
                                                            _gelBabyNumber--;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                    #endregion
                    #region "Broken Walls"
                    if (tempTile.Type == Tile.TileType.TileWall)
                    {
                        if (CalculatePercent(_brokenWallPercent))
                        {
                            tempTile.Type = Tile.TileType.TileBrokenwall;
                        }
                    }
                    #endregion
                    #region "Trapdoors"
                    if (tempTile.Type == Tile.TileType.TileFloor &&
                        tempTile.GetCountAdjacent(Tile.TileType.TileWall) < 3)
                    {
                        if (TerrainNumber >= TrapDoorStart)
                        {
                            if (CalculatePercent(_trapDoorPercent))
                            {
                                tempTile.Type = Tile.TileType.TileTrapdoor;
                            }
                        }
                    }
                    #endregion
                    #region "Special Zombie"
                    if (_currentSpecial == SpecialRoom.SrZombie)
                    {
                        if (tempTile.Type == Tile.TileType.TileFloor)
                        {
                            if (_zombieNumber > 0)
                            {
                                tempTile.Type = Tile.TileType.TileZombie;
                                _zombieNumber--;
                            }
                        }
                    }

                    #endregion
                    #region "Special Evil Eye"
                    if (_currentSpecial == SpecialRoom.SrEvileye)
                    {
                        if (tempTile.Type == Tile.TileType.TileFloor)
                        {
                            if (_evilEyeNumber > 0)
                            {
                                tempTile.Type = Tile.TileType.TileEvileye;
                                _evilEyeNumber--;

                                if (TerrainNumber >= EvilEyeSentryStart && _evilEyeSentryNumber > 0)
                                {
                                    tempTile.Type = Tile.TileType.TileEvileyesentry;
                                    _evilEyeSentryNumber--;
                                }
                            }
                            else if (tempTile.GetCountAdjacent(Tile.TileType.TileFloor) > 7)
                            {
                                tempTile.Type = Tile.TileType.TileWall;
                            }
                        }
                    }
                    #endregion
                    #region "Special Roach Queen Queen"
                    if (_currentSpecial == SpecialRoom.SrRoachqueen)
                    {
                        if (tempTile.Type == Tile.TileType.TileFloor)
                        {
                            if (_roachQueenNumber > 0)
                            {
                                tempTile.Type = Tile.TileType.TileRoachqueen;
                                tempTile.Flags += "spawns roach queens";
                                _roachQueenNumber--;
                            }
                        }
                    }
                    #endregion
                    #region "Special Gel Baby"
                    if (_currentSpecial == SpecialRoom.SrGelbaby)
                    {
                        if (tempTile.Type == Tile.TileType.TileFloor)
                        {
                            if (_gelBabyNumber > 0)
                            {
                                tempTile.Type = Tile.TileType.TileGelbaby;
                                _gelBabyNumber--;
                            }
                        }
                    }
                    #endregion
                    #region "Special Trapdoor"
                    if (_currentSpecial == SpecialRoom.SrTrapdoor)
                    {
                        if (tempTile.Type != Tile.TileType.TileFloor) continue;
                        tempTile.Type = Tile.TileType.TileTrapdoor;

                        if (_wraithwingNumber > 0)
                        {
                            tempTile.Type = Tile.TileType.TileWraithwing;
                            _wraithwingNumber--;
                        }
                        #endregion
                    }
                }
            }

            #region "Orthogonal Squares"
            foreach (Tile tempTile in from tempRoom in terrain.Rooms
                where TerrainNumber >= OrthoSquareStart
                where CalculatePercent(_orthoSquarePercent)
                from tempTile in tempRoom.Tiles
                select tempTile)
            {
                if(tempTile.Type != Tile.TileType.TileOrb)
                tempTile.Flags += "orthosquare";
            }
            #endregion

            for (int iY = 0; iY < dY; iY++)
            {
                for (int iX = 0; iX < dX; iX++)
                {
                    Tile tempTile = terrain.Tiles[iX, iY];

                    if (tempTile.X <= 1 || tempTile.X >= terrain.SizeX - 1 || tempTile.Y <= 1 ||
                        tempTile.Y >= terrain.SizeY - 1) continue;
                    #region "Special Orthosquares"
                    if (_currentSpecial == SpecialRoom.SrOrthosquare)
                    {
                        if (tempTile.Type == Tile.TileType.TileFloor && tempTile.Type != Tile.TileType.TileOrb)
                        {
                            tempTile.Flags += "orthosquare";
                        }
                    }
                    #endregion
                    #region "Special Broken Walls"
                    if (_currentSpecial != SpecialRoom.SrBrokenwall) continue;
                    if (tempTile.Type != Tile.TileType.TileFloor || tempTile.Type == Tile.TileType.TileOrb) continue;
                    if (CalculatePercent(66))
                    {
                        tempTile.Type = Tile.TileType.TileBrokenwall;
                    }
                    #endregion
                }
            }

            terrain.GetRandomTile(Tile.TileType.TileWall).Type = Tile.TileType.TileFloor;
            terrain.GetRandomTile(Tile.TileType.TileWall).Type = Tile.TileType.TileFloor;
            terrain.GetRandomTile(Tile.TileType.TileWall).Type = Tile.TileType.TileFloor;

            if (_shop)
            {
                terrain.GetRandomTile(Tile.TileType.TileFloor).Type = Tile.TileType.TileShop;
            }
        }

        private static void FinalizeTerrain()
        {
            switch(TerrainNumber)
            {
                case RoachStart:
                    Log.AddEntry("If blocked diagonally, tries to move vertically, then horizontally.");
                    Log.AddEntry("New element -- Roach: tries to move diagonally towards the player every turn.");
                    break;
                case RoachQueenStart:
                    Log.AddEntry("Every 30 turns it creates Roach Eggs in the 8 adjacent squares. After 4 turns, they hatch.");
                    Log.AddEntry("New element -- Roach Queen: calculates how a Roach would move and moves inversely.");
                    break;
                case EvilEyeStart:
                    Log.AddEntry("When awake it moves like a Roach. It can see the player over pits and creatures.");
                    Log.AddEntry("New element -- Evil Eye: sleeps until the player steps in its line of sight.");
                    break;
                case WraithwingStart:
                    Log.AddEntry("When in group, it attacks like a Roach. It can fly over pits.");
                    Log.AddEntry("New element -- Wraithwing: moves like a Roach Queen when alone.");
                    break;
                case GelBabyStart:
                    Log.AddEntry("If it can't move diagonally, though, it stops.");
                    Log.AddEntry("New element -- Gel Baby: moves like a Roach.");
                    break;
                case OrthoSquareStart:
                    Log.AddEntry("New element -- Orthogonal Square: prevents diagonal movement.");
                    break;
                case TrapDoorStart:
                    Log.AddEntry("New element -- Trapdoor: when the player steps off it, it becomes a pit.");
                    break;
                case EvilEyeSentryStart:
                    Log.AddEntry("When awake, it doesn't move, but it spawns an awake Evil Eye towards the player.");
                    Log.AddEntry("New element -- Evil Eye Sentry: can be awakened like an Evil Eye.");
                    break;
                case ZombieStart:
                    Log.AddEntry("If blocked by an obstacle, it tries to sidestep it.");
                    Log.AddEntry("Every turn it leaves an impassable trail that lasts 7 turns and destroys orth. tiles.");
                    Log.AddEntry("New element -- Zombie: flees until the player is near, then attack.");
                    break;
                case SpecialRoomStart:
                    Log.AddEntry("New mechanic -- Special Rooms: from now on, any room can be a special room.");
                    break;
                case BrainYellowStart:
                    Log.AddEntry("New element -- Yellow brain: makes all monsters sidestep obstacles until killed.");
                    break;
            }

            if(_shop)
                Log.AddEntry("A shop is in this room!");

            Log.AddEntry("---***--- Current room: " + TerrainNumber + ". ---***---");

            switch (_currentSpecial)
            {
                case SpecialRoom.SrEvileye:
                    Log.AddEntry("Special room: " + Game.Player.Name + " is surrounded by evil eyes!");
                    break;
                case SpecialRoom.SrGelbaby:
                    Log.AddEntry("Special room: " + Game.Player.Name + " is surrounded by gel babies!");
                    break;
                case SpecialRoom.SrRoachqueen:
                    Log.AddEntry("Special room: " + Game.Player.Name + " encounters a strange type of roach queen.");
                    break;
                case SpecialRoom.SrTrapdoor:
                    Log.AddEntry("Special room: there are trapdoors everywhere!");
                    break;
                case SpecialRoom.SrOrthosquare:
                    Log.AddEntry("Special room: there are orthogonal squares everywhere!");
                    break;
                case SpecialRoom.SrBrokenwall:
                    Log.AddEntry("Special room: there are broken walls everywhere!");
                    break;
                case SpecialRoom.SrZombie:
                    Log.AddEntry("Special room: " + Game.Player.Name + " is surrounded by zombies!");
                    break;
            }
        }
    }
}