﻿namespace SwiftArcadeMode.Features.Humans.Perks.Content.Shopkeeper
{
    public abstract class ShopElement
    {
        public Shopkeeper Parent { get; protected set; }

        public virtual void Init(Shopkeeper parent) => Parent = parent;

        public virtual void Restock() { }

        public virtual void Remove() { }
    }
}
