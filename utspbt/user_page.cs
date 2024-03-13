using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace utspbt
{
    public partial class user_page : Form
    {
        public user_page()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            peminjaman form2 = new peminjaman();
            form2.Show();

            // Tutup halaman Form1 (opsional, tergantung pada kebutuhan Anda)
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            @return form2 = new @return();
            form2.Show();

            // Tutup halaman Form1 (opsional, tergantung pada kebutuhan Anda)
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            login login = new login();
            login.Show();
            this.Hide();
        }
    }
}
