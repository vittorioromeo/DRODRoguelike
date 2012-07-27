#region
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SFML.Audio;
using SFML.Graphics;

#endregion

namespace DRODRoguelike
{
    public static class Resources
    {
        public static bool Sounds { get; set; }
        public static bool Music { get; set; }

        public static Image TitleBar { get; set; }

        public static List<Image> Styles { get; set; }
        public static List<Tileset> Tilesets { get; set; }
        public static Image GeneralTilesImage { get; set; }
        public static Tileset GeneralTiles { get; set; }

        public static Image ParticleBlood { get; set; }
        public static Image ParticleDebris { get; set; }
        public static Image ParticleGel { get; set; }
        public static Image ParticleBomb { get; set; }

        public static Image HUDBackground { get; set; }

        public static Image IconNull { get; set; }
        public static Image IconPickAxe { get; set; }
        public static Image IconHandBomb { get; set; }
        public static Image IconTrapdoor { get; set; }
        public static Image IconShield { get; set; }
        public static Image IconPrism { get; set; }
        public static Image IconThrowingKnife { get; set; }
        public static Image IconMimicPotion { get; set; }

        public static SoundBuffer SoundBufferBrokenWall { get; set; }
        public static SoundBuffer SoundBufferDeath { get; set; }
        public static SoundBuffer SoundBufferEvilEye { get; set; }
        public static SoundBuffer SoundBufferKill1 { get; set; }
        public static SoundBuffer SoundBufferKill2 { get; set; }
        public static SoundBuffer SoundBufferKill3 { get; set; }
        public static SoundBuffer SoundBufferLaugh1 { get; set; }
        public static SoundBuffer SoundBufferLaugh2 { get; set; }
        public static SoundBuffer SoundBufferLaugh3 { get; set; }
        public static SoundBuffer SoundBufferMimic { get; set; }
        public static SoundBuffer SoundBufferNoBrains { get; set; }
        public static SoundBuffer SoundBufferPitBump { get; set; }
        public static SoundBuffer SoundBufferPotion { get; set; }
        public static SoundBuffer SoundBufferStep1 { get; set; }
        public static SoundBuffer SoundBufferStep2 { get; set; }
        public static SoundBuffer SoundBufferSwing { get; set; }
        public static SoundBuffer SoundBufferTrapdoor { get; set; }
        public static SoundBuffer SoundBufferWallBump { get; set; }
        public static SoundBuffer SoundBufferOrbHit { get; set; }

        public static Sound SoundBrokenWall { get; set; }
        public static Sound SoundDeath { get; set; }
        public static Sound SoundEvilEye { get; set; }
        public static Sound SoundMimic { get; set; }
        public static Sound SoundNoBrains { get; set; }
        public static Sound SoundPitBump { get; set; }
        public static Sound SoundPotion { get; set; }
        public static Sound SoundSwing { get; set; }
        public static Sound SoundTrapdoor { get; set; }
        public static Sound SoundWallBump { get; set; }
        public static Sound SoundOrbHit { get; set; }

        public static Sound SoundKill { get; set; }
        public static Sound SoundLaugh { get; set; }
        public static Sound SoundStep { get; set; }

        public static Font Epilog { get; set; }
        public static Font TomsNewRoman { get; set; }

        public static Music CurrentMusic { get; set; }

        public static void Initialize()
        {
            Sounds = Helper.INIParser.GetSetting("Audio", "Sound") == "1";
            Music = Helper.INIParser.GetSetting("Audio", "Music") == "1";
            InitializeStyles ();
            InitializeImages ();
            InitializeFonts ();
            InitializeSoundBuffers ();
            InitializeSounds ();
            InitializeMusic ();
        }

        private static void InitializeStyles()
        {
            Styles = new List<Image> ();
            Tilesets = new List<Tileset> ();

            DirectoryInfo d = new DirectoryInfo(Environment.CurrentDirectory + @"/Data/Images/Styles/");

            foreach (FileInfo f in d.GetFiles ().Where(f => f.Extension.EndsWith("png")))
            {
                Styles.Add(new Image(f.FullName));
            }

            foreach (Image t in Styles)
            {
                Tilesets.Add(new Tileset(t, 14));
            }
        }

        private static void InitializeImages()
        {
            TitleBar = new Image(Environment.CurrentDirectory + @"/Data/Images/TitleBar.png");

            GeneralTilesImage = new Image(Environment.CurrentDirectory + @"/Data/Images/GeneralTiles.png");
            ParticleBlood = new Image(Environment.CurrentDirectory + @"/Data/Images/Particles/ParticleBlood.png");
            ParticleDebris = new Image(Environment.CurrentDirectory + @"/Data/Images/Particles/ParticleDebris.png");
            ParticleGel = new Image(Environment.CurrentDirectory + @"/Data/Images/Particles/ParticleGel.png");
            ParticleBomb = new Image(Environment.CurrentDirectory + @"/Data/Images/Particles/ParticleBomb.png");

            IconNull = new Image(Environment.CurrentDirectory + @"/Data/Images/Icons/IconNull.png");
            IconHandBomb = new Image(Environment.CurrentDirectory + @"/Data/Images/Icons/IconHandBomb.png");
            IconPickAxe = new Image(Environment.CurrentDirectory + @"/Data/Images/Icons/IconPickAxe.png");
            IconTrapdoor = new Image(Environment.CurrentDirectory + @"/Data/Images/Icons/IconTrapdoor.png");
            IconShield = new Image(Environment.CurrentDirectory + @"/Data/Images/Icons/IconShield.png");
            IconPrism = new Image(Environment.CurrentDirectory + @"/Data/Images/Icons/IconPrism.png");
            IconThrowingKnife = new Image(Environment.CurrentDirectory + @"/Data/Images/Icons/IconThrowingKnife.png");
            IconMimicPotion = new Image(Environment.CurrentDirectory + @"/Data/Images/Icons/IconMimicPotion.png");

            HUDBackground = new Image(Environment.CurrentDirectory + @"/Data/Images/HUDBackground.png");

            GeneralTiles = new Tileset(GeneralTilesImage, 14);
        }

        private static void InitializeFonts()
        {
            Epilog = new Font(Environment.CurrentDirectory + @"/Data/Fonts/epilog.ttf");
            TomsNewRoman = new Font(Environment.CurrentDirectory + @"/Data/Fonts/tomnr.ttf");
        }

        private static void InitializeSoundBuffers()
        {
            SoundBufferBrokenWall = new SoundBuffer(Environment.CurrentDirectory + @"/Data/Sounds/SoundBrokenWall.wav");
            SoundBufferDeath = new SoundBuffer(Environment.CurrentDirectory + @"/Data/Sounds/SoundDeath.wav");
            SoundBufferEvilEye = new SoundBuffer(Environment.CurrentDirectory + @"/Data/Sounds/SoundEvilEye.wav");
            SoundBufferKill1 = new SoundBuffer(Environment.CurrentDirectory + @"/Data/Sounds/SoundKill1.wav");
            SoundBufferKill2 = new SoundBuffer(Environment.CurrentDirectory + @"/Data/Sounds/SoundKill2.wav");
            SoundBufferKill3 = new SoundBuffer(Environment.CurrentDirectory + @"/Data/Sounds/SoundKill3.wav");
            SoundBufferLaugh1 = new SoundBuffer(Environment.CurrentDirectory + @"/Data/Sounds/SoundLaugh1.wav");
            SoundBufferLaugh2 = new SoundBuffer(Environment.CurrentDirectory + @"/Data/Sounds/SoundLaugh2.wav");
            SoundBufferLaugh3 = new SoundBuffer(Environment.CurrentDirectory + @"/Data/Sounds/SoundLaugh3.wav");
            SoundBufferMimic = new SoundBuffer(Environment.CurrentDirectory + @"/Data/Sounds/SoundMimic.wav");
            SoundBufferNoBrains = new SoundBuffer(Environment.CurrentDirectory + @"/Data/Sounds/SoundNoBrains.wav");
            SoundBufferPitBump = new SoundBuffer(Environment.CurrentDirectory + @"/Data/Sounds/SoundPitBump.wav");
            SoundBufferPotion = new SoundBuffer(Environment.CurrentDirectory + @"/Data/Sounds/SoundPotion.wav");
            SoundBufferStep1 = new SoundBuffer(Environment.CurrentDirectory + @"/Data/Sounds/SoundStep1.wav");
            SoundBufferStep2 = new SoundBuffer(Environment.CurrentDirectory + @"/Data/Sounds/SoundStep2.wav");
            SoundBufferSwing = new SoundBuffer(Environment.CurrentDirectory + @"/Data/Sounds/SoundSwing.wav");
            SoundBufferTrapdoor = new SoundBuffer(Environment.CurrentDirectory + @"/Data/Sounds/SoundTrapdoor.wav");
            SoundBufferWallBump = new SoundBuffer(Environment.CurrentDirectory + @"/Data/Sounds/SoundWallBump.wav");
            SoundBufferOrbHit = new SoundBuffer(Environment.CurrentDirectory + @"/Data/Sounds/SoundOrbHit.wav");
        }

        private static void InitializeSounds()
        {
            SoundBrokenWall = new Sound(SoundBufferBrokenWall);
            SoundDeath = new Sound(SoundBufferDeath);
            SoundEvilEye = new Sound(SoundBufferEvilEye);
            SoundKill = new Sound(SoundBufferKill1);
            SoundLaugh = new Sound(SoundBufferLaugh1);
            SoundMimic = new Sound(SoundBufferMimic);
            SoundNoBrains = new Sound(SoundBufferNoBrains);
            SoundPitBump = new Sound(SoundBufferPitBump);
            SoundPotion = new Sound(SoundBufferPotion);
            SoundStep = new Sound(SoundBufferStep1);
            SoundSwing = new Sound(SoundBufferSwing);
            SoundTrapdoor = new Sound(SoundBufferTrapdoor);
            SoundWallBump = new Sound(SoundBufferWallBump);
            SoundOrbHit = new Sound(SoundBufferOrbHit);
            SoundOrbHit.Pitch = 20;
        }

        public static void InitializeMusic()
        {
            if (!Music) return;
            string rndmusic = GetRandomMusic ();
            if (rndmusic == null) return;

            CurrentMusic = new Music(rndmusic) {Loop = true};
            CurrentMusic.Play ();
        }

        public static Sound GetSoundKill()
        {
            int soundNumber = Helper.Random.Next(0, 3);

            switch (soundNumber)
            {
                case 0:
                    SoundKill.SoundBuffer = SoundBufferKill1;
                    break;
                case 1:
                    SoundKill.SoundBuffer = SoundBufferKill2;
                    break;
                case 2:
                    SoundKill.SoundBuffer = SoundBufferKill3;
                    break;
            }

            return SoundKill;
        }
        public static Sound GetSoundLaugh()
        {
            int soundNumber = Helper.Random.Next(0, 3);

            switch (soundNumber)
            {
                case 0:
                    SoundLaugh.SoundBuffer = SoundBufferLaugh1;
                    break;
                case 1:
                    SoundLaugh.SoundBuffer = SoundBufferLaugh2;
                    break;
                case 2:
                    SoundLaugh.SoundBuffer = SoundBufferLaugh3;
                    break;
            }

            return SoundLaugh;
        }
        public static Sound GetSoundStep()
        {
            int soundNumber = Helper.Random.Next(0, 2);

            switch (soundNumber)
            {
                case 0:
                    SoundStep.SoundBuffer = SoundBufferStep1;
                    break;
                case 1:
                    SoundStep.SoundBuffer = SoundBufferStep2;
                    break;
            }

            return SoundStep;
        }

        public static string GetRandomMusic()
        {
            DirectoryInfo d = new DirectoryInfo(Environment.CurrentDirectory + @"/Data/Music/");
            List<string> paths = (from f in d.GetFiles () where f.Extension.EndsWith("ogg") select f.FullName).ToList ();

            if (paths.Count == 0)
            {
                Music = false;
                return null;
            }

            return paths[Helper.Random.Next(0, paths.Count)];
        }

        public static Tileset GetRandomTileset()
        {
            return Tilesets[Helper.Random.Next(0, Tilesets.Count)];
        }
    }
}