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
    public partial class peminjaman : Form
    {

        //membuat global data  string sql
        string conn = "server=127.0.0.1;database=db_ptspbt;uid=root;pwd=";

        MySqlConnection myConnection;
        public peminjaman()
        {
            InitializeComponent();
            comboBox2.SelectedIndexChanged += comboBox2_SelectedIndexChanged;

            // Panggil metode untuk mengisi ComboBox dengan data kategori
            FillCategoryComboBox();


        }

        private void peminjaman_Load(object sender, EventArgs e)
        {
            LoadDataPeminjaman();
        }

        private void LoadDataPeminjaman()
        {
            // Mengisi DataGridView dengan data peminjaman
            string query = "SELECT buku.id, buku.buku_nama AS title, buku.isbn, category.category_name AS category, buku.quantity " +
                               "FROM buku " +
                               "JOIN category ON buku.category_id = category.id ";

            myConnection = new MySqlConnection(conn);

            try
            {
                myConnection.Open();
                MySqlCommand cmd = new MySqlCommand(query, myConnection);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                // Set data source DataGridView dengan data peminjaman
                dataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                myConnection.Close();
            }
        }






        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void search_Load(object sender, EventArgs e)
        {
            // Membuka koneksi ke database
            myConnection = new MySqlConnection(conn);
            myConnection.Open();

            // Query SQL untuk mengambil semua data dengan JOIN ke tabel category
            string query = "SELECT buku.id, buku.buku_nama AS title, buku.isbn, category.category_name AS category, buku.quantity FROM buku " +
                           "JOIN category ON buku.category_id = category.id";

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

            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            string selectedBook = textBox2.Text;
            string peminjam = textBox5.Text;

            if (!string.IsNullOrEmpty(selectedBook) && !string.IsNullOrEmpty(peminjam))
            {
                if (ProcessPeminjaman(selectedBook, peminjam))
                {
                    MessageBox.Show("Peminjaman berhasil.");
                }
                else
                {
                    MessageBox.Show("Gagal melakukan peminjaman. Silakan coba lagi.");
                }
            }
            else
            {
                MessageBox.Show("Pilih buku dan isi nama peminjam terlebih dahulu.");
            }
        }

        private bool ProcessPeminjaman(string selectedBook, string peminjam)
        {
            using (MySqlConnection myConnection = new MySqlConnection(conn))
            {
                myConnection.Open();

                // Mulai transaksi
                MySqlTransaction transaction = myConnection.BeginTransaction();

                try
                {
                    // Eksekusi query UPDATE untuk mengurangkan quantity di tabel buku
                    using (MySqlCommand cmdUpdate = new MySqlCommand("UPDATE buku SET quantity = quantity - 1 WHERE buku_nama = @Book", myConnection, transaction))
                    {
                        cmdUpdate.Parameters.AddWithValue("@Book", selectedBook);
                        cmdUpdate.ExecuteNonQuery();
                    }

                    // Eksekusi query INSERT untuk menambahkan data peminjam ke tabel peminjam
                    using (MySqlCommand cmdInsert = new MySqlCommand("INSERT INTO peminjam (nama_peminjam, buku_id, tanggal_pinjam) VALUES (@Peminjam, (SELECT id FROM buku WHERE buku_nama = @Book), @TanggalPinjam)", myConnection, transaction))
                    {
                        cmdInsert.Parameters.AddWithValue("@Peminjam", peminjam);
                        cmdInsert.Parameters.AddWithValue("@Book", selectedBook);

                        // Set nilai untuk tanggal_pinjam (gunakan DateTime.Now untuk mendapatkan tanggal saat ini)
                        cmdInsert.Parameters.AddWithValue("@TanggalPinjam", DateTime.Now.Date);

                        cmdInsert.ExecuteNonQuery();
                    }

                    // Commit transaksi jika semua perintah berhasil
                    transaction.Commit();

                    // Mengembalikan nilai true untuk menunjukkan bahwa peminjaman berhasil
                    return true;
                }
                catch (Exception ex)
                {
                    // Rollback transaksi jika terjadi kesalahan
                    transaction.Rollback();

                    // Tampilkan pesan kesalahan di konsol atau simpan ke file log
                    Console.WriteLine("Error: " + ex.Message);

                    // Mengembalikan nilai false jika ada masalah dengan peminjaman
                    return false;
                }
            }
        }






        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Mendapatkan nilai yang dipilih dari ComboBox
            string selectedCategory = comboBox2.SelectedItem.ToString();

            // Menampilkan pesan dengan kategori yang dipilih
            MessageBox.Show("Kategori yang dipilih: " + selectedCategory);
        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            // Memastikan bahwa baris yang di-klik bukanlah header
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                // Mengisi nilai ke TextBox dan ComboBox sesuai kolom yang dipilih
                textBox2.Text = row.Cells["title"].Value.ToString();
                textBox3.Text = row.Cells["isbn"].Value.ToString();
                comboBox2.Text = row.Cells["category"].Value.ToString();
                textBox3.ReadOnly = true;
                comboBox2.Enabled = false;
            }


        }

        private void FillCategoryComboBox()
        {
            // Membuka koneksi ke database
            using (MySqlConnection connection = new MySqlConnection(conn))
            {
                connection.Open();

                // Query SQL untuk mengambil semua kategori
                string query = "SELECT category_name FROM category";

                // Membuat objek MySqlCommand
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    // Membuat objek MySqlDataReader untuk membaca hasil query
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        // Membersihkan item-item sebelum menambahkan yang baru
                        comboBox2.Items.Clear();

                        // Membaca setiap baris hasil query dan menambahkannya ke ComboBox
                        while (reader.Read())
                        {
                            string categoryName = reader["category_name"].ToString();
                            comboBox2.Items.Add(categoryName);
                        }
                    }
                }
            }
        }
        // Metode untuk mendapatkan ID kategori berdasarkan nama kategori
        private int GetCategoryIdByCategoryName(string categoryName)
        {
            // Kode untuk mendapatkan ID kategori dari database berdasarkan nama kategori
            // Sesuaikan dengan struktur tabel dan koneksi database kamu
            int categoryId = 0;

            // Contoh query, sesuaikan dengan struktur tabel kategori
            string query = "SELECT id FROM category WHERE category_name = @categoryName";

            // Membuat objek MySqlCommand
            MySqlCommand cmd = new MySqlCommand(query, myConnection);

            // Menambahkan parameter untuk parameterized query
            cmd.Parameters.AddWithValue("@categoryName", categoryName);

            try
            {
                // Membuka koneksi ke database jika belum terbuka
                if (myConnection.State != ConnectionState.Open)
                {
                    myConnection.Open();
                }

                // Mengeksekusi query untuk mendapatkan ID kategori
                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    // Konversi hasil ke tipe data yang sesuai
                    categoryId = Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                // Menangani exception jika terjadi kesalahan
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {

            }

            return categoryId;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            // Ambil teks dari TextBox pencarian (textBoxSearch)
            string searchTerm = textBox1.Text.Trim();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                // Membuka koneksi ke database
                myConnection = new MySqlConnection(conn);
                myConnection.Open();

                // Query SQL untuk mencari data berdasarkan buku_nama
                string query = "SELECT buku.id, buku.buku_nama AS title, buku.isbn, category.category_name AS category, buku.quantity " +
                               "FROM buku " +
                               "JOIN category ON buku.category_id = category.id " +
                               "WHERE buku.buku_nama LIKE @searchTerm";

                // Membuat objek MySqlCommand
                MySqlCommand cmd = new MySqlCommand(query, myConnection);

                // Menambahkan parameter untuk parameterized query
                cmd.Parameters.AddWithValue("@searchTerm", "%" + searchTerm + "%");

                try
                {
                    // Membuat objek DataTable untuk menyimpan hasil query
                    DataTable dt = new DataTable();

                    // Membuat objek MySqlDataAdapter
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

                    // Mengisi DataTable dengan hasil query
                    adapter.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        // Menampilkan data ke dalam DataGridView
                        dataGridView1.DataSource = dt;

                        // Menyembunyikan kolom-kolom lain
                        foreach (DataGridViewColumn column in dataGridView1.Columns)
                        {
                            if (column.Name != "id" &&
                                column.Name != "title" &&
                                column.Name != "isbn" &&
                                column.Name != "category" &&
                                column.Name != "quantity")
                            {
                                column.Visible = false;
                            }
                        }
                    }
                    else
                    {
                        // Tampilkan pesan jika tidak ada data yang cocok
                        MessageBox.Show("Nama buku tidak terdaftar.");
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
            }
            else
            {
                // Tampilkan pesan jika kotak pencarian kosong
                MessageBox.Show("Masukkan kata kunci pencarian.");
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            user_page form2 = new user_page();
            form2.Show();

            // Tutup halaman Form1 (opsional, tergantung pada kebutuhan Anda)
            this.Hide();
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }
    }
}
