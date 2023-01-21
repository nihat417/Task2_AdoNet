using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Task2_AdoNet;


public partial class MainWindow : Window
{
    SqlConnection? conn = null;
    SqlDataReader? reader = null;
    DataTable? table = null;

    SqlCommandBuilder? builder = null;
    DataSet? dataset = null;
    SqlDataAdapter? adapter = null;

    public MainWindow()
    {
        InitializeComponent();
        Configure();
    }

    private void Configure()
    {
        string connectionString = "Data Source=NIKO;Initial Catalog=Library;Integrated Security=True;";
        conn =new SqlConnection(connectionString);
    }

    private void FillBtn_Click(object sender, RoutedEventArgs e)
    {
        string SelectSqlcm = "SELECT * \r\nFROM Authors";
        adapter = new SqlDataAdapter(SelectSqlcm, conn);
        dataset=new DataSet();
        adapter.Fill(dataset);
        foreach (DataTable datatb in dataset.Tables)
        {
            DataGridView.ItemsSource= datatb.AsDataView();
        }
    }

    private void InsertlBtn_Click(object sender, RoutedEventArgs e)
    {
        SqlCommand InsertCm = new SqlCommand()
        {
            CommandText = "INSERT Authors VALUES(@Id,@FirstName,@LastName)",
            Connection = conn
        };
        InsertCm.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int));
        InsertCm.Parameters.Add(new SqlParameter("@FirstName", SqlDbType.NVarChar));
        InsertCm.Parameters.Add(new SqlParameter("@LastName", SqlDbType.NVarChar));

        InsertCm.Parameters["@Id"].SourceVersion=DataRowVersion.Current;
        InsertCm.Parameters["@Id"].SourceColumn = "Id";

        InsertCm.Parameters["@FirstName"].SourceVersion=DataRowVersion.Current;
        InsertCm.Parameters["@FirstName"].SourceColumn= "FirstName";

        InsertCm.Parameters["@LastName"].SourceVersion = DataRowVersion.Current;
        InsertCm.Parameters["@LastName"].SourceColumn = "LastName";


        adapter.InsertCommand= InsertCm;
        adapter.Update(dataset);
    }


    private void UpdateBtn_Click(object sender, RoutedEventArgs e)
    {
        SqlCommand updateCm = new SqlCommand()
        {
            CommandText ="usp_UpdateAuthors",
            Connection = conn,
            CommandType= CommandType.StoredProcedure,
        };


        updateCm.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int));
        updateCm.Parameters.Add(new SqlParameter("@FirstName", SqlDbType.NVarChar));
        updateCm.Parameters.Add(new SqlParameter("@LastName", SqlDbType.NVarChar));


        updateCm.Parameters["@Id"].SourceVersion = DataRowVersion.Current;
        updateCm.Parameters["@Id"].SourceColumn="Id";

        updateCm.Parameters["@FirstName"].SourceVersion = DataRowVersion.Current;
        updateCm.Parameters["@FirstName"].SourceColumn = "FirstName";

        updateCm.Parameters["@LastName"].SourceVersion = DataRowVersion.Current;
        updateCm.Parameters["@LastName"].SourceColumn = "LastName";


        adapter.UpdateCommand = updateCm;
        adapter.Update(dataset);
    }

    private void DeleteBtn_Click(object sender, RoutedEventArgs e)
    {
        SqlCommand delCommand = new SqlCommand()
        {
            CommandText = "DELETE Authors \nWHERE Id = @Id",
            Connection = conn,
        };
        delCommand.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int));
        delCommand.Parameters["@Id"].SourceVersion = DataRowVersion.Current;
        delCommand.Parameters["@Id"].SourceColumn = "Id";

        adapter.DeleteCommand = delCommand;
        adapter.Update(dataset);
    }

    private void SearchTextbox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (adapter is null) return;
        dataset?.Clear();
        string txt = SearchTextbox.Text;
        adapter.SelectCommand.CommandText = $"SELECT * FROM Authors WHERE UPPER(FirstName) LIKE UPPER('%{txt}%') OR UPPER(LastName) LIKE UPPER('%{txt}%')";
        adapter.Fill(dataset);
        foreach (DataTable item in dataset.Tables)
        {
            DataGridView.ItemsSource = item.AsDataView();
        }
        adapter.SelectCommand.CommandText = "SELECT * FROM Authors";
    }
}
