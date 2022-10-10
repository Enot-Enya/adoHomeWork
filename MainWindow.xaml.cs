using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace adoHomeWork
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection con;
        OleDbConnection accessConnection;
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Открывает и скрывает поля для ввода логина и пароля в зависимоти от выбора пользователя
        /// </summary>
        private void UserChooseDBComboBox(object sender, RoutedEventArgs e)
        {
            loginLocalDB.Visibility = Visibility.Visible;
            passwordLocalDb.Visibility = Visibility.Visible;
            loginAccessDB.Visibility = Visibility.Visible;
            passwordAccessDb.Visibility = Visibility.Visible;

            if (UserChooselocalDBCB.IsSelected)
            {
                loginAccessDB.Visibility = Visibility.Collapsed;
                passwordAccessDb.Visibility = Visibility.Collapsed;
            } else if (UserChooseAccesDBCB.IsSelected)
            {
                loginLocalDB.Visibility = Visibility.Collapsed;
                passwordLocalDb.Visibility = Visibility.Collapsed;
            }

        }

        /// <summary>
        /// Нажатие на кнопку войти. Установка соединения с базами данных и проверка корректности ввода
        /// </summary>
        private void TryToConnectButton(object sender, RoutedEventArgs e)
        {
            try
            {
#region Проверка, что поля с логином и паролем не пустые
                if (((loginLocalDB.Visibility==Visibility.Visible) && (String.IsNullOrEmpty(loginLocalDB.Text))) || 
                    ((loginAccessDB.Visibility==Visibility.Visible) && (String.IsNullOrEmpty(loginAccessDB.Text))))
                {
                    throw new MyExceptions.EmptyLoginExeption();
                }
                if (((passwordLocalDb.Visibility == Visibility.Visible) && (String.IsNullOrEmpty(passwordLocalDb.Password))) || 
                    ((passwordAccessDb.Visibility == Visibility.Visible) && (String.IsNullOrEmpty(passwordAccessDb.Password))))
                {
                    throw new MyExceptions.EmptyPasswordExeption();
                }
#endregion
                
#region Попытка подключения
                // localdb
                // login - user
                // password - localpassword
                if (ComboBoxDBChooser.SelectedIndex != 1)
                {
                    SqlConnectionStringBuilder sqlConstring = new SqlConnectionStringBuilder()
                    {
                        DataSource = @"(localdb)\mssqllocaldb",
                        InitialCatalog = "localdb",
                        UserID = loginLocalDB.Text,
                        Password = passwordLocalDb.Password,
                        Pooling = false
                    };
                    con = new SqlConnection(sqlConstring.ConnectionString);
                    con.Open();
                }
                // access
                // password - 123
                if (ComboBoxDBChooser.SelectedIndex != 0)
                {
                    var accessConString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=D:\rep\adoHomeWork\db\AccessDatabase.accdb;Jet OLEDB:Database Password="+ $"{passwordAccessDb.Password}";
                    accessConnection = new OleDbConnection(accessConString);
                    accessConnection.Open();
                }
                #endregion

#region Создание нового  окна если все ок
                Workspace workspace = new Workspace(con, accessConnection);
                this.Hide();
                workspace.ShowDialog();
                this.Close();
                #endregion
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
