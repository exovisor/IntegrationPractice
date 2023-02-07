using CurrencyViewer.Models;
using CurrencyViewer.Context;
using System;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Drawing;

namespace CurrencyViewer.Forms
{
    public partial class CurrenciesForm : Form
    {
        private static readonly string[] _columns = new string[]
        {
            "Наименование",
            "Номинал",
            "Курс",
            "Код"
        };

        private bool showOnlyFavorites = false;
        private CurrencyContext context = null;
        private Dictionary<string, string> currencyDynamics = new Dictionary<string, string>();

        public CurrenciesForm(User user = null, bool onlyFavorites = false)
        {
            InitializeComponent();

            showOnlyFavorites = onlyFavorites;

            // Setup grid view
            using (var client = new ExchangeService.DailyInfoSoapClient())
            {
                var currenciesDataSet = client.GetCursOnDate(DateTime.Now);
                var numCodeColumn = currenciesDataSet.Tables[0].Columns[3];
                currenciesDataSet.Tables[0].Columns.Remove(numCodeColumn);

                currenciesDataGridView.DataSource = currenciesDataSet;
                currenciesDataGridView.DataMember = "ValuteCursOnDate";
                for (int i = 0; i < _columns.Length; i++)
                {
                    currenciesDataGridView.Columns[i].Name = _columns[i];
                    currenciesDataGridView.Columns[i].HeaderText = _columns[i];

                    currenciesDataGridView.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }

                currenciesDataGridView.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                // Trim currency names
                var nameColumnIndex = currenciesDataGridView.Columns["Наименование"].Index;
                foreach (DataGridViewRow row in currenciesDataGridView.Rows )
                {
                    var cell = row.Cells[nameColumnIndex];
                    cell.Value = cell.Value.ToString().Trim();
                }

                // Prefetch dynamics for currencies
                GetDynamicsForCurrencies();

                var dynamicsColumn = new DataGridViewTextBoxColumn();
                dynamicsColumn.Name = "Динамика";
                dynamicsColumn.HeaderText = "Динамика";
                currenciesDataGridView.Columns.Add(dynamicsColumn);

                // Add "Add to favourites" column if user logged in
                if (user != null)
                {
                    var favoriteColumn = new DataGridViewButtonColumn();
                    favoriteColumn.Name = "Избранное";
                    favoriteColumn.HeaderText = "Избранное";
                    currenciesDataGridView.Columns.Add(favoriteColumn);
                }
            }

            if (user == null)
            {
                return;
            }

            // Load DB Context
            context = new CurrencyContext();
            currenciesDataGridView.CellClick += CurrenciesDataGridView_CellClick;
        }

        private void FillFavoriteButtons()
        {
            if (context == null)
            {
                return;
            }

            var favorites = context.FavoriteCurrencies;
            var favoritesColumnIndex = currenciesDataGridView.Columns["Избранное"].Index;
            var codeColumnIndex = currenciesDataGridView.Columns["Код"].Index;

            var rowsToRemove = new List<DataGridViewRow>();
            foreach (DataGridViewRow row in currenciesDataGridView.Rows)
            {
                var code = (string)row.Cells[codeColumnIndex].Value;
                if (favorites.Any(f => f.Code == code))
                {
                    currenciesDataGridView.Rows[row.Index].Cells[favoritesColumnIndex].Value = "Убрать";
                }
                else
                {
                    rowsToRemove.Add(row);
                    currenciesDataGridView.Rows[row.Index].Cells[favoritesColumnIndex].Value = "Добавить";
                }
            }

            if (showOnlyFavorites)
            {
                foreach (var row in rowsToRemove)
                {
                    currenciesDataGridView.Rows.Remove(row);
                }
            }
        }

        private void FillDynamics()
        {
            var dynamicsColumnIndex = currenciesDataGridView.Columns["Динамика"].Index;
            var cursColumnIndex = currenciesDataGridView.Columns["Курс"].Index;
            var codeColumnIndex = currenciesDataGridView.Columns["Код"].Index;

            foreach (DataGridViewRow row in currenciesDataGridView.Rows)
            {
                var code = (string)row.Cells[codeColumnIndex].Value;
                var cell = row.Cells[dynamicsColumnIndex];
                var curs = double.Parse(row.Cells[cursColumnIndex].Value.ToString());
                var val = double.Parse(currencyDynamics[code]);
                var diff = curs - val;

                var str = string.Empty;

                if (diff > 0)
                {
                    cell.Style.ForeColor = Color.Green;
                    str = $"▼ {val} ({string.Format("{0:0.0}", (diff / curs) * 100)}%)";
                } else
                {
                    cell.Style.ForeColor = Color.Red;
                    str = $"▲ {val} ({string.Format("{0:0.0}", (diff / curs) * 100)}%)";
                }

                cell.Value = str;

            }
        }

        private void GetDynamicsForCurrencies()
        {
            var enumCodes = new Dictionary<string, string>();
            using (var client = new ExchangeService.DailyInfoSoapClient())
            {
                var ds = client.EnumValutes(false);
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    enumCodes[row[6].ToString()] = row[0].ToString();
                }

                var codeColumnIndex = currenciesDataGridView.Columns["Код"].Index;
                foreach (DataGridViewRow row in currenciesDataGridView.Rows)
                {
                    var code = row.Cells[codeColumnIndex].Value.ToString();
                    var enumCode = enumCodes[code];

                    var dynamic = client.GetCursDynamic(DateTime.Now.AddMonths(-1), DateTime.Now, enumCode);
                    currencyDynamics.Add(code, dynamic.Tables[0].Rows[0][3].ToString());
                }
            }
        }

        // Handle add or remove from favorites
        private void CurrenciesDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (context == null)
            {
                return;
            }

            var favoritesColumnIndex = currenciesDataGridView.Columns["Избранное"].Index;
            if (e.ColumnIndex == favoritesColumnIndex && e.RowIndex >= 0)
            {
                var codeColumnIndex = currenciesDataGridView.Columns["Код"].Index;
                var nameColumnIndex = currenciesDataGridView.Columns["Наименование"].Index;
                var code = (string)currenciesDataGridView[codeColumnIndex, e.RowIndex].Value;
                var name = (string)currenciesDataGridView[nameColumnIndex, e.RowIndex].Value;

                var favorites = context.FavoriteCurrencies;

                if (favorites.Any((f) => f.Code == code))
                {
                    // Remove from favorites
                    var favoriteEntry = context.FavoriteCurrencies.FirstOrDefault((f) => f.Code == code);
                    if (favoriteEntry != null)
                    {
                        favorites.Remove(favoriteEntry);
                    }

                    if (showOnlyFavorites)
                    {
                        currenciesDataGridView.Rows.RemoveAt(e.RowIndex);
                    } else
                    {
                        currenciesDataGridView.Rows[e.RowIndex].Cells[favoritesColumnIndex].Value = "Добавить";
                    }

                    MessageBox.Show($"[{code}] {name.Trim()} была убрана из избранных");
                } else
                {
                    // Add to favorites
                    var favoriteEntry = new FavoriteCurrency { Code = code, Name = name };
                    favorites.Add(favoriteEntry);

                    currenciesDataGridView.Rows[e.RowIndex].Cells[favoritesColumnIndex].Value = "Убрать";
                    MessageBox.Show($"[{code}] {name.Trim()} была добавлена в избранные");
                }
            }
            context.SaveChanges();
        }

        private void CurrenciesDataGridView_Layout(object sender, LayoutEventArgs e)
        {
            currenciesDataGridView.Focus();
        }

        private void CurrenciesDataGridView_Sorted(object sender, EventArgs e)
        {
            FillFavoriteButtons();
            FillDynamics();
        }

        private void CurrenciesDataGridView_Enter(object sender, EventArgs e)
        {
            FillDynamics();
            FillFavoriteButtons();
        }
    }
}
