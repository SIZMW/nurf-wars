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
    public class Nurf : Sprite
    {
        /*
         * Sprite characteristics
         */
        private int playerNumber;
        private const int NURF_SPEED = 500;

        /*
         * Sprite movement constants
         */
        private const int MOVE_LEFT = -1;
        private const int MOVE_RIGHT = 1;
        private const int spriteFallSpeed = -25; // Fall speed for jumping

        /*
         * Sprite game states
         */ 
        enum State
        {
            Walking,
            Dead, 
            Jumping
        }
        private State currentState = State.Walking;
        private KeyboardState previousKeyBoardState;

        /*
         * Sprite movment variables
         */
        private Vector2 currentDirection = Vector2.Zero;
        private Vector2 startingPosition = Vector2.Zero;
        private Vector2 spriteAcceleration = new Vector2(0, 10);

        /*
         * Jump gravity variables
         */
        private int jumpCount = 0;
        private float currentJumpSpeed = 0;
        private float currentSpriteY;
        private bool isJumping = false;
        
        /*
         * Nurf constructor
         * 
         * @param
         * player - The player number to define control scheme
         * windowWith - The screen X size
         * windowHeight - The screen Y size
         */ 
        public Nurf(int player, int windowWidth, int windowHeight)
        {
            playerNumber = player;

            if (playerNumber == 1)
            {
                flipSpriteTexture = true;
                spriteStartPosition = new Vector2(windowWidth, windowHeight);
            }
            else
            {
                flipSpriteTexture = false;
                spriteStartPosition = new Vector2(0, windowHeight);
            }

            spriteScale = 0.15f;

            base.MAX_WIDTH = windowWidth;
            base.MAX_HEIGHT = windowHeight;
        }

        /*
         * Load content from ContentManager from Game class
         * 
         * @param
         * contentManager - The ContentManager of the Game class
         * assetName - The name of file to load for Texture2D
         */
        public void LoadContent(ContentManager contentManager, string assetName)
        {
            spritePosition = spriteStartPosition;
            currentSpriteY = spritePosition.Y;
            base.LoadContent(contentManager, assetName);
        }

        /*
         * Updates nurf movement, jumping, and location
         * 
         * @param
         * gameTime - The GameTime from the Game class
         */ 
        public void Update(GameTime gameTime)
        {
            KeyboardState currentKeyBoardState = Keyboard.GetState();

            UpdateMovement(currentKeyBoardState);
            UpdateJumping(currentKeyBoardState, previousKeyBoardState);
            spriteRectangle = new Rectangle((int)spritePosition.X, (int)spritePosition.Y, (int)(spriteTexture.Width * spriteScale), (int)(spriteTexture.Height * spriteScale));

            this.BoundsCollision();

            previousKeyBoardState = currentKeyBoardState;
            spritePosition += currentDirection * spriteVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        /*
         * Updates sprite jump movement. Can jump by pressing space, not holding. Falls with gravity constant.
         * 
         * @param
         * currKeyState - The current Keyboard state
         * prevKeyState - The previous Keyboard state
         */ 
        private void UpdateJumping(KeyboardState currKeyState, KeyboardState prevKeyState)
        {
            if (isJumping)
            {
                spritePosition.Y += currentJumpSpeed;
                currentJumpSpeed += 1;

                if (spritePosition.Y >= currentSpriteY)
                {
                    spritePosition.Y = currentSpriteY;
                    isJumping = false;
                }
            }
            else
            {
                if (playerNumber == 1)
                {
                    if (currKeyState.IsKeyDown(Keys.Up) && prevKeyState.IsKeyUp(Keys.Up))
                    {
                        isJumping = true;
                        currentJumpSpeed = spriteFallSpeed;
                        jumpCount++;
                    }
                }
                else if (playerNumber == 2)
                {
                    if (currKeyState.IsKeyDown(Keys.W) && prevKeyState.IsKeyUp(Keys.W))
                    {
                        isJumping = true;
                        currentJumpSpeed = spriteFallSpeed;
                        jumpCount++;
                    }
                }
            }
        }

        /*
         * Returns how many times nurf sprite has jumped
         * 
         * @return
         * jumpCount - The number of jumps by the nurf player
         */ 
        public int GetJumpCount()
        {
            return jumpCount;
        }

        /*
         * Updates sprite movement based on key presses.
         * Flips image if necessary. Moves left and right as well.
         * Up and down movement currently removed.
         * 
         * @param
         * currKeyState - The current Keyboard state
         */ 
        private void UpdateMovement(KeyboardState currKeyState)
        {
            if (currentState == State.Walking)
            {
                spriteVelocity = Vector2.Zero;
                currentDirection = Vector2.Zero;

                if (playerNumber == 1)
                {
                    if (currKeyState.IsKeyDown(Keys.Left))
                    {
                        spriteVelocity.X = NURF_SPEED;
                        currentDirection.X = MOVE_LEFT;
                        flipSpriteTexture = true;
                    }
                    else if (currKeyState.IsKeyDown(Keys.Right))
                    {
                        spriteVelocity.X = NURF_SPEED;
                        currentDirection.X = MOVE_RIGHT;
                        flipSpriteTexture = false;
                    }
                }
                else if (playerNumber == 2)
                {
                    if (currKeyState.IsKeyDown(Keys.A))
                    {
                        spriteVelocity.X = NURF_SPEED;
                        currentDirection.X = MOVE_LEFT;
                        flipSpriteTexture = true;
                    }
                    else if (currKeyState.IsKeyDown(Keys.D))
                    {
                        spriteVelocity.X = NURF_SPEED;
                        currentDirection.X = MOVE_RIGHT;
                        flipSpriteTexture = false;
                    }
                }
            }
        }

        /*
         * Draws nurf to screen. Uses base class for drawing
         * 
         * @param
         * spriteBatch - The SpriteBatch from Game class
         */ 
        public void Draw(SpriteBatch spriteBatch)
        {
            if (currentState != State.Dead)
            {
                base.Draw(spriteBatch);
            }
        }

        /*
         * Gets player index
         */
        public int GetPlayerNumber()
        {
            return playerNumber;
        }

        /*
         * Checks sprite position if it is within the window bounds.
         * If outside the boundaries, place sprite at the boundary
         */
        private void BoundsCollision()
        {
            if (spritePosition.X > MAX_WIDTH - (spriteTexture.Width * spriteScale))
            {
                spritePosition.X = MAX_WIDTH - (spriteTexture.Width * spriteScale);
            }
            else if (spritePosition.X < 0 || spriteRectangle.X < 0)
            {
                spritePosition.X = 1;
            }

            if (spritePosition.Y > MAX_HEIGHT - (spriteTexture.Height * spriteScale))
            {
                spritePosition.Y = MAX_HEIGHT - (spriteTexture.Height * spriteScale);
            }
            else if (spritePosition.Y < 0)
            {
                spritePosition.Y = 1;
            }
        }

        /*
         * Resets nurf
         */ 
        public void ResetNurf()
        {
            spritePosition = spriteStartPosition;
            currentSpriteY = spritePosition.Y;
            jumpCount = 0;
            spriteVelocity = Vector2.Zero;
            currentDirection = Vector2.Zero;
            currentJumpSpeed = 0;
        }
    }
}