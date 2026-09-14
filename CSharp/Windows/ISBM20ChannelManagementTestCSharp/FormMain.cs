/*
 * Purpose: Main user interface for the ISBM Channel Management sample.
 * It demonstrates channel management operations using ChannelManagementService.
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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using RapidRedPanda.ISBM.ClientAdapter;
using RapidRedPanda.ISBM.ClientAdapter.EndpointOptions;
using RapidRedPanda.ISBM.ClientAdapter.Enums;
using RapidRedPanda.ISBM.ClientAdapter.ResponseType;
using RapidRedPanda.ISBM.ClientAdapter.ServerOptions;

namespace ISBM20ChannelManagementTestCSharp
{
    public partial class FormMain : Form
    {
        private ChannelManagementService myChannelManagementService;

        public FormMain()
        {
            InitializeComponent();

            if (!IsDesignTime())
            {
                myChannelManagementService = new ChannelManagementService();
            }
        }

        private ChannelManagementService ChannelManagementService
        {
            get
            {
                if (myChannelManagementService == null)
                {
                    myChannelManagementService = new ChannelManagementService();
                }

                return myChannelManagementService;
            }
        }

        private bool IsDesignTime()
        {
            return DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime;
        }

        private async void buttonGetChannels_Click(object sender, EventArgs e)
        {
            dataGridViewChannels.DataSource = null;

            ChannelManagementService.Credential.Username = textBoxUserName.Text;
            ChannelManagementService.Credential.Password = textBoxPassword.Text;
            GetChannelsResponse myGetChannelsResponse = await ChannelManagementService.GetChannelsAsync(textBoxHostName.Text, CancellationToken.None);

            //ISBM Adapter Response
            textBoxStatusCode.Text = myGetChannelsResponse.StatusCode.ToString();
            textBoxReasonPhrase.Text = myGetChannelsResponse.ReasonPhrase;
            textBoxResponse.Text = myGetChannelsResponse.ISBMHTTPResponse;

            if (myGetChannelsResponse.StatusCode == 200 && myGetChannelsResponse.Channels.Count > 0)
            {
                // Create a new DataTable
                DataTable dataTableChannels = new DataTable("Channels");

                // Define the columns in the DataTable
                dataTableChannels.Columns.Add("Uri", typeof(string));
                dataTableChannels.Columns.Add("ChannelType", typeof(string));
                dataTableChannels.Columns.Add("Description", typeof(string));

                // Add the data from myObjectList to the DataTable
                foreach (GetChannelsResponse.Channel channel in myGetChannelsResponse.Channels)
                {
                    DataRow row = dataTableChannels.NewRow();
                    row["Uri"] = channel.URI;
                    row["ChannelType"] = channel.ChannelType;
                    row["Description"] = channel.Description;
                    dataTableChannels.Rows.Add(row);
                }

                // Bound the data table to the data grid view
                dataGridViewChannels.DataSource = dataTableChannels;

                // Set the AutoSizeMode of the first column
                dataGridViewChannels.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

                // Adjust the column width to fit the content
                dataGridViewChannels.Columns[0].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                dataGridViewChannels.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGridViewChannels.Columns[0].DefaultCellStyle.WrapMode = DataGridViewTriState.False;

                dataGridViewChannels.Refresh();
            }
           
        }

        private async void buttonGetChannel_Click(object sender, EventArgs e)
        {
            dataGridViewChannels.DataSource = null;

            ChannelManagementService.Credential.Username = textBoxUserName.Text;
            ChannelManagementService.Credential.Password = textBoxPassword.Text;
            GetChannelResponse myGetChannelResponse = await ChannelManagementService.GetChannelAsync(textBoxHostName.Text, textBoxChannelId.Text, CancellationToken.None);

            //ISBM Adapter Response
            textBoxStatusCode.Text = myGetChannelResponse.StatusCode.ToString();
            textBoxReasonPhrase.Text = myGetChannelResponse.ReasonPhrase;
            textBoxResponse.Text = myGetChannelResponse.ISBMHTTPResponse;

            if (myGetChannelResponse.StatusCode == 200)
            {
                // Create a new DataTable
                DataTable dataTableChannels = new DataTable("Channels");

                // Define the columns in the DataTable
                dataTableChannels.Columns.Add("Uri", typeof(string));
                dataTableChannels.Columns.Add("ChannelType", typeof(string));
                dataTableChannels.Columns.Add("Description", typeof(string));

                // Add the data from myObjectList to the DataTable
                foreach (GetChannelResponse.Channel channel in myGetChannelResponse.Channels)
                {
                    DataRow row = dataTableChannels.NewRow();
                    row["Uri"] = channel.URI;
                    row["ChannelType"] = channel.ChannelType;
                    row["Description"] = channel.Description;
                    dataTableChannels.Rows.Add(row);
                }

                // Bound the data table to the data grid view
                dataGridViewChannels.DataSource = dataTableChannels;

                // Set the AutoSizeMode of the first column
                dataGridViewChannels.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

                // Adjust the column width to fit the content
                dataGridViewChannels.Columns[0].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                dataGridViewChannels.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGridViewChannels.Columns[0].DefaultCellStyle.WrapMode = DataGridViewTriState.False;

                dataGridViewChannels.Refresh();
            }



        }

        private async void buttonCreateChannel_Click(object sender, EventArgs e)
        {
            FormAddChannel fAddChannel = new FormAddChannel();
            fAddChannel.ShowDialog();

            string channelId = fAddChannel.channelId;
            string channelType = fAddChannel.channelType;
            string description = fAddChannel.description;

            CreateChannelOptions myCreateChannelOptions = fAddChannel.myCreateChannelOptions;
            bool isCanceled = fAddChannel.isCanceled;

            fAddChannel.Dispose();

            if (isCanceled == true)
            {
                return;
            }

            ChannelManagementService.Credential.Username = textBoxUserName.Text;
            ChannelManagementService.Credential.Password = textBoxPassword.Text;

            CreateChannelResponse myCreateChannelResponse = new CreateChannelResponse();
            if (myCreateChannelOptions.SecurityTokens.Count > 0)
            {
                myCreateChannelResponse = await ChannelManagementService.CreateChannelAsync(textBoxHostName.Text, channelId, channelType, description, myCreateChannelOptions, CancellationToken.None);
            }
            else
            {
                myCreateChannelResponse = await ChannelManagementService.CreateChannelAsync(textBoxHostName.Text, channelId, channelType, description, CancellationToken.None);
            }    

            //ISBM Adapter Response
            textBoxStatusCode.Text = myCreateChannelResponse.StatusCode.ToString();
            textBoxReasonPhrase.Text = myCreateChannelResponse.ReasonPhrase;
            textBoxResponse.Text = myCreateChannelResponse.ISBMHTTPResponse;

            //if (myCreateChannelResponse.StatusCode == 201)
            //{
            //    // Calling the button get channels event handler as a function call
            //    //buttonGetChannels_Click(null, EventArgs.Empty);
            //}    
        }
        
        private void dataGridViewChannels_SelectionChanged(object sender, EventArgs e)
        {
            // Check if any row is selected
            if (dataGridViewChannels.SelectedRows.Count > 0)
            {
                // Get the selected row
                DataGridViewRow selectedRow = dataGridViewChannels.SelectedRows[0];

                // Access the value in the first column of the selected row
                textBoxChannelId.Text = selectedRow.Cells[0].Value.ToString();
            }
        }

        private async void buttonAddSecurityTokens_Click(object sender, EventArgs e)
        {
            FormAddRemoveSecurityTokens fAddSecurityTokens = new FormAddRemoveSecurityTokens();
            fAddSecurityTokens.channelId = textBoxChannelId.Text;
            fAddSecurityTokens.action = "Add";

            fAddSecurityTokens.ShowDialog();

            string channelId = fAddSecurityTokens.channelId;

            DataTable dataTableSecurityTokens = fAddSecurityTokens.dataTableSecurityTokens;
            bool isCanceled = fAddSecurityTokens.isCanceled;

            fAddSecurityTokens.Dispose();

            if (isCanceled == true)
            {
                return;
            }

            AddSecurityTokensOptions myAddSecurityTokensOptions = new AddSecurityTokensOptions();

            AddSecurityTokensOptions.SecurityToken mySecurityToken;
            if (dataTableSecurityTokens.Rows.Count > 0)
            {
                foreach (DataRow row in dataTableSecurityTokens.Rows)
                {
                    //Add each security token to the CreateChannelOptions object
                    mySecurityToken = new AddSecurityTokensOptions.SecurityToken();
                    mySecurityToken.type = "UsernameToken";
                    mySecurityToken.username = row["User Name"].ToString();
                    mySecurityToken.password = row["Password"].ToString();
                    myAddSecurityTokensOptions.SecurityTokens.Add(mySecurityToken);
                }
            }

            ChannelManagementService.Credential.Username = textBoxUserName.Text;
            ChannelManagementService.Credential.Password = textBoxPassword.Text;

            AddSecurityTokensResponse myAddSecurityTokensResponse = await ChannelManagementService.AddSecurityTokensAsync(textBoxHostName.Text, channelId, myAddSecurityTokensOptions, CancellationToken.None);
           
            //ISBM Adapter Response
            textBoxStatusCode.Text = myAddSecurityTokensResponse.StatusCode.ToString();
            textBoxReasonPhrase.Text = myAddSecurityTokensResponse.ReasonPhrase;
            textBoxResponse.Text = myAddSecurityTokensResponse.ISBMHTTPResponse;
        }

        private async void buttonRemoveSecurityTokens_Click(object sender, EventArgs e)
        {
            FormAddRemoveSecurityTokens fRemoveSecurityTokens = new FormAddRemoveSecurityTokens();
            fRemoveSecurityTokens.channelId = textBoxChannelId.Text;
            fRemoveSecurityTokens.action = "Remove";

            fRemoveSecurityTokens.ShowDialog();

            string channelId = fRemoveSecurityTokens.channelId;

            DataTable dataTableSecurityTokens = fRemoveSecurityTokens.dataTableSecurityTokens;
            bool isCanceled = fRemoveSecurityTokens.isCanceled;

            fRemoveSecurityTokens.Dispose();

            if (isCanceled == true)
            {
                return;
            }

            RemoveSecurityTokensOptions myRemoveSecurityTokensOptions = new RemoveSecurityTokensOptions();
            RemoveSecurityTokensOptions.SecurityToken mySecurityToken;
            if (dataTableSecurityTokens.Rows.Count > 0)
            {
                foreach (DataRow row in dataTableSecurityTokens.Rows)
                {
                    //Add each security token to the CreateChannelOptions object
                    mySecurityToken = new RemoveSecurityTokensOptions.SecurityToken();
                    mySecurityToken.type = "UsernameToken";
                    mySecurityToken.username = row["User Name"].ToString();
                    mySecurityToken.password = row["Password"].ToString();
                    myRemoveSecurityTokensOptions.SecurityTokens.Add(mySecurityToken);
                }
            }

            ChannelManagementService.Credential.Username = textBoxUserName.Text;
            ChannelManagementService.Credential.Password = textBoxPassword.Text;

            RemoveSecurityTokensResponse myRemoveSecurityTokensResponse = await ChannelManagementService.RemoveSecurityTokensAsync(textBoxHostName.Text, channelId, myRemoveSecurityTokensOptions, CancellationToken.None);
           
            //ISBM Adapter Response
            textBoxStatusCode.Text = myRemoveSecurityTokensResponse.StatusCode.ToString();
            textBoxReasonPhrase.Text = myRemoveSecurityTokensResponse.ReasonPhrase;
            textBoxResponse.Text = myRemoveSecurityTokensResponse.ISBMHTTPResponse;
        }

        private async void buttonDeleteChannel_Click(object sender, EventArgs e)
        {
            if (textBoxChannelId.Text == "")
            {
                MessageBox.Show("The Channel ID cannot be blank", "Security Token", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ChannelManagementService.Credential.Username = textBoxUserName.Text;
            ChannelManagementService.Credential.Password = textBoxPassword.Text;
            //myChannelManagementService.ISBMServerType = ISBM21ClientAdapter.Enums.ServerType.IIS;
            DeleteChannelResponse myDeleteChannelResponse = await ChannelManagementService.DeleteChannelAsync(textBoxHostName.Text, textBoxChannelId.Text, CancellationToken.None);

            //ISBM Adapter Response
            textBoxStatusCode.Text = myDeleteChannelResponse.StatusCode.ToString();
            textBoxReasonPhrase.Text = myDeleteChannelResponse.ReasonPhrase;
            textBoxResponse.Text = myDeleteChannelResponse.ISBMHTTPResponse;
        }
    }
}
