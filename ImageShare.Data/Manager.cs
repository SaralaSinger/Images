using System.Data.SqlClient;

namespace ImageShare.Data
{
    public class Manager
    {
        private string _connectionString;

        public Manager(string connectionString)
        {
            _connectionString = connectionString;
        }
        public void Add(Image img)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Images VALUES (@views, @password, @fileName) SELECT SCOPE_IDENTITY()";
            command.Parameters.AddWithValue("@views", 0);
            command.Parameters.AddWithValue("@password", img.Password);
            command.Parameters.AddWithValue("@fileName", img.FileName);
            connection.Open();
            img.Id = (int)(decimal)command.ExecuteScalar();
            img.Views = 0;
        }
        public Image GetImageById(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Images WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            var image = new Image();
            var reader = command.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }
            else
            {
                image.Id = (int)reader["Id"];
                image.Password = (string)reader["Password"];
                image.FileName = (string)reader["FileName"];
                image.Views = (int)reader["Views"];
            }
            return image;
        }
        public void IncrementViews(int views, int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "UPDATE Images SET VIEWS = @views WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@views", views + 1);
            connection.Open();
            command.ExecuteNonQuery();
        }
        
    }

    public class Image
    {
        public int Id { get; set; }
        public string Password { get; set; }
        public int Views { get; set; }
        public string FileName { get; set; }

    }
}