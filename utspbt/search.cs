﻿using System;
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
    public partial class search : Form
    {
        //membuat global data  string sql
        string conn = "server=127.0.0.1;database=db_ptspbt;uid=root;pwd=";
        MySqlConnection myConnection;
        public search()
        {
            InitializeComponent();
            // Menambahkan event handler untuk event SelectedIndexChanged
            comboBox2.SelectedIndexChanged += comboBox2_SelectedIndexChanged;

            // Panggil metode untuk mengisi ComboBox dengan data kategori
            FillCategoryComboBox();
        }

        private void search_Load(object sender, EventArgs e)
        {
            RefreshData();
        }



        private void button1_Click(object sender, EventArgs e)
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

        private void textBox4_TextChanged(object sender, EventArgs e)
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
                    textBox5.Text = row.Cells["quantity"].Value.ToString();
                    textBox5.ReadOnly = true;
                 }
            

        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Memastikan bahwa ada baris yang dipilih
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Mendapatkan baris yang dipilih
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                // Mendapatkan nilai ID dari baris yang dipilih
                int selectedId = Convert.ToInt32(selectedRow.Cells["id"].Value);

                // Membuka koneksi ke database
                myConnection = new MySqlConnection(conn);
                myConnection.Open();

                // Query SQL untuk melakukan update data
                string query = "UPDATE buku SET buku_nama = @title, isbn = @isbn, category_id = @categoryId, quantity = @quantity " +
                               "WHERE id = @id";

                // Membuat objek MySqlCommand
                MySqlCommand cmd = new MySqlCommand(query, myConnection);

                // Menambahkan parameter untuk parameterized query
                cmd.Parameters.AddWithValue("@id", selectedId);
                cmd.Parameters.AddWithValue("@title", textBox2.Text.Trim());
                cmd.Parameters.AddWithValue("@isbn", textBox3.Text.Trim());
                cmd.Parameters.AddWithValue("@categoryId", GetCategoryIdByCategoryName(comboBox2.Text.Trim())); // Menggunakan metode untuk mendapatkan ID kategori berdasarkan nama kategori
                cmd.Parameters.AddWithValue("@quantity", textBox5.Text.Trim());

                try
                {
                    // Melakukan eksekusi query untuk update
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        // Jika berhasil, tampilkan pesan sukses
                        MessageBox.Show("Data berhasil diperbarui.");
                    }
                    else
                    {
                        // Jika tidak ada baris yang terpengaruh, tampilkan pesan gagal
                        MessageBox.Show("Gagal memperbarui data.");
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
                // Jika tidak ada baris yang dipilih, tampilkan pesan
                MessageBox.Show("Pilih baris yang ingin diedit.");
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


        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Mendapatkan nilai yang dipilih dari ComboBox
            string selectedCategory = comboBox2.SelectedItem.ToString();

            // Menampilkan pesan dengan kategori yang dipilih
            MessageBox.Show("Kategori yang dipilih: " + selectedCategory);

            // Jika perlu, tambahkan logika atau tindakan lain sesuai kebutuhan
        }

        // Contoh metode untuk mengisi ComboBox dengan data kategori
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



        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            admin_page form2 = new admin_page();
            form2.Show();

            // Tutup halaman Form1 (opsional, tergantung pada kebutuhan Anda)
            this.Hide();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Memastikan bahwa ada baris yang dipilih
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Mendapatkan baris yang dipilih
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                // Mendapatkan nilai ID dari baris yang dipilih
                int selectedId = Convert.ToInt32(selectedRow.Cells["id"].Value);

                // Membuka koneksi ke database
                myConnection = new MySqlConnection(conn);
                myConnection.Open();

                // Mengecek apakah ada peminjaman terkait dengan buku yang akan dihapus
                string checkPeminjamQuery = "SELECT COUNT(*) FROM peminjam WHERE buku_id = @id";

                MySqlCommand checkCmd = new MySqlCommand(checkPeminjamQuery, myConnection);
                checkCmd.Parameters.AddWithValue("@id", selectedId);

                int relatedPeminjamCount = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (relatedPeminjamCount > 0)
                {
                    MessageBox.Show("Tidak dapat menghapus buku karena terdapat peminjaman terkait.");
                }
                else
                {
                    // Query SQL untuk menghapus data buku
                    string deleteBukuQuery = "DELETE FROM buku WHERE id = @id";

                    // Membuat objek MySqlCommand
                    MySqlCommand deleteCmd = new MySqlCommand(deleteBukuQuery, myConnection);

                    // Menambahkan parameter untuk parameterized query
                    deleteCmd.Parameters.AddWithValue("@id", selectedId);

                    try
                    {
                        // Melakukan eksekusi query untuk menghapus data buku
                        int rowsAffected = deleteCmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            // Jika berhasil, tampilkan pesan sukses
                            MessageBox.Show("Data buku berhasil dihapus.");
                        }
                        else
                        {
                            // Jika tidak ada baris yang terpengaruh, tampilkan pesan gagal
                            MessageBox.Show("Gagal menghapus data buku.");
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
            else
            {
                // Jika tidak ada baris yang dipilih, tampilkan pesan
                MessageBox.Show("Pilih baris yang ingin dihapus.");
            }
        }




        private void RefreshData()
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
                // Menutup koneksi ke database
                myConnection.Close();
            }
        }
    }
}
