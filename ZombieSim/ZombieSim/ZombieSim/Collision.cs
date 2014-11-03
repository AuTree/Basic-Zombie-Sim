using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZombieSim
{
    class Collision
    {
        private float screenH, screenW;
        private int cellSizeInPixels;

        //Bounds equal dead space that is not covered by the cells. Stop creatures from going there!
        public int widthBounds;
        public int heightBounds;

        private List<List<Cell>> CollisionGrid = new List<List<Cell>>();

        public Collision (float screenHeight, float screenWidth, int cellSize)
        {
            screenH = screenHeight;
            screenW = screenWidth;
            cellSizeInPixels = cellSize;
            SetupCells();
        }

        private void SetupCells()
        {
            int cellsAcross = (int)screenW / cellSizeInPixels;
            int cellsHeight = (int)screenH / cellSizeInPixels;
            widthBounds = ((int)screenW - (cellsAcross * cellSizeInPixels)) / 2;
            heightBounds = ((int)screenH -(cellsHeight * cellSizeInPixels)) / 2;

            for (int i = 0; i < cellsHeight; i++)
            {
                List<Cell> Row = new List<Cell>();
                for (int j = 0; j < cellsAcross; j++)
                {
                    Row.Add(new Cell());
                }

                CollisionGrid.Add(Row);
            }
        }

        public void CreatureCollideCheck(Creature thisCreature)
        {
            int rowLookUp;
            int columnLookUp;

            //Lets found out what cell we should be in
            if (thisCreature.pos.X < cellSizeInPixels)
                rowLookUp = 0;
            else
                rowLookUp = ((int)thisCreature.pos.X / cellSizeInPixels) - 1;

            if(thisCreature.pos.Y < cellSizeInPixels)
                columnLookUp = 0;
            else
                columnLookUp = ((int)thisCreature.pos.Y / cellSizeInPixels) - 1;

            Cell currentCell = CollisionGrid[columnLookUp][rowLookUp];

            if(currentCell != thisCreature.myCurrentCell && thisCreature.myCurrentCell != null)
            {//Where have an old cell we need to change
                thisCreature.myCurrentCell.RemoveFromCell(thisCreature);
                currentCell.AddToCell(thisCreature);
                thisCreature.myCurrentCell = currentCell;
            }
            else if(thisCreature.myCurrentCell == null)
            {//this is our first time getting a cell; we was just created
                thisCreature.myCurrentCell = currentCell;
                currentCell.AddToCell(thisCreature);
            }

            List<Creature> collidedWith = currentCell.CheckForCollision(thisCreature);

            if(collidedWith.Count > 0)
            {
                /*
                bool atLeastOneZombie = false;

                foreach (Creature other in collidedWith)
                {
                    if (other.currentState == Creature.State.Zombie)
                        atLeastOneZombie = true;
                }

                if(atLeastOneZombie)*/
                    thisCreature.OnCollide(collidedWith);
            }
        }

        public void UpdatePosition(Creature whoToUpdate)
        {//Use this for zombies
            int rowLookUp;
            int columnLookUp;

            //Lets found out what cell we should be in
            if (whoToUpdate.pos.X < cellSizeInPixels)
                rowLookUp = 0;
            else
                rowLookUp = ((int)whoToUpdate.pos.X / cellSizeInPixels) - 1;

            if (whoToUpdate.pos.Y < cellSizeInPixels)
                columnLookUp = 0;
            else
                columnLookUp = ((int)whoToUpdate.pos.Y / cellSizeInPixels) - 1;

            Cell currentCell = CollisionGrid[columnLookUp][rowLookUp];

            if (currentCell != whoToUpdate.myCurrentCell && whoToUpdate.myCurrentCell != null)
            {//Where have an old cell we need to change
                whoToUpdate.myCurrentCell.RemoveFromCell(whoToUpdate);
                currentCell.AddToCell(whoToUpdate);
                whoToUpdate.myCurrentCell = currentCell;
            }
            else if (whoToUpdate.myCurrentCell == null)
            {//this is our first time getting a cell; we was just created
                whoToUpdate.myCurrentCell = currentCell;
                currentCell.AddToCell(whoToUpdate);
            }
        }

    }
}
