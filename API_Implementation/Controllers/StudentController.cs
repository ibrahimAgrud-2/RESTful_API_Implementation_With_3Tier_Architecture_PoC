using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using StudentBusinessLayer;
using StudentDataAccessLayer;
using System;
using System.Data;

namespace API_Implementation.Controllers
{
    //Genel API URL'inde API/Students olarak gözükecek.
    [Route("api/Students")]
    [ApiController]
    public class StudentController : ControllerBase
    {

        /*Bu da tamamen çalışır ama 2.yöntem daha profesyoneldir.
        //Çünkü bu yöntemde status code'u geri dönerme gibi bir şansımız yok.
        //mesela üstüden listesi boşsa notFound, veya client'in izni yoksa 
        //Forbidden gibi status dönderebilmek için action result kullanmalıyız
        //[HttpGet]
        //public List<Student> GetStudentList()
        //{
        //    return StudentDataSimulation.StudentList;
        //}*/


        /*Yukarda açıkladığımız sebepten ötürü veri döndereceğin zaman ActionResult ile döndermelisin
        //Action Result geriye veriyi ve o request'in status code'unu action result kutusu içinde gönderir kodu dönderir  
        //IEnumarable ise client kısmı için list, array, dictionary fark etmeyeceği, onu sadece JSON formatında veri alacağı için hangi türden veri döndürdüğümüzün pek önemi yok. Daha doğrusu sadeec List<Student> demek yerine durumu daha geneleştiriyoruz. IEnumarable<student> diyoruz. IEnumarable zaten list'in daha geniş halidir. Bu durumda biz geriye bir koleksiyon döncekek. Ne olduğu pek önemli diğer onu al ve JSON olarak istediğini yap şeklinde düşünüyoruz.
        //!!!! Arka planda ASP framework JSON serialization ile veriyi JSON'a çeviriyor*/
        [HttpGet("All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<StudentDTO>> GetStudentList()
        {

            List<StudentDTO> StudentList = StudentBusinessLayer.Student.GetAllStudents();
            if(StudentList.Count==0)
            {
                return NotFound("No Data Available in the Table");
            }

            return Ok(StudentList);
        }
        
        //isimlendirmeler ile (all, passed) attriburte'lar birbirinden farklı olmuş oldu. Ama daha okunaklı URL'lere adına Attrüburlara her zaman değişkenlerde olduğu gibi alakalı isim vermek gerekir.    
        [HttpGet("Passed")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<StudentDTO>> GetPassedStudents()
        {
            //logic business layerda olur
            //return Ok(StudentDataSimulation.StudentList.Where(student=>student.Grade>50).ToList());

            List<StudentDTO> StudentList = StudentBusinessLayer.Student.GetPassedStudents();
            if (StudentList.Count == 0)
            {
                return NotFound("No Data Available in the Table");
            }

            return Ok(StudentList);
        }


        [HttpGet("AverageGrade")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<double> GetAverageGrade()
        {


            //Çeşitlilik olamsı açısından bu sefer eğer hiç student yoksa notFound status code'u dönderelim
            //if (StudentDataSimulation.StudentList.Count==0)
            //{
            //    //bu No Student Available mesajı body'de gidecek. 
            //    return NotFound("No Student Available");
            //}
            //return Ok(StudentDataSimulation.StudentList.Average(student=>student.Grade));

            double averageGrade = StudentBusinessLayer.Student.GetAverageGrade();

            if(averageGrade==0)
            {
                return NotFound("No data Avaliable in the table");
            }

            return Ok(averageGrade);

        }

        [HttpGet("{ID}", Name = "GetStudentById")]
        //Normalde default olarak sadeec 200 ok success kodu dokümante edilmiş olur. Ama bu fonksiyonsa 3 farklı durum var.
        //Yani 3 farklı dönüş tipi olaiblir. Bunu bu API dökümanstasyonuna eklemek için bu attribut'ları ekleriz
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        //Bu attributalar ile birlikte artık mesela swaggerda bu fonksiyonun 3 farklı respons türü olduğu belli olur. Veya bu Endpoint'ı reflection ile okuyan herhangi bir araç içinde bu geçelidir. Bu Endpoint için metadata eklemiş olduk
        public ActionResult<StudentDTO> GetStudentByID(int ID)
        {
            if (ID < 1)
            {
                return BadRequest($"Not Accepted ID {ID }");
            }

            //burada tüm student geldi. 
            var student = StudentBusinessLayer.Student.Find(ID);
            if (student == null)
            {
                return NotFound($"No Student with ID {ID}");
            }

            //veri  return ederken student objesini döndürmeyiz. Çünkü student nesnesi içinde client tarafın ihtiyacı olmayan üyeler var. Ayrıca büyük boyut veri demek daha yavaş transfer demektir. Bu nedenle sadece DTO nesnesini döndürürüz
            //normalde DVLD gibi projelerde de Find yaptığımızda tüm objeyi döndürüyorduk fonksiyonşarı ile birlikte. Ama burada client zaten o fonksiyonları kullanamaz. Bu yüzden JSON dosyasını şişirmeke adına  direk client'in işine yarayacak olan 
            //Bilgilileri dönderiyoruz.
            return Ok(student.studentDTO);
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<Student> AddNewStudent(StudentDTO newStudentDTO)
        {

            //we validate the data here
            if (newStudentDTO == null || string.IsNullOrEmpty(newStudentDTO.Name) || newStudentDTO.Age < 0 || newStudentDTO.Grade < 0)
            {
                return BadRequest("Invalid student data.");
            }

            //newStudent.Id = StudentDataSimulation.StudentsList.Count > 0 ? StudentDataSimulation.StudentsList.Max(s => s.Id) + 1 : 1;

            StudentBusinessLayer.Student student = new StudentBusinessLayer.Student(newStudentDTO);
            student.Save();

            newStudentDTO.ID = student.ID;

            //we return the DTO only not the full student object
            //we dont return Ok here,we return createdAtRoute: this will be status code 201 created.
            return CreatedAtRoute("GetStudentById", new { id = newStudentDTO.ID }, newStudentDTO);

        }

    }
}
