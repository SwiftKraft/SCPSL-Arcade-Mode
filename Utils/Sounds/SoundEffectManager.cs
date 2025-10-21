using LabApi.Features.Wrappers;
using System;
using System.IO;
using UnityEngine;

namespace SwiftArcadeMode.Utils.Sounds
{
    public static class SoundEffectManager
    {
        public static bool Disabled { get; private set; } = false;
        public static string BasePath;

        public static void PreloadClip(string id, params string[] folders)
        {
            if (Disabled || Type.GetType(nameof(AudioClipStorage)) == null)
            {
                Disabled = true;
                return;
            }

            if (AudioClipStorage.AudioClips.ContainsKey(id))
                return;

            AudioClipStorage.LoadClip(Path.Combine(BasePath, Path.Combine(folders)), id);
        }

        public static void PlaySound(Player player, string id, string name = "speaker", float volume = 1f, bool loop = false, bool destroyOnEnd = true, float minDist = 5f, float maxDist = 15f)
        {
            if (Disabled || Type.GetType(nameof(AudioPlayer)) == null)
            {
                Disabled = true;
                return;
            }

            AudioPlayer audioPlayer = AudioPlayer.CreateOrGet($"Player {player.Nickname}", onIntialCreation: (p) =>
            {
                p.transform.parent = player.GameObject.transform;
                Speaker speaker = p.AddSpeaker(name, isSpatial: true, minDistance: minDist, maxDistance: maxDist);

                speaker.transform.parent = player.GameObject.transform;
                speaker.transform.localPosition = Vector3.zero;
            });

            audioPlayer.AddClip(id, volume, loop, destroyOnEnd);
        }

        public static void PlaySound(Vector3 position, string id, string name = "speaker", float volume = 1f, bool loop = false, float minDist = 5f, float maxDist = 15f)
        {
            if (Disabled || Type.GetType(nameof(AudioPlayer)) == null)
            {
                Disabled = true;
                return;
            }

            AudioPlayer audioPlayer = AudioPlayer.CreateOrGet(position.ToString(), onIntialCreation: (p) =>
            {
                Speaker speaker = p.AddSpeaker(name, isSpatial: true, minDistance: minDist, maxDistance: maxDist);
                speaker.transform.position = position;
            });

            audioPlayer.AddClip(id, volume, loop, true);
        }
    }
}
