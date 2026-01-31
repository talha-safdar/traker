using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Traker.States;

namespace Traker.ViewModels
{
    public class AddRowEntryViewModel : Screen
    {
        #region Private View Variables
        private decimal _price;
        #endregion

        #region Public State Variable
        public AppState State { get; } // state binding variable accessible from other viewmodels
        #endregion

        public AddRowEntryViewModel(AppState appState)
        {
            Price = 0.00M;

            State = appState;
        }

        public async Task Exit()
        {
            Debug.WriteLine("EXIT ADD");

            // argument true only for dialogs
            // false for the rest
            State.IsAddRowEntryOpen = false;
            await TryCloseAsync();
        }

        //void SaveRow(DashboardRow row)
        //{
        //    using var cmd = conn.CreateCommand();
        //    cmd.CommandText = @"
        //INSERT OR REPLACE INTO Jobs
        //(ClientName, Description, Amount, Status, DueDate, IsPaid)
        //VALUES (@client, @desc, @amount, @status, @due, @paid)";

        //    cmd.Parameters.AddWithValue("@client", row.ClientName);
        //    cmd.Parameters.AddWithValue("@desc", row.Description);
        //    cmd.Parameters.AddWithValue("@amount", row.Amount);
        //    cmd.Parameters.AddWithValue("@status", row.Status);
        //    cmd.Parameters.AddWithValue("@due", row.DueDate);
        //    cmd.Parameters.AddWithValue("@paid", row.IsPaid);

        //    cmd.ExecuteNonQuery();
        //}

        #region Public View Variables
        public decimal Price
        {
            get => _price;
            set
            {
                _price = value;
                NotifyOfPropertyChange(() => Price);
            }
        }
        #endregion
    }
}
