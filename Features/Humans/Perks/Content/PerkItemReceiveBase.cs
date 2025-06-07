using InventorySystem.Items;
using LabApi.Features.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftUHC.Features.Humans.Perks.Content
{
    public abstract class PerkItemReceiveBase(PerkInventory inv) : PerkCooldownBase(inv)
    {
        public abstract ItemType ItemType { get; }

        public virtual int Limit => 1;

        public override void Effect()
        {
            if (GetCount() <= Limit)
                GiveItem();
        }

        public override void Tick()
        {
            base.Tick();

            Trigger();
        }

        public int GetCount() => Player.Items.Count((i) => i.Type == ItemType);

        public virtual Item GiveItem() => Player.AddItem(ItemType, ItemAddReason.PickedUp);
    }
}
