using System;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace DbSync
{
    public partial class Form1 : Form
    {
        private DataAccess dataAccess;
        private string mssqlConnectionString;
        private string sqliteConnectionString;
        public Form1()
        {
            InitializeComponent();
            mssqlConnectionString = "Data Source=DESKTOP-71GF00O\\SQLEXPRESS;Initial Catalog=CustomerDb;Integrated Security=True;";
            sqliteConnectionString = "Data Source=D:\\sqlitedb\\Customerdb.db";

            dataAccess = new DataAccess(mssqlConnectionString, sqliteConnectionString);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Interval = 10000; // default interval is 10 seconds
            timer1.Start();

        }


        private void button1_Click(object sender, EventArgs e)
        {
            DataTable customers = dataAccess.GetCustomersFromMSSQL();
            DataTable locations = dataAccess.GetLocationsFromMSSQL();

            dataGridView1.DataSource = customers;
            dataGridView2.DataSource = locations;

            dataAccess.SyncDataToSQLite(customers, locations);

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DataTable customers = dataAccess.GetCustomersFromMSSQL();
            DataTable locations = dataAccess.GetLocationsFromMSSQL();

            dataGridView1.DataSource = customers;
            dataGridView2.DataSource = locations;

            dataAccess.SyncDataToSQLite(customers, locations);
        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {
            try
            {
                int interval = int.Parse(textBox1.Text);
                timer1.Interval = interval * 1000;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Invalid interval");
            }

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
