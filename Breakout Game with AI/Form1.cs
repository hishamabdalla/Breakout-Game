using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace BreakingOut
{
    public partial class Form1 : Form
    {
        PictureBox[] blockArray; // Array to store blocks
        bool goLeft;
        bool goRight;
        bool isGameOver; 
        int score; 
        int ballx; 
        int bally;
        int playerSpeed; 
        Random rnd = new Random();

        // Initializes the form and sets up the game by placing blocks and calling the setupGame method
        public Form1()
        {
            InitializeComponent();
            PlaceBlocks();
            setupGame(); 
        }
        // Sets up the initial game parameters and starts the game
        private void setupGame()
        {
            isGameOver = false; 
            score = 0; 
            ballx = 5; 
            bally = 5; 
            playerSpeed = 12; 
            UpdateScoreText();
            ResetBallAndPlayerPositions(); 
            gameTimer.Start(); 
            ChangeBlockColors(); 
        }

        // Changes the colors of blocks randomly
        private void ChangeBlockColors()
        {
            
            foreach (Control x in this.Controls)
            {
                if (x is PictureBox && (string)x.Tag == "blocks")
                {

                    x.BackColor = Color.FromArgb(rnd.Next(100, 200), rnd.Next(50, 150), rnd.Next(256));
                }
            }
        }

        // Updates the score display on the form
        private void UpdateScoreText()
        {
         
            txtScore.Text = "Score: " + score;
        }

        // Reset ball and player paddle positions
        private void ResetBallAndPlayerPositions()
        {
           
            ball.Left = 403;
            ball.Top = 380;
            player.Left = 362;
            player.Top = 429;
        }

        // Handles the game over event by stopping the game timer and displaying a message box
        private void GameOver(bool isWin)
        {
            
            isGameOver = true;
            gameTimer.Stop();

            
            string message = isWin ? "You Win !!" : "Game Over !!";

            DialogResult result = MessageBox.Show(message + " Do you want to play again?", isWin ? "You Win" : "Game Over", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                removeBlocks();
                PlaceBlocks();
                setupGame();
            }
            else
            {
                this.Close();
            }
        }

        // Places blocks on the game screen
        private void PlaceBlocks()
        {

            // Place blocks on the game screen
            blockArray = new PictureBox[49];
            int top = 50;
            int left = 50;

            for (int i = 0; i < blockArray.Length; i++)
            {
                blockArray[i] = new PictureBox();
                blockArray[i].Height = 15;
                blockArray[i].Width = 100;
                blockArray[i].Tag = "blocks";
                blockArray[i].BackColor = Color.White;

                if (i % 7 == 0 && i != 0)
                {
                    top += 25;
                    left = 50;
                }
                blockArray[i].Left = left;
                blockArray[i].Top = top;
                this.Controls.Add(blockArray[i]);
                left += 107;
            }
        }

        private void removeBlocks()
        {
            // Remove blocks from the game screen
            foreach (var block in blockArray)
            {
                this.Controls.Remove(block);
            }
        }

        // Main game loop that handles player movement, ball movement, collisions, and game over conditions
        private void mainGameTimer(object sender, EventArgs e)
        {
         
            txtScore.Text = "Score: " + score;

            if (goLeft == true && player.Left > 0)
            {
                player.Left -= playerSpeed;
            }

            if (goRight == true && player.Left < 733)
            {
                player.Left += playerSpeed; 
            }

            ball.Left += ballx;
            ball.Top += bally;
            CheckBallCollisions(); 

            // Check for collision with blocks
            foreach (Control x in this.Controls)
            {
                if (x is PictureBox && (string)x.Tag == "blocks")
                {
                    if (ball.Bounds.IntersectsWith(x.Bounds))
                    {
                        score += 1; // Increase score
                        bally = -bally; // Reverse ball's vertical direction
                        this.Controls.Remove(x); // Remove block from screen
                    }
                }
            }

            // Check game over conditions
            if (score == 49)
            {
                GameOver(true); 
            }
            // Check if the player loses the game
            else if (ball.Top > 500)
            {
                GameOver(false); 
            }
            // Call AI control method
            if (!isGameOver)
            {
                AIControl();
            }
        }

        private void CheckBallCollisions()
        {
            // Check ball collisions with walls and player paddle
            if (ball.Left < 0 || ball.Left > 815)
                ballx = -ballx;

            if (ball.Top < 0)
                bally = -bally;

            if (ball.Bounds.IntersectsWith(player.Bounds))
            {
                bally = -rnd.Next(5, 8); // Reverse ball's vertical direction
                ballx = rnd.Next(5, 8) * (ballx < 0 ? -1 : 1); // Randomize ball's horizontal direction
            }
        }

        private void keyisdown(object sender, KeyEventArgs e)
        {
            // Handle key down events for player paddle movement
            if (e.KeyCode == Keys.Left)
            {
                goLeft = true;
            }
            if (e.KeyCode == Keys.Right)
            {
                goRight = true;
            }
        }

        private void keyisup(object sender, KeyEventArgs e)
        {
            // Handle key up events for player paddle movement
            if (e.KeyCode == Keys.Left)
            {
                goLeft = false;
            }
            if (e.KeyCode == Keys.Right)
            {
                goRight = false;
            }

            // Handle Enter key press to restart the game
            if (e.KeyCode == Keys.Enter && isGameOver == true)
            {
                removeBlocks();
                PlaceBlocks();
                setupGame();
            }
        }
        #region uninformed search
        private void AIControl()
        {
            Queue<int> queue = new Queue<int>();
            HashSet<int> visited = new HashSet<int>();
            Dictionary<int, int> parent = new Dictionary<int, int>();

            int paddlePosition = player.Left + (player.Width / 2);
            int ballPosition = ball.Left + (ball.Width / 2);

            queue.Enqueue(paddlePosition);
            visited.Add(paddlePosition);

            while (queue.Count > 0)
            {
                int current = queue.Dequeue();

                // If the current position matches the ball position, backtrack to find the optimal path
                if (current == ballPosition)
                {
                    while (current != paddlePosition)
                    {
                        ballPosition = parent[current];
                        current = ballPosition;
                    }
                    break;
                }

                // Explore neighboring positions
                if (current - playerSpeed >= 0 && !visited.Contains(current - playerSpeed))
                {
                    queue.Enqueue(current - playerSpeed);
                    visited.Add(current - playerSpeed);
                    parent[current - playerSpeed] = current;
                }
                if (current + playerSpeed <= this.Width && !visited.Contains(current + playerSpeed))
                {
                    queue.Enqueue(current + playerSpeed);
                    visited.Add(current + playerSpeed);
                    parent[current + playerSpeed] = current;
                }
            }

            // Move the paddle towards the ball
            if (ballPosition < paddlePosition)
            {
                goLeft = true;
                goRight = false;
            }
            else if (ballPosition > paddlePosition)
            {
                goLeft = false;
                goRight = true;
            }
            else
            {
                goLeft = false;
                goRight = false;
            }
        }

        #endregion

        #region First one
        //private void AIControl()
        //{
        //    // AI control logic to move the player paddle based on the position of the ball
        //    int playerCenter = player.Left + (player.Width / 2);
        //    int ballCenter = ball.Left + (ball.Width / 2);

        //    if (ballCenter < playerCenter)
        //    {
        //        goLeft = true; // Move paddle left
        //        goRight = false;
        //    }
        //    else if (ballCenter > playerCenter)
        //    {
        //        goLeft = false;
        //        goRight = true; // Move paddle right
        //    }
        //    else
        //    {
        //        goLeft = false; // Stop moving
        //        goRight = false;
        //    }
        //}
        #endregion


        private void Form1_Load(object sender, EventArgs e)
        {
            // Event handler for form load
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

    }
}
