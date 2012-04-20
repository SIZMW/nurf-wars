using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace NurfWars
{
    public class Ball : Sprite
    {
        /*
         * Ball characteristics
         */ 
        private float ballMass = 5f;
        private float ballRadius;
        private float ballScale = 0.8f;

        /*
         * Randomly generated start position coordinates
         */ 
        private int randomStartX;
        private int randomStartY;

        /*
         * Randomly generated start speed components
         */ 
        private int randomSpeedX;
        private int randomSpeedY;

        /*
         * Ball constructor
         * 
         * @param
         * width - The screen width
         * height - The screen height
         * randGenerator - The generator for speed and start position
         */
        public Ball(int maxWidth, int maxHeight, Random randGenerator)
        {
            randomStartX = randGenerator.Next(maxWidth / 2 - 100, maxWidth / 2 + 100);
            randomStartY = randGenerator.Next(0, 10);

            randomSpeedX = randGenerator.Next(3, 8);
            randomSpeedX *= (randGenerator.Next(1, 3) < 2) ? -1 : 1;

            randomSpeedY = randGenerator.Next(3, 8);

            spriteVelocity = new Vector2(randomSpeedX, randomSpeedY);

            base.MAX_WIDTH = maxWidth;
            base.MAX_HEIGHT = maxHeight;
        }

        /*
         * Load content from ContentManager
         * 
         * @param
         * contentManager - The contentManager from Game class
         * assetName - The file to load for Texture2D
         */ 
        public void LoadContent(ContentManager contentManager, string assetName)
        {
            base.LoadContent(contentManager, assetName);
            spriteRectangle = new Rectangle(randomStartX, randomStartY, (int)(spriteTexture.Width * ballScale), (int)(spriteTexture.Height * ballScale));
            ballRadius = (float)((spriteTexture.Width * ballScale) / 2);
        }

        /*
         * Update the ball position
         */ 
        public void Update()
        {
            spriteRectangle.X += (int)(spriteVelocity.X);
            spriteRectangle.Y += (int)(spriteVelocity.Y);

            this.BallBoundCollision();
            this.BallOutOfBoundsPositioning();
        }

        /*
         * Draw the ball on screen
         * 
         * @param
         * spriteBatch - The SpriteBatch from Game class
         */ 
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(spriteTexture, spriteRectangle, Color.White);
        }

        /*
        * Ball bounds collision checking
        */
        private void BallBoundCollision()
        {
            if (spriteRectangle.X <= 0)
            {
                spriteVelocity.X *= -1;
            }
            if (spriteRectangle.X + spriteTexture.Width * ballScale >= MAX_WIDTH)
            {
                spriteVelocity.X *= -1;
            }
            if (spriteRectangle.Y <= 0)
            {
                spriteVelocity.Y *= -1;
            }
            if (spriteRectangle.Y + spriteTexture.Height * ballScale >= MAX_HEIGHT)
            {
                spriteVelocity.Y *= -1;
            }
        }

        /*
         * Checks ball velocity if it is actually moving
         */
        public void CheckBallVelocity()
        {
            if (spriteVelocity.X > 0 && spriteVelocity.X < 1)
            {
                spriteVelocity.X = 2;
            }
            else if (spriteVelocity.X < 0 && spriteVelocity.X > -1)
            {
                spriteVelocity.X = -2;
            }

            if (spriteVelocity.Y > 0 && spriteVelocity.Y < 1)
            {
                spriteVelocity.Y = 2;
            }
            else if (spriteVelocity.Y < 0 && spriteVelocity.Y > -1)
            {
                spriteVelocity.X = -2;
            }
        }

        /*
         * Checks if ball is partially off screen or stuck offscreen and corrects it
         */
        public void BallOutOfBoundsPositioning()
        {
            if (spriteRectangle.X < 0)
            {
                spriteRectangle.X = 0;
            }
            else if (spriteRectangle.X + (ballRadius * 2) > MAX_WIDTH)
            {
                spriteRectangle.X = (int)(MAX_WIDTH - (ballRadius * 2));
            }

            if (spriteRectangle.Y < 0)
            {
                spriteRectangle.Y = 0;
            }
            else if (spriteRectangle.Y + (ballRadius * 2) > MAX_HEIGHT)
            {
                spriteRectangle.Y = (int)(MAX_HEIGHT - (ballRadius * 2));
            }
        }

        /*
         * Resets ball to original spawn location
         */ 
        public void ResetBall()
        {
            spriteRectangle = new Rectangle(randomStartX, randomStartY, (int)(spriteTexture.Width * ballScale), (int)(spriteTexture.Height * ballScale));
            spriteVelocity = new Vector2(randomSpeedX, randomSpeedX);
        }

        /*
         * Returns ball velocity Vector2
         */
        public Vector2 GetVelocity()
        {
            return spriteVelocity;
        }

        /*
         * Sets ball velocity due to collision
         * 
         * @param
         * newVelocity - The new velocity Vector2
         */ 
        public void SetVelocity(Vector2 newVelocity)
        {
            spriteVelocity = newVelocity;
            this.CheckBallVelocity();
        }

        /*
         * Returns ball mass
         */ 
        public float GetBallMass()
        {
            return ballMass;
        }

        /*
         * Returns ball radius
         */ 
        public float GetBallRadius()
        {
            return ballRadius;
        }
    }
}
