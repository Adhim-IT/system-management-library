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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace utspbt
{
    public partial class manage_categorycs : Form
    {

        //membuat global data  string sql
        string conn = "server=127.0.0.1;database=db_ptspbt;uid=root;pwd=";

        MySqlConnection myConnection;
        public manage_categorycs()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Baca kategori dari TextBox
                string category = textBox1.Text;

                // Validasi: Pastikan kategori tidak kosong
                if (string.IsNullOrWhiteSpace(category))
                {
                    MessageBox.Show("Kategori tidak boleh kosong.");
                    return;
                }

                // Buat koneksi
                using (MySqlConnection connection = new MySqlConnection(conn))
                {
                    connection.Open();

                    // Cek apakah kategori sudah ada sebelumnya
                    string checkQuery = "SELECT COUNT(*) FROM category WHERE category_name = @category";
                    using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, connection))
                    {
                        checkCmd.Parameters.AddWithValue("@category", category);
                        int categoryCount = Convert.ToInt32(checkCmd.ExecuteScalar());

                        if (categoryCount > 0)
                        {
                            MessageBox.Show("Kategori sudah ada.");
                            return;
                        }
                    }

                    // Buat pernyataan SQL INSERT
                    string insertQuery = "INSERT INTO category (category_name) VALUES (@category)";

                    // Buat objek perintah SQL
                    using (MySqlCommand cmd = new MySqlCommand(insertQuery, connection))
                    {
                        // Tambahkan parameter ke pernyataan SQL
                        cmd.Parameters.AddWithValue("@category", category);

                        // Eksekusi perintah SQL
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Kategori berhasil ditambahkan ke database.");
                }
                RefreshData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            admin_page admin_Page = new admin_page();
            admin_Page.Show();
            this.Hide();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void manage_categorycs_Load(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void RefreshData()
        {
            // Membuka koneksi ke database
            myConnection = new MySqlConnection(conn);
            myConnection.Open();


            string query = "SELECT category.id, category.category_name AS category FROM category";


            // Membuat objek MySqlCommand
            MySqlCommand cmd = new MySqlCommand(query, myConnection);

            try
            {
                // Membuat objek DataTable untuk menyimpan hasil query
                DataTable dt = new DataTable();

                // Membuat objek MySqlDataAdapter
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

                // Mengisi DataTable dengan hasil query
                adapter.Fill(dt);

                // Menampilkan data ke dalam DataGridView
                dataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                // Menangani exception jika terjadi kesalahan
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                // Menutup koneksi ke database
                myConnection.Close();
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Memastikan bahwa ada baris yang dipilih
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Mendapatkan baris yang dipilih
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                // Mendapatkan nilai ID dari baris yang dipilih
                int selectedCategoryId = Convert.ToInt32(selectedRow.Cells["id"].Value);

                // Membuka koneksi ke database
                myConnection = new MySqlConnection(conn);
                myConnection.Open();

                try
                {
                    // Query SQL untuk menghapus data kategori
                    string deleteCategoryQuery = "DELETE FROM category WHERE id = @id";

                    // Membuat objek MySqlCommand
                    MySqlCommand deleteCmd = new MySqlCommand(deleteCategoryQuery, myConnection);

                    // Menambahkan parameter untuk parameterized query
                    deleteCmd.Parameters.AddWithValue("@id", selectedCategoryId);

                    // Melakukan eksekusi query untuk menghapus data kategori
                    int rowsAffected = deleteCmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        // Jika berhasil, tampilkan pesan sukses
                        MessageBox.Show("Data kategori berhasil dihapus.");
                    }
                    else
                    {
                        // Jika tidak ada baris yang terpengaruh, tampilkan pesan gagal
                        MessageBox.Show("Gagal menghapus data kategori.");
                    }
                }
                catch (Exception ex)
                {
                    // Menangani exception jika terjadi kesalahan
                    MessageBox.Show("Error: " + ex.Message);
                }
                finally
                {
                    // Menutup koneksi ke database
                    myConnection.Close();
                }

                // Refresh data setelah menghapus
                RefreshData();
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Memastikan bahwa baris yang di-klik bukanlah header
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                // Mengisi nilai ke TextBox dan ComboBox sesuai kolom yang dipilih
                textBox2.Text = row.Cells["category"].Value.ToString();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Mendapatkan nilai baru dari form edit
            string newCategoryName = textBox2.Text;
            // Jika ada kolom lain, dapatkan nilai sesuai kebutuhan

            // Mendapatkan baris yang dipilih
            DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

            // Mendapatkan nilai ID dari baris yang dipilih
            int selectedCategoryId = Convert.ToInt32(selectedRow.Cells["id"].Value);

            // Membuka koneksi ke database
            using (myConnection = new MySqlConnection(conn))
            {
                try
                {
                    myConnection.Open();

                    // Query SQL untuk mengupdate data kategori
                    string updateCategoryQuery = "UPDATE category SET category_name = @newCategoryName WHERE id = @id";

                    // Membuat objek MySqlCommand
                    using (MySqlCommand updateCmd = new MySqlCommand(updateCategoryQuery, myConnection))
                    {
                        // Menambahkan parameter untuk parameterized query
                        updateCmd.Parameters.AddWithValue("@newCategoryName", newCategoryName);
                        updateCmd.Parameters.AddWithValue("@id", selectedCategoryId);

                        // Melakukan eksekusi query untuk mengupdate data kategori
                        int rowsAffected = updateCmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            // Jika berhasil, tampilkan pesan sukses
                            MessageBox.Show("Data kategori berhasil diupdate.");

                            // Refresh data setelah mengupdate
                            RefreshData();
                        }
                        else
                        {
                            // Jika tidak ada baris yang terpengaruh, tampilkan pesan gagal
                            MessageBox.Show("Gagal mengupdate data kategori.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Menangani exception jika terjadi kesalahan
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }



    }
}
