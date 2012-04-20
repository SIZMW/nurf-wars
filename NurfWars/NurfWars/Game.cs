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
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class NurfGame : Microsoft.Xna.Framework.Game
    {
        /*
         * Game graphics variables
         */
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private SpriteFont gameFont;

        /*
         * Sprite variables and management
         */
        private Nurf nurfSprite1;
        private Nurf nurfSprite2;
        private BallManager ballManager;

        /*
         * Menu button variables
         */
        private Rectangle startButtonRectangle;
        private Rectangle helpButtonRectangle;
        private Rectangle exitButtonRectangle;
        private Texture2D startButton;
        private Texture2D helpButton;
        private Texture2D exitButton;

        /*
         * Game state variables
         */
        private enum GameState
        {
            StartMenu,
            Help,
            Game,
            Results,
        }

        private MouseState currentMouseState;
        private MouseState previousMouseState;
        private KeyboardState previousKeyboardState;
        private KeyboardState currentKeyboardState;
        private GameState currentGameState;

        /*
         * Timer related variables
         */
        private const int DISPLAY_RESULTS_COUNT = 250;
        private int displayResultsCount = 0;
        private bool collisionsOn = true;
        private string finalSurviveTime = "";

        private double totalGameMinutes = 0;
        private double totalGameSeconds = 0;
        private double totalGameMilliseconds = 0;

        /*
         * Various game textures
         */
        private Texture2D backgroundImage;
        private Texture2D gameOverImage;
        private Texture2D titleImage;
        private Texture2D helpImage;
        private Texture2D helpScreenTitle;
        private Texture2D helpText;
        private Texture2D playerOneControlsText;
        private Texture2D playerTwoControlsText;

        /*
         * Scoring variables
         */
        private int playerOneJumpCount = 0;
        private int playerTwoJumpCount = 0;

        private string playerOneResult = "";
        private string playerTwoResult = "";

        /*
         * Screen variables
         */
        private int SCREEN_WIDTH;
        private int SCREEN_HEIGHT;

        /*
         * Music variables
         */
        private Song gameMusicNoBorders;
        private Song gameMusicChannelBlackout;
        private int songNumber = 1;

        /*
         * Game constructor
         */
        public NurfGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 1440;
            graphics.PreferredBackBufferHeight = 960;

            SCREEN_WIDTH = 1440;
            SCREEN_HEIGHT = 960;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            nurfSprite1 = new Nurf(1, SCREEN_WIDTH, SCREEN_HEIGHT);
            nurfSprite2 = new Nurf(2, SCREEN_WIDTH, SCREEN_HEIGHT);

            ballManager = new BallManager(SCREEN_WIDTH, SCREEN_HEIGHT);
            ballManager.Initialize();

            previousMouseState = Mouse.GetState();
            previousKeyboardState = Keyboard.GetState();

            currentGameState = GameState.StartMenu;

            this.IsMouseVisible = true;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            /*
             * Images
             */
            nurfSprite1.LoadContent(this.Content, "images/nurf-blue");
            nurfSprite2.LoadContent(this.Content, "images/nurf-red");

            ballManager.LoadContent(this.Content, "images/fireball");

            startButton = Content.Load<Texture2D>("images/start-text");
            helpButton = Content.Load<Texture2D>("images/help");
            exitButton = Content.Load<Texture2D>("images/exit");

            backgroundImage = Content.Load<Texture2D>("images/background-image");
            gameOverImage = Content.Load<Texture2D>("images/game-over-text");
            titleImage = Content.Load<Texture2D>("images/title-text");

            helpImage = Content.Load<Texture2D>("images/help-screen");
            helpScreenTitle = Content.Load<Texture2D>("images/objective-title-text");
            helpText = Content.Load<Texture2D>("images/help-text");
            playerOneControlsText = Content.Load<Texture2D>("images/player-one-controls-text");
            playerTwoControlsText = Content.Load<Texture2D>("images/player-two-controls-text");

            gameFont = Content.Load<SpriteFont>("fonts/font");

            /*
             * Music
             */
            gameMusicNoBorders = Content.Load<Song>("music/no-borders-pound");
            gameMusicChannelBlackout = Content.Load<Song>("music/channel-blackout");

            /*
             * Menu button placement
             */
            startButtonRectangle = new Rectangle((SCREEN_WIDTH / 2) - startButton.Width / 2, (SCREEN_HEIGHT / 2) - (startButton.Height * 2),
                startButton.Width / (SCREEN_WIDTH / SCREEN_HEIGHT), startButton.Height / (SCREEN_WIDTH / SCREEN_HEIGHT));
            helpButtonRectangle = new Rectangle((SCREEN_WIDTH / 2) - helpButton.Width / 2, (SCREEN_HEIGHT / 2)
                - (helpButton.Height / 2), helpButton.Width, helpButton.Height);
            exitButtonRectangle = new Rectangle((SCREEN_WIDTH / 2) - exitButton.Width / 2, (SCREEN_HEIGHT / 2)
                + (exitButton.Height), exitButton.Width, exitButton.Height);

            MediaPlayer.Play((songNumber == 1) ? gameMusicNoBorders : gameMusicChannelBlackout);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            Content.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            currentMouseState = Mouse.GetState();
            currentKeyboardState = Keyboard.GetState();

            /*
             * Play song based on button press
             */
            if (previousKeyboardState.IsKeyDown(Keys.M) && currentKeyboardState.IsKeyUp(Keys.M))
            {
                if (songNumber == 2)
                {
                    MediaPlayer.Play(gameMusicNoBorders);
                    songNumber = 1;
                }
                else if (songNumber == 1)
                {
                    MediaPlayer.Play(gameMusicChannelBlackout);
                    songNumber = 2;
                }
            }

            /*
             * Update timer only when game is running
             */
            if (currentGameState != GameState.StartMenu || currentGameState != GameState.Results || currentGameState != GameState.Help)
            {
                this.TimerCheckAndUpdate(gameTime);
            }

            /*
             * Start menu actions
             */
            if (currentGameState == GameState.StartMenu)
            {
                if (previousMouseState.LeftButton == ButtonState.Pressed && currentMouseState.LeftButton == ButtonState.Released)
                {
                    MouseClicked(previousMouseState.X, previousMouseState.Y);
                }

                if (previousKeyboardState.IsKeyDown(Keys.Escape) && currentKeyboardState.IsKeyUp(Keys.Escape))
                {
                    this.ExitGame();
                }
            }

            if (currentGameState == GameState.Help)
            {
                if (previousKeyboardState.IsKeyDown(Keys.Escape) && currentKeyboardState.IsKeyUp(Keys.Escape))
                {
                    currentGameState = GameState.StartMenu;
                }
            }

            /*
             * Results screen actions
             */
            if (currentGameState == GameState.Results)
            {
                if (previousKeyboardState.IsKeyDown(Keys.Escape) && currentKeyboardState.IsKeyUp(Keys.Escape))
                {
                    this.ResetGame();
                    currentGameState = GameState.StartMenu;
                }
            }

            /*
             * Game screen actions
             */
            if (currentGameState == GameState.Game)
            {
                if (previousKeyboardState.IsKeyDown(Keys.Escape) && currentKeyboardState.IsKeyUp(Keys.Escape))
                {
                    currentGameState = GameState.Results;
                    this.GetGameResults(0);
                }

                if (ballManager.CheckNurfToBallCollisions(nurfSprite1.GetSpriteRectangle()) && collisionsOn)
                {
                    currentGameState = GameState.Results;
                    this.GetGameResults(nurfSprite1.GetPlayerNumber());
                }
                else if (ballManager.CheckNurfToBallCollisions(nurfSprite2.GetSpriteRectangle()) && collisionsOn)
                {
                    currentGameState = GameState.Results;
                    this.GetGameResults(nurfSprite2.GetPlayerNumber());
                }
                else
                {
                    nurfSprite1.Update(gameTime);
                    nurfSprite2.Update(gameTime);
                    this.UpdateJumpCounts();

                    ballManager.Update();

                    /*
                     * Update ball collisions
                     */
                    for (int i = 0; i < ballManager.GetBallIndex(); i++)
                    {
                        for (int j = 1; j < ballManager.GetBallIndex(); j++)
                        {
                            /*
                             * Checks if balls are touching
                             */
                            if (ballManager.CheckBallToBallCollisions(i, j))
                            {
                                ballManager.UpdateBallToBallCollisions(i, j);
                            }
                        }
                    }
                }
            }

            previousMouseState = currentMouseState;
            previousKeyboardState = currentKeyboardState;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            spriteBatch.Draw(backgroundImage, new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT), Color.White);

            /*
             * Startmenu draw actions
             */
            if (currentGameState == GameState.StartMenu)
            {
                spriteBatch.Draw(titleImage, new Rectangle((SCREEN_WIDTH / 2) - (titleImage.Width / 2), 100, titleImage.Width, titleImage.Height), Color.White);
                spriteBatch.Draw(startButton, startButtonRectangle, Color.White);
                spriteBatch.Draw(helpButton, helpButtonRectangle, Color.White);
                spriteBatch.Draw(exitButton, exitButtonRectangle, Color.White);
                this.ResetTimers();
            }

            /*
             * Help draw actions
             */
            if (currentGameState == GameState.Help)
            {
                spriteBatch.Draw(helpScreenTitle, new Rectangle((SCREEN_WIDTH / 2) - (helpScreenTitle.Width / 2), SCREEN_HEIGHT / 10, helpScreenTitle.Width, helpScreenTitle.Height), Color.White);
                spriteBatch.Draw(helpText, new Rectangle((SCREEN_WIDTH / 2) - (helpText.Width / 2), 200 + helpScreenTitle.Height, helpText.Width, helpText.Height), Color.White);
                spriteBatch.Draw(playerOneControlsText, new Rectangle((SCREEN_WIDTH / 2) - (helpText.Width / 2) - 100, (SCREEN_HEIGHT / 2) + 100, playerOneControlsText.Width, playerOneControlsText.Height), Color.White); ;
                spriteBatch.Draw(playerTwoControlsText, new Rectangle((SCREEN_WIDTH / 2) + (helpText.Width / 4) - 100, (SCREEN_HEIGHT / 2) + 100, playerTwoControlsText.Width, playerTwoControlsText.Height), Color.White);
            }

            /*
             * Game draw actions
             */
            else if (currentGameState == GameState.Game)
            {
                ballManager.Draw(spriteBatch);

                if ((int)totalGameSeconds % ballManager.GetBallSpawnInterval() == 0 && gameTime.TotalGameTime.Milliseconds % 360 == 0)
                {
                    ballManager.IncrementBallIndex();
                }

                nurfSprite1.Draw(this.spriteBatch);
                nurfSprite2.Draw(this.spriteBatch);

                spriteBatch.DrawString(gameFont,
                    "Elapsed Game Time: " + totalGameMinutes + ":" + totalGameSeconds.ToString().Substring(0, 5),
                    new Vector2((SCREEN_WIDTH / 2) - (gameFont.MeasureString("Elapsed Game Time: "
                    + totalGameMinutes + ":" + "0.0000").X / 2), 0), Color.White);

                if (totalGameSeconds % ballManager.GetBallSpawnInterval() > 4 || totalGameSeconds % ballManager.GetBallSpawnInterval() < 1)
                {
                    spriteBatch.DrawString(gameFont, "LAUNCHING BALL...",
                        new Vector2((SCREEN_WIDTH / 2) - (gameFont.MeasureString
                        ("LAUNCHING BALL...").X / 2), 30), Color.Red);
                }
            }

            /*
             * Results draw actions
             */
            else if (currentGameState == GameState.Results)
            {
                if (displayResultsCount < DISPLAY_RESULTS_COUNT)
                {
                    spriteBatch.Draw(gameOverImage, new Rectangle((SCREEN_WIDTH / 2) - (gameOverImage.Width / 2), (SCREEN_HEIGHT / 2) - (gameOverImage.Height / 2), gameOverImage.Width, gameOverImage.Height), Color.White);
                    spriteBatch.DrawString(gameFont, "  " + "Player One: " + playerOneResult, Vector2.Zero, Color.Blue);

                    spriteBatch.DrawString(gameFont, "Player Two: " + playerTwoResult + "  ", new Vector2(SCREEN_WIDTH
                        - (gameFont.MeasureString("Player Two: " + playerTwoResult + "  ").X), 0), Color.Red);

                    spriteBatch.DrawString(gameFont, "  " + "Player 1 Jump Count: " + playerOneJumpCount, new Vector2(0, gameFont.MeasureString("M").Y), Color.Blue);

                    spriteBatch.DrawString(gameFont, "Player 2 Jump Count: " + playerTwoJumpCount + "  ", new Vector2(SCREEN_WIDTH
                        - (gameFont.MeasureString("Player 2 Jump Count: " + playerTwoJumpCount + "  ")).X, gameFont.MeasureString("M").Y), Color.Red);

                    spriteBatch.DrawString(gameFont, "Total Time Survived: " + finalSurviveTime, new Vector2((SCREEN_WIDTH / 2) - gameFont.MeasureString("Total Time Survived: " + finalSurviveTime).X / 2, 0), Color.White);

                    displayResultsCount++;
                }
                else
                {
                    currentGameState = GameState.StartMenu;
                    this.ResetGame();
                }
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        /*
         * Checks where mouse has been clicked and what action to perform based on press location
         * 
         * @param
         * x - The x coordinate of the click
         * y - The y coordinate of the click
         */
        private void MouseClicked(int x, int y)
        {
            Rectangle clickRectangle = new Rectangle(x, y, 10, 10);

            if (currentGameState == GameState.StartMenu)
            {
                if (clickRectangle.Intersects(startButtonRectangle))
                {
                    currentGameState = GameState.Game;
                }
                else if (clickRectangle.Intersects(helpButtonRectangle))
                {
                    currentGameState = GameState.Help;
                }
                else if (clickRectangle.Intersects(exitButtonRectangle))
                {
                    this.ExitGame();
                }
            }
        }

        /*
         * Gets results for end of game
         * 
         * @param
         * playerNumber - The player that has lost
         */
        private void GetGameResults(int playerNumber)
        {
            if (playerNumber == 1)
            {
                playerOneResult = "Loser";
                playerTwoResult = "Winner";
            }
            else if (playerNumber == 2)
            {
                playerTwoResult = "Loser";
                playerOneResult = "Winner";
            }
            else
            {
                playerOneResult = "QUIT";
                playerTwoResult = "QUIT";
            }
            finalSurviveTime = totalGameMinutes + ":" + (int)totalGameSeconds + "." + (int)totalGameMilliseconds;
        }

        /*
         * Gets jump counts from both sprites
         */
        private void UpdateJumpCounts()
        {
            playerOneJumpCount = nurfSprite1.GetJumpCount();
            playerTwoJumpCount = nurfSprite2.GetJumpCount();
        }

        /*
         * Updates all timer related variables
         */
        private void TimerCheckAndUpdate(GameTime gameTime)
        {
            totalGameSeconds += gameTime.ElapsedGameTime.TotalSeconds;
            totalGameMilliseconds += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (totalGameSeconds >= 60)
            {
                totalGameMinutes++;
                totalGameSeconds %= 60;
            }
        }

        /*
         * Resets game by resetting sprites, balls and resetting game time
         */
        private void ResetGame()
        {
            nurfSprite1.ResetNurf();
            nurfSprite2.ResetNurf();

            ballManager.ResetBallList();

            displayResultsCount = 0;

            this.ResetTimers();
        }

        /*
         * Reset game timers
         */
        private void ResetTimers()
        {
            totalGameMinutes = 0;
            totalGameSeconds = 0;
            totalGameMilliseconds = 0;
        }

        /*
         * Quits game by unloading content and exiting
         */
        private void ExitGame()
        {
            this.UnloadContent();
            this.Exit();
        }
    }
}
