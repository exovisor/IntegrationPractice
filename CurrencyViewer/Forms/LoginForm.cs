using CurrencyViewer.Models;
using CurrencyViewer.Utils;
using System;
using System.Windows.Forms;

namespace CurrencyViewer.Forms
{
    public partial class LoginForm : Form
    {
        public delegate void AuthHandler(User user);
        public event AuthHandler UserLoggedIn;
        public LoginForm()
        {
            InitializeComponent();
        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            var loginDto = (Username: usernameTextBox.Text, Password: passwordTextBox.Text);

            var userHash = Properties.Resources.UserHash;
            if (userHash == null)
            {
                return;
            }

            var isValid = HashUtils.VerifyHash(userHash, loginDto.Username + loginDto.Password);
            if (!isValid) 
            {
                MessageBox.Show("В имени пользователя и/или пароле допущена ошибка.", "Вход не выполнен", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } else
            {
                UserLoggedIn?.Invoke(new User { Username = loginDto.Username });
            }
        }
    }
}
