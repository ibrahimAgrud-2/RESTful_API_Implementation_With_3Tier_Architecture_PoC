namespace API_Implementation
{
    public class Student
    {
        public int Age { get; set; }
        public int ID { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }
        public double Grade { get; set; }

        public Student(int ID,int Age,string FirstName,string LastName,double  Grade)
        {
            this.Age = Age;
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.ID = ID;
            this.Grade = Grade;
        }
    }
}
