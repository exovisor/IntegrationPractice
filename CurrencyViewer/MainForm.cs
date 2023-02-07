using CurrencyViewer.Forms;
using CurrencyViewer.Models;
using System;
using System.Timers;
using System.Windows.Forms;

namespace CurrencyViewer
{
    public partial class MainForm : Form
    {
        private readonly System.Timers.Timer oneSecondTimer;

        private (Form form, Panel view) currentView = (null, null);
        private User user = null;

        public MainForm()
        {
            InitializeComponent();

            // Init status strip
            UpdateTimeStripStatusLabel();

            // Setup timer
            oneSecondTimer = new System.Timers.Timer(1000);
            oneSecondTimer.Elapsed += (object sender, ElapsedEventArgs e) => UpdateTimeStripStatusLabel();
            oneSecondTimer.Start();

            // Hide Auth-only components
            favoritesToolStripMenuItem.Visible = false;
            logoutToolStripMenuItem.Visible = false;

            // Load start page
            var currenciesForm = new CurrenciesForm(user, true);
            SetView(currenciesForm, currenciesForm.viewPanel);
        }

        #region [ Methods ]

        private void UpdateTimeStripStatusLabel()
        {
            timeStripStatusLabel.Text = DateTime.Now.ToString("g");
        }

        private void SetView(Form newForm, Panel newView)
        {
            if (currentView.form != null && currentView.view != null)
            {
                currentView.view.Parent = currentView.form;
                currentView.form.Dispose();
            }

            currentView = (newForm, newView);
            currentView.view.Parent = viewRoot;
        }

        #endregion

        #region [ Events ]

        private void ExchangeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var currenciesForm = new CurrenciesForm(user, false);
            SetView(currenciesForm, currenciesForm.viewPanel);
        }

        private void LoginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var loginForm = new LoginForm();
            loginForm.UserLoggedIn += LoginForm_UserLoggedIn;
            SetView(loginForm, loginForm.viewPanel);
        }

        private void FavoritesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var currenciesForm = new CurrenciesForm(user, true);
            SetView(currenciesForm, currenciesForm.viewPanel);
        }

        private void LogoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            user = null;

            loginToolStripMenuItem.Visible = true;
            favoritesToolStripMenuItem.Visible = false;
            logoutToolStripMenuItem.Visible = false;

            var currenciesForm = new CurrenciesForm(user, false);
            SetView(currenciesForm, currenciesForm.viewPanel);
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var aboutForm = new AboutForm();
            SetView(aboutForm, aboutForm.viewPanel);
        }

        private void QuitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void LoginForm_UserLoggedIn(User user)
        {
            this.user = user;

            loginToolStripMenuItem.Visible = false;
            favoritesToolStripMenuItem.Visible = true;
            logoutToolStripMenuItem.Visible = true;

            var currenciesForm = new CurrenciesForm(user, true);
            SetView(currenciesForm, currenciesForm.viewPanel);
        }

        #endregion
    }
}
