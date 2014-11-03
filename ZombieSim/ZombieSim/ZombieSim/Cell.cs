using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZombieSim
{
    class Cell
    {
        List<Creature> cellContents;

        public Cell()
        {
            cellContents = new List<Creature>();
        }

        public List<Creature> CheckForCollision(Creature whoToCheckFor)
        {
            List<Creature> collides = new List<Creature>();
            int diameter = whoToCheckFor.diameter;

            foreach (Creature item in cellContents)
            {
                Vector2 dist = whoToCheckFor.pos - item.pos;

                if (dist.LengthSquared() < diameter * diameter && item != whoToCheckFor)
                    collides.Add(item);
            }

            return collides;
        }

        public void AddToCell(Creature whoToAdd)
        {
            cellContents.Add(whoToAdd);
        }

        public void RemoveFromCell(Creature whoToRemove)
        {
            cellContents.Remove(whoToRemove);
        }
    }
}
