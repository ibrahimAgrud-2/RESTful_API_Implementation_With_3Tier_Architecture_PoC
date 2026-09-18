using StudentDataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentBusinessLayer
{
    public class Student
    {
        
        public enum Mode {enAddNew=1,enUpdate=2 };
        public Mode mode;

        public StudentDTO studentDTO;

        public static List<StudentDTO> GetAllStudents()
        {
            return StudentDataAccess.GetAllStudents();
        }
        public static List<StudentDTO> GetPassedStudents()
        {
            return StudentDataAccess.GetPassedStudents();
        }
        public static double GetAverageGrade()
        {
            return StudentDataAccess.GetAverageGrade();
        }
        public static StudentDTO GetStudentByID(int ID)
        {
            return StudentDataAccess.GetStudentByID(ID);
        }
    }
}
