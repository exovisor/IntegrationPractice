using System;
using System.Windows.Forms;

namespace CurrencyViewer.Forms
{
    public partial class CurrenciesForm : Form
    {
        private static readonly (string Name, int Index)[] _columns = new (string, int)[]
        {
            ("Наименование", 1),
            ("Номинал", 2),
            ("Курс", 3),
            ("Код", 0)
        };

        public CurrenciesForm()
        {
            InitializeComponent();

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
                    currenciesDataGridView.Columns[i].HeaderText = _columns[i].Name;
                    currenciesDataGridView.Columns[i].DisplayIndex = _columns[i].Index;

                    currenciesDataGridView.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }

                currenciesDataGridView.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
        }
    }
}
