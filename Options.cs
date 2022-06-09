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
    public partial class Options : Form
    {
        public Options()
        {
            InitializeComponent();
        }

        public int GetMilliSec()
        {
            return (int)numericUpDown1.Value;
        }
        public void SetMilliSec(int time)
        {
            numericUpDown1.Value = time;
        }

        public int GetWidth()
        {
            return (int)numericUpDown2.Value;
        }
        public void SetWidth(int w)
        {
            numericUpDown2.Value = w;
        }

        public int GetHeight()
        {
            return (int)numericUpDown3.Value;
        }
        public void SetHeight(int h)
        {
            numericUpDown3.Value = h;
        }
    }
}
