using SwiftArcadeMode.Features.Humans.Perks;
using SwiftArcadeMode.Features.Humans.Perks.Content;
using SwiftArcadeMode.Features.Humans.Perks.Content.Caster;
using System;

namespace SwiftArcadeMode.Features.Game.Modes
{
    public class CasterBattle : GameModeBase
    {
        public override PerkSpawnRulesBase OverrideSpawnRules => new PerkRules();

        public override void End() { }

        public override void Start() { }

        public override void Tick() { }

        public class PerkRules : PerkSpawnRulesBasic
        {
            public static Type[] Pool = [
                typeof(SuperRegeneration),
                typeof(Regeneration),
                typeof(PerkSlotUpgrade),
                typeof(Ninjutsu),
                typeof(Vampire),
                typeof(Resilience),
                typeof(Marathoner),
                typeof(Streamer),
                typeof(Wizard),
                typeof(Druid),
                typeof(Sorcerer)
                ];

            public override Func<PerkAttribute, bool> Criteria => (p) => Pool.Contains(p.Perk);
        }
    }
}
