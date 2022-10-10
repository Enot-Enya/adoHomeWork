using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace adoHomeWork
{
    /// <summary>
    /// Логика взаимодействия для allOrdersOneClientWindow.xaml
    /// </summary>
    public partial class allOrdersOneClientWindow : Window
    {
        OleDbDataAdapter daAccess;
        OleDbConnection accessConnection;
        string selectedEmail;
        public allOrdersOneClientWindow(OleDbDataAdapter daAccess, OleDbConnection accessConnection, string selectedEmail)
        {
            this.daAccess = daAccess;
            this.accessConnection = accessConnection;
            this.selectedEmail = selectedEmail;
            InitializeComponent();
            Start();
        }

        private void Start()
        {
            DataTable dtAccess = new DataTable();
            var accessSqlSelect = $@"SELECT        Id, email,   
                                    product_name AS [Код товара], 
                                    product_number AS [Наименование товара]
                                    FROM            ORDERS
                                    WHERE        (email = '{selectedEmail}')";
            daAccess.SelectCommand = new OleDbCommand(accessSqlSelect, accessConnection);
            daAccess.Fill(dtAccess);
            gridViewOneClientOrders.DataContext = dtAccess.DefaultView;
        }
    }
}
