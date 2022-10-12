using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Reflection.Emit;
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
using static System.Net.Mime.MediaTypeNames;

namespace adoHomeWork
{
    /// <summary>
    /// Логика взаимодействия для Workspace.xaml
    /// </summary>
    public partial class Workspace : Window
    {
        DataTable dtLocal;
        SqlDataAdapter daLocal;
        DataRowView drLocal;
        DataTable dtAccess;
        OleDbDataAdapter daAccess;
        DataRowView drAccess;
        SqlConnection con;
        OleDbConnection accessConnection;
        public Workspace(SqlConnection con , OleDbConnection accessConnection)
        {
            this.con = con;
            this.accessConnection = accessConnection;
            InitializeComponent();
            Preparing();
        }
        /// <summary>
        /// Задает базовые команды типо insert delete и тд. А так же выводит информацию на экран
        /// </summary>
        private void Preparing()
        {
            #region localdb
            if (con != null)
            {
                this.dtLocal = new DataTable();
                this.daLocal = new SqlDataAdapter();
                #region SELECT
                var localSqlSelect = @"Select *
                                        FROM Client;";
                daLocal.SelectCommand = new SqlCommand(localSqlSelect, con);
                #endregion

                #region DELETE
                var deletLocalSqlCommand = "DELETE FROM Client WHERE id = @id";
                daLocal.DeleteCommand = new SqlCommand(deletLocalSqlCommand, con);
                daLocal.DeleteCommand.Parameters.Add("@id", SqlDbType.Int, 4, "id");
                #endregion

                #region Update
                var sqlUpdate = @"UPDATE Client SET 
                           surname = @surname,
                           name = @name, 
                           patronymic = @patronymic,
                           phoneNumber = @phoneNumber,
                           email = @emaillocal
                           WHERE id = @idlocal";

                daLocal.UpdateCommand = new SqlCommand(sqlUpdate, con);
                daLocal.UpdateCommand.Parameters.Add("@idlocal", SqlDbType.Int, 0, "Id").SourceVersion = DataRowVersion.Original;
                daLocal.UpdateCommand.Parameters.Add("@surname", SqlDbType.NVarChar, 50, "surname");
                daLocal.UpdateCommand.Parameters.Add("@name", SqlDbType.NVarChar, 50, "name");
                daLocal.UpdateCommand.Parameters.Add("@patronymic", SqlDbType.NVarChar, 50, "patronymic");
                daLocal.UpdateCommand.Parameters.Add("@phoneNumber", SqlDbType.NVarChar, 20, "phoneNumber");
                daLocal.UpdateCommand.Parameters.Add("@emaillocal", SqlDbType.NVarChar, 20, "email");
                #endregion

                #region Insert
                var sql = @"INSERT INTO Client (surname,  name,  patronymic, phoneNumber, email) 
                                 VALUES (@surname,  @name,  @patronymic, @phoneNumber, @email); 
                     SET @id = @@IDENTITY;";

                daLocal.InsertCommand = new SqlCommand(sql, con);

                daLocal.InsertCommand.Parameters.Add("@Id", SqlDbType.Int, 4, "Id").Direction = ParameterDirection.Output;
                daLocal.InsertCommand.Parameters.Add("@surname", SqlDbType.NVarChar, 50, "surname");
                daLocal.InsertCommand.Parameters.Add("@name", SqlDbType.NVarChar, 50, "name");
                daLocal.InsertCommand.Parameters.Add("@patronymic", SqlDbType.NVarChar, 50, "patronymic");
                daLocal.InsertCommand.Parameters.Add("@phoneNumber", SqlDbType.NVarChar, 20, "phoneNumber");
                daLocal.InsertCommand.Parameters.Add("@email", SqlDbType.NVarChar, 20, "email");
                #endregion
                daLocal.Fill(dtLocal);
                gridViewLocal.DataContext = dtLocal.DefaultView;
            }
            #endregion

            #region Access
            if (accessConnection != null)
            {
                this.dtAccess = new DataTable();
                this.daAccess = new OleDbDataAdapter();
                #region SELECT
                var accessSqlSelect = @"SELECT *
                                    FROM ORDERS";
                daAccess.SelectCommand = new OleDbCommand(accessSqlSelect, accessConnection);
                #endregion

                #region DELETE
                var deleteAccessSqlCommand = "DELETE FROM ORDERS WHERE id = @id";
                daAccess.DeleteCommand = new OleDbCommand(deleteAccessSqlCommand, accessConnection);
                daAccess.DeleteCommand.Parameters.Add("@id", OleDbType.Integer, 4, "id");
                #endregion

                #region UPDATE
                var accessUpdate = $@"UPDATE ORDERS SET 
                           email = @emailAccess,
                           product_number = @product_number, 
                           product_name = @product_name
                           WHERE Id = @idAccess";

                daAccess.UpdateCommand = new OleDbCommand(accessUpdate, accessConnection);
                daAccess.UpdateCommand.Parameters.AddWithValue("@idAccess", "Id");
                daAccess.UpdateCommand.Parameters.Add("@emailAccess", OleDbType.WChar, 0, "email");
                daAccess.UpdateCommand.Parameters.Add("@product_number", OleDbType.WChar, 0, "product_number");
                daAccess.UpdateCommand.Parameters.Add("@product_name", OleDbType.WChar, 0, "product_name");
                #endregion

                #region Insert
                var sql = @"INSERT INTO ORDERS (email,  product_number,  product_name) 
                                 VALUES (@email,  @product_number,  @product_name);";

                daAccess.InsertCommand = new OleDbCommand(sql, accessConnection);

                daAccess.InsertCommand.Parameters.Add("@email", OleDbType.WChar, 0, "email");
                daAccess.InsertCommand.Parameters.Add("@product_number", OleDbType.WChar, 0, "product_number");
                daAccess.InsertCommand.Parameters.Add("@product_name", OleDbType.WChar, 0, "product_name");
                #endregion
                daAccess.Fill(dtAccess);
                gridViewAccess.DataContext = dtAccess.DefaultView;
            }
            #endregion
        }

        #region Кнопки удаления
        private void DeleteItemAccess(object sender, RoutedEventArgs e)
        {
            drAccess = (DataRowView)gridViewAccess.SelectedItem;
            drAccess.Row.Delete();
            daAccess.Update(dtAccess);
        }
        private void DeleteItemLocal(object sender, RoutedEventArgs e)
        {
            drLocal = (DataRowView)gridViewLocal.SelectedItem;
            drLocal.Row.Delete();
            daLocal.Update(dtLocal);
        }
        #endregion

        #region Редактирование записей
        private void ChangedLocal(object sender, EventArgs e)
        {
                if (drLocal == null) return;
                drLocal.EndEdit();
                daLocal.Update(dtLocal);
        }

        private void ChangedAccess(object sender, EventArgs e)
        { 
                if (drAccess == null) return;
                drAccess.EndEdit();
            TextBlock selectedTextBlock = gridViewAccess.Columns[0].GetCellContent(gridViewAccess.Items[gridViewAccess.SelectedIndex]) as TextBlock;
            int selectedId = int.Parse(selectedTextBlock?.Text);
            var accessUpdate = $@"UPDATE ORDERS SET 
                           email = @emailAccess,
                           product_number = @product_number, 
                           product_name = @product_name
                           WHERE Id = {selectedId}";

            daAccess.UpdateCommand = new OleDbCommand(accessUpdate, accessConnection);
            daAccess.UpdateCommand.Parameters.Add("@emailAccess", OleDbType.WChar, 0, "email");
            daAccess.UpdateCommand.Parameters.Add("@product_number", OleDbType.WChar, 0, "product_number");
            daAccess.UpdateCommand.Parameters.Add("@product_name", OleDbType.WChar, 0, "product_name");
            daAccess.Update(dtAccess);
        }


        private void ChangedEndingAccess(object sender, DataGridCellEditEndingEventArgs e)
        {
            drAccess = (DataRowView)gridViewAccess.SelectedItem;
            drAccess.BeginEdit();
        }

        private void ChangedEndingLocal(object sender, DataGridCellEditEndingEventArgs e)
        {
            drLocal = (DataRowView)gridViewLocal.SelectedItem;
            drLocal.BeginEdit();
        }
        #endregion

        #region Добавление новых записей
        #region Открытие полей для добавления
        private void addNewClientButton(object sender, RoutedEventArgs e)
        {
            addNewClientSP.Visibility = Visibility.Visible;
        }

        private void AddNewOrderButton(object sender, RoutedEventArgs e)
        {
            addNewOrderSP.Visibility = Visibility.Visible;
        }
        #endregion

        #region Добавление новых пользователей
        private void addNewClientButtonSP(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(newSurname.Text) || string.IsNullOrEmpty(newName.Text) || string.IsNullOrEmpty(newPatronymic.Text) || string.IsNullOrEmpty(newEmaillocal.Text))
                {
                    throw new MyExceptions.EmptyfieldExeption();
                }
                DataRow r = dtLocal.NewRow();
                r["surname"] = newSurname.Text;
                r["name"] = newName.Text;
                r["patronymic"] = newPatronymic.Text;
                r["phoneNumber"] = newPhoneNumber.Text;
                r["email"] = newEmaillocal.Text;
                dtLocal.Rows.Add(r);
                daLocal.Update(dtLocal);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            newSurname.Text = null;
            newName.Text = null;
            newPatronymic.Text = null;
            newPhoneNumber.Text = null;
            newEmaillocal.Text = null;
            addNewClientSP.Visibility=Visibility.Collapsed;
        }
        #endregion
        #region Добавление новых заказов
        private void addNewOrderButtonSP(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(newEmailAccess.Text) || string.IsNullOrEmpty(newProductNumber.Text) || string.IsNullOrEmpty(newProductName.Text))
                {
                    throw new MyExceptions.EmptyfieldExeption();
                }
                DataRow r = dtAccess.NewRow();
                r["email"] = newEmailAccess.Text;
                r["product_number"] = newProductNumber.Text;
                r["product_name"] = newProductName.Text;
                dtAccess.Rows.Add(r);
                daAccess.Update(dtAccess);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            newEmailAccess.Text = null;
            newProductNumber.Text = null;
            newProductName.Text = null;
            addNewOrderSP.Visibility = Visibility.Collapsed;
        }
        #endregion

        #endregion
        /// <summary>
        /// Просмотр списка покупок 1 человека
        /// </summary>
        private void allOrdersOneClientButton(object sender, RoutedEventArgs e)
        {
            try
            {
                if (accessConnection == null)
                {
                    throw new MyExceptions.NoConnectionExeption();
                }
                TextBlock selectedTextBlock = gridViewLocal.Columns[5].GetCellContent(gridViewLocal.Items[gridViewLocal.SelectedIndex]) as TextBlock;
                string selectedEmail = selectedTextBlock?.Text;
                new allOrdersOneClientWindow(daAccess, accessConnection, selectedEmail).ShowDialog();
            }catch (Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }

        }
    }
}
