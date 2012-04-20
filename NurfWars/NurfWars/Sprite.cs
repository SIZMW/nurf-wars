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
    public class Sprite
    {
        /*
         * Sprite texture and sizing
         */
        protected Texture2D spriteTexture;
        protected Rectangle spriteRectangle;
        protected Vector2 spritePosition;
        protected Vector2 spriteStartPosition;
        protected Vector2 spriteVelocity;

        /*
         * Sprite characteristics
         */ 
        protected float spriteScale;
        protected bool flipSpriteTexture = false;

        /*
        * Window constants for collisions
        */
        protected int MAX_HEIGHT;
        protected int MAX_WIDTH;

        /*
         * Load textures and images for sprites
         * 
         * @param
         * contentManager - The ContentManager from the Game class
         * assetName - The name of the file to load for textures
         */ 
        public void LoadContent(ContentManager contentManager, string assetName)
        {
            spriteTexture = contentManager.Load<Texture2D>(assetName);
            spriteRectangle = new Rectangle(0, 0, (int)(spriteTexture.Width * spriteScale), (int)(spriteTexture.Height * spriteScale));
        }

        /*
         * Update position based on speed and direction
         * 
         * @param
         * gameTime - The GameTime from the Game class
         * speed - The Vector2 defining the speed to move
         * direction - The Vector2 defining the direction to move
         */ 
        public void Update(GameTime gameTime, Vector2 speed, Vector2 direction)
        {
            spritePosition += direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        /*
         * Draw sprite to screen. Flips if necessary.
         * 
         * @param
         * spriteBatch - The SpriteBatch from the Game class
         */
        protected void Draw(SpriteBatch spriteBatch)
        {
            if (flipSpriteTexture)
            {
                spriteBatch.Draw(spriteTexture, spritePosition, new Rectangle(0, 0, spriteTexture.Width, spriteTexture.Height), Color.White, 0.0f, Vector2.Zero, spriteScale, SpriteEffects.FlipHorizontally, 0);
            }
            else
            {
                spriteBatch.Draw(spriteTexture, spritePosition, new Rectangle(0, 0, spriteTexture.Width, spriteTexture.Height), Color.White, 0.0f, Vector2.Zero, spriteScale, SpriteEffects.None, 0);
            }
        }

        /*
         * Returns rectangle of sprite to use for collisions
         */
        public Rectangle GetSpriteRectangle()
        {
            return spriteRectangle;
        }
    }
}
