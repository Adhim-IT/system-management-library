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
    public partial class manage_book : Form
    {

        //membuat global data  string sql
        string conn = "server=127.0.0.1;database=db_ptspbt;uid=root;pwd=";
        MySqlConnection myConnection;


        public manage_book()
        {
            InitializeComponent();
        }

        private void manage_book_Load(object sender, EventArgs e)
        {
            // Mengisi ComboBox1 dengan kategori
            PopulateCategoriesComboBox(comboBox1);

            // Mengisi ComboBox2 dengan kategori
            PopulateCategoriesComboBox(comboBox2);

            // Tambahkan debug statement untuk memeriksa apakah ComboBox berhasil diisi
            Console.WriteLine("ComboBox1 items count: " + comboBox1.Items.Count);
            Console.WriteLine("ComboBox2 items count: " + comboBox2.Items.Count);
        }

        private void PopulateCategoriesComboBox(ComboBox comboBox)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(conn))
                {
                    connection.Open();

                    // Buat pernyataan SQL SELECT untuk mendapatkan kategori
                    string selectCategoriesQuery = "SELECT DISTINCT category_name FROM category";

                    using (MySqlCommand cmd = new MySqlCommand(selectCategoriesQuery, connection))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Tambahkan kategori ke ComboBox
                                comboBox.Items.Add(reader["category_name"].ToString());
                            }
                        }
                    }

                    // Tambahkan debug statement untuk memeriksa apakah ComboBox berhasil diisi
                    Console.WriteLine("ComboBox items count: " + comboBox.Items.Count);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading categories: " + ex.Message);
            }
        }



        private void btnadd_Click(object sender, EventArgs e)
        {
            try
            {
                // Baca data dari kontrol input
                string bukuNama = textBox2.Text;
                ulong isbn;
                if (!ulong.TryParse(textBox1.Text.Replace("-", ""), out isbn))
                {
                    MessageBox.Show("Invalid ISBN.");
                    return;
                }

                // Ambil category_id berdasarkan nama kategori
                string kategori = comboBox1.SelectedItem?.ToString();
                int category_id = GetCategoryId(kategori);

                // Validasi category_id
                if (category_id == -1)
                {
                    MessageBox.Show("Kategori tidak valid.");
                    return;
                }

                int quantity;
                if (!int.TryParse(textBox3.Text, out quantity))
                {
                    MessageBox.Show("Invalid quantity.");
                    return;
                }

                // Buat koneksi
                using (MySqlConnection connection = new MySqlConnection(conn))
                {
                    connection.Open();

                    // Buat pernyataan SQL INSERT
                    string insertQuery = "INSERT INTO buku (buku_nama, isbn, category_id, quantity) VALUES (@buku_nama, @isbn, @category_id, @quantity)";

                    // Buat objek perintah SQL
                    using (MySqlCommand cmd = new MySqlCommand(insertQuery, connection))
                    {
                        // Tambahkan parameter ke pernyataan SQL
                        cmd.Parameters.Add("@buku_nama", MySqlDbType.VarChar).Value = bukuNama;
                        cmd.Parameters.Add("@isbn", MySqlDbType.VarChar).Value = isbn.ToString(); // Store ISBN as string
                        cmd.Parameters.Add("@category_id", MySqlDbType.Int32).Value = category_id;
                        cmd.Parameters.Add("@quantity", MySqlDbType.Int32).Value = quantity;

                        // Eksekusi perintah SQL
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Buku berhasil ditambahkan ke database.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // Metode untuk mendapatkan category_id berdasarkan nama kategori
        private int GetCategoryId(string categoryName)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(conn))
                {
                    connection.Open();

                    // Buat pernyataan SQL SELECT untuk mendapatkan category_id berdasarkan nama kategori
                    string selectCategoryIdQuery = "SELECT id FROM category WHERE category_name = @categoryName";

                    using (MySqlCommand cmd = new MySqlCommand(selectCategoryIdQuery, connection))
                    {
                        cmd.Parameters.Add("@categoryName", MySqlDbType.VarChar).Value = categoryName;

                        // Execute the SQL command and read the result
                        object result = cmd.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            return Convert.ToInt32(result);
                        }
                    }
                }

                return -1; // Kembalikan nilai -1 jika kategori tidak ditemukan
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return -1;
            }
        }


        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnhps_Click(object sender, EventArgs e)
        {
            try
            {
                // Read ISBN and quantity to be subtracted
                ulong isbn;
                if (!ulong.TryParse(textBox4.Text.Replace("-", ""), out isbn))
                {
                    MessageBox.Show("Invalid ISBN.");
                    return;
                }

                int quantityToSubtract;
                if (!int.TryParse(textBox7.Text, out quantityToSubtract))
                {
                    MessageBox.Show("Invalid quantity to subtract.");
                    return;
                }

                // Create connection
                using (MySqlConnection connection = new MySqlConnection(conn))
                {
                    connection.Open();

                    // Check the current quantity
                    string selectQuery = "SELECT quantity FROM buku WHERE isbn = @isbn";

                    using (MySqlCommand cmdSelect = new MySqlCommand(selectQuery, connection))
                    {
                        cmdSelect.Parameters.Add("@isbn", MySqlDbType.VarChar).Value = isbn.ToString();

                        object result = cmdSelect.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            int currentQuantity = Convert.ToInt32(result);

                            // Check if there's enough quantity to subtract
                            if (currentQuantity >= quantityToSubtract)
                            {
                                // Update the quantity in the database
                                string updateQuery = "UPDATE buku SET quantity = quantity - @quantityToSubtract WHERE isbn = @isbn";

                                using (MySqlCommand cmdUpdate = new MySqlCommand(updateQuery, connection))
                                {
                                    cmdUpdate.Parameters.Add("@isbn", MySqlDbType.VarChar).Value = isbn.ToString();
                                    cmdUpdate.Parameters.Add("@quantityToSubtract", MySqlDbType.Int32).Value = quantityToSubtract;

                                    cmdUpdate.ExecuteNonQuery();

                                    MessageBox.Show("Quantity updated successfully.");
                                }
                            }
                            else
                            {
                                MessageBox.Show("Not enough quantity to subtract.");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Book not found.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }


        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                // Read ISBN for searching
                ulong isbn;
                if (!ulong.TryParse(textBox4.Text.Replace("-", ""), out isbn))
                {
                    MessageBox.Show("Invalid ISBN.");
                    return;
                }

                // Create connection
                using (MySqlConnection connection = new MySqlConnection(conn))
                {
                    connection.Open();

                    // Create SQL SELECT statement
                    string selectQuery = "SELECT buku_nama, category_id, quantity FROM buku WHERE isbn = @isbn";

                    // Create SQL command object
                    using (MySqlCommand cmd = new MySqlCommand(selectQuery, connection))
                    {
                        // Add parameter to the SQL statement
                        cmd.Parameters.Add("@isbn", MySqlDbType.VarChar).Value = isbn.ToString(); // Store ISBN as string

                        // Execute the SQL command and read the results
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Display the results in your UI elements
                                textBox5.Text = reader["buku_nama"].ToString();

                                // Get category_id from the reader
                                int category_id;
                                if (int.TryParse(reader["category_id"].ToString(), out category_id))
                                {
                                    // Use the GetCategoryNameById method to get the category name
                                    string categoryName = GetCategoryNameById(category_id);
                                    comboBox2.SelectedItem = categoryName;
                                }
                                else
                                {
                                    MessageBox.Show("Invalid category ID.");
                                }

                                textBox6.Text = reader["quantity"].ToString();
                            }
                            else
                            {
                                MessageBox.Show("Data not found.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        private string GetCategoryNameById(int categoryId)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(conn))
                {
                    connection.Open();

                    // Create SQL SELECT statement to get category name
                    string selectCategoryQuery = "SELECT category_name FROM category WHERE id = @categoryId";

                    using (MySqlCommand cmd = new MySqlCommand(selectCategoryQuery, connection))
                    {
                        cmd.Parameters.Add("@categoryId", MySqlDbType.Int32).Value = categoryId;

                        // Execute the SQL command and read the result
                        object result = cmd.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            return result.ToString();
                        }
                    }
                }

                return "Category not found"; // Return a default value if category not found
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return "Error retrieving category";
            }
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            admin_page admin_Page = new admin_page();
            admin_Page.Show();
            this.Hide();  
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
