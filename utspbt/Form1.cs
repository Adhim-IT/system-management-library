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


    public partial class login : Form
    {

        //membuat global data  string sql
        string conn = "server=127.0.0.1;database=db_ptspbt;uid=root;pwd=";

        MySqlConnection myConnection;
        public login()
        {
            InitializeComponent();
        }

        private void login_Load(object sender, EventArgs e)
        {

        }

        private void btnLog_Click(object sender, EventArgs e)
        {
            string name, password, role;

            name = txtLog.Text;
            password = textPass.Text;

            try
            {
                string query = "SELECT * FROM user WHERE name = @name AND password = @password";
                using (MySqlConnection connection = new MySqlConnection(conn))
                {
                    connection.Open();

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@name", txtLog.Text);
                        cmd.Parameters.AddWithValue("@password", textPass.Text);

                        using (MySqlDataAdapter sda = new MySqlDataAdapter(cmd))
                        {
                            DataTable dtable = new DataTable();
                            sda.Fill(dtable);

                            if (dtable.Rows.Count > 0)
                            {
                                DataRow row = dtable.Rows[0];
                                role = row["role"].ToString();

                                if (role == "admin")
                                {
                                    admin_page action = new admin_page();
                                    action.Show();
                                }
                                else if (role == "user")
                                {
                                    user_page peminjaman = new user_page();
                                    peminjaman.Show();
                                }

                                Hide();
                            }
                            else
                            {
                                MessageBox.Show("Invalid login details", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                txtLog.Clear();
                                textPass.Clear();
                                txtLog.Focus();
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


        private void btnClear_Click(object sender, EventArgs e)
        {
            txtLog.Clear();
            textPass.Clear();


            txtLog.Focus();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult res;

            res = MessageBox.Show("Yakin ingin keluar?", "Exit", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (res == DialogResult.OK)
            {
                Application.Exit();
            }
        }


        private void textPass_TextChanged(object sender, EventArgs e)
        {
            textPass.PasswordChar = '*';
        }
    }
}
