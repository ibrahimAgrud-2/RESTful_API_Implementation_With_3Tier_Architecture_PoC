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
    public class StudentDataAccess
    {

       static string _connectionString = "Data Source=IBRAHIM;Initial Catalog=StudentsDB;Integrated Security=True;Trust Server Certificate=True";
        public static List<StudentDTO> GetAllStudents()
        {
           
            List<StudentDTO> Students=new List<StudentDTO>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "select * from students ";

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
                                       read.GetInt32(read.GetOrdinal("grade")),
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

        public static List<StudentDTO> GetPassedStudents()
        {

            List<StudentDTO> Students = new List<StudentDTO>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
          //g3gergergergerge
                using (SqlCommand cmd = new SqlCommand("SP_GetPassedStudents", connection))
                {


                    try
                    {
                        connection.Open();
                        using (SqlDataReader read = cmd.ExecuteReader())
                        {
                            while (read.Read())
                            {
                                Students.Add
                                    (new StudentDTO(
                                       read.GetInt32(read.GetOrdinal("ID")),
                                       read.GetInt32(read.GetOrdinal("age")),
                                       read.GetInt32(read.GetOrdinal("grade")),
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
