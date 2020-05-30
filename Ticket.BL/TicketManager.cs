using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Ticket.Data;

namespace Ticket.BL
{
    public class TicketManager
    {
        public ScenarioResult SaveTicket(TicketEntry ticket)
        {
            return Create(ticket);
        }

        private ScenarioResult Create(TicketEntry ticket)
        {
            using (SQLiteConnection sqlconn = new SQLiteConnection("Data Source=ticketdata.sqlite"))
            {
                sqlconn.Open();
                ScenarioResult result = new ScenarioResult();
                SQLiteCommand insertSQL = new SQLiteCommand("INSERT INTO TicketEntry (TicketID, TicketNo, Name, PhoneNumber, Email, Address, Status, Priority, Category, Resolver, Description,EntryDate,DueDate)" +
                    " VALUES (@ticketID,@ticketNo,@Name,@phoneNumber,@email,@address,@status,@priority,@category,@resolver,@description,@entryDate,@dueDate)", sqlconn);
                insertSQL.Parameters.AddWithValue("@ticketID", ticket.TicketID);
                insertSQL.Parameters.AddWithValue("@ticketNo", ticket.TicketNo);
                insertSQL.Parameters.AddWithValue("@Name", ticket.Name);
                insertSQL.Parameters.AddWithValue("@phoneNumber", ticket.PhoneNumber);
                insertSQL.Parameters.AddWithValue("@email", ticket.Email);
                insertSQL.Parameters.AddWithValue("@address", ticket.Address);
                insertSQL.Parameters.AddWithValue("@status", ticket.Status);
                insertSQL.Parameters.AddWithValue("@priority", ticket.Priority);
                insertSQL.Parameters.AddWithValue("@category", ticket.Category);
                insertSQL.Parameters.AddWithValue("@resolver", ticket.Resolver);
                insertSQL.Parameters.AddWithValue("@description", ticket.Description);
                insertSQL.Parameters.AddWithValue("@entryDate", ticket.EntryDate);
                insertSQL.Parameters.AddWithValue("@dueDate", ticket.DueDate);
                try
                {
                    insertSQL.ExecuteNonQuery();
                    result.Success = true;
                }
                catch (Exception ex)
                {
                    result.ErrorMessage = ex.Message;
                }
                return result;
            }
        }

        public List<TicketEntry> GetDataSearchResult(string txtSearchTicketNo, string txtSearchName, string txtSearchPhone)
        {
            string finalQuery =  generateQuery(txtSearchTicketNo, txtSearchName, txtSearchPhone);
            List<TicketEntry> ticketEntries = new List<TicketEntry>();
            using (SQLiteConnection sqlconn = new SQLiteConnection("Data Source=ticketdata.sqlite"))
            {
                sqlconn.Open();
                finalQuery = finalQuery.Replace("@ticketNo", txtSearchTicketNo);
                finalQuery = finalQuery.Replace("@Name", txtSearchName);
                finalQuery = finalQuery.Replace("@phoneNumber", txtSearchPhone);
                SQLiteCommand searchQuery = new SQLiteCommand(finalQuery,sqlconn);
               
               
                try
                {
                    SQLiteDataReader reader = searchQuery.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            TicketEntry ticketEntry = new TicketEntry();
                            ticketEntry.TicketID = reader["TicketID"].ToString();
                            ticketEntry.TicketNo = reader["TicketNo"].ToString();
                            ticketEntry.Name = reader["Name"].ToString();
                            ticketEntry.PhoneNumber = reader["PhoneNumber"].ToString();
                            ticketEntry.Email = reader["Email"].ToString();
                            ticketEntry.Address = reader["Address"].ToString();
                            ticketEntry.Description = reader["Description"].ToString();
                            ticketEntry.Category = reader["Category"].ToString();
                            Enum.TryParse(reader["Status"].ToString(), out TicketStatus status);
                            ticketEntry.Status = status;
                            Enum.TryParse(reader["Priority"].ToString(), out TicketPriority priority);
                            ticketEntry.Priority = priority;
                            ticketEntry.Resolver = reader["Resolver"].ToString();
                            DateTime.TryParse(reader["EntryDate"].ToString(), out DateTime entryDate);
                            ticketEntry.EntryDate = entryDate;
                            DateTime.TryParse(reader["DueDate"].ToString(), out DateTime dueDate);
                            ticketEntry.DueDate = dueDate;
                            ticketEntries.Add(ticketEntry);
                        }
                    }
                }
                catch (Exception ex)
                {
                    
                }

                sqlconn.Close();
            }

            return ticketEntries;
           
        }

        private bool isNotEmpty(string text)
        {
            return !string.IsNullOrEmpty(text) || !string.IsNullOrWhiteSpace(text);
        }

        string AddCondition(string clause, string appender, string condition)
        {
            if (clause.Length <= 0)
            {
                return String.Format("WHERE {0}", condition);
            }
            return string.Format("{0} {1} {2}", clause, appender, condition);
        }


        private string generateQuery(string txtSearchTicketNo, string txtSearchName, string txtSearchPhone)
        {
            string sqlquery = @"Select * from TicketEntry";

            string whereClause = string.Empty;

            if (isNotEmpty(txtSearchTicketNo))
                whereClause = AddCondition(whereClause, "AND", @"TicketNo like '%@ticketNo%'");

            if (isNotEmpty(txtSearchName))
                whereClause = AddCondition(whereClause, "AND", @"Name like '%@Name%'");

            if (isNotEmpty(txtSearchPhone))
                whereClause = AddCondition(whereClause, "AND", @"PhoneNumber like '%@phoneNumber%'");

            string finalQuery = string.Format("{0} {1}", sqlquery, whereClause);

            return finalQuery;
        }

        public ScenarioResult UpdateTicketEntry(TicketEntry original,TicketEntry updated)
        {
            using (SQLiteConnection sqlconn = new SQLiteConnection("Data Source=ticketdata.sqlite"))
            {
                sqlconn.Open();
                ScenarioResult result = new ScenarioResult();
                SQLiteCommand updateSQL = new SQLiteCommand("Update TicketEntry Set TicketNo=@ticketNo" +
                    ", Name=@Name , PhoneNumber=@phoneNumber, Email=@email, Address=@address, Status=@status, " +
                    "Priority=@priority, Category=@category, Resolver=@resolver, Description=@description" +
                    ",EntryDate=@entryDate,DueDate=@dueDate where TicketID=@ticketID", sqlconn);
                updateSQL.Parameters.AddWithValue("@ticketID", original.TicketID);
                updateSQL.Parameters.AddWithValue("@ticketNo", updated.TicketNo);
                updateSQL.Parameters.AddWithValue("@Name", updated.Name);
                updateSQL.Parameters.AddWithValue("@phoneNumber", updated.PhoneNumber);
                updateSQL.Parameters.AddWithValue("@email", updated.Email);
                updateSQL.Parameters.AddWithValue("@address", updated.Address);
                updateSQL.Parameters.AddWithValue("@status", updated.Status);
                updateSQL.Parameters.AddWithValue("@priority", updated.Priority);
                updateSQL.Parameters.AddWithValue("@category", updated.Category);
                updateSQL.Parameters.AddWithValue("@resolver", updated.Resolver);
                updateSQL.Parameters.AddWithValue("@description", updated.Description);
                updateSQL.Parameters.AddWithValue("@entryDate", updated.EntryDate);
                updateSQL.Parameters.AddWithValue("@dueDate", updated.DueDate);
                try
                {
                    updateSQL.ExecuteNonQuery();
                    result.Success = true;
                }
                catch (Exception ex)
                {
                    result.ErrorMessage = ex.Message;
                }
                return result;
            }
        }

        public List<TicketEntry> GetTimeSearchResult(DateTime startDate, DateTime endDate)
        {
            string finalQuery = "Select * from TicketEntry where EntryDate >= '" + startDate.Date.ToString("yyyy-MM-dd") + "' and EntryDate <= '" + endDate.Date.AddDays(1).ToString("yyyy-MM-dd") + "'";

            List<TicketEntry> ticketEntries = new List<TicketEntry>();
            using (SQLiteConnection sqlconn = new SQLiteConnection("Data Source=ticketdata.sqlite"))
            {
                sqlconn.Open();
              
                SQLiteCommand searchQuery = new SQLiteCommand(finalQuery, sqlconn);


                try
                {
                    SQLiteDataReader reader = searchQuery.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            TicketEntry ticketEntry = new TicketEntry();
                            ticketEntry.TicketID = reader["TicketID"].ToString();
                            ticketEntry.TicketNo = reader["TicketNo"].ToString();
                            ticketEntry.Name = reader["Name"].ToString();
                            ticketEntry.PhoneNumber = reader["PhoneNumber"].ToString();
                            ticketEntry.Email = reader["Email"].ToString();
                            ticketEntry.Address = reader["Address"].ToString();
                            ticketEntry.Description = reader["Description"].ToString();
                            ticketEntry.Category = reader["Category"].ToString();
                            Enum.TryParse(reader["Status"].ToString(), out TicketStatus status);
                            ticketEntry.Status = status;
                            Enum.TryParse(reader["Priority"].ToString(), out TicketPriority priority);
                            ticketEntry.Priority = priority;
                            ticketEntry.Resolver = reader["Resolver"].ToString();
                            DateTime.TryParse(reader["EntryDate"].ToString(), out DateTime entryDate);
                            ticketEntry.EntryDate = entryDate;
                            DateTime.TryParse(reader["DueDate"].ToString(), out DateTime dueDate);
                            ticketEntry.DueDate = dueDate;
                            ticketEntries.Add(ticketEntry);
                        }
                    }
                }
                catch (Exception ex)
                {

                }

                sqlconn.Close();
            }

            return ticketEntries;
        }
    }
}
