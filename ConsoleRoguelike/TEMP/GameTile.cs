using System;
using System.Collections.Generic;
using ConsoleRoguelike.Terrains;

namespace ConsoleRoguelike.Entities
{
    public class GameTile
    {
        private const string Character = "☺";
        private const string Container = "♦";
        private const string DarkDarkness = "▓";
        private const string Floor = " ";
        private const string LowDarkness = "░";
        private const string Player = "☻";
        private const string Wall = "█";
        private Entity _entity;

        public Entity Entity
        {
            get { return _entity; }
            set { _entity = value; }
        }

        public bool IsVisibleFrom(CharacterEntity characterEntity)
        {
            if (characterEntity == null) throw new ArgumentNullException("characterEntity");
            List<Point> linePoints = Utils.LinePoints(new Point(characterEntity.X, characterEntity.Y), new Point(Entity.X, Entity.Y));
            GameTile[,,] gameTiles = Entity.Game.GameTiles;

            if (Utils.Distance(characterEntity.X, characterEntity.Y, Entity.X, Entity.Y) < 3)
            {
                if (characterEntity != Entity)
                {
                    characterEntity.VisibleThisTurn.Add(Entity);
                }
                return true;
            }

            foreach (Point tempPoint in linePoints)
            {
                if (gameTiles[tempPoint.X, tempPoint.Y, 2].Entity.Obstacle)
                {
                    return false;
                }
            }

            if (characterEntity != Entity)
            {
                characterEntity.VisibleThisTurn.Add(Entity);
            }
            return true;
        }

        public override string ToString()
        {
            string result = "";

            if (Utils.Distance(Entity.Game.Player.X, Entity.Game.Player.Y, Entity.X, Entity.Y) >
                Entity.Game.Player.PerceptionRange)
            {
                return DarkDarkness;
            }

            if (Entity.Game.LOS == (true && IsVisibleFrom(Entity.Game.Player) == false))
            {
                if (Entity is Floor)
                {
                    return LowDarkness;
                }

                return DarkDarkness;
            }
            if (Entity is Floor)
            {
                result = Floor;
            }
            else if (Entity is Wall)
            {
                result = Wall;
            }
            else if (Entity is Pit)
            {
                result = DarkDarkness;
            }
            else if (Entity is ContainerEntity)
            {
                result = Container;
            }
            else if (Entity is CharacterEntity)
            {
                if (Entity.Special == "P")
                {
                    result = Player;
                }
                else
                {
                    result = Character;
                }
            }

            return result;
        }
    }
}