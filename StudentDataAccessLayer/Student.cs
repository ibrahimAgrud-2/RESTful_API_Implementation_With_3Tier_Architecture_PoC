using Microsoft.Data.SqlClient;


namespace StudentDataAccessLayer
{
    public class StudentDTO
    {
        public int ID { get; set; }
        public int Age { get; set; }
        public float Grade { get; set; }
        public string Name { get; set; }

        public StudentDTO(int ID,int age,float grade,string name)
        {
            this.ID = ID;
            Age = age;
            Grade = grade;
            Name = name;
        }
    }
    public class Student
    {


        public static List<StudentDTO> GetAllStudents()
        {
            string _connectionString = "Server=localhost;Database=StudentsDB;User Id=sa;Password=sa;Encrypt=False;TrustServerCertificate=True;Connection Timeout=30;";
            List<StudentDTO> Students=new List<StudentDTO>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "select * from People ";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {


                    try
                    {
                        connection.Open();
                        using (SqlDataReader read = cmd.ExecuteReader())
                        {
                           while(read.Read())
                            {
                                Students.Add
                                    (new StudentDTO(
                                       read.GetInt32(read.GetOrdinal("ID")),
                                       read.GetInt32(read.GetOrdinal("age")),
                                       read.GetFloat(read.GetOrdinal("grade")),
                                       read.GetString(read.GetOrdinal("Name"))
                                    ));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("An error Accourred");
                    }
                }
            }
            return Students;
        }

    }
}
