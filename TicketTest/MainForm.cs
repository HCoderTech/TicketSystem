using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using Ticket.BL;
using Ticket.Data;

namespace TicketTest
{
    public partial class MainForm : Form
    {
        private TicketManager ticketManager=new TicketManager();

        public MainForm()
        {
            InitializeComponent();
            Utility.fitFormToScreen(this, 768, 1366);
            this.CenterToScreen();
            WindowState = FormWindowState.Maximized;
            setDefaults();
            setupDatabase();
        }

        private void setupDatabase()
        {
            if (!System.IO.File.Exists("ticketdata.sqlite"))
            {
                
                SQLiteConnection.CreateFile("ticketdata.sqlite");

                using (var sqlite = new SQLiteConnection("Data Source=ticketdata.sqlite"))
                {
                    sqlite.Open();
                    string sql = "CREATE TABLE IF NOT EXISTS TicketEntry (" +
                                 "TicketID VARCHAR PRIMARY KEY," +
                                    "TicketNo VARCHAR NOT NULL," +
                                    "Name VARCHAR NOT NULL," +
                                    "PhoneNumber VARCHAR NOT NULL," +
                                    "Email VARCHAR," +
                                    "Address VARCHAR," +
                                    "Status VARCHAR," +
                                    "Priority VARCHAR," +
                                    "Category VARCHAR," +
                                    "Resolver VARCHAR," +
                                    "Description VARCHAR," +
                                    "EntryDate DATE," +
                                    "DueDate DATE);";
                    SQLiteCommand command = new SQLiteCommand(sql, sqlite);
                    command.ExecuteNonQuery();
                }
            }
        }

        private void setDefaults()
        {
            txtName.Text = "";
            txtAddress.Text = "";
            txtEmail.Text = "";
            txtPhoneNumber.Text = "";
            txtDescription.Text = "";
            txtResolver.Text = "";
            txtTicketNo.Text = "";
            cbStatus.SelectedIndex = 0;
            cbPriority.SelectedIndex = 1;
            cbCategory.SelectedIndex = 0;
            dateEntry.Value = DateTime.Now.Date;
            dateDue.Value = DateTime.Now.Date.AddDays(7);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            TicketEntry ticket = getTicketDetails();
            ScenarioResult ValidationResult = ticket.IsValid();
            if (ValidationResult.Success)
            {
                ScenarioResult saveTicketResult=ticketManager.SaveTicket(ticket);
                if (saveTicketResult.Success)
                {
                    MessageBox.Show("Ticket Entry Saved!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    setDefaults();
                }
                else
                    MessageBox.Show("Save Operation Failed :" + saveTicketResult.ErrorMessage, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show(ValidationResult.ErrorMessage,"Input Error",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            }
        }

        private TicketEntry getTicketDetails()
        {
            TicketEntry ticketEntry = new TicketEntry();
            ticketEntry.Name = txtName.Text;
            ticketEntry.Address = txtAddress.Text;
            ticketEntry.PhoneNumber = txtPhoneNumber.Text;
            TicketPriority priority = TicketPriority.Normal;
            Enum.TryParse(cbPriority.Text, out priority);
            ticketEntry.Priority = priority;
            ticketEntry.Category = cbCategory.Text;
            ticketEntry.Description = txtDescription.Text;
            ticketEntry.EntryDate = dateEntry.Value.Date;
            ticketEntry.DueDate = dateDue.Value.Date;
            ticketEntry.Resolver = txtResolver.Text;
            ticketEntry.TicketNo = txtTicketNo.Text;
            ticketEntry.Email = txtEmail.Text;
            TicketStatus status = TicketStatus.New;
            Enum.TryParse(cbStatus.Text, out status);
            ticketEntry.Status = status;
            ticketEntry.TicketID = generateID();
            return ticketEntry;
        }

        private string generateID()
        {
            byte[] hash;
            using (MD5 md5 = MD5.Create())
            {
                hash = md5.ComputeHash(Encoding.UTF8.GetBytes(DateTime.Now.ToString()));
            }
            return Convert.ToBase64String(hash);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            setDefaults();
        }

       

       
        private void txtDataSearch_Click(object sender, EventArgs e)
        {
            List<TicketEntry> tickets=ticketManager.GetDataSearchResult(txtSearchTicketNo.Text,txtSearchName.Text,txtSearchPhone.Text);
            updateTicketsToDataGrid(tickets);
            
        }

        private void updateTicketsToDataGrid(List<TicketEntry> ticketEntries)
        {
            dataResults.Rows.Clear();

            foreach (TicketEntry ticket in ticketEntries)
            {
                var index = dataResults.Rows.Add();
                dataResults.Rows[index].Cells["dTicketNo"].Value = ticket.TicketNo;
                dataResults.Rows[index].Cells["dName"].Value = ticket.Name;
                dataResults.Rows[index].Cells["dPhone"].Value = ticket.PhoneNumber;
                dataResults.Rows[index].Cells["dEmail"].Value = ticket.Email;
                dataResults.Rows[index].Cells["dEntryDate"].Value = ticket.EntryDate.Date;
                dataResults.Rows[index].Cells["dStatus"].Value = ticket.Status.ToString();
                dataResults.Rows[index].Cells["dPriority"].Value = ticket.Priority.ToString();
                dataResults.Rows[index].Tag = ticket;

            }
        }

        private void dataResults_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            TicketEntry ticket = (TicketEntry)dataResults.Rows[e.RowIndex].Tag;
            if (ticket != null)
            {
                gbTicketDetails.Tag = ticket;
                txtUpdateTicketNo.Text = ticket.TicketNo;
                txtUpdateName.Text = ticket.Name;
                txtUpdateDescription.Text = ticket.Description;
                txtUpdateEmail.Text = ticket.Email;
                txtUpdateAddress.Text = ticket.Address;
                txtUpdatePhoneNumber.Text = ticket.PhoneNumber;
                txtUpdateResolver.Text = ticket.Resolver;
                cbUpdateStatus.Text = ticket.Status.ToString();
                cbUpdatePriority.Text = ticket.Priority.ToString();
                cbUpdateCategory.Text = ticket.Category.ToString();
                entryDate.Value = ticket.EntryDate.Date;
                dueDate.Value = ticket.DueDate.Date;

            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            TicketEntry originalEntry = (TicketEntry)gbTicketDetails.Tag;
            TicketEntry updatedEntry = getUpdatedEntry();
            ScenarioResult scenarioResult = ticketManager.UpdateTicketEntry(originalEntry,updatedEntry);

            if (scenarioResult.Success)
            {
                MessageBox.Show("Ticket Entry Updated!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                dataResults.CurrentRow.Tag = updatedEntry;

                dataResults.CurrentRow.Cells["dTicketNo"].Value = updatedEntry.TicketNo;
                dataResults.CurrentRow.Cells["dName"].Value = updatedEntry.Name;
                dataResults.CurrentRow.Cells["dPhone"].Value = updatedEntry.PhoneNumber;
                dataResults.CurrentRow.Cells["dEmail"].Value = updatedEntry.Email;
                dataResults.CurrentRow.Cells["dEntryDate"].Value = updatedEntry.EntryDate.Date;
                dataResults.CurrentRow.Cells["dStatus"].Value = updatedEntry.Status.ToString();
                dataResults.CurrentRow.Cells["dPriority"].Value = updatedEntry.Priority.ToString();
            }
            else
                MessageBox.Show("Update Operation Failed :" + scenarioResult.ErrorMessage, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private TicketEntry getUpdatedEntry()
        {
            TicketEntry ticketEntry = new TicketEntry();
            ticketEntry.Name = txtUpdateName.Text;
            ticketEntry.Address = txtUpdateAddress.Text;
            ticketEntry.PhoneNumber = txtUpdatePhoneNumber.Text;
            TicketPriority priority = TicketPriority.Normal;
            Enum.TryParse(cbUpdatePriority.Text, out priority);
            ticketEntry.Priority = priority;
            ticketEntry.Category = cbUpdateCategory.Text;
            ticketEntry.Description = txtUpdateDescription.Text;
            ticketEntry.EntryDate = entryDate.Value.Date;
            ticketEntry.DueDate = dueDate.Value.Date;
            ticketEntry.Resolver = txtUpdateResolver.Text;
            ticketEntry.TicketNo = txtUpdateTicketNo.Text;
            ticketEntry.Email = txtUpdateEmail.Text;
            TicketStatus status = TicketStatus.New;
            Enum.TryParse(cbUpdateStatus.Text, out status);
            ticketEntry.Status = status;
            return ticketEntry;
        }

        private void btnTimeSearch_Click(object sender, EventArgs e)
        {
            List<TicketEntry> tickets = ticketManager.GetTimeSearchResult(dateSearchStart.Value.Date,dateSearchEnd.Value.Date);
            updateTicketsToDataGrid(tickets);
        }
    }
}
