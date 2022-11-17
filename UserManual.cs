using PCEnvanter.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PCEnvanter
{
    public partial class UserManual : Form
    {
        public UserManual()
        {
            InitializeComponent();
            richTextBox1.Text = Resources.kk;
        }
    }
}
