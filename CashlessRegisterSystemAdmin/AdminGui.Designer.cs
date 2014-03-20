namespace ViltjesSysteemAdmin
{
    partial class AdminGui
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
            this.btGenerate = new System.Windows.Forms.Button();
            this.btFilterTransfers = new System.Windows.Forms.Button();
            this.btGenerateTransfer = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.tbBankUnprocessedFile = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btBrowseBankTransfers = new System.Windows.Forms.Button();
            this.lblStatus2 = new System.Windows.Forms.Label();
            this.btBrowseBonnetjesFile = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.tbBonnetjesFile = new System.Windows.Forms.TextBox();
            this.btBrowseBankFile = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.tbBankFile = new System.Windows.Forms.TextBox();
            this.btBrowseIncassoFile = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.tbIncassoFile = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.btOutputDir = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.tbOutputDir = new System.Windows.Forms.TextBox();
            this.btLastMonthBalance = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.tbLastMonthBalance = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btGenerate
            // 
            this.btGenerate.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btGenerate.Location = new System.Drawing.Point(12, 554);
            this.btGenerate.Name = "btGenerate";
            this.btGenerate.Size = new System.Drawing.Size(623, 60);
            this.btGenerate.TabIndex = 0;
            this.btGenerate.Text = "8) : Genereer schuldenlijst";
            this.btGenerate.UseVisualStyleBackColor = true;
            this.btGenerate.Click += new System.EventHandler(this.btGenerate_Click);
            // 
            // btFilterTransfers
            // 
            this.btFilterTransfers.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btFilterTransfers.Location = new System.Drawing.Point(6, 162);
            this.btFilterTransfers.Name = "btFilterTransfers";
            this.btFilterTransfers.Size = new System.Drawing.Size(623, 60);
            this.btFilterTransfers.TabIndex = 1;
            this.btFilterTransfers.Text = "2) : Filter bank transacties";
            this.btFilterTransfers.UseVisualStyleBackColor = true;
            this.btFilterTransfers.Click += new System.EventHandler(this.btGenerateTransferExcelClick);
            // 
            // btGenerateTransfer
            // 
            this.btGenerateTransfer.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btGenerateTransfer.Location = new System.Drawing.Point(12, 385);
            this.btGenerateTransfer.Name = "btGenerateTransfer";
            this.btGenerateTransfer.Size = new System.Drawing.Size(623, 60);
            this.btGenerateTransfer.TabIndex = 2;
            this.btGenerateTransfer.Text = "5) : Genereer transfer bestanden";
            this.btGenerateTransfer.UseVisualStyleBackColor = true;
            this.btGenerateTransfer.Click += new System.EventHandler(this.btGenerateTransferFilesClick);
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(22, 100);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(593, 28);
            this.textBox1.TabIndex = 3;
            this.textBox1.Text = "1) : Download de giro bestanden van de maand (\'238417_*.cv\') naar deze directory\r" +
    "\n";
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(18, 77);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(146, 20);
            this.label1.TabIndex = 4;
            this.label1.Text = "Voorbereidingen:";
            // 
            // textBox2
            // 
            this.textBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox2.Location = new System.Drawing.Point(22, 243);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(595, 57);
            this.textBox2.TabIndex = 5;
            this.textBox2.Text = "3) : Controleer de gegenereerde bank transactie Excel (\'gefilterdebanktransfers.x" +
    "lsx\')\r\n4) : Sluit de bestanden!";
            // 
            // textBox3
            // 
            this.textBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox3.Location = new System.Drawing.Point(20, 451);
            this.textBox3.Multiline = true;
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(597, 57);
            this.textBox3.TabIndex = 6;
            this.textBox3.Text = "6) : Controleer de gegenereerde transactie bestanden (\'transfers-<jaar>-<maand>.c" +
    "sv\')\r\n7) : Sluit de bestanden!";
            // 
            // tbBankUnprocessedFile
            // 
            this.tbBankUnprocessedFile.Location = new System.Drawing.Point(101, 136);
            this.tbBankUnprocessedFile.Name = "tbBankUnprocessedFile";
            this.tbBankUnprocessedFile.Size = new System.Drawing.Size(435, 20);
            this.tbBankUnprocessedFile.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 139);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Bank bestand:";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // btBrowseBankTransfers
            // 
            this.btBrowseBankTransfers.Location = new System.Drawing.Point(542, 134);
            this.btBrowseBankTransfers.Name = "btBrowseBankTransfers";
            this.btBrowseBankTransfers.Size = new System.Drawing.Size(75, 23);
            this.btBrowseBankTransfers.TabIndex = 10;
            this.btBrowseBankTransfers.Text = "Browse";
            this.btBrowseBankTransfers.UseVisualStyleBackColor = true;
            this.btBrowseBankTransfers.Click += new System.EventHandler(this.btBrowseBankTransfers_Click);
            // 
            // lblStatus2
            // 
            this.lblStatus2.AutoSize = true;
            this.lblStatus2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblStatus2.Location = new System.Drawing.Point(0, 603);
            this.lblStatus2.Name = "lblStatus2";
            this.lblStatus2.Padding = new System.Windows.Forms.Padding(5, 5, 0, 0);
            this.lblStatus2.Size = new System.Drawing.Size(5, 18);
            this.lblStatus2.TabIndex = 11;
            // 
            // btBrowseBonnetjesFile
            // 
            this.btBrowseBonnetjesFile.Location = new System.Drawing.Point(540, 304);
            this.btBrowseBonnetjesFile.Name = "btBrowseBonnetjesFile";
            this.btBrowseBonnetjesFile.Size = new System.Drawing.Size(75, 23);
            this.btBrowseBonnetjesFile.TabIndex = 14;
            this.btBrowseBonnetjesFile.Text = "Browse";
            this.btBrowseBonnetjesFile.UseVisualStyleBackColor = true;
            this.btBrowseBonnetjesFile.Click += new System.EventHandler(this.btBrowseBonnetjesFile_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 309);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(98, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Bonnetjes bestand:";
            // 
            // tbBonnetjesFile
            // 
            this.tbBonnetjesFile.Location = new System.Drawing.Point(121, 306);
            this.tbBonnetjesFile.Name = "tbBonnetjesFile";
            this.tbBonnetjesFile.Size = new System.Drawing.Size(415, 20);
            this.tbBonnetjesFile.TabIndex = 12;
            // 
            // btBrowseBankFile
            // 
            this.btBrowseBankFile.Location = new System.Drawing.Point(540, 330);
            this.btBrowseBankFile.Name = "btBrowseBankFile";
            this.btBrowseBankFile.Size = new System.Drawing.Size(75, 23);
            this.btBrowseBankFile.TabIndex = 17;
            this.btBrowseBankFile.Text = "Browse";
            this.btBrowseBankFile.UseVisualStyleBackColor = true;
            this.btBrowseBankFile.Click += new System.EventHandler(this.btBrowseBankFile_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 335);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(76, 13);
            this.label4.TabIndex = 16;
            this.label4.Text = "Bank bestand:";
            // 
            // tbBankFile
            // 
            this.tbBankFile.Location = new System.Drawing.Point(121, 332);
            this.tbBankFile.Name = "tbBankFile";
            this.tbBankFile.Size = new System.Drawing.Size(415, 20);
            this.tbBankFile.TabIndex = 15;
            // 
            // btBrowseIncassoFile
            // 
            this.btBrowseIncassoFile.Location = new System.Drawing.Point(540, 356);
            this.btBrowseIncassoFile.Name = "btBrowseIncassoFile";
            this.btBrowseIncassoFile.Size = new System.Drawing.Size(75, 23);
            this.btBrowseIncassoFile.TabIndex = 20;
            this.btBrowseIncassoFile.Text = "Browse";
            this.btBrowseIncassoFile.UseVisualStyleBackColor = true;
            this.btBrowseIncassoFile.Click += new System.EventHandler(this.btBrowseIncassoFile_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(17, 361);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(88, 13);
            this.label5.TabIndex = 19;
            this.label5.Text = "Incasso bestand:";
            // 
            // tbIncassoFile
            // 
            this.tbIncassoFile.Location = new System.Drawing.Point(121, 358);
            this.tbIncassoFile.Name = "tbIncassoFile";
            this.tbIncassoFile.Size = new System.Drawing.Size(415, 20);
            this.tbIncassoFile.TabIndex = 18;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(19, 19);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(43, 13);
            this.label6.TabIndex = 22;
            this.label6.Text = "Maand:";
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.CustomFormat = "MMMM-yyyy";
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker1.Location = new System.Drawing.Point(76, 19);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(200, 20);
            this.dateTimePicker1.TabIndex = 23;
            this.dateTimePicker1.ValueChanged += new System.EventHandler(this.dateTimePicker1_ValueChanged);
            // 
            // btOutputDir
            // 
            this.btOutputDir.Location = new System.Drawing.Point(542, 47);
            this.btOutputDir.Name = "btOutputDir";
            this.btOutputDir.Size = new System.Drawing.Size(75, 23);
            this.btOutputDir.TabIndex = 26;
            this.btOutputDir.Text = "Browse";
            this.btOutputDir.UseVisualStyleBackColor = true;
            this.btOutputDir.Click += new System.EventHandler(this.btOutputDir_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(19, 52);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(87, 13);
            this.label7.TabIndex = 25;
            this.label7.Text = "Uitvoer directory:";
            // 
            // tbOutputDir
            // 
            this.tbOutputDir.Location = new System.Drawing.Point(112, 49);
            this.tbOutputDir.Name = "tbOutputDir";
            this.tbOutputDir.Size = new System.Drawing.Size(424, 20);
            this.tbOutputDir.TabIndex = 24;
            // 
            // btLastMonthBalance
            // 
            this.btLastMonthBalance.Location = new System.Drawing.Point(540, 526);
            this.btLastMonthBalance.Name = "btLastMonthBalance";
            this.btLastMonthBalance.Size = new System.Drawing.Size(75, 23);
            this.btLastMonthBalance.TabIndex = 29;
            this.btLastMonthBalance.Text = "Browse";
            this.btLastMonthBalance.UseVisualStyleBackColor = true;
            this.btLastMonthBalance.Click += new System.EventHandler(this.btLastMonthBalance_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(17, 531);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(109, 13);
            this.label8.TabIndex = 28;
            this.label8.Text = "Balans vorige maand:";
            // 
            // tbLastMonthBalance
            // 
            this.tbLastMonthBalance.Location = new System.Drawing.Point(132, 528);
            this.tbLastMonthBalance.Name = "tbLastMonthBalance";
            this.tbLastMonthBalance.Size = new System.Drawing.Size(404, 20);
            this.tbLastMonthBalance.TabIndex = 27;
            // 
            // AdminGui
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(652, 621);
            this.Controls.Add(this.btLastMonthBalance);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.tbLastMonthBalance);
            this.Controls.Add(this.btOutputDir);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.tbOutputDir);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btBrowseIncassoFile);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.tbIncassoFile);
            this.Controls.Add(this.btBrowseBankFile);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbBankFile);
            this.Controls.Add(this.btBrowseBonnetjesFile);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbBonnetjesFile);
            this.Controls.Add(this.lblStatus2);
            this.Controls.Add(this.btBrowseBankTransfers);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbBankUnprocessedFile);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.btGenerateTransfer);
            this.Controls.Add(this.btFilterTransfers);
            this.Controls.Add(this.btGenerate);
            this.Name = "AdminGui";
            this.Text = "ViltjesSysteemAdmin";
            this.Load += new System.EventHandler(this.AdminGui_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btGenerate;
        private System.Windows.Forms.Button btFilterTransfers;
        private System.Windows.Forms.Button btGenerateTransfer;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox tbBankUnprocessedFile;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btBrowseBankTransfers;
        private System.Windows.Forms.Label lblStatus2;
        private System.Windows.Forms.Button btBrowseBonnetjesFile;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbBonnetjesFile;
        private System.Windows.Forms.Button btBrowseBankFile;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbBankFile;
        private System.Windows.Forms.Button btBrowseIncassoFile;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbIncassoFile;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Button btOutputDir;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbOutputDir;
        private System.Windows.Forms.Button btLastMonthBalance;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbLastMonthBalance;
    }
}

