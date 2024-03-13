using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.Types;
using System.Data.SqlClient;

namespace utspbt
{
    public partial class data_peminjam : Form
    {

        string conn = "server=127.0.0.1;database=db_ptspbt;uid=root;pwd=";

        MySqlConnection myConnection;
        public data_peminjam()
        {
            InitializeComponent();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void LoadDataBukuPeminjaman()
        {
            try
            {
                using (MySqlConnection myConnection = new MySqlConnection(conn))
                {
                    myConnection.Open();

                    // Query untuk mengambil informasi buku dan peminjaman
                    string query = "SELECT peminjam.nama_peminjam AS 'Name', " +
                     "buku.buku_nama AS 'title', " +
                     "buku.isbn, " +
                     "buku.category_id, " +
                     "category.category_name AS 'category', " +
                     "peminjam.tanggal_pinjam AS 'loan date', " +
                     "peminjam.tanggal_kembali AS 'date of return' " +
                     "FROM buku " +
                     "JOIN peminjam ON buku.id = peminjam.buku_id " +
                     "JOIN category ON buku.category_id = category.id;";


                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(query, myConnection))
                    {
                        // DataSet untuk menyimpan hasil query
                        DataSet dataSet = new DataSet();

                        // Isi DataSet dengan hasil query
                        adapter.Fill(dataSet, "BukuPeminjaman");

                        // Set DataGridView dengan data dari DataSet
                        dataGridView1.DataSource = dataSet.Tables["BukuPeminjaman"];

                        // Sembunyikan kolom category_id
                        dataGridView1.Columns["category_id"].Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }


        private void data_peminjam_Load(object sender, EventArgs e)
        {
            LoadDataBukuPeminjaman();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            admin_page form2 = new admin_page();
            form2.Show();

            this.Hide();
        }
    }
}
