using Hints;
using LabApi.Features.Wrappers;

namespace SwiftUHC.Features.Humans.Perks
{
    public abstract class PerkBase(PerkInventory inv)
    {
        public Rarity Rarity { get; set; }

        public virtual string Name => "Unnamed Perk";

        public virtual string Description => "Functions unknown.";

        public readonly PerkInventory Inventory = inv;
        public Player Player => Inventory.Parent;

        public virtual void Init() { }

        public virtual void Tick() { }

        public virtual void Remove() { }

        public void SendMessage(string message, float duration = 3)
        {
            Player.SendHint($"<color={Rarity.GetColor()}><b>{Name}</b></color>\n{message}", [HintEffectPresets.FadeOut()], duration);
        }
    }

    public static class RarityExtensions
    {
        public static string GetColor(this Rarity rarity) => 
            rarity switch
        {
            Rarity.Common => "#FFFFFF",
            Rarity.Uncommon => "#00FF00",
            Rarity.Rare => "#00FFFF",
            Rarity.Epic => "#FF00FF",
            Rarity.Legendary => "#FFFF00",
            Rarity.Secret => "#FF0000",
            _ => "#FFFFFF"
        };
    }

    public enum Rarity : int
    {
        Common = 25,
        Uncommon = 18,
        Rare = 15,
        Epic = 9,
        Legendary = 5,
        Secret = 1
    }
}
