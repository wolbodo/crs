using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using CashlessRegisterSystemCore;
using CashlessRegisterSystemCore.Helpers;
using CashlessRegisterSystemCore.Model;
using CashlessRegisterSystemCore.Tasks;
using Timer = System.Threading.Timer;

namespace ViltjesSysteem
{
    public partial class GUI : Form
    {
        private Timer synchronizeTransactionsTimer;
        private int number;
        private int decimals = -1;
        private int memory;
        private bool plus;
        private bool times;
        private Label nameClickSource;
        private Transaction cancelTransaction;
        private System.Timers.Timer checkTimer = new System.Timers.Timer();
        private static TimeSpan idleThreshold = TimeSpan.FromMinutes(3);

        public delegate void MessageEventHandler(MessageEventArgs message);
        public static EventHandler dataChange;
        public static MessageEventHandler messageNotice;

        private TransactionList transactionList;
        private MemberList memberList;
        private TransferList transferList;

        public GUI()
        {
            InitializeComponent();

            memberList = new MemberList(false, Environment.CurrentDirectory, watch:true);
            transactionList = TransactionList.LoadFromFile();
            transferList = TransferList.LoadAndWatchFromFile();
            memberList.ClearAndAddTransfers(transferList.All);
            memberList.ClearAndAddTransactions(transactionList.All);
            memberList.dataChange += UpdateGUI;
            memberList.messageNotice += MessageNotice;
            transactionList.dataChange += UpdateGUI;
            transactionList.messageNotice += MessageNotice;

            dataChange += UpdateGUI;
            messageNotice += MessageNotice;
           // int initOrder = Member.All.Count + Transaction.All.Count + Transfer.All.Count;
            checkTimer.Elapsed += TimedCheck;
            checkTimer.Interval = 30000;
            checkTimer.Enabled = true;
        }

        private void MainScreen_Load(object sender, EventArgs e)
        {
            this.Location = Screen.AllScreens.Count() == 1 || !Screen.AllScreens[0].Primary ?
                                new Point(Screen.AllScreens[0].Bounds.X, Screen.AllScreens[0].Bounds.Y) :
                                new Point(Screen.AllScreens[1].Bounds.X, Screen.AllScreens[1].Bounds.Y);
            //Cursor.Hide();
            synchronizeTransactionsTimer = new Timer(OnSynchronizeTransactions, null, 1000, 60*1000);

            Cursor.Position = new Point(this.Location.X + this.Width / 2, this.Location.Y + this.Height / 2);

            LastNames();
            FillHistory();
            names_select.BringToFront();
            message_overlay.BringToFront();
            correct_overlay.BringToFront();
        }

        private bool _writingTransaction = false;
        private DateTime lastSuccesfullSync = DateTime.MinValue;

        private void OnSynchronizeTransactions(object state)
        {
            // make sure all remote files are copies of the 
            if (!_writingTransaction)
            {
                var errorMessage = SynchronizeFiles.Execute(Settings.LocalTransactionsPath, Settings.RemoteTransactionsPath);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    lastSuccesfullSync = DateTime.Now;
                }
                GenerateMonthTransactionsExcel.Execute();
            }

        }

        private static Color GetColor(Label lbl)
        {
            switch ((String)lbl.Tag)
            {
                case "Blue":
                    return Color.Blue;
                case "Green":
                    return System.Drawing.Color.FromArgb(0, 192, 0);
                case "Yellow":
                    return Color.Yellow;
                case "Red":
                    return Color.Red;
                default:
                    return Color.Navy;
            }
        }
                
        private void UpdateGUI(object sender, EventArgs args)
        {
            Invoke((MethodInvoker)delegate
            {
                //TODO, no updates instead just Refresh();
                LastNames();
                if (nameClickSource != null)
                {
                    Names_Click(nameClickSource, null);
                }
            });
            Thread.Sleep(1000);
            Invoke((MethodInvoker)delegate
            {
                message_overlay.Visible = false;
            });
        }

        private void MessageNotice(MessageEventArgs message)
        {
            Invoke((MethodInvoker)delegate
            {
                this.message.Text = message.Message;
                ignore.Visible = true;
                switch(message.Type)
                {
                    case MessageType.Service:
                        this.message.BackColor = Color.Gray;
                        this.message.ForeColor = Color.White;
                        ignore.Visible = false;
                         break;
                    case MessageType.Info:
                         this.message.BackColor = Color.Blue;
                         this.message.ForeColor = Color.White;
                         break;
                    case MessageType.Warning:
                        this.message.BackColor = Color.Yellow;
                        this.message.ForeColor = Color.Black;
                        break;
                    default:
                    case MessageType.FatalError:
                        this.message.BackColor = Color.Red;
                        this.message.ForeColor = Color.White;
                        break;
                }
                message_overlay.Visible = true;
            });
        }

        private void Ignore_Click(object sender, EventArgs e)
        {
            message_overlay.Visible = false;
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                this.Location = (Screen.AllScreens.Count() == 1 || this.Location.X == Screen.AllScreens[1].Bounds.X) ?
                                    new Point(Screen.AllScreens[0].Bounds.X, Screen.AllScreens[0].Bounds.Y):
                                    new Point(Screen.AllScreens[1].Bounds.X, Screen.AllScreens[1].Bounds.Y);
                Cursor.Position = new Point(this.Location.X + this.Width / 2, this.Location.Y + this.Height / 2);
            }
        }

        void TimedCheck(object sender, EventArgs e)
        {
            if (User32Interop.GetLastInput() > idleThreshold)
            {
                Invoke((MethodInvoker)delegate
                {
                    KeypadClear_Click(sender, e);
                    NameClear_Click(sender, e);
                    Cursor.Position = new Point(this.Location.X + this.Width / 2, this.Location.Y + this.Height / 2);
                });
            }
            foreach (TransactionLabel tl in history_transactions.Controls.OfType<TransactionLabel>())
            {
                tl.ReCheckCancelable();
            }
        }

        private void LastNames()
        {
            HashSet<Member> last = new HashSet<Member>();

            for (int i = transactionList.All.Count - 1; last.Count < 6 && i >= 0; i--)
            {
                Member member;
                if (memberList.FromName.TryGetValue(transactionList.All[i].MemberName, out member))
                {
                    last.Add(member);
                }
            }

            names_last.Controls.Clear();

            foreach (Member member in last)
            {
                try
                {
                    names_last.Controls.Add(GenerateMemberLabel(member, false));
                }
                catch { }
            }
        }

        private Label GenerateMemberLabel(Member member, bool listView)
        {
            var memberLabel = new MemberLabel(member);
            memberLabel.Size = new Size(listView ? 240 : 260, listView ? 60 : 80);
            memberLabel.Margin = listView ? new Padding(5) : new Padding(0, 0, 0, 10);
            memberLabel.Click += Member_Click;
            memberLabel.MouseDown += Feedback_MouseDown;
            memberLabel.MouseUp += Feedback_MouseUp;
            return memberLabel;
        }

        private void Member_Click(object sender, EventArgs e)
        {
            var member = ((MemberLabel)sender).Member;
            names_select.Visible = false;
            names_back.Visible = false;
            clear_name.Visible = true;
            paying_member.Text = member.Name;
            nameClickSource = null;
        }
        
        private void Names_Click(object sender, EventArgs e)
        {
            nameClickSource = (Label)sender;
            names_select.Controls.Clear();
            char start = char.ToUpper(nameClickSource.Text[0]);
            char end = char.ToUpper(nameClickSource.Text[nameClickSource.Text.Count()-1]);
            int i = 0;
            //Char.ToLower was needed because of ugly input
            while (i < memberList.All.Count && Char.ToLower(memberList.All.Keys[i][0]) < Char.ToLower(start)) i++;
            while (i < memberList.All.Count && Char.ToLower(memberList.All.Keys[i][0]) >= Char.ToLower(start) && Char.ToLower(memberList.All.Keys[i][0]) <= Char.ToLower(end))
            {
                try
                {
                    names_select.Controls.Add(GenerateMemberLabel(memberList.All.Values[i], true));
                }
                catch { }
                i++;
            }
            names_select.Visible = true;
            names_back.Visible = true;
            names_back.BringToFront();
        }

        private void FillHistory()
        {
            history_transactions.Controls.Clear();
            DateTime lastDate = new DateTime();
            for (int i = transactionList.All.Count - 1; i > 0 && i > transactionList.All.Count - 100; i--)
            {
                try
                {
                    Transaction transaction = transactionList.All[i];

                    if (lastDate.Date != transaction.TransactionDate.Date)
                    {
                        Label historyDateLabel = new Label();
                        historyDateLabel.Text = transaction.TransactionDate.ToString("dd-MM-yyyy:");
                        historyDateLabel.Font = new Font("Segoe UI", 16F);
                        historyDateLabel.BackColor = System.Drawing.Color.Black;
                        historyDateLabel.ForeColor = System.Drawing.Color.White;
                        historyDateLabel.Size = new Size(280, 50);
                        historyDateLabel.Margin = new Padding(0, 5, 0, 5);
                        historyDateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                        historyDateLabel.MouseDown += HistoryTransactions_MouseDown;
                        historyDateLabel.MouseMove += HistoryTransactions_MouseMove;
                        lastDate = transaction.TransactionDate;
                        history_transactions.Controls.Add(historyDateLabel);
                    }

                    TransactionLabel historyNameLabel = new TransactionLabel(transaction, transactionList);
                    historyNameLabel.MouseDown += HistoryTransactions_MouseDown;
                    historyNameLabel.MouseUp += HistoryTransactions_MouseUp;
                    historyNameLabel.MouseMove += HistoryTransactions_MouseMove;
                    historyNameLabel.Click += HistoryTransactions_Click;

                    history_transactions.Controls.Add(historyNameLabel);
                }
                catch { }
            }
        }

        private void Calc()
        {
            if (plus)
            {
                number += memory;
            }
            else if (times)
            {
                number = (int)(number * memory / 100.0);
            }
            memory = 0;
            CheckMax();
            decimals = number == 0 ? -1 : 2;
            times = false;
            plus = false;
        }

        private void Keypad_Click(object sender, EventArgs e)
        {
            bool error = false;
            Label src = (Label) sender;
            switch (src.Text)
            {
                case "ok":
                    int amount = number;
                    if (paying_member.Text != "" && amount != 0)
                    {
                        _writingTransaction = true;
                        transactionList.New(amount, paying_member.Text, memberList);
                        _writingTransaction = false;
                        KeypadClear_Click(sender, e);
                        NameClear_Click(sender, e);
                        LastNames();
                        FillHistory();
                    }
                    else
                    {
                        error = true;
                    }
                    break;
                case "=":
                    Calc();
                    keypad_ok.Text = "ok";
                    break;
                case "+":
                case "×":
                    if (number != 0)
                    {
                        Calc();
                    }
                    else
                    {
                        error = true;
                        break;
                    }
                    plus = (src.Text == "+");
                    times = !plus;
                    decimals = -1;
                    keypad_ok.Text = "=";
                    break;
                case ".":
                    if (decimals == -1)
                    {
                        decimals = 0;
                    }
                    else
                    {
                        error = true;
                    }
                    break;
                default:
                    clear_amount.Visible = true;
                    if (decimals == 2)
                    {
                        error = true;
                        // fuck you
                    }
                    else if (decimals > -1)
                    {
                        if (plus || times)
                        {
                            memory += int.Parse(src.Text)*(int) Math.Pow(10, 2 - ++decimals);
                            CheckMax();
                        }
                        else
                        {
                            number += int.Parse(src.Text)*(int) Math.Pow(10, 2 - ++decimals);
                            CheckMax();
                        }
                    }
                    else
                    {
                        if (plus || times)
                        {
                            memory = memory*10 + int.Parse(src.Text)*100;
                            CheckMax();
                        }
                        else
                        {
                            number = number*10 + int.Parse(src.Text)*100;
                            CheckMax();
                        }
                    }
                    break;
            }
            RenderDisplay();
            if (error)
                new Thread(BlinkErrorLabel).Start(src);
        }

        private void Feedback_MouseDown(object sender, MouseEventArgs e)
        {
            Label lbl = (Label)sender;
            lbl.BackColor = Color.Red;
            if (GetColor(lbl) == Color.Yellow)
                lbl.ForeColor = Color.White;
            if (GetColor(lbl) == Color.Red)
                lbl.BackColor = Color.DarkRed;
        }

        private void Feedback_MouseUp(object sender, MouseEventArgs e)
        {
            Label lbl = (Label)sender;
            lbl.BackColor = GetColor(lbl);
            if (GetColor(lbl) == Color.Yellow)
                lbl.ForeColor = Color.Red;
        }
                
        private void CheckMax()
        {
            if (memory > 25000)//> € 250.00
            {
                memory = 25000;
                new Thread(BlinkTopLabel).Start(display);
            }
            if (number > 25000)
            {
                number = 25000;
                new Thread(BlinkTopLabel).Start(display);
            }
        }

        private void BlinkErrorLabel(object obj)
        {
            Label lbl = (Label)obj;
            Color originalColor = lbl.BackColor;
            while (originalColor == Color.Red)
            {
                Thread.Sleep(10);
                originalColor = lbl.BackColor;
            }
            for (int i = 0; i < 3; i++)
            {
                Invoke((MethodInvoker)delegate
                {
                    lbl.BackColor = Color.Red;
                });
                Thread.Sleep(50);
                Invoke((MethodInvoker)delegate
                {
                    lbl.BackColor = originalColor;
                });
                Thread.Sleep(50);
            }
        }

        private void RenderDisplay()
        {
            display.Text = string.Format("€ {0:0.00}", number / 100.0);
            if (plus || times)
            {
                if (plus)
                    display.Text += " + € ";
                else if (times)
                    display.Text += " × ";
                display.Text += string.Format("{0:0.00}", memory / 100.0);
            }
        }

        private void NameClear_Click(object sender, EventArgs e)
        {
            paying_member.Text = "";
            clear_name.Visible = false;
            new Thread(BlinkTopLabel).Start(paying_member);
        }

        private void KeypadClear_Click(object sender, EventArgs e)
        {
            memory = 0;
            number = 0;
            decimals = -1;
            plus = false;
            times = false;
            clear_amount.Visible = false;
            RenderDisplay();
            new Thread(BlinkTopLabel).Start(display);
        }

        private void BlinkTopLabel(object obj)
        {
            Label label = (Label)obj;
            Invoke((MethodInvoker)delegate
            {
                label.BackColor = Color.Red;
            });
            Thread.Sleep(250);
            Invoke((MethodInvoker)delegate
            {
                label.BackColor = Color.Yellow;
            });
        }

        private int lastY = 0, lastValue = 0;
        private void HistoryTransactions_MouseMove(object sender, MouseEventArgs e)
        {
            //history_transactions.AutoScrollMinSize = new Size(0, 0);
            if (e.Button == MouseButtons.Left)
            {
                history_transactions.VerticalScroll.Value = Math.Max(history_transactions.VerticalScroll.Minimum, Math.Min(history_transactions.VerticalScroll.Maximum, lastValue + (lastY - Cursor.Position.Y)));
                //Trace.WriteLine("MOVE Scroll: " + lastValue + " Mouse: " + lastY + " Now: " + Cursor.Position.Y + " Result: " + (lastValue + (lastY - Cursor.Position.Y)));
            }
        }

        private void HistoryTransactions_MouseDown(object sender, MouseEventArgs e)
        {
            lastValue = history_transactions.VerticalScroll.Value;
            lastY = Cursor.Position.Y;
            TransactionLabel tl = sender as TransactionLabel;
            if (tl != null)
            {
                tl.BackColor = tl.isCancelable ? Color.Red : Color.Gray;
            }
        }

        private void HistoryTransactions_MouseUp(object sender, MouseEventArgs e)
        {
            TransactionLabel tl = (TransactionLabel)sender;
            tl.BackColor = Color.FromArgb(255, 32, 32, 32);
        }

        private void HistoryTransactions_Click(object sender, EventArgs e)
        {
            TransactionLabel tl = (TransactionLabel)sender;
            if (tl.isCancelable)
            {
                cancelTransaction = tl.Transaction;
                correct.Text = string.Format("Corrigeren van deze transactie:\r\n{0}?\r\n\r\nN.B. O.a. penningmeester controleert gecorrigeerde transacties.", cancelTransaction);
                correct_overlay.Visible = true;
            }
            /*else
            {
                MessageNotice(new MessageEventArgs{Level = Type.Info, Message = string.Format("Transactie:\r\n\r\n{0}\r\n\r\n", tl.Transaction)});
            }*/
        }
        
        private void NamesStop_Click(object sender, EventArgs e)
        {
            names_select.Visible = false;
            names_back.Visible = false;
        }

        private void correct_ok_Click(object sender, EventArgs e)
        {
            transactionList.Cancel(cancelTransaction, memberList);
            KeypadClear_Click(sender, e);
            NameClear_Click(sender, e);
            LastNames();
            FillHistory();
            correct_overlay.Visible = false;
        }

        private void correct_cancel_Click(object sender, EventArgs e)
        {
            correct_overlay.Visible = false;
        }
    }

    public class MemberLabel : Label
    {
        private StringFormat stringFormat;
        private Font balanceFont;
        private StringFormat balanceStringFormat;
        private SolidBrush solidBrush;

        public Member Member { get; private set; }

        public MemberLabel(Member member)
        {
            stringFormat = new StringFormat();
            stringFormat.LineAlignment = StringAlignment.Center;
            stringFormat.Alignment = StringAlignment.Center;
            Font = new Font("Segoe UI", 28F);
            BackColor = Color.Navy;
            ForeColor = Color.White;
            solidBrush = new SolidBrush(this.ForeColor);
            balanceFont = new Font("Segoe UI", 14F);
            balanceStringFormat = new StringFormat();
            balanceStringFormat.LineAlignment = StringAlignment.Far;
            balanceStringFormat.Alignment = StringAlignment.Far;
            this.Member = member;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.DrawString(Member.Name, Font, solidBrush, ClientRectangle, stringFormat);
            if (Member.Name=="Zklant")
                e.Graphics.DrawString(string.Format("€ {0:0.00}", Member.CurrentBalanceAmountInCents / 100.0), balanceFont, BackColor == Color.Red ? solidBrush : Member.CurrentBalanceAmountInCents < 0 && Member.Payment == Member.PaymentMethod.PREPAID ? new SolidBrush(Color.Red) : solidBrush, ClientRectangle, balanceStringFormat);
        }
    }

    public class TransactionLabel : Label
    {
        private StringFormat beginNameStringFormat;
        private StringFormat endStringFormat;
        private RectangleF paddedTextLayoutRectangle;
        private RectangleF paddedLayoutRectangle;
        private SolidBrush solidBrush;
        private SolidBrush redSolidBrush;
        private static TimeSpan cancelTime = TimeSpan.FromMinutes(15);
        public bool isCancelable = false;

        public Transaction Transaction {get; private set;}
        public TransactionList TransactionList { get; private set; }

        public TransactionLabel(Transaction transaction, TransactionList transactionList)
        {
            Transaction = transaction;
            TransactionList = transactionList;
            beginNameStringFormat = new StringFormat();
            beginNameStringFormat.LineAlignment = StringAlignment.Center;
            beginNameStringFormat.Alignment = StringAlignment.Near;
            Font = new Font("Segoe UI", 22F);
            BackColor = Color.FromArgb(255, 32, 32, 32);
            ForeColor = Color.White;
            solidBrush = new SolidBrush(this.ForeColor);
            endStringFormat = new StringFormat();
            endStringFormat.LineAlignment = StringAlignment.Center;
            endStringFormat.Alignment = StringAlignment.Far;
            Margin = new Padding(0, 5, 0, 5);
            Size = new Size(334, 50);
            paddedTextLayoutRectangle = new RectangleF(Location.X + 10, Location.Y, Width - 20, Height);
            isCancelable = IsCancelable();
            if (isCancelable || transaction.AmountInCents < 0)
            {
                Size = new Size(374, 50);
                paddedLayoutRectangle = new RectangleF(Location.X + 10, Location.Y, Width - 20, Height);
                redSolidBrush = new SolidBrush(Color.Red);
            }
        }

        public void ReCheckCancelable()
        {
            if (isCancelable && !IsCancelable())
            {
                isCancelable = false;
                this.Invoke((MethodInvoker)delegate
                {
                    Size = new Size(334, 50);
                });
            }
        }

        private bool IsCancelable()
        {
            Transaction correctedTransaction;
            return Transaction.AmountInCents > 0
                && DateTime.Now - Transaction.TransactionDate < cancelTime
                && !(TransactionList.Corrected.TryGetValue(Transaction.TransactionDate, out correctedTransaction)
                    && correctedTransaction.MemberName == Transaction.MemberName
                    && -correctedTransaction.AmountInCents == Transaction.AmountInCents);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.DrawString(Transaction.MemberName, Font, solidBrush, paddedTextLayoutRectangle, beginNameStringFormat);
            e.Graphics.DrawString(string.Format("{0:0.00}", Transaction.AmountInCents / 100.0), Font, Transaction.AmountInCents > 0 ? solidBrush : redSolidBrush, paddedTextLayoutRectangle, endStringFormat);
            if (Transaction.AmountInCents < 0)
            {
                e.Graphics.DrawString("cr", Font, solidBrush, paddedLayoutRectangle, endStringFormat);
            }
            else if (isCancelable)
            {
                e.Graphics.DrawString("×", Font, BackColor == Color.Red ? solidBrush : redSolidBrush, paddedLayoutRectangle, endStringFormat);
            }
        }
    }

    public class DescendedDateComparer : IComparer<DateTime>
    {
        public int Compare(DateTime x, DateTime y)
        {
            return -Comparer<DateTime>.Default.Compare(x, y);
        }
    }

    //public enum MessageType
    //{
    //    Service, Info, Warning, FatalError
    //}

    //public class MessageEventArgs : EventArgs
    //{
    //    public string Message { get; set; }
    //    public MessageType Type { get; set; }
    //}

    //See http://stackoverflow.com/a/745227
    //For mono: no idea yet!
    public static class User32Interop
    {
        public static TimeSpan GetLastInput()
        {
            var plii = new LASTINPUTINFO();
            plii.cbSize = (uint)Marshal.SizeOf(plii);

            if (GetLastInputInfo(ref plii))
                return TimeSpan.FromMilliseconds(Environment.TickCount - plii.dwTime);
            else
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        struct LASTINPUTINFO
        {
            public uint cbSize;
            public uint dwTime;
        }
    }
}
