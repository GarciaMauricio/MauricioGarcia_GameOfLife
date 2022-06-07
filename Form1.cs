using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MauricioGarcia_GameOfLife
{
    public partial class Form1 : Form
    {
        // The universe array
        bool[,] universe = new bool[30, 30];

        //Created ScratchPad here...
        bool[,] ScratchPad = new bool[30, 30];

        // Drawing colors
        Color LColor = Color.Black;
        Color gridColor = Color.Black;
        Color cellColor = Color.Black;

        // The Timer class
        Timer timer = new Timer();

        // Generation count
        int generations = 0;

        public Form1()
        {
            InitializeComponent();

            // Setup the timer
            timer.Interval = 100; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = false; // start timer not running
        }

        // Calculate the next generation of cells
        private void NextGeneration()
        {
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    //get neighbor count for each living cell, option
                    int count = CountNeighborsFinite(x, y); // default
                    if (finiteToolStripMenuItem.Checked == true)
                    {
                        count = CountNeighborsFinite(x, y);
                    }
                    else if (torisToolStripMenuItem.Checked == true)
                    {
                        count = CountNeighborsToroidal(x, y);
                    }
                    //Apply the rules here
                    if (universe[x, y] == true)
                    {
                        if (count < 2)
                        {
                            ScratchPad[x, y] = false;
                        }
                        else if (count > 3)
                        {
                            ScratchPad[x, y] = false;
                        }
                        else if (count == 2 || count == 3)
                        {
                            ScratchPad[x, y] = true;
                        }
                    }
                    else
                    {
                        if (count == 3)
                        {
                            ScratchPad[x, y] = true;
                        }
                    }
                }
            }

            //Copy what is in the ScratchPad to the universe
            //( Swap them )
            bool[,] hold = universe;
            universe = ScratchPad;
            ScratchPad = hold;
            //2nd time the NextGeneration executes just clear out anything in the ScratchPad that shouldn't be turned on
            bool[,] Empty = new bool[30, 30];
            ScratchPad = Empty;

            // Increment generation count
            generations++;

            // Update status strip generations
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();

            //Invalidate graphicsPanel1
            graphicsPanel1.Invalidate();
        }

        // The event called by the timer every Interval milliseconds.
        private void Timer_Tick(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {
            // Calculate the width and height of each cell in pixels
        //FLOATS MAKE PROGRAM MUCH BETTER( REPLACE INTS ) 
        //CONVERT TO FLOATS!!!
            //( Calculating the width and the height of each window )
            // CELL WIDTH = WINDOW WIDTH / NUMBER OF CELLS IN X
            float cellWidth = graphicsPanel1.ClientSize.Width / (float)universe.GetLength(0);
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            float cellHeight = graphicsPanel1.ClientSize.Height / (float)universe.GetLength(1);

            // A Pen for drawing the grid lines (color, width)
            Pen gridPen = new Pen(gridColor, 1);


            Pen boldPen = new Pen(LColor, 3);

            // A Brush for filling living cells interiors (color)
            Brush cellBrush = new SolidBrush(cellColor);

            // Iterate through the universe in the y, top to bottom
                //started with y top then x on bottom, not necessary, this just moves through it the way westerns read
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // A rectangle to represent each cell in pixels

                    //Use RectangleF!!! after replacing cellWidth and cellHeight with floats 
                    RectangleF cellRect = RectangleF.Empty;
                    cellRect.X = x * cellWidth;
                    cellRect.Y = y * cellHeight;
                    cellRect.Width = cellWidth;
                    cellRect.Height = cellHeight;
                    

                    // Fill the cell with a brush if alive
                    if (universe[x, y] == true)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                    }

                    // Outline the cell with a pen
                    e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
                    
                    if (x % 10 == 0)
                    {                        
                        int _y = universe.GetLength(1);
                        Point point1 = new Point( (int)(x * cellWidth), 0 );
                        Point point2 = new Point( (int)(x * cellWidth), (int)( (_y + 1) * (cellHeight) ) );
                        e.Graphics.DrawLine(boldPen, point1, point2);
                    }
                    if (y % 10 == 0)
                    {
                        int _x = universe.GetLength(0);
                        Point point1 = new Point(0,(int)(y * cellHeight));
                        Point point2 = new Point( (int)( (_x + 1) * (cellWidth) ), (int)(y * cellHeight) );
                        e.Graphics.DrawLine(boldPen, point1, point2);
                    }
                }
            }

            // Cleaning up pens and brushes
            gridPen.Dispose();
            cellBrush.Dispose();
        }

        private void graphicsPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            // If the left mouse button was clicked
            if (e.Button == MouseButtons.Left)
            {
            //Convert to FLOATS!!! AS WELL!!!
                // Calculate the width and height of each cell in pixels
                float cellWidth = graphicsPanel1.ClientSize.Width / (float)universe.GetLength(0);
                float cellHeight = graphicsPanel1.ClientSize.Height / (float)universe.GetLength(1);

                // Calculate the cell that was clicked in
                // CELL X = MOUSE X / CELL WIDTH
                int x = (int)(e.X / cellWidth);
                // CELL Y = MOUSE Y / CELL HEIGHT
                int y = (int)(e.Y / cellHeight);

                // Toggle the cell's state
                universe[x, y] = !universe[x, y];

                // Tell Windows you need to repaint
                graphicsPanel1.Invalidate();
                //CALL INVALIDATE WHEN UPDATING, CLICKING, TURNING ON OR OFF THINGS
                //NEVER PLACE INVALIDATE INSIDE YOUR PAINT
            }
        }


        //Start timer button( starts the timer to true, Line 35 )
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            timer.Enabled = true;
        }


        //Pause timer button( disables the time to false, Line 35 )
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
        }


        //Next timer button( calls nextgeneration once, Line 39 )
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            NextGeneration();
        }


        //File New button( repeats nested for loops, Line 93 ) ( Inside both nested loops set universe[x,y] = false; inside function Invalidate GraphicsPanel!!!)
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = false;
                }
            }
            generations = 0;
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            graphicsPanel1.Invalidate();
        }


        //Exit Button
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        //Finite View
        private void finiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (finiteToolStripMenuItem.Checked == true)
            {
                torisToolStripMenuItem.Checked = false;
            }
            else if (finiteToolStripMenuItem.Checked == false)
            {
                torisToolStripMenuItem.Checked = true;
            }

        }
        private int CountNeighborsFinite(int x, int y)
        {
            int count = 0;
            int xLen = universe.GetLength(0);
            int yLen = universe.GetLength(1);
            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                for (int xOffset = -1; xOffset <= 1; xOffset++)
                {
                    int xCheck = x + xOffset;
                    int yCheck = y + yOffset;

                    // if xOffset and yOffset are both equal to 0 then continue
                    if (xOffset == 0 && yOffset == 0)
                    {
                        continue;
                    }
                    // if xCheck is less than 0 then continue
                    if (xCheck < 0)
                    {
                        continue;
                    }
                    // if yCheck is less than 0 then continue
                    if (yCheck < 0)
                    {
                        continue;
                    }
                    // if xCheck is greater than or equal too xLen then continue
                    if (xCheck >= xLen)
                    {
                        continue;
                    }
                    // if yCheck is greater than or equal too yLen then continue
                    if (yCheck >= yLen)
                    {
                        continue;
                    }

                    if (universe[xCheck, yCheck] == true) count++;
                }
            }
            return count;
        }


        //Toroidal View
        private void torisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (torisToolStripMenuItem.Checked == true)
            {
                finiteToolStripMenuItem.Checked = false;
            }
            else if (torisToolStripMenuItem.Checked == false)
            {
                finiteToolStripMenuItem.Checked = true;
            }
        }
        private int CountNeighborsToroidal(int x, int y)
        {
            int count = 0;
            int xLen = universe.GetLength(0);
            int yLen = universe.GetLength(1);
            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                for (int xOffset = -1; xOffset <= 1; xOffset++)
                {
                    int xCheck = x + xOffset;
                    int yCheck = y + yOffset;
                    // if xOffset and yOffset are both equal to 0 then continue
                    if (xOffset == 0 && yOffset == 0)
                    {
                        continue;
                    }
                    // if xCheck is less than 0 then set to xLen - 1
                    if (xCheck < 0)
                    {
                        xCheck = xLen -1;
                    }
                    // if yCheck is less than 0 then set to yLen - 1
                    if (yCheck < 0)
                    {
                        yCheck = yLen -1;
                    }
                    // if xCheck is greater than or equal too xLen then set to 0
                    if (xCheck >= xLen)
                    {
                        xCheck = 0;
                    }
                    // if yCheck is greater than or equal too yLen then set to 0
                    if (yCheck >= yLen)
                    {
                        yCheck = 0;
                    }

                    if (universe[xCheck, yCheck] == true) count++;
                }
            }
            return count;
        }

        //New Click
        private void New_Click(object sender, EventArgs e)
        {
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = false;
                }
            }
            generations = 0;
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            graphicsPanel1.Invalidate();
        }
    }
}
