namespace ISBM20ChannelManagementTestCSharp
{
    partial class FormAddRemoveSecurityTokens
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
            this.textBoxChannelId = new System.Windows.Forms.TextBox();
            this.labelChannelID = new System.Windows.Forms.Label();
            this.buttonAddRemoveSecurityTokens = new System.Windows.Forms.Button();
            this.buttonDeleteToken = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.textBoxUserName = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.buttonAddToken = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.dataGridViewSecurityToken = new System.Windows.Forms.DataGridView();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSecurityToken)).BeginInit();
            this.SuspendLayout();
            // 
            // textBoxChannelId
            // 
            this.textBoxChannelId.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.textBoxChannelId.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxChannelId.Location = new System.Drawing.Point(151, 26);
            this.textBoxChannelId.Name = "textBoxChannelId";
            this.textBoxChannelId.Size = new System.Drawing.Size(548, 26);
            this.textBoxChannelId.TabIndex = 138;
            // 
            // labelChannelID
            // 
            this.labelChannelID.AutoSize = true;
            this.labelChannelID.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelChannelID.ForeColor = System.Drawing.Color.Black;
            this.labelChannelID.Location = new System.Drawing.Point(22, 29);
            this.labelChannelID.Name = "labelChannelID";
            this.labelChannelID.Size = new System.Drawing.Size(89, 20);
            this.labelChannelID.TabIndex = 137;
            this.labelChannelID.Text = "Channel ID";
            // 
            // buttonAddRemoveSecurityTokens
            // 
            this.buttonAddRemoveSecurityTokens.Location = new System.Drawing.Point(335, 341);
            this.buttonAddRemoveSecurityTokens.Name = "buttonAddRemoveSecurityTokens";
            this.buttonAddRemoveSecurityTokens.Size = new System.Drawing.Size(103, 41);
            this.buttonAddRemoveSecurityTokens.TabIndex = 152;
            this.buttonAddRemoveSecurityTokens.Text = "Add Tokens";
            this.buttonAddRemoveSecurityTokens.UseVisualStyleBackColor = true;
            this.buttonAddRemoveSecurityTokens.Click += new System.EventHandler(this.buttonAddRemoveSecurityTokens_Click);
            // 
            // buttonDeleteToken
            // 
            this.buttonDeleteToken.Location = new System.Drawing.Point(600, 288);
            this.buttonDeleteToken.Name = "buttonDeleteToken";
            this.buttonDeleteToken.Size = new System.Drawing.Size(99, 32);
            this.buttonDeleteToken.TabIndex = 151;
            this.buttonDeleteToken.Text = "Remove from list";
            this.buttonDeleteToken.UseVisualStyleBackColor = true;
            this.buttonDeleteToken.Click += new System.EventHandler(this.buttonDeleteToken_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBoxPassword);
            this.groupBox1.Controls.Add(this.textBoxUserName);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(151, 240);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(434, 83);
            this.groupBox1.TabIndex = 150;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Security";
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.textBoxPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxPassword.Location = new System.Drawing.Point(172, 51);
            this.textBoxPassword.Name = "textBoxPassword";
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
            // buttonAddToken
            // 
            this.buttonAddToken.Location = new System.Drawing.Point(600, 250);
            this.buttonAddToken.Name = "buttonAddToken";
            this.buttonAddToken.Size = new System.Drawing.Size(99, 32);
            this.buttonAddToken.TabIndex = 149;
            this.buttonAddToken.Text = "Add to list";
            this.buttonAddToken.UseVisualStyleBackColor = true;
            this.buttonAddToken.Click += new System.EventHandler(this.buttonAddToken_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(22, 67);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 20);
            this.label1.TabIndex = 148;
            this.label1.Text = "Security Token";
            // 
            // dataGridViewSecurityToken
            // 
            this.dataGridViewSecurityToken.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.dataGridViewSecurityToken.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewSecurityToken.Location = new System.Drawing.Point(151, 67);
            this.dataGridViewSecurityToken.Name = "dataGridViewSecurityToken";
            this.dataGridViewSecurityToken.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewSecurityToken.Size = new System.Drawing.Size(434, 167);
            this.dataGridViewSecurityToken.TabIndex = 147;
            // 
            // FormAddRemoveSecurityTokens
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.RosyBrown;
            this.ClientSize = new System.Drawing.Size(717, 403);
            this.Controls.Add(this.buttonAddRemoveSecurityTokens);
            this.Controls.Add(this.buttonDeleteToken);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonAddToken);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dataGridViewSecurityToken);
            this.Controls.Add(this.textBoxChannelId);
            this.Controls.Add(this.labelChannelID);
            this.Name = "FormAddRemoveSecurityTokens";
            this.Text = "Add Security Tokens";
            this.Load += new System.EventHandler(this.FormAddRemoveSecurityTokens_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSecurityToken)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxChannelId;
        private System.Windows.Forms.Label labelChannelID;
        private System.Windows.Forms.Button buttonAddRemoveSecurityTokens;
        private System.Windows.Forms.Button buttonDeleteToken;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.TextBox textBoxUserName;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button buttonAddToken;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dataGridViewSecurityToken;
    }
}