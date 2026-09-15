/*
 * Purpose: Dialog for defining channel creation options used by the
 * ISBM Channel Management sample.
 *
 * Updated: 2026
 *
 * Licensed under the MIT License.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RapidRedPanda.ISBM.ClientAdapter.EndpointOptions;

namespace ISBM20ChannelManagementTestCSharp
{
    public partial class FormAddChannel : Form
    {
        public CreateChannelOptions myCreateChannelOptions = new CreateChannelOptions();
        public string channelId;
        public string channelType;
        public string description;

        public bool isCanceled = true;

        // Create a new DataTable
        DataTable dataTableSecurityTokens = new DataTable("SecurityTokens");

        public FormAddChannel()
        {
            InitializeComponent();
        }

        private void buttonAddToken_Click(object sender, EventArgs e)
        {
            // Check if the user name or passord field is empty
            if (textBoxUserName.Text == "")
            {
                MessageBox.Show("The User Name cannot be blank", "Security Token", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Use the Select method to filter rows based on the search condition
            DataRow[] foundRows = dataTableSecurityTokens.Select("[User Name] = '" + textBoxUserName.Text + "'");

            // Check if any rows were found
            if (foundRows.Length > 0)
            {
                MessageBox.Show("The Security Token already exists", "Security Token", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            // Add data row from the GetChannelResponse
            DataRow row = dataTableSecurityTokens.NewRow();
            row["User Name"] = textBoxUserName.Text;
            row["Password"] = textBoxPassword.Text;
            dataTableSecurityTokens.Rows.Add(row);

            // Bound the data table to the data grid view
            dataGridViewSecurityToken.DataSource = dataTableSecurityTokens;
        }

        private void FormAddChannel_Load(object sender, EventArgs e)
        {
            // Define the columns in the DataTable
            dataTableSecurityTokens.Columns.Add("User Name", typeof(string));
            dataTableSecurityTokens.Columns.Add("Password", typeof(string));
        }

        private void buttonDeleteToken_Click(object sender, EventArgs e)
        {
            // Check if any row is selected
            if (dataGridViewSecurityToken.SelectedRows.Count > 0)
            {
                // Get the selected row
                DataGridViewRow selectedRow = dataGridViewSecurityToken.SelectedRows[0];

                // Find the row you want to delete based on a condition
                string usernameToDelete = selectedRow.Cells[0].Value.ToString();
                DataRow[] rowsToDelete = dataTableSecurityTokens.Select("[User Name] = '" + usernameToDelete + "'");

                // Loop through the rows to delete
                foreach (DataRow row in rowsToDelete)
                {
                    dataTableSecurityTokens.Rows.Remove(row);
                }
            }       
        }

        private void buttonCreateChannel_Click(object sender, EventArgs e)
        {
            // Check if channed ID is blank
            if (textBoxChannelId.Text == "")
            {
                MessageBox.Show("The Channel ID cannot be blank", "Security Token", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                channelId = textBoxChannelId.Text;
            }

            // Check if channed Type is correct
            if (textBoxChannelType.Text.ToLower() == "publication" || textBoxChannelType.Text.ToLower() == "request")
            {
                channelType = textBoxChannelType.Text;
            }
            else
            {
                MessageBox.Show("The Channel Type must be Publication or Request Type", "Security Token", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            description = textBoxDescription.Text;

            CreateChannelOptions.SecurityToken mySecurityToken;
            if (dataTableSecurityTokens.Rows.Count > 0)
            {
                foreach (DataRow row in dataTableSecurityTokens.Rows)
                {
                    //Add each security token to the CreateChannelOptions object
                    mySecurityToken = new CreateChannelOptions.SecurityToken();
                    mySecurityToken.type = "UsernameToken";
                    mySecurityToken.username = row["User Name"].ToString();
                    mySecurityToken.password = row["Password"].ToString();
                    myCreateChannelOptions.SecurityTokens.Add(mySecurityToken);
                }
            }

            isCanceled = false;

            this.Hide();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void textBoxDescription_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void dataGridViewSecurityToken_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
