namespace ISBM20ChannelManagementTestCSharp
{
    partial class FormMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textBoxHostName = new System.Windows.Forms.TextBox();
            this.labelHostName = new System.Windows.Forms.Label();
            this.buttonCreateChannel = new System.Windows.Forms.Button();
            this.buttonGetChannels = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.textBoxUserName = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.textBoxResponse = new System.Windows.Forms.TextBox();
            this.labelResponse = new System.Windows.Forms.Label();
            this.textBoxReasonPhrase = new System.Windows.Forms.TextBox();
            this.textBoxStatusCode = new System.Windows.Forms.TextBox();
            this.labelReasonPhrase = new System.Windows.Forms.Label();
            this.labelStatusCode = new System.Windows.Forms.Label();
            this.dataGridViewChannels = new System.Windows.Forms.DataGridView();
            this.buttonGetChannel = new System.Windows.Forms.Button();
            this.textBoxChannelId = new System.Windows.Forms.TextBox();
            this.labelChannelID = new System.Windows.Forms.Label();
            this.buttonAddSecurityTokens = new System.Windows.Forms.Button();
            this.buttonRemoveSecurityTokens = new System.Windows.Forms.Button();
            this.buttonDeleteChannel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewChannels)).BeginInit();
            this.SuspendLayout();
            // 
            // textBoxHostName
            // 
            this.textBoxHostName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.textBoxHostName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxHostName.Location = new System.Drawing.Point(148, 28);
            this.textBoxHostName.Name = "textBoxHostName";
            this.textBoxHostName.Size = new System.Drawing.Size(548, 26);
            this.textBoxHostName.TabIndex = 97;
            this.textBoxHostName.Text = "https://localhost:44384/isbm/2.0";
            // 
            // labelHostName
            // 
            this.labelHostName.AutoSize = true;
            this.labelHostName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelHostName.ForeColor = System.Drawing.Color.Black;
            this.labelHostName.Location = new System.Drawing.Point(19, 31);
            this.labelHostName.Name = "labelHostName";
            this.labelHostName.Size = new System.Drawing.Size(89, 20);
            this.labelHostName.TabIndex = 96;
            this.labelHostName.Text = "Host Name";
            // 
            // buttonCreateChannel
            // 
            this.buttonCreateChannel.Location = new System.Drawing.Point(14, 528);
            this.buttonCreateChannel.Name = "buttonCreateChannel";
            this.buttonCreateChannel.Size = new System.Drawing.Size(103, 41);
            this.buttonCreateChannel.TabIndex = 107;
            this.buttonCreateChannel.Text = "Create Channel";
            this.buttonCreateChannel.UseVisualStyleBackColor = true;
            this.buttonCreateChannel.Click += new System.EventHandler(this.buttonCreateChannel_Click);
            // 
            // buttonGetChannels
            // 
            this.buttonGetChannels.Location = new System.Drawing.Point(448, 528);
            this.buttonGetChannels.Name = "buttonGetChannels";
            this.buttonGetChannels.Size = new System.Drawing.Size(103, 41);
            this.buttonGetChannels.TabIndex = 108;
            this.buttonGetChannels.Text = "Get Channels";
            this.buttonGetChannels.UseVisualStyleBackColor = true;
            this.buttonGetChannels.Click += new System.EventHandler(this.buttonGetChannels_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBoxPassword);
            this.groupBox1.Controls.Add(this.textBoxUserName);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(262, 410);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(434, 83);
            this.groupBox1.TabIndex = 124;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Security";
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.textBoxPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxPassword.Location = new System.Drawing.Point(172, 51);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.PasswordChar = '*';
            this.textBoxPassword.Size = new System.Drawing.Size(228, 26);
            this.textBoxPassword.TabIndex = 108;
            // 
            // textBoxUserName
            // 
            this.textBoxUserName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.textBoxUserName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxUserName.Location = new System.Drawing.Point(172, 19);
            this.textBoxUserName.Name = "textBoxUserName";
            this.textBoxUserName.Size = new System.Drawing.Size(228, 26);
            this.textBoxUserName.TabIndex = 107;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.ForeColor = System.Drawing.Color.Black;
            this.label10.Location = new System.Drawing.Point(77, 22);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(89, 20);
            this.label10.TabIndex = 106;
            this.label10.Text = "User Name";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.Color.Black;
            this.label9.Location = new System.Drawing.Point(77, 54);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(78, 20);
            this.label9.TabIndex = 105;
            this.label9.Text = "Password";
            // 
            // textBoxResponse
            // 
            this.textBoxResponse.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.textBoxResponse.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxResponse.Location = new System.Drawing.Point(148, 222);
            this.textBoxResponse.Name = "textBoxResponse";
            this.textBoxResponse.Size = new System.Drawing.Size(548, 26);
            this.textBoxResponse.TabIndex = 130;
            // 
            // labelResponse
            // 
            this.labelResponse.AutoSize = true;
            this.labelResponse.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelResponse.ForeColor = System.Drawing.Color.Black;
            this.labelResponse.Location = new System.Drawing.Point(19, 225);
            this.labelResponse.Name = "labelResponse";
            this.labelResponse.Size = new System.Drawing.Size(82, 20);
            this.labelResponse.TabIndex = 129;
            this.labelResponse.Text = "Response";
            // 
            // textBoxReasonPhrase
            // 
            this.textBoxReasonPhrase.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.textBoxReasonPhrase.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxReasonPhrase.Location = new System.Drawing.Point(148, 141);
            this.textBoxReasonPhrase.Multiline = true;
            this.textBoxReasonPhrase.Name = "textBoxReasonPhrase";
            this.textBoxReasonPhrase.Size = new System.Drawing.Size(548, 66);
            this.textBoxReasonPhrase.TabIndex = 128;
            // 
            // textBoxStatusCode
            // 
            this.textBoxStatusCode.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.textBoxStatusCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxStatusCode.Location = new System.Drawing.Point(148, 102);
            this.textBoxStatusCode.Name = "textBoxStatusCode";
            this.textBoxStatusCode.Size = new System.Drawing.Size(69, 26);
            this.textBoxStatusCode.TabIndex = 127;
            // 
            // labelReasonPhrase
            // 
            this.labelReasonPhrase.AutoSize = true;
            this.labelReasonPhrase.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelReasonPhrase.ForeColor = System.Drawing.Color.Black;
            this.labelReasonPhrase.Location = new System.Drawing.Point(19, 141);
            this.labelReasonPhrase.Name = "labelReasonPhrase";
            this.labelReasonPhrase.Size = new System.Drawing.Size(115, 20);
            this.labelReasonPhrase.TabIndex = 126;
            this.labelReasonPhrase.Text = "ReasonPhrase";
            // 
            // labelStatusCode
            // 
            this.labelStatusCode.AutoSize = true;
            this.labelStatusCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelStatusCode.ForeColor = System.Drawing.Color.Black;
            this.labelStatusCode.Location = new System.Drawing.Point(19, 105);
            this.labelStatusCode.Name = "labelStatusCode";
            this.labelStatusCode.Size = new System.Drawing.Size(98, 20);
            this.labelStatusCode.TabIndex = 125;
            this.labelStatusCode.Text = "Status Code";
            // 
            // dataGridViewChannels
            // 
            this.dataGridViewChannels.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridViewChannels.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewChannels.Location = new System.Drawing.Point(721, 28);
            this.dataGridViewChannels.Name = "dataGridViewChannels";
            this.dataGridViewChannels.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewChannels.Size = new System.Drawing.Size(511, 541);
            this.dataGridViewChannels.TabIndex = 131;
            this.dataGridViewChannels.SelectionChanged += new System.EventHandler(this.dataGridViewChannels_SelectionChanged);
            // 
            // buttonGetChannel
            // 
            this.buttonGetChannel.Location = new System.Drawing.Point(557, 528);
            this.buttonGetChannel.Name = "buttonGetChannel";
            this.buttonGetChannel.Size = new System.Drawing.Size(103, 41);
            this.buttonGetChannel.TabIndex = 132;
            this.buttonGetChannel.Text = "Get Channel";
            this.buttonGetChannel.UseVisualStyleBackColor = true;
            this.buttonGetChannel.Click += new System.EventHandler(this.buttonGetChannel_Click);
            // 
            // textBoxChannelId
            // 
            this.textBoxChannelId.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.textBoxChannelId.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxChannelId.Location = new System.Drawing.Point(148, 63);
            this.textBoxChannelId.Name = "textBoxChannelId";
            this.textBoxChannelId.Size = new System.Drawing.Size(548, 26);
            this.textBoxChannelId.TabIndex = 134;
            // 
            // labelChannelID
            // 
            this.labelChannelID.AutoSize = true;
            this.labelChannelID.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelChannelID.ForeColor = System.Drawing.Color.Black;
            this.labelChannelID.Location = new System.Drawing.Point(19, 66);
            this.labelChannelID.Name = "labelChannelID";
            this.labelChannelID.Size = new System.Drawing.Size(89, 20);
            this.labelChannelID.TabIndex = 133;
            this.labelChannelID.Text = "Channel ID";
            // 
            // buttonAddSecurityTokens
            // 
            this.buttonAddSecurityTokens.Location = new System.Drawing.Point(232, 528);
            this.buttonAddSecurityTokens.Name = "buttonAddSecurityTokens";
            this.buttonAddSecurityTokens.Size = new System.Drawing.Size(102, 41);
            this.buttonAddSecurityTokens.TabIndex = 135;
            this.buttonAddSecurityTokens.Text = "Add Tokens";
            this.buttonAddSecurityTokens.UseVisualStyleBackColor = true;
            this.buttonAddSecurityTokens.Click += new System.EventHandler(this.buttonAddSecurityTokens_Click);
            // 
            // buttonRemoveSecurityTokens
            // 
            this.buttonRemoveSecurityTokens.Location = new System.Drawing.Point(340, 528);
            this.buttonRemoveSecurityTokens.Name = "buttonRemoveSecurityTokens";
            this.buttonRemoveSecurityTokens.Size = new System.Drawing.Size(102, 41);
            this.buttonRemoveSecurityTokens.TabIndex = 136;
            this.buttonRemoveSecurityTokens.Text = "Remove Tokens";
            this.buttonRemoveSecurityTokens.UseVisualStyleBackColor = true;
            this.buttonRemoveSecurityTokens.Click += new System.EventHandler(this.buttonRemoveSecurityTokens_Click);
            // 
            // buttonDeleteChannel
            // 
            this.buttonDeleteChannel.Location = new System.Drawing.Point(123, 528);
            this.buttonDeleteChannel.Name = "buttonDeleteChannel";
            this.buttonDeleteChannel.Size = new System.Drawing.Size(103, 41);
            this.buttonDeleteChannel.TabIndex = 137;
            this.buttonDeleteChannel.Text = "Delete Channel";
            this.buttonDeleteChannel.UseVisualStyleBackColor = true;
            this.buttonDeleteChannel.Click += new System.EventHandler(this.buttonDeleteChannel_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.RosyBrown;
            this.ClientSize = new System.Drawing.Size(1254, 591);
            this.Controls.Add(this.buttonDeleteChannel);
            this.Controls.Add(this.buttonRemoveSecurityTokens);
            this.Controls.Add(this.buttonAddSecurityTokens);
            this.Controls.Add(this.textBoxChannelId);
            this.Controls.Add(this.labelChannelID);
            this.Controls.Add(this.buttonGetChannel);
            this.Controls.Add(this.dataGridViewChannels);
            this.Controls.Add(this.textBoxResponse);
            this.Controls.Add(this.labelResponse);
            this.Controls.Add(this.textBoxReasonPhrase);
            this.Controls.Add(this.textBoxStatusCode);
            this.Controls.Add(this.labelReasonPhrase);
            this.Controls.Add(this.labelStatusCode);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonGetChannels);
            this.Controls.Add(this.buttonCreateChannel);
            this.Controls.Add(this.textBoxHostName);
            this.Controls.Add(this.labelHostName);
            this.Name = "FormMain";
            this.Text = "Channel Management Test";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewChannels)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxHostName;
        private System.Windows.Forms.Label labelHostName;
        private System.Windows.Forms.Button buttonCreateChannel;
        private System.Windows.Forms.Button buttonGetChannels;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.TextBox textBoxUserName;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBoxResponse;
        private System.Windows.Forms.Label labelResponse;
        private System.Windows.Forms.TextBox textBoxReasonPhrase;
        private System.Windows.Forms.TextBox textBoxStatusCode;
        private System.Windows.Forms.Label labelReasonPhrase;
        private System.Windows.Forms.Label labelStatusCode;
        private System.Windows.Forms.DataGridView dataGridViewChannels;
        private System.Windows.Forms.Button buttonGetChannel;
        private System.Windows.Forms.TextBox textBoxChannelId;
        private System.Windows.Forms.Label labelChannelID;
        private System.Windows.Forms.Button buttonAddSecurityTokens;
        private System.Windows.Forms.Button buttonRemoveSecurityTokens;
        private System.Windows.Forms.Button buttonDeleteChannel;
    }
}

