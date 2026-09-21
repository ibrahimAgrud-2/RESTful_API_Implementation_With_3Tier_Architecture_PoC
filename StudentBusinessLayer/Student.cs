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

        public int ID { get; set; }
        public int Age { get; set; }
        public float Grade { get; set; }
        public string Name { get; set; }

        public StudentDTO studentDTO
        {
            get { return new StudentDTO(this.ID,this.Age,this.Grade,this.Name);}
        }

        public Student(StudentDTO studentDTO,Mode mode)
        {
            this.Age = studentDTO.Age;
            this.ID = studentDTO.ID;
            this.Grade = studentDTO.Grade;
            this.Name = studentDTO.Name;

            this.mode = mode;
        }

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
        public static Student Find(int ID)
        {
            StudentDTO studentDTO = StudentDataAccess.Find(ID);

            if (studentDTO != null)
                return new Student(studentDTO, Mode.enUpdate);
            else
            {
                return null;
            }
              
        }
    }
}
