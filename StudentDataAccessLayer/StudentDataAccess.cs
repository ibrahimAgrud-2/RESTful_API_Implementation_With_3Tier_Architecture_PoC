using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols;
using System.Net;
using System.Numerics;


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


        public static StudentDTO GetStudentByID(int studentID)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "select * from students where ID=@ID";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@ID", studentID);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader read = cmd.ExecuteReader())
                        {
                            if (read.Read())
                            {

                                return new StudentDTO(
                                       read.GetInt32(read.GetOrdinal("ID")),
                                       read.GetInt32(read.GetOrdinal("age")),
                                       read.GetInt32(read.GetOrdinal("grade")),
                                       read.GetString(read.GetOrdinal("Name"))
                                    );
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
            
                    }
                }
            }
            return null;
        }
    }
}
