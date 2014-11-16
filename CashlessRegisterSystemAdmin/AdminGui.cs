using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using CashlessRegisterSystemCore;
using CashlessRegisterSystemCore.Model;
using CashlessRegisterSystemCore.Tasks;

namespace ViltjesSysteemAdmin
{
    public partial class AdminGui : Form
    {
        static MemberList members;
        private DateTime date;
        private string outputDir;

        public AdminGui()
        {
            InitializeComponent();
            dateTimePicker1.Value = DateTime.Now.AddMonths(-1);
            //date = DateTime.Now.AddMonths(-1);
            tbOutputDir.Text = Environment.CurrentDirectory;
            outputDir = tbOutputDir.Text;
            UpdateDirs();
        }

        private void UpdateDirs()
        {
            if (File.Exists(Path.Combine(outputDir, Settings.MembersFile)))
            {
                members = new MemberList(true, outputDir);
            }

            var files = Directory.GetFiles(outputDir, "238417*");
            if (files.Length > 0) SetBankTransferFile(files[0]);

            var bonnetjeFile = "bonnetjes " + date.Year + "-" + date.Month + ".xlsx";
            SetBonnetjesFile(Path.Combine(outputDir, bonnetjeFile));

            var bankFile = Path.Combine(outputDir, "gefilterdebanktransfers.xlsx");
            SetBankFile(bankFile);

            var filesIncasso = Directory.GetFiles(outputDir, "Incassobatch*");
            if (filesIncasso.Length > 0) SetIncassoFile(files[0]);
        }

        private void Status(string text)
        {
            lblStatus2.Text = text;
        }

        private void AdminGui_Load(object sender, EventArgs e)
        {
        }

        private void btGenerateTransferExcelClick(object sender, EventArgs e)
        {
            GenerateBankTransfersExcel(tbBankUnprocessedFile.Text);
        }

        private void btGenerateTransferFilesClick(object sender, EventArgs e)
        {
            GenerateTransferFiles(date.Month, date.Year);
        }

        private void btGenerate_Click(object sender, EventArgs e)
        {
            GenerateBalanceExcel(date.Month, date.Year);
        }

        private void GenerateBankTransfersExcel(string file)
        {
            if (!File.Exists(file))
            {
                MessageBox.Show("Bank transactie bestand bestaat niet", "Bank transactie bestand bestaat niet", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Status("Bank transacties filteren...");
            btGenerateTransfer.Enabled = false;
            var lines = ProcessBankTransfers.ReadBankTransferLines(file);
            var transfers = ProcessBankTransfers.ParseBankTransferLines(lines);
            List<BankTransfer> filtered;
            var memberTransfers = ProcessBankTransfers.FilterAssociateMembers(transfers, members.AllList, out filtered);
            string fileName = Path.Combine(outputDir, "gefilterdebanktransfers.xlsx");
            CashlessRegisterSystemCore.Tasks.GenerateBankTransfersExcel.Generate(fileName, memberTransfers, filtered);
            btGenerateTransfer.Enabled = true;
            Status("'gefilterdebanktransfers.xlsx' aangemaakt.");
        }

        private void GenerateTransferFiles(int month, int year)
        {
            try
            {
                Status("Transfer bestand aanmaken...");
                var bonnetjeFile = tbBonnetjesFile.Text;
                if (!File.Exists(bonnetjeFile))
                {
                    MessageBox.Show("Bonnetjes bestand bestaat niet", "Bonnetjes bestand bestaat niet",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                var bankFile = tbBankFile.Text;
                if (!File.Exists(bankFile))
                {
                    MessageBox.Show("Gefilterde bank transacties bestand bestaat niet",
                                    "Gefilterde bank transacties bestand bestaat niet", MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                    return;
                }
                var incassoFile = tbIncassoFile.Text;
                if (!File.Exists(incassoFile))
                {
                    MessageBox.Show("Incasso bestand bestaat niet", "Incasso bestand bestaat niet", MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                    return;
                }

                btGenerateTransfer.Enabled = false;
                var bonnetjesTransfers = ProcessManualTransfers.ReadBonnetjesTransfers(bonnetjeFile, members.AllList,
                                                                                       month, year);
                var bankTransfers = ProcessBankTransfers.ReadBankTransfers(bankFile, members.AllList, month, year);
                var incassoTransfers = ProcessIncassoTransfers.ParseIncassoTransferLines(incassoFile, members.AllList,
                                                                                         month, year);
                var transfers = new List<Transfer>();
                transfers.AddRange(bonnetjesTransfers);
                transfers.AddRange(bankTransfers);
                transfers.AddRange(incassoTransfers);
                GenerateTransfers.WriteTransferFiles(outputDir, members.AllList, transfers);
                btGenerateTransfer.Enabled = true;
                Status("Transfer bestand aangemaakt.");
            }
            catch (Exception e)
            {
                btGenerateTransfer.Enabled = true;
                Status(string.Empty);
                MessageBox.Show("Fout: " + e.Message + Environment.NewLine + e.StackTrace, "Fout met genereren transacties", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        private void GenerateBalanceExcel(int month, int year)
        {

            try
            {
                btGenerate.Enabled = false;
                Status("Excel aan het genereren...");
                var lastMonthBalance = tbLastMonthBalance.Text;
                if (!File.Exists(lastMonthBalance))
                {
                    MessageBox.Show("Balans van vorige maand bestand bestaat niet", "Balans bestaat niet",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                var balance = GenerateMonthBalances.Generate(month, year, members.AllList, outputDir, lastMonthBalance);
                GenerateMonthBalances.WriteMonthBalanceFile(outputDir, balance);

                string fileName = Path.Combine(outputDir, "Schuldenlijst " + year + "-" + month + ".xlsx");
                GenerateMonthBalanceExcel.Generate(fileName, balance);

                // update members txt
                GenerateMonthBalances.UpdateMembersBalance(balance);
                string backupFile = Path.Combine(outputDir, "members " + balance.Year + "-" + balance.Month + ".txt");
                string membersFile = Path.Combine(outputDir, Settings.MembersFile);
                if (File.Exists(membersFile))
                {
                    if (File.Exists(backupFile)) File.Delete(backupFile);
                    File.Move(membersFile, backupFile);
                }
                File.WriteAllText(membersFile, members.CreateCsv());

                Status("Excel balans aangemaakt");
                btGenerate.Enabled = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + Environment.NewLine + e.StackTrace);
                Status(string.Empty);
                btGenerate.Enabled = true;
                MessageBox.Show("Fout: " + e.Message + Environment.NewLine + e.StackTrace, "Fout met genereren excel balans", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void btBrowseBankTransfers_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.FileName =  Path.GetFileName(tbBankUnprocessedFile.Text);
            dialog.InitialDirectory = Directory.Exists(tbBonnetjesFile.Text) ? Path.GetDirectoryName(tbBankUnprocessedFile.Text) : string.Empty;
            dialog.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
            dialog.Title = "Kies een csv bestand met de bank transacties";
            if (dialog.ShowDialog() != DialogResult.OK) return;
            //var filename = Path.GetFileName(dialog.FileName);
            //var path = Path.GetDirectoryName(dialog.FileName);
            //tbBankUnprocessedFile.Text = dialog.FileName;
            SetBankTransferFile(dialog.FileName);
        }

        private void SetBankTransferFile(string file)
        {
            tbBankUnprocessedFile.Text = file;
            tbBankUnprocessedFile.SelectionStart = tbBankUnprocessedFile.Text.Length;
            tbBankUnprocessedFile.ScrollToCaret();
        }

        private void btBrowseBonnetjesFile_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.FileName = Path.GetFileName(tbBonnetjesFile.Text);
            dialog.InitialDirectory = Directory.Exists(tbBonnetjesFile.Text) ? Path.GetDirectoryName(tbBonnetjesFile.Text) : string.Empty;
            dialog.Filter = "xlsx files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            dialog.Title = "Kies een xlsx bestand met de bonnetjes";
            if (dialog.ShowDialog() != DialogResult.OK) return;
            SetBonnetjesFile(dialog.FileName);
        }
        
        private void SetBonnetjesFile(string file)
        {
            tbBonnetjesFile.Text = file;
            tbBonnetjesFile.SelectionStart = tbBonnetjesFile.Text.Length;
            tbBonnetjesFile.ScrollToCaret();
        }

        private void btBrowseBankFile_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.FileName = Path.GetFileName(tbBankFile.Text);
            dialog.InitialDirectory = Directory.Exists(tbBonnetjesFile.Text) ? Path.GetDirectoryName(tbBankFile.Text) : string.Empty;
            dialog.Filter = "xlsx files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            dialog.Title = "Kies een xlsx bestand met de gefilterde bank transacties";
            if (dialog.ShowDialog() != DialogResult.OK) return;
            SetBankFile(dialog.FileName);
        }

        private void SetBankFile(string file)
        {
            tbBankFile.Text = file;
            tbBankFile.SelectionStart = tbBankFile.Text.Length;
            tbBankFile.ScrollToCaret();
        }

        private void btBrowseIncassoFile_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.FileName = Path.GetFileName(tbIncassoFile.Text);
            dialog.InitialDirectory = Directory.Exists(tbBonnetjesFile.Text) ? Path.GetDirectoryName(tbIncassoFile.Text) : string.Empty;
            dialog.Filter = "cli files (*.cli)|*.cli|All files (*.*)|*.*";
            dialog.Title = "Kies een xlsx bestand met de incasso's";
            if (dialog.ShowDialog() != DialogResult.OK) return;
            SetIncassoFile(dialog.FileName);
        }

        private void SetIncassoFile(string file)
        {
            tbIncassoFile.Text = file;
            tbIncassoFile.SelectionStart = tbIncassoFile.Text.Length;
            tbIncassoFile.ScrollToCaret();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            date = dateTimePicker1.Value;
        }

        private void btOutputDir_Click(object sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            //dialog.FileName = Path.GetFileName(tbOutputDir.Text);
            dialog.SelectedPath = Directory.Exists(tbOutputDir.Text) ? Path.GetDirectoryName(tbOutputDir.Text) : string.Empty;
            dialog.ShowNewFolderButton = true;// Filter = "cli files (*.cli)|*.cli|All files (*.*)|*.*";
            dialog.Description = "Kies een folder voor de output";
            if (dialog.ShowDialog() != DialogResult.OK) return;
            tbOutputDir.Text = dialog.SelectedPath;
            outputDir = tbOutputDir.Text;
            UpdateDirs();
        }

        private void btLastMonthBalance_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.FileName = Path.GetFileName(tbBankUnprocessedFile.Text);
            dialog.InitialDirectory = Directory.Exists(tbBonnetjesFile.Text) ? Path.GetDirectoryName(tbBankUnprocessedFile.Text) : string.Empty;
            dialog.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
            dialog.Title = "Kies een csv bestand met de bank transacties";
            if (dialog.ShowDialog() != DialogResult.OK) return;
            tbLastMonthBalance.Text = dialog.FileName;
        }
    }
}
