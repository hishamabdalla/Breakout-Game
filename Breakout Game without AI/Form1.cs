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
        bool isGameOver; // Indicates whether the game is over
        int score; // Current score
        int ballx; // Ball's horizontal velocity
        int bally; // Ball's vertical velocity
        int playerSpeed; // Speed of the player paddle
        Random rnd = new Random(); // Random number generator

        public Form1()
        {
            InitializeComponent();
            PlaceBlocks(); // Initialize and place blocks
            setupGame(); // Setup game parameters
        }

        private void setupGame()
        {
            isGameOver = false; // Reset game over flag
            score = 0; // Reset score
            ballx = 50; // Reset ball horizontal velocity
            bally = 50; // Reset ball vertical velocity
            playerSpeed = 22; // Reset player paddle speed
            UpdateScoreText(); // Update score display
            ResetBallAndPlayerPositions(); // Reset ball and player paddle positions
            gameTimer.Start(); // Start game timer
            ChangeBlockColors(); // Change colors of blocks
        }

        private void ChangeBlockColors()
        {
            // Change colors of blocks randomly
            foreach (Control x in this.Controls)
            {
                if (x is PictureBox && (string)x.Tag == "blocks")
                {

                    x.BackColor = Color.FromArgb(rnd.Next(100, 200), rnd.Next(50, 150), rnd.Next(256));
                }
            }
        }

        private void UpdateScoreText()
        {
            // Update score display
            txtScore.Text = "Score: " + score;
        }

        private void ResetBallAndPlayerPositions()
        {
            // Reset ball and player paddle positions
            ball.Left = 403;
            ball.Top = 380;
            player.Left = 362;
            player.Top = 429;
        }

        private void GameOver(bool isWin)
        {
            // Handle game over event
            isGameOver = true;
            gameTimer.Stop();

            // Determine message based on win or lose condition
            string message = isWin ? "You Win !!" : "Game Over !!";

            // Display message box with appropriate title and buttons
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

        private void mainGameTimer(object sender, EventArgs e)
        {
            // Main game loop
            txtScore.Text = "Score: " + score;

            // Move the player paddle based on the input flags (goLeft and goRight)
            if (goLeft == true && player.Left > 0)
            {
                player.Left -= playerSpeed;// Move the player paddle to the left
            }

            if (goRight == true && player.Left < 733)
            {
                player.Left += playerSpeed; // Move the player paddle to the right
            }

            // Move ball
            ball.Left += ballx;
            ball.Top += bally;
            CheckBallCollisions(); // Check ball collisions

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
                GameOver(true); // Player wins
            }
            // Check if the player loses the game
            else if (ball.Top > 500)
            {
                GameOver(false); // Player loses
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
