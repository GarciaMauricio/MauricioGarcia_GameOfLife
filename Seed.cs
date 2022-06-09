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
    public partial class Seed : Form
    {
        public Seed()
        {
            InitializeComponent();
        }

        //Get the number and set the number inside the NumericUpDown Box
        public int GetSeed()
        {
            return (int)numericUpDown1.Value;
        }

        public void SetSeed(int Vseed)
        {
            //numericUpDown1.Value = num/* num = seed number */;
        }
    }
}
