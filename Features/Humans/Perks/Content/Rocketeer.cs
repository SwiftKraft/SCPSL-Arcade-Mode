using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using UnityEngine;

namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    [Perk("Rocketeer", Rarity.Rare)]
    public class Rocketeer(PerkInventory inv) : PerkBase(inv)
    {
        public override string Name => "Rocketeer";

        public override string Description => "Throwing a grenade will turn it into a rocket projectile.";

        public override void Init()
        {
            base.Init();
            PlayerEvents.ThrewProjectile += OnThrewProjectile;
        }

        private void OnThrewProjectile(PlayerThrewProjectileEventArgs ev)
        {
            if (ev.Player != Player || ev.Projectile is not ExplosiveGrenadeProjectile proj)
                return;

            ConvertRocket(Player, proj, 10f);
        }

        public override void Remove()
        {
            base.Remove();
            PlayerEvents.ThrewProjectile -= OnThrewProjectile;
        }

        public static void ConvertRocket(Player player, ExplosiveGrenadeProjectile proj, float speed)
        {
            proj.Rigidbody.position = player.Camera.position;
            proj.Rigidbody.rotation = player.Camera.rotation;
            proj.Rigidbody.useGravity = false;
            proj.Rigidbody.linearVelocity = player.Camera.forward * speed;
            proj.Rigidbody.angularVelocity = default;
            proj.Rigidbody.mass = 99999f;
            proj.Base.OnCollided += OnCollide;

            void OnCollide(Collision col)
            {
                if (col.collider.isTrigger)
                    return;

                proj.Base.OnCollided -= OnCollide;
                proj.FuseEnd();
            }
        }
    }
}
