using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
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
        public ActionResult<Student> GetStudentByID(int ID)
        {
            if (ID < 1)
            {
                return BadRequest($"Not Accepted ID {ID}");
            }

            var student = StudentDataSimulation.StudentList.FirstOrDefault(student => student.ID == ID);
            if (student == null)
            {
                return NotFound($"No Student with ID {ID}");
            }
            return Ok(student);
        }


        //POST işlemlerinin endüstri standardı başarı kodu 200 OK değil, 201 Created'dır.
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public ActionResult<Student> AddNewStudent(Student newStudent)
        {
            if(newStudent==null||newStudent.FirstName==""||newStudent.LastName==""||newStudent.ID<0||newStudent.Grade<0||newStudent.Age<0)
            {
                return BadRequest("Data not valid");
            }
            StudentDataSimulation.StudentList.Add(newStudent);

            /*Normalde bir obje ekledikten sonra ID'sini dönderirdik. 
            API'da URL söz konusu oldığu için ID yerine direk URL'inin döndermek daha kullanışlı. Sonuçta o veriye erişmek için sadee ID'si yetmez tam bir EndPoint lazım. Mesela https://localhost:7140/api/Students/12 gibi dadece 12 döndermekten daha iyidir. Bu en Profesyonel yaklaşımdır yani API, client'a "bu kaynağa nasıl ulaşacağını" da söylemeli

            createdAtRoute fonksiyonu 3 şeyi birlikte döndürebilir. 1-201 status code, 2- Oluşan veriye erişmek için tam URL'i Respons Body'de oluşan verini tam halini dönderir.
            Asıl olay Oluşan URL'i nasıl döndürebileceğimiz. Şimdi bunu direk manual olarak ta vermenin yolları var ama profesyonel değil. Bunu yapmak için şöyle düşünüyoruz:
            Bize ne lazım? Bize sadece Bir URL lazım. Oluşan kaynağın URL'i. Bunu nasıl alabiliriz? Bunu almanın en iyi yolu 
            Enpoint URL'lerinin tutulduğu routing tablosuna gideriz. Bu tablo framework tarafında ilk program ilk başladığında endpointler taranarak oluşturulur. Bu tabloda şu şekilde URL'ler olur
                    Route Adı           →  URL Kalıbı
                    ─────────────────────────────────────
                    "GetStudentById"     →  / api / Students /{ id}
                    (isimsiz POST)       →  / api / Students
            Burada görüleceği üzere bizim istediğimiz URL GetStudentById isimli attribute'ın URL'i. Bu yüzden GetStudentById'i ilk parametre olarka verdik. Framework'e URL oluşturmak için GetStudentById'in URL'ini al. GetStudentById'de ID parametresi olduğu için bunu da vermemiz gerek. İşte 2.parameter bu yüzden var. 2.Paramterde URL'de tam olarak hangi paramtere neyle eşleşecek söylüyoruz. Bu durumda tek bir paramtere olduğu için sadece ID = dedik. Şu da olabilidi new { classId = newStudent.ClassId, id = newStudent.ID } eğer attrubte'a  birden fazla paramtere olsadı. 3.Parametre newStudent objesi response body'de eklenecek.  Sonuç olacak CreatedAtRoute fonksiyonunun istediği tüm bilgileri vermiş olduk

             */

            return CreatedAtRoute("GetStudentById", new { ID=newStudent.ID},newStudent);

        }


        [HttpDelete("{ID}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public ActionResult DeleteStudent(int ID)
        {

            if(ID<1)
            {
                return BadRequest($"ID is not Valid. ID= {ID}");
            }
            if(StudentDataSimulation.StudentList.Count(student => student.ID == ID) == 0)
            {
                return NotFound($"Student with ID {ID} could not be found.");
            }

            StudentDataSimulation.StudentList.RemoveAll(student=>student.ID==ID);

            return NoContent();

            //buda çalışır ama REST API'da delete işlemi yapıldıktan sonra 204 no content code body'siz döndürülür
            //  return Ok(true);


        }


 
        [HttpPut("{id}", Name = "UpdateStudent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Student> UpdateStudent(int id, Student updatedStudent)
        {
            if (id < 1 || updatedStudent == null || string.IsNullOrEmpty(updatedStudent.FirstName) || updatedStudent.Age < 0 || updatedStudent.Grade < 0)
            {
                return BadRequest("Invalid student data.");
            }

            var student = StudentDataSimulation.StudentList.FirstOrDefault(s => s.ID == id);
            if (student == null)
            {
                return NotFound($"Student with ID {id} not found.");
            }

            student.FirstName = updatedStudent.FirstName;
            student.LastName = updatedStudent.LastName;
            student.Age = updatedStudent.Age;
            student.Grade = updatedStudent.Grade;

            return Ok(student);
        }


    }
}
