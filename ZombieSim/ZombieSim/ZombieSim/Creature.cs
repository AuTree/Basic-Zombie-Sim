using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZombieSim
{
    class Creature
    {
        public enum State
        {
            Alive,
            Bitten,
            Zombie,
            Dead
        }
        public State currentState = State.Alive;

        private Texture2D aliveSprite;
        private Texture2D heroSprite;
        private Texture2D deadSprite;
        private Texture2D bittenSprite;
        private Texture2D zombieSprite;
        private Rectangle drawRect;

        //Our vars
        public Vector2 pos;
        public Cell myCurrentCell;
        public int diameter;

        private Collision Collider;
        private Vector2 gameBounds;
        private Vector2 direction;
        private Vector2 dest;
        private static Random rand;
        private DateTime timeNow;
        private DateTime timeUntilZombie;

        private int health;
        private int updateCollisionTime;
        private int collisionFrameTime;
        private int maxBittenTime;
        private int fightingChance;
        private int chanceToBeBitten;
        private int hasWeaponChance;
        private int hurtChance;

        private bool reachedDest;
        private bool hasWeapon;
        private bool isBitten;
        private bool overrideTravelCheck;

        private int screenH;
        private int screenW;
        private float speed;

        public Creature(Texture2D alive, Texture2D hero, Texture2D dead, Texture2D bitten, Texture2D zombie, int screenHeight, int screenWidth, Collision collider, Random generator, Vector2 worldBounds, int uCT)
        {
            //Init this
            Game1.onDrawUpdate += DrawUpdate;
            Game1.onGameUpdate += GameUpdate;

            aliveSprite = alive;
            heroSprite = hero;
            deadSprite = dead;
            bittenSprite = bitten;
            zombieSprite = zombie;
            screenH = screenHeight;
            screenW = screenWidth;
            Collider = collider;
            gameBounds = worldBounds;
            rand = generator;
            updateCollisionTime = uCT;

            pos = GenerateRandomPosition();
            reachedDest = true;
            isBitten = false;
            overrideTravelCheck = true;
            hasWeapon = false;

            drawRect.Height = 16;
            drawRect.Width = 16;
            chanceToBeBitten = 75;
            fightingChance = 35;
            maxBittenTime = 5;
            health = 4;
            diameter = 16;
            collisionFrameTime = 0;
            speed = 20.0f;
            hasWeaponChance = 25;
            hurtChance = 60;

            int tempWeaponChance = rand.Next(100);
            if (tempWeaponChance <= hasWeaponChance)
                hasWeapon = true;
        }

        private void GameUpdate(GameTime gameTime)
        {
            timeNow = DateTime.Now;
            collisionFrameTime++;

            if (collisionFrameTime > 4)//Update the collision every 4th frame;
                collisionFrameTime = 1;

            switch (currentState)
            {
                case State.Alive:
                    if (health <= 0)
                    {
                        currentState = State.Dead;
                        break;
                    }
                        
                    Travel(gameTime);

                    if(collisionFrameTime == updateCollisionTime)
                        Collider.CreatureCollideCheck(this);
                     
                    if (isBitten)
                        currentState = State.Bitten;

                    break;

                case State.Bitten:
                    Travel(gameTime);
                    if (timeUntilZombie <= timeNow)
                        currentState = State.Zombie;

                    break;

                case State.Dead:
                    //Just died, lets do some cleanup
                    Game1.onDrawUpdate -= DrawUpdate;
                    Game1.onGameUpdate -= GameUpdate;
                    break;

                case State.Zombie:
                    if (health <= 0)
                    {
                        currentState = State.Dead;
                        break;
                    }

                    Travel(gameTime);

                    if (collisionFrameTime == updateCollisionTime)
                        Collider.UpdatePosition(this);

                    break;

                default://In case something goes wrong
                    currentState = State.Dead;
                    break;
            }
            
        }

        private void Travel(GameTime gTime)
        {
            if (overrideTravelCheck)
                reachedDest = true;
            
            if (reachedDest)
            {//Get new destination
                dest = GenerateRandomPosition();
                direction = dest - pos;

                if(direction.LengthSquared() > 0)
                    direction.Normalize();

                if (overrideTravelCheck)
                    overrideTravelCheck = false;

                reachedDest = false;
            }
            else
            {  //travel there
                float elasped = (float)gTime.ElapsedGameTime.TotalSeconds;
                pos += direction * speed * elasped;

                float distanceTillDest = Vector2.Distance(pos, dest);
                if (distanceTillDest < 1)
                    reachedDest = true;
            }

            drawRect.X = (int)pos.X;
            drawRect.Y = (int)pos.Y;
        }

        private void DrawUpdate(SpriteBatch batch)
        {
            switch(currentState)
            {
                case State.Alive:
                    if (hasWeapon)
                        batch.Draw(heroSprite, drawRect, Color.White);
                    else
                        batch.Draw(aliveSprite, drawRect, Color.White);
                    break;
                case State.Bitten:
                    batch.Draw(bittenSprite, drawRect, Color.White);
                    break;
                case State.Dead:
                    batch.Draw(deadSprite, drawRect, Color.White);
                    break;
                case State.Zombie:
                    batch.Draw(zombieSprite, drawRect, Color.White);
                    break;
            }
        }

        public void OnCollide(List<Creature> collisions)
        {
            foreach (Creature other in collisions)
            {
                if(other.currentState == State.Zombie)
                {//Fight Code
                    int chance = rand.Next(100);
                    if(chance < fightingChance && hasWeapon)
                    {//We got that Zombie!
                        other.health -= 4;
                        fightingChance += 10; //More experience from the fight!
                        chanceToBeBitten -= 5;
                        hurtChance -= 5;

                        if (fightingChance > 90)
                            fightingChance = 90;

                        if (chanceToBeBitten < 10)
                            chanceToBeBitten = 10;

                        if (hurtChance < 10)
                            hurtChance = 10;
                    }

                    int gotHurt = rand.Next(100);
                    if (gotHurt < hurtChance)
                        health -= 1;

                    int gotBitten = rand.Next(100);
                    if(gotBitten < chanceToBeBitten)
                    {//we got bitten
                        currentState = State.Bitten;
                        timeUntilZombie = timeNow.AddSeconds((double)rand.Next(2, maxBittenTime));
                    }
                }
            }
        }

        private Vector2 GenerateRandomPosition()
        {
            Vector2 tempVector;
            tempVector.Y = (float)rand.Next((int)gameBounds.Y + (int) diameter, screenH - ((int)gameBounds.Y + (int)diameter));
            tempVector.X = (float)rand.Next((int)gameBounds.X + (int)diameter, screenW - ((int)gameBounds.X + (int)diameter));
            return tempVector;
        }

        private int GetRandomIntValue(int limit)
        {
            int chance = rand.Next(100);
            int value = rand.Next(limit);
            if (chance > 49)
                return value;
            else
                return value * -1;
        }
    }
}
