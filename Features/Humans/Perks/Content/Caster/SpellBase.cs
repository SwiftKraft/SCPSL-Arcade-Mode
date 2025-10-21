﻿using SwiftArcadeMode.Utils.Sounds;
using System.IO;
using UnityEngine;

namespace SwiftArcadeMode.Features.Humans.Perks.Content.Caster
{
    public abstract class SpellBase
    {
        public CasterBase Caster { get; private set; }

        public abstract string Name { get; }
        public abstract Color BaseColor { get; }
        public abstract int RankIndex { get; }
        public abstract float CastTime { get; }

        public virtual string[] SoundList => [];

        public virtual void Init(CasterBase caster)
        {
            if (!SoundEffectManager.Disabled)
                LoadSounds(SoundList);
            Caster = caster;
        }

        public void LoadSounds(params string[] fileNames)
        {
            string path = Path.Combine(SoundEffectManager.BasePath, "Spells", Name);
            for (int i = 0; i < fileNames.Length; i++)
                SoundEffectManager.PreloadClip(GetType().Name + "." + fileNames[i], path, fileNames[i] + ".ogg");
        }

        public void PlaySound(Vector3 pos, string id, string name = "speaker", float volume = 1f, bool loop = false, float minDist = 5f, float maxDist = 15f) => SoundEffectManager.PlaySound(pos, GetType().Name + "." + id, name, volume, loop, minDist, maxDist);

        public void PlaySound(string id, string name = "speaker", float volume = 1f, bool loop = false, bool destroyOnEnd = true, float minDist = 5f, float maxDist = 15f) => SoundEffectManager.PlaySound(Caster.Player, GetType().Name + "." + id, name, volume, loop, destroyOnEnd, minDist, maxDist);

        public abstract void Cast();
    }
}
