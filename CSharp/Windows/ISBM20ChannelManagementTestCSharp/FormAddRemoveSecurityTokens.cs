/*
 * Purpose: Dialog for defining security-token add and remove operations
 * used by the ISBM Channel Management sample.
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
    public partial class FormAddRemoveSecurityTokens : Form
    {
        public string channelId;
        public string action = "";

        public bool isCanceled = true;

        // Create a new DataTable to hold security tokens
        public DataTable dataTableSecurityTokens = new DataTable("SecurityTokens");

        public FormAddRemoveSecurityTokens()
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

        private void FormAddRemoveSecurityTokens_Load(object sender, EventArgs e)
        {
            // Set user selected Channel Id 
            textBoxChannelId.Text = channelId;

            // Set form caption 
            switch (action)
            {
                case "Add":
                    buttonAddRemoveSecurityTokens.Text = "Add Tokens";
                    this.Text = "Add Security Tokens";
                    break;
                case "Remove":
                    buttonAddRemoveSecurityTokens.Text = "Remove Tokens";
                    this.Text = "Remove Security Tokens";
                    break;
            }
            
            // Define the columns in the DataTable
            dataTableSecurityTokens.Columns.Add("User Name", typeof(string));
            dataTableSecurityTokens.Columns.Add("Password", typeof(string));
        }

        private void buttonAddRemoveSecurityTokens_Click(object sender, EventArgs e)
        {
            // Check if channed ID is blank
            if (textBoxChannelId.Text == "")
            {
                MessageBox.Show("The Channel ID cannot be blank", "Security Token", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Hide();
                return;
            }
            else
            {
                channelId = textBoxChannelId.Text;
            }

            // Check if no security tokens are selected
            if (dataTableSecurityTokens.Rows.Count == 0)
            { 
                MessageBox.Show("No security tokens are selected.", "Security Token", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Hide();
                return;
            }
            else
            {
                isCanceled = false;
                this.Hide();
            }
        }
    }
}
