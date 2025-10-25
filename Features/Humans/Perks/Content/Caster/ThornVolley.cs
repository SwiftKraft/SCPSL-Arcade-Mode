using MEC;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SwiftArcadeMode.Features.Humans.Perks.Content.Caster
{
    public class ThornVolley : SpellBase
    {
        public override string Name => "Thorn Volley";

        public override Color BaseColor => new(0f, 0.4f, 0f);

        public override int RankIndex => 1;

        public override float CastTime => 0.5f;

        CoroutineHandle coroutine;

        public override void Cast()
        {
            new ThornShot.Projectile(Caster.Player.Camera.position, Caster.Player.Camera.rotation, Caster.Player.Camera.forward * 60f, 10f, Caster.Player);
            PlaySound("cast");

            coroutine = Timing.CallPeriodically(0.6f, 0.05f, () =>
            {
                if (!Caster.Player.IsAlive)
                {
                    Timing.KillCoroutines(coroutine);
                    return;
                }

                new ThornShot.Projectile(Caster.Player.Camera.position + Caster.Player.Camera.forward * 0.4f + Random.insideUnitSphere * 0.3f, Caster.Player.Camera.rotation, Caster.Player.Camera.forward * 60f, 10f, Caster.Player);
            });
        }
    }
}
