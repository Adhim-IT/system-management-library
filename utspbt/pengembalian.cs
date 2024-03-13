using MySql.Data.MySqlClient;
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
    public partial class @return : Form
    {

        //membuat global data  string sql
        string conn = "server=127.0.0.1;database=db_ptspbt;uid=root;pwd=";

        MySqlConnection myConnection;
        public @return()
        {
            InitializeComponent();
            LoadDataBuku();
        }

        private void LoadDataBuku()
        {
            // Mengisi ComboBox dengan data buku yang masih dipinjam
            string query = "SELECT DISTINCT b.buku_nama FROM peminjam p INNER JOIN buku b ON p.buku_id = b.id WHERE p.tanggal_kembali IS NULL";
            myConnection = new MySqlConnection(conn);

            try
            {
                myConnection.Open();
                MySqlCommand cmd = new MySqlCommand(query, myConnection);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                // Set data source ComboBox dengan data buku
                comboBox1.DataSource = dt;
                comboBox1.DisplayMember = "buku_nama";
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

        private void button1_Click(object sender, EventArgs e)
        {
            // Ambil nilai dari TextBox (nama peminjam) dan ComboBox (buku yang dikembalikan)
            string namaPeminjam = textBox2.Text;
            string bukuDikembalikan = comboBox1.Text;

            // Lakukan validasi
            if (!string.IsNullOrEmpty(namaPeminjam) && !string.IsNullOrEmpty(bukuDikembalikan))
            {
                // Lakukan pemrosesan pengembalian
                if (ProcessPengembalian(namaPeminjam, bukuDikembalikan))
                {
                    // Tampilkan pesan sukses
                    MessageBox.Show("Pengembalian berhasil.");
                    // Refresh data buku setelah pengembalian
                    LoadDataBuku();
                }
                else
                {
                    // Tampilkan pesan gagal jika ada masalah
                    MessageBox.Show("Gagal melakukan pengembalian. Silakan coba lagi.");
                }
            }
            else
            {
                // Tampilkan pesan bahwa nama peminjam atau buku yang dikembalikan tidak valid
                MessageBox.Show("Isi nama peminjam dan pilih buku yang dikembalikan terlebih dahulu.");
            }
        }

        private bool ProcessPengembalian(string namaPeminjam, string bukuDikembalikan)
        {
            try
            {
                myConnection.Open();

                // Perintah SQL untuk mengatur tanggal pengembalian di tabel peminjam
                string updatePeminjamQuery = "UPDATE peminjam SET tanggal_kembali = CURRENT_DATE " +
                                              "WHERE nama_peminjam = @NamaPeminjam AND buku_id IN (SELECT id FROM buku WHERE buku_nama = @BukuDikembalikan)";

                MySqlCommand updatePeminjamCmd = new MySqlCommand(updatePeminjamQuery, myConnection);
                updatePeminjamCmd.Parameters.AddWithValue("@NamaPeminjam", namaPeminjam);
                updatePeminjamCmd.Parameters.AddWithValue("@BukuDikembalikan", bukuDikembalikan);

                // Eksekusi perintah SQL untuk update peminjam
                int rowsAffectedPeminjam = updatePeminjamCmd.ExecuteNonQuery();

                if (rowsAffectedPeminjam > 0)
                {
                    // Jika pengembalian peminjam berhasil, tambahkan quantity di tabel buku
                    string updateBukuQuery = "UPDATE buku SET quantity = quantity + 1 WHERE buku_nama = @BukuDikembalikan";
                    MySqlCommand updateBukuCmd = new MySqlCommand(updateBukuQuery, myConnection);
                    updateBukuCmd.Parameters.AddWithValue("@BukuDikembalikan", bukuDikembalikan);

                    // Eksekusi perintah SQL untuk update buku
                    int rowsAffectedBuku = updateBukuCmd.ExecuteNonQuery();

                    if (rowsAffectedBuku > 0)
                    {
                        // Jika berhasil menambah quantity di buku, return true
                        return true;
                    }
                    else
                    {
                        // Jika gagal menambah quantity di buku, tampilkan pesan gagal
                        MessageBox.Show("Gagal menambah quantity di buku.");
                        return false;
                    }
                }
                else
                {
                    // Jika pengembalian peminjam gagal, tampilkan pesan gagal
                    MessageBox.Show("Gagal melakukan pengembalian peminjam.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return false;
            }
            finally
            {
                myConnection.Close();
            }
        }


        private void return_Load(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            user_page form2 = new user_page();
            form2.Show();

            // Tutup halaman Form1 (opsional, tergantung pada kebutuhan Anda)
            this.Hide();
        }
    }
}
