using Microsoft.Data.SqlClient;
using System.Data;


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


        public static double GetAverageGrade()
        {

            double average=0;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {

                using (SqlCommand cmd = new SqlCommand("SP_GetAverageGrade", connection))
                {


                    try
                    {
                        connection.Open();
                        object result = cmd.ExecuteScalar();
                        if (result != DBNull.Value)
                        {
                            average = Convert.ToDouble(result);
                        }
                        else
                            average = 0;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("An error Accurred");
                    }
                }
            }
            return average;
        }


        public static StudentDTO GetStudentById(int studentId)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("SP_GetStudentById", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@StudentId", studentId);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new StudentDTO
                        (
                            reader.GetInt32(reader.GetOrdinal("Id")), 
                            reader.GetInt32(reader.GetOrdinal("Age")),
                               reader.GetInt32(reader.GetOrdinal("Grade")),
                            reader.GetString(reader.GetOrdinal("Name"))
                         
                        );
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }


        public static int AddStudent(StudentDTO StudentDTO)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("SP_AddStudent", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Name", StudentDTO.Name);
                command.Parameters.AddWithValue("@Age", StudentDTO.Age);
                command.Parameters.AddWithValue("@Grade", StudentDTO.Grade);
                var outputIdParam = new SqlParameter("@NewStudentId", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(outputIdParam);

                connection.Open();
                command.ExecuteNonQuery();

                return (int)outputIdParam.Value;
            }
        }

        public static bool UpdateStudent(StudentDTO StudentDTO)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("SP_UpdateStudent", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@StudentId", StudentDTO.ID);
                command.Parameters.AddWithValue("@Name", StudentDTO.Name);
                command.Parameters.AddWithValue("@Age", StudentDTO.Age);
                command.Parameters.AddWithValue("@Grade", StudentDTO.Grade);

                connection.Open();
                command.ExecuteNonQuery();
                return true;

            }
        }

    }
}
