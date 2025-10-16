using SwiftArcadeMode.Features.Humans.Perks;
using SwiftArcadeMode.Features.Humans.Perks.Content.Caster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftArcadeMode.Features.Game.Modes
{
    public class WizardBattle : GameModeBase
    {
        public override PerkSpawnRulesBase OverrideSpawnRules => new PerkRules();

        public override void End() { }

        public override void Start() { }

        public override void Tick() { }

        public class PerkRules : PerkSpawnRulesBasic
        {
            public static Type[] Pool = [
                typeof(Wizard),
                typeof(Druid),
                typeof(Sorcerer)
                ];

            public override Func<PerkAttribute, bool> Criteria => (p) => Pool.Contains(p.Perk);
        }
    }
}
