﻿using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Library_Management_System
{
    public partial class borrowedDetails : Form
    {

        string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\LibraryManagementSystem.mdf;Integrated Security=True;Connect Timeout=30";
        public borrowedDetails()
        {
            InitializeComponent();
        }

        private void borrowedDetails_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            SqlConnection connection = new SqlConnection(connectionString);

            try
            {
                connection.Open();
                SqlCommand command = new SqlCommand("Select * From borrowings", connection);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();

                adapter.Fill(dataTable);

                dataTable.Columns["userId"].ColumnName = "User ID";
                dataTable.Columns["name"].ColumnName = "Name";
                dataTable.Columns["status"].ColumnName = "Status";
                dataTable.Columns["borrowingId"].ColumnName = "Borrowing ID";
                dataTable.Columns["isbn"].ColumnName = "ISBN";
                dataTable.Columns["borrowDate"].ColumnName = "Borrow Date";
                dataTable.Columns["returnDate"].ColumnName = "Return Date";



                borrowedDetailsDataGrid.DataSource = dataTable;
                foreach (DataGridViewColumn column in borrowedDetailsDataGrid.Columns)
                {
                    column.HeaderCell.Style.Font = new Font("Arial", 10, FontStyle.Bold);
                }

                borrowedDetailsDataGrid.DefaultCellStyle = new DataGridViewCellStyle();

                DataGridViewButtonColumn deleteButtonColumn = new DataGridViewButtonColumn();
                deleteButtonColumn.Name = "Delete";
                deleteButtonColumn.Text = "Delete";
                deleteButtonColumn.HeaderText = "Delete";
                deleteButtonColumn.UseColumnTextForButtonValue = true;

                deleteButtonColumn.HeaderCell.Style.Font = new Font("Arial", 10, FontStyle.Bold);
                borrowedDetailsDataGrid.Columns.Add(deleteButtonColumn);

                deleteButtonColumn.DefaultCellStyle.BackColor = Color.Red;
                deleteButtonColumn.DefaultCellStyle.ForeColor = Color.White;
                deleteButtonColumn.DefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Bold);
                deleteButtonColumn.FlatStyle = FlatStyle.Flat;
                deleteButtonColumn.DefaultCellStyle.SelectionBackColor = Color.Red;

                DataGridViewButtonColumn updateButtonColumn = new DataGridViewButtonColumn();
                updateButtonColumn.Name = "Update";
                updateButtonColumn.Text = "Update";
                updateButtonColumn.UseColumnTextForButtonValue = true;

                updateButtonColumn.HeaderCell.Style.Font = new Font("Arial", 10, FontStyle.Bold);
                borrowedDetailsDataGrid.Columns.Add(updateButtonColumn);

                updateButtonColumn.DefaultCellStyle.BackColor = Color.Green;
                updateButtonColumn.DefaultCellStyle.ForeColor = Color.White;
                updateButtonColumn.DefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Bold);
                updateButtonColumn.FlatStyle = FlatStyle.Flat;
                updateButtonColumn.DefaultCellStyle.SelectionBackColor = Color.Green;

                borrowedDetailsDataGrid.AllowUserToAddRows = false;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            string searchKey = searchTextBox.Text;

            if (string.IsNullOrEmpty(searchKey))
            {
                MessageBox.Show("Please enter a search term.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SqlConnection connection = new SqlConnection(connectionString);

            try
            {
                connection.Open();
                SqlCommand searchCommand = new SqlCommand("SELECT * FROM borrowings WHERE name LIKE @searchKey OR userId LIKE @searchKey OR borrowingId LIKE @searchKey", connection);
                searchCommand.Parameters.AddWithValue("@searchKey", "%" + searchKey + "%");
                SqlDataAdapter adapter = new SqlDataAdapter(searchCommand);
                DataTable dataTable = new DataTable();

                adapter.Fill(dataTable);

                dataTable.Columns["userId"].ColumnName = "User ID";
                dataTable.Columns["name"].ColumnName = "Name";
                dataTable.Columns["status"].ColumnName = "Status";
                dataTable.Columns["borrowingId"].ColumnName = "Borrowing ID";
                dataTable.Columns["isbn"].ColumnName = "ISBN";
                dataTable.Columns["borrowDate"].ColumnName = "Borrow Date";
                dataTable.Columns["returnDate"].ColumnName = "Return Date";

                borrowedDetailsDataGrid.DataSource = dataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                connection.Close();
            }
        }

        private void viewAllButton_Click(object sender, EventArgs e)
        {
            borrowedDetailsDataGrid.DataSource = null;
            borrowedDetailsDataGrid.Rows.Clear();
            borrowedDetailsDataGrid.Columns.Clear();
            LoadData();
        }

        private void newBorrowingButton_Click(object sender, EventArgs e)
        {
            addBorrowings obj1 = new addBorrowings();
            obj1.Show();
            obj1.FormClosed += UpdateFormClosed;
        }

        private void pendingButton_Click(object sender, EventArgs e)
        {
            SqlConnection connection = new SqlConnection(connectionString);

            try
            {
                connection.Open();
                string query = "SELECT * FROM borrowings WHERE status=@status";

                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@status", "Pending");
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();

                adapter.Fill(dataTable);

                dataTable.Columns["userId"].ColumnName = "User ID";
                dataTable.Columns["name"].ColumnName = "Name";
                dataTable.Columns["status"].ColumnName = "Status";
                dataTable.Columns["borrowingId"].ColumnName = "Borrowing ID";
                dataTable.Columns["isbn"].ColumnName = "ISBN";
                dataTable.Columns["borrowDate"].ColumnName = "Borrow Date";
                dataTable.Columns["returnDate"].ColumnName = "Return Date";

                borrowedDetailsDataGrid.DataSource = dataTable;
                

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                connection.Close();
            }
        }

        private void borrowedDetailsDataGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = borrowedDetailsDataGrid.Rows[e.RowIndex];


                if (e.ColumnIndex == borrowedDetailsDataGrid.Columns["Delete"].Index)
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to delete this entry?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        string id = row.Cells["Borrowing ID"].Value.ToString();
                        string isbn= row.Cells["ISBN"].Value.ToString();

                        SqlConnection connection = new SqlConnection(connectionString);
                        try
                        {
                            connection.Open();
                            SqlCommand deleteCommand = new SqlCommand("DELETE FROM borrowings WHERE borrowingId = @borrowingId", connection);
                            deleteCommand.Parameters.AddWithValue("@borrowingId", id);
                            int rowsAffected = deleteCommand.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {

                                borrowedDetailsDataGrid.Rows.RemoveAt(e.RowIndex);

                                try
                                {

                                    string query = "UPDATE physicalbooks SET status=@status WHERE isbn=@isbn";
                                    SqlCommand updateCommand = new SqlCommand(query, connection);
                                    updateCommand.Parameters.AddWithValue("@status", "Available");
                                    updateCommand.Parameters.AddWithValue("@isbn", isbn);

                                    if (Convert.ToInt32(updateCommand.ExecuteNonQuery()) > 0)
                                    {
                                        MessageBox.Show("Entry deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        
                                    }

                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
   
                            }
                            else
                            {
                                MessageBox.Show("Failed to delete the entry.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        finally
                        {
                            connection.Close();
                        }
                    }
                }

                else if (e.ColumnIndex == borrowedDetailsDataGrid.Columns["Update"].Index)
                {
                    string borrowingId = row.Cells["Borrowing ID"].Value.ToString();
                    string userId = row.Cells["User ID"].Value.ToString();
                    string name = row.Cells["Name"].Value.ToString();
                    string isbn = row.Cells["ISBN"].Value.ToString();
                    string borrowDate = row.Cells["Borrow Date"].Value.ToString();
                    string returnDate = row.Cells["Borrow Date"].Value.ToString();
                    string status = row.Cells["Status"].Value.ToString();


                    updateBorrowings obj1 = new updateBorrowings(borrowingId,userId, name, isbn, borrowDate,returnDate,status);
                    obj1.Show();
                    obj1.FormClosed += UpdateFormClosed;

                }
            }
        }
        private void UpdateFormClosed(object sender, FormClosedEventArgs e)
        {
            borrowedDetailsDataGrid.DataSource = null;
            borrowedDetailsDataGrid.Rows.Clear();
            borrowedDetailsDataGrid.Columns.Clear();

            LoadData();
        }
    }
}
