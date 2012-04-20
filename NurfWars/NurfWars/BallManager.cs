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
    public class BallManager
    {
        /*
         * BallManager variables
         */ 
        private Ball[] ballList = new Ball[1000];
        private Random randGenerator = new Random();

        private const int LAUNCH_BALL_TIMEOUT = 5;
        private int ballIndex = 0;

        /*
         * Screen size constants
         */ 
        private int SCREEN_WIDTH;
        private int SCREEN_HEIGHT;

        /*
         * BallManager constructor
         * 
         * @param
         * screenWidth - The screen width
         * screenHeight - The screen height
         */ 
        public BallManager(int screenWidth, int screenHeight)
        {
            SCREEN_WIDTH = screenWidth;
            SCREEN_HEIGHT = screenHeight;
        }

        /*
         * Initialize array of Ball objects
         */ 
        public void Initialize()
        {
            for (int i = 0; i < ballList.Length; i++)
            {
                ballList[i] = new Ball(SCREEN_WIDTH, SCREEN_HEIGHT, randGenerator);
            }
        }

        /*
         * Load texture for array of balls
         */
        public void LoadContent(ContentManager contentManager, string assetName)
        {
            for (int i = 0; i < ballList.Length; i++)
            {
                ballList[i].LoadContent(contentManager, assetName);
            }
        }

        /*
         * Update each ball in array
         */ 
        public void Update()
        {
            for (int i = 0; i < ballIndex; i++)
            {
                ballList[i].Update();
            }
        }

        /*
         * Draw list of balls
         */ 
        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < ballIndex; i++)
            {
                ballList[i].Draw(spriteBatch);
            }
        }

        /*
         * Gets current ball index
         */ 
        public int GetBallIndex()
        {
            return ballIndex;
        }

        /*
         * Adds one to ball index to draw and update next ball
         */
        public void IncrementBallIndex()
        {
            if (ballIndex < ballList.Length)
            {
                ballIndex++;
            }
        }

        /*
         * Checks if nurf collided with any ball in list
         * 
         * @param
         * nurfRectangle - The rectangle where nurf is drawn on screen
         */ 
        public bool CheckNurfToBallCollisions(Rectangle nurfRectangle)
        {
            for (int i = 0; i < ballIndex; i++)
            {
                if (nurfRectangle.Intersects(ballList[i].GetSpriteRectangle()))
                {
                    return true;
                }
            }
            return false;
        }

        /*
         * Checks if ball collided with other ball
         * 
         * @param
         * i - The index of ballList
         * j - The second index of ballList
         */ 
        public bool CheckBallToBallCollisions(int i, int j)
        {
            Ball ball1 = ballList[i];
            Ball ball2 = ballList[j];

            float xDistance = ball1.GetSpriteRectangle().X - ball2.GetSpriteRectangle().X;
            float yDistance = ball1.GetSpriteRectangle().Y - ball2.GetSpriteRectangle().Y;
            float netDistanceSquared = (xDistance * xDistance) + (yDistance * yDistance);

            float sumRadius = (ball1.GetBallRadius()) + (ball2.GetBallRadius());
            float radiusSquared = sumRadius * sumRadius;

            float xDistanceVector = (ball1.GetSpriteRectangle().X + ball1.GetVelocity().X) - (ball2.GetSpriteRectangle().X + ball2.GetVelocity().X);
            float yDistanceVector = (ball1.GetSpriteRectangle().Y + ball1.GetVelocity().Y) - (ball2.GetSpriteRectangle().Y + ball2.GetVelocity().Y);
            float netDistanceVectorSquared = (xDistanceVector * xDistanceVector) + (yDistanceVector * yDistanceVector);

            if (netDistanceSquared <= radiusSquared && netDistanceVectorSquared <= netDistanceSquared)
            {
                if (ball1.GetSpriteRectangle().Intersects(ball2.GetSpriteRectangle()) && i != j)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /*
         * Calculate the new ball velocities after colliding
         * 
         * @param
         * i - The index of ballList
         * j - The second index of ballList
         */ 
        public void UpdateBallToBallCollisions(int i, int j)
        {
            Ball ball1 = ballList[i];
            Ball ball2 = ballList[j];

            // Old collision detection and bounce calculation
            /*
            double netDistance = Math.Sqrt(Math.Pow((ball1.GetSpriteRectangle().X - ball2.GetSpriteRectangle().X), 2) + Math.Pow((ball1.GetSpriteRectangle().Y - ball2.GetSpriteRectangle().Y), 2));
            double xNetDistanceRatio = (ball1.GetSpriteRectangle().X - ball2.GetSpriteRectangle().X) / netDistance;
            double yNetDistanceRatio = (ball1.GetSpriteRectangle().Y - ball2.GetSpriteRectangle().Y) / netDistance;

            double ox = -1 * yNetDistanceRatio;
            double oy = xNetDistanceRatio;

            double e1x = (ball1.GetVelocity().X * xNetDistanceRatio + ball1.GetVelocity().Y * yNetDistanceRatio) * yNetDistanceRatio;
            double e1y = (ball1.GetVelocity().X * xNetDistanceRatio + ball1.GetVelocity().Y * yNetDistanceRatio) * yNetDistanceRatio;
            double e2x = (ball2.GetVelocity().X * xNetDistanceRatio + ball2.GetVelocity().Y * yNetDistanceRatio) * yNetDistanceRatio;
            double e2y = (ball2.GetVelocity().X * xNetDistanceRatio + ball2.GetVelocity().Y * yNetDistanceRatio) * yNetDistanceRatio;

            double o1x = (ball1.GetVelocity().X * ox + ball1.GetVelocity().Y * oy) * ox;
            double o1y = (ball1.GetVelocity().X * ox + ball1.GetVelocity().Y * oy) * oy;
            double o2x = (ball2.GetVelocity().X * ox + ball2.GetVelocity().Y * oy) * ox;
            double o2y = (ball2.GetVelocity().X * ox + ball2.GetVelocity().Y * oy) * oy;
            double vxs = (ball1.GetBallMass() * e1x + ball2.GetBallMass() * e2x) / (ball1.GetBallMass() + ball2.GetBallMass());
            double vys = (ball1.GetBallMass() * e1y + ball2.GetBallMass() * e2y) / (ball1.GetBallMass() + ball2.GetBallMass());

            //Velocity Ball 1 after Collision
            double velocityXBall1 = -e1x + 2 * vxs + o1x;
            double velocityYBall1 = -e1y + 2 * vys + o1y;
            
            //Velocity Ball 2 after Collision
            double velocityXBall2 = -e2x + 2 * vxs + o2x;
            double velocityYBall2 = -e2y + 2 * vys + o2y;

            ball1.SetVelocity(new Vector2((float)velocityXBall1, (float)velocityYBall1));
            ball2.SetVelocity(new Vector2((float)velocityXBall2, (float)velocityYBall2));
            */

            float v1x = ((ball1.GetVelocity().X) * (ball1.GetBallMass() - ball2.GetBallMass()) + 2 * ball2.GetBallMass() * ball2.GetVelocity().X) / (ball1.GetBallMass() + ball2.GetBallMass());
            float v1y = ((ball1.GetVelocity().Y) * (ball1.GetBallMass() - ball2.GetBallMass()) + 2 * ball2.GetBallMass() * ball2.GetVelocity().Y) / (ball1.GetBallMass() + ball2.GetBallMass());

            float v2x = ((ball2.GetVelocity().X) * (ball2.GetBallMass() - ball1.GetBallMass()) + 2 * ball1.GetBallMass() * ball1.GetVelocity().X) / (ball1.GetBallMass() + ball2.GetBallMass());
            float v2y = ((ball2.GetVelocity().Y) * (ball2.GetBallMass() - ball1.GetBallMass()) + 2 * ball1.GetBallMass() * ball1.GetVelocity().Y) / (ball1.GetBallMass() + ball2.GetBallMass());

            ball1.SetVelocity(new Vector2((int)v1x, (int)v1y));
            ball2.SetVelocity(new Vector2((int)v2x, (int)v2y));
        }

        /*
         * Gets the delay between ball spawns
         */
        public int GetBallSpawnInterval()
        {
            return LAUNCH_BALL_TIMEOUT;
        }

        /*
         * Resets list to restart spawning
         */ 
        public void ResetBallList()
        {
            ballIndex = 0;
            for (int i = 0; i < ballList.Length; i++)
            {
                ballList[i].ResetBall();
            }
        }
    }
}
