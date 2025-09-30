using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using MEC;
using SwiftArcadeMode.Utils.Projectiles;
using System;
using System.Linq;
using UnityEngine;

namespace SwiftArcadeMode.Features.Humans.Perks.Content.Caster
{
    public abstract class CasterBase(PerkInventory inv) : PerkItemReceiveBase(inv)
    {
        public abstract Type[] ListSpells();

        public Type[] Spells { get; private set; }

        public override ItemType ItemType => ItemType.KeycardCustomTaskForce;
        public override string PerkDescription => $"Allows you to cast {Name} spells.\nDrop the keycard to change spell, inspect to cast.";

        public virtual float KillCooldownReduction => 4f;
        public virtual float LessItemsCooldown => 3f;
        public abstract float RegularCooldown { get; }
        public override float Cooldown => Player == null || Player.Items.Count() < 3 ? LessItemsCooldown : RegularCooldown;
        public override int Limit => int.MaxValue;

        public override string ReadyMessage => Player.IsInventoryFull ? "Failed to refresh, no space in inventory." : "Spells refreshed!";

        public SpellBase CurrentSpell { get; private set; }
        public Item CurrentSpellItem { get; private set; }
        public ushort CurrentSpellItemSerial { get; private set; }
        public int CurrentSpellIndex
        {
            get => currentSpellIndex;
            set
            {
                if (value < 0 || Spells.Length <= 0)
                    return;

                currentSpellIndex = value % Spells.Length;
                CurrentSpell = Activator.CreateInstance(Spells[currentSpellIndex]) as SpellBase;
                CurrentSpell?.Init(this);
            }
        }
        int currentSpellIndex;

        bool casting;

        public override void Init()
        {
            base.Init();
            PlayerEvents.DroppingItem += OnDroppingItem;
            PlayerEvents.DroppedItem += OnDroppedItem;
            PlayerEvents.InspectingKeycard += OnInspectingKeycard;
            PlayerEvents.Dying += OnDying;
            PlayerEvents.ChangingItem += OnChangingItem;

            Spells = ListSpells();

            if (Player != null && Spells.Length <= 0)
                Player.GetPerkInventory().RemovePerk(this);

            CurrentSpellIndex = 0;
        }

        public override void Remove()
        {
            base.Remove();
            PlayerEvents.DroppingItem -= OnDroppingItem;
            PlayerEvents.DroppedItem -= OnDroppedItem;
            PlayerEvents.InspectingKeycard -= OnInspectingKeycard;
            PlayerEvents.Dying -= OnDying;
            PlayerEvents.ChangingItem -= OnChangingItem;

            RemoveCurrentSpellItem();
        }

        public override void Tick()
        {
            if (CurrentSpellItem == null || !Player.Items.Any(i => i.Serial == CurrentSpellItemSerial))
                base.Tick();
        }

        private void OnDroppedItem(LabApi.Events.Arguments.PlayerEvents.PlayerDroppedItemEventArgs ev)
        {
            if (ev.Pickup.Serial != CurrentSpellItemSerial)
                return;

            ev.Pickup.Destroy();
        }

        private void OnChangingItem(LabApi.Events.Arguments.PlayerEvents.PlayerChangingItemEventArgs ev)
        {
            if (ev.Player == Player && ev.NewItem == CurrentSpellItem)
                SendMessage("Equipped " + CurrentSpell.Name);

            if (!casting || ev.Player != Player || ev.OldItem != CurrentSpellItem)
                return;

            ev.IsAllowed = false;
        }

        private void OnInspectingKeycard(LabApi.Events.Arguments.PlayerEvents.PlayerInspectingKeycardEventArgs ev)
        {
            if (casting || ev.Player != Player || ev.KeycardItem != CurrentSpellItem)
                return;

            SendMessage("Casting " + CurrentSpell.Name, CurrentSpell.CastTime);
            casting = true;
            Timing.CallDelayed(CurrentSpell.CastTime, () =>
            {
                casting = false;
                if (!Player.IsAlive)
                    return;

                CurrentSpell.Cast();
                RemoveCurrentSpellItem();
                SendMessage("Casted " + CurrentSpell.Name, 1f);
            });
        }

        private void OnDying(LabApi.Events.Arguments.PlayerEvents.PlayerDyingEventArgs ev)
        {
            if (ev.Player != Player)
            {
                if (ev.Attacker == Player)
                    CooldownTimer.Tick(KillCooldownReduction);

                return;
            }

            casting = false;
            RemoveCurrentSpellItem();
        }

        private void RemoveCurrentSpellItem()
        {
            if (CurrentSpellItem != null)
            {
                Player.RemoveItem(CurrentSpellItem);
                CurrentSpellItem = null;
                CurrentSpellItemSerial = 0;
            }
        }

        private void OnDroppingItem(LabApi.Events.Arguments.PlayerEvents.PlayerDroppingItemEventArgs ev)
        {
            if (casting || ev.Player != Player || ev.Item != CurrentSpellItem)
                return;

            ev.IsAllowed = false;
            CurrentSpellIndex++;
            bool held = ev.Player.CurrentItem == CurrentSpellItem;

            Item it = GiveItem();

            if (held)
                ev.Player.CurrentItem = it;
        }

        public override Item GiveItem()
        {
            RemoveCurrentSpellItem();

            CurrentSpellItem = KeycardItem.CreateCustomKeycardTaskForce(Player, CurrentSpell.Name, CurrentSpell.Name, default, CurrentSpell.BaseColor, CurrentSpell.BaseColor, default, CurrentSpell.RankIndex);
            if (CurrentSpellItem != null)
                CurrentSpellItemSerial = CurrentSpellItem.Serial;

            return CurrentSpellItem;
        }

        public abstract class MagicProjectileBase(Vector3 initialPosition, Quaternion initialRotation, Vector3 initialVelocity, float lifetime = 10, Player owner = null) : ProjectileBase(initialPosition, initialRotation, initialVelocity, lifetime, owner)
        {
            protected PrimitiveObjectToy[] balls;
            protected LightSourceToy[] lights;

            public bool UseGravity { get; protected set; }
            public float SpinSpeed { get; protected set; }
            public float LightIntensity { get; protected set; }
            public Color BaseColor { get; protected set; }
            public Color LightColor { get; protected set; }

            public abstract LightSourceToy[] CreateLights();
            public abstract PrimitiveObjectToy[] CreateBalls();

            public override void Construct()
            {
                balls = CreateBalls();
                lights = CreateLights();
                Rigidbody.useGravity = UseGravity;
            }

            public override void Init()
            {
                base.Init();

                if (balls != null)
                    foreach (var ball in balls)
                    {
                        ball.Type = PrimitiveType.Sphere;
                        ball.Color = BaseColor;
                        ball.Flags = AdminToys.PrimitiveFlags.Visible;
                        ball.Spawn();
                    }

                if (lights != null)
                    foreach (var light in lights)
                    {
                        light.Color = LightColor;
                        light.Intensity = LightIntensity;
                        light.Spawn();
                    }
            }

            public override void Tick()
            {
                base.Tick();
                Rigidbody.transform.Rotate(Vector3.forward * (Time.fixedDeltaTime * SpinSpeed), Space.Self);
            }

            public override void Destroy()
            {
                base.Destroy();

                DestroyBalls();
                DestroyLights();
            }

            public void DestroyBalls()
            {
                if (balls != null)
                    foreach (var ball in balls)
                        if (ball.GameObject != null)
                            ball.Destroy();
                balls = null;
            }

            public void DestroyLights()
            {
                if (lights != null)
                    foreach (var light in lights)
                        if (light.GameObject != null)
                            light.Destroy();
                lights = null;
            }
        }
    }
}
