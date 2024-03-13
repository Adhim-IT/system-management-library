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
    public partial class admin_page : Form
    {
        public admin_page()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            search form2 = new search();
            form2.Show();

            // Tutup halaman Form1 (opsional, tergantung pada kebutuhan Anda)
            this.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            data_peminjam form2 = new data_peminjam();
            form2.Show();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            manage_book manage_Book = new manage_book();
            manage_Book.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            manage_categorycs manage_Categorycs = new manage_categorycs();
            manage_Categorycs.Show();
            this.Hide();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            login login = new login();
            login.Show();
            this.Hide();
        }
    }
}
