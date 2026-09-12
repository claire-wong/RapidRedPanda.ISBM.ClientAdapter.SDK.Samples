namespace ISBM20ChannelManagementTestCSharp
{
    partial class FormAddChannel
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
            this.dataGridViewSecurityToken = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonAddToken = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.textBoxUserName = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.buttonDeleteToken = new System.Windows.Forms.Button();
            this.buttonCreateChannel = new System.Windows.Forms.Button();
            this.textBoxChannelType = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSecurityToken)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxChannelId
            // 
            this.textBoxChannelId.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.textBoxChannelId.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxChannelId.Location = new System.Drawing.Point(155, 24);
            this.textBoxChannelId.Name = "textBoxChannelId";
            this.textBoxChannelId.Size = new System.Drawing.Size(548, 26);
            this.textBoxChannelId.TabIndex = 136;
            // 
            // labelChannelID
            // 
            this.labelChannelID.AutoSize = true;
            this.labelChannelID.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelChannelID.ForeColor = System.Drawing.Color.Black;
            this.labelChannelID.Location = new System.Drawing.Point(26, 27);
            this.labelChannelID.Name = "labelChannelID";
            this.labelChannelID.Size = new System.Drawing.Size(89, 20);
            this.labelChannelID.TabIndex = 135;
            this.labelChannelID.Text = "Channel ID";
            // 
            // dataGridViewSecurityToken
            // 
            this.dataGridViewSecurityToken.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.dataGridViewSecurityToken.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewSecurityToken.Location = new System.Drawing.Point(155, 170);
            this.dataGridViewSecurityToken.Name = "dataGridViewSecurityToken";
            this.dataGridViewSecurityToken.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewSecurityToken.Size = new System.Drawing.Size(434, 167);
            this.dataGridViewSecurityToken.TabIndex = 137;
            this.dataGridViewSecurityToken.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewSecurityToken_CellContentClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(26, 170);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 20);
            this.label1.TabIndex = 138;
            this.label1.Text = "Security Token";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // buttonAddToken
            // 
            this.buttonAddToken.Location = new System.Drawing.Point(604, 353);
            this.buttonAddToken.Name = "buttonAddToken";
            this.buttonAddToken.Size = new System.Drawing.Size(99, 32);
            this.buttonAddToken.TabIndex = 139;
            this.buttonAddToken.Text = "Add Token";
            this.buttonAddToken.UseVisualStyleBackColor = true;
            this.buttonAddToken.Click += new System.EventHandler(this.buttonAddToken_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBoxPassword);
            this.groupBox1.Controls.Add(this.textBoxUserName);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(155, 343);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(434, 83);
            this.groupBox1.TabIndex = 140;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Security";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
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
            // buttonDeleteToken
            // 
            this.buttonDeleteToken.Location = new System.Drawing.Point(604, 391);
            this.buttonDeleteToken.Name = "buttonDeleteToken";
            this.buttonDeleteToken.Size = new System.Drawing.Size(99, 32);
            this.buttonDeleteToken.TabIndex = 141;
            this.buttonDeleteToken.Text = "Delete Token";
            this.buttonDeleteToken.UseVisualStyleBackColor = true;
            this.buttonDeleteToken.Click += new System.EventHandler(this.buttonDeleteToken_Click);
            // 
            // buttonCreateChannel
            // 
            this.buttonCreateChannel.Location = new System.Drawing.Point(339, 444);
            this.buttonCreateChannel.Name = "buttonCreateChannel";
            this.buttonCreateChannel.Size = new System.Drawing.Size(103, 41);
            this.buttonCreateChannel.TabIndex = 142;
            this.buttonCreateChannel.Text = "Create Channel";
            this.buttonCreateChannel.UseVisualStyleBackColor = true;
            this.buttonCreateChannel.Click += new System.EventHandler(this.buttonCreateChannel_Click);
            // 
            // textBoxChannelType
            // 
            this.textBoxChannelType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.textBoxChannelType.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxChannelType.Location = new System.Drawing.Point(155, 63);
            this.textBoxChannelType.Name = "textBoxChannelType";
            this.textBoxChannelType.Size = new System.Drawing.Size(135, 26);
            this.textBoxChannelType.TabIndex = 144;
            this.textBoxChannelType.TabStop = false;
            this.textBoxChannelType.Text = "Publication";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(26, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 20);
            this.label2.TabIndex = 143;
            this.label2.Text = "Channel Type";
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.textBoxDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxDescription.Location = new System.Drawing.Point(155, 100);
            this.textBoxDescription.Multiline = true;
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxDescription.Size = new System.Drawing.Size(548, 58);
            this.textBoxDescription.TabIndex = 146;
            this.textBoxDescription.TabStop = false;
            this.textBoxDescription.TextChanged += new System.EventHandler(this.textBoxDescription_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(26, 103);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 20);
            this.label3.TabIndex = 145;
            this.label3.Text = "Description";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // FormAddChannel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.RosyBrown;
            this.ClientSize = new System.Drawing.Size(730, 497);
            this.Controls.Add(this.textBoxDescription);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxChannelType);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonCreateChannel);
            this.Controls.Add(this.buttonDeleteToken);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonAddToken);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dataGridViewSecurityToken);
            this.Controls.Add(this.textBoxChannelId);
            this.Controls.Add(this.labelChannelID);
            this.Name = "FormAddChannel";
            this.Text = "Add Channel";
            this.Load += new System.EventHandler(this.FormAddChannel_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSecurityToken)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxChannelId;
        private System.Windows.Forms.Label labelChannelID;
        private System.Windows.Forms.DataGridView dataGridViewSecurityToken;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonAddToken;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.TextBox textBoxUserName;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button buttonDeleteToken;
        private System.Windows.Forms.Button buttonCreateChannel;
        private System.Windows.Forms.TextBox textBoxChannelType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxDescription;
        private System.Windows.Forms.Label label3;
    }
}