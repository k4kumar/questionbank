using Microsoft.AspNetCore.Mvc;
using Quiz_Application.Services.Entities;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quiz_Application.Services.Repository.Interfaces;
using Quiz_Application.Web.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.IO;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using System.Data.OleDb;

namespace Quiz_Application.Web.Controllers
{
    [BasicAuthentication]
    public class QuestionController : Controller
    {
        private readonly ILogger<QuestionController> _logger;
        private readonly IExam<Services.Entities.Exam> _exam;
        private readonly IQuestion<Services.Entities.Question> _question;
        private readonly IResult<Services.Entities.Result> _result;
        public QuestionController(ILogger<QuestionController> logger, IExam<Services.Entities.Exam> exam, IQuestion<Services.Entities.Question> question, IResult<Services.Entities.Result> result)
        {
            _logger = logger;
            _exam = exam;
            _question = question;
            _result = result;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IEnumerable<Question> lst = await _question.GetQuestionList();
            return View(lst.OrderByDescending(e => e.QuestionID));
        }

        [HttpGet]
        public IActionResult AddQuestion()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddQuestion(Question model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            await this._question.AddQuestion(model);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Route("~/api/Questions/GetAll")]
        public async Task<IActionResult> Questions()
        {
            try
            {
                IEnumerable<Question> lst = await _question.GetQuestionList();
                return Ok(lst.ToList().OrderByDescending(e => e.QuestionID));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {
            }
        }

        // Confirm Delete
        public async Task<ActionResult> DeleteConfirm(int id)
        {
            Question question = await _question.GetQuestion(id);
            return View(question);
        }


        public async Task<ActionResult> Delete(int id)
        {            
            Question question = await _question.GetQuestion(id);
            int output = await _question.DeleteQuestion(question);
            return RedirectToAction("Index", "Question");
        }

        [HttpGet]
        public async Task<ActionResult> UpdateQuestion(int id)
        {
            Question question = await _question.GetQuestion(id);
            ViewBag.ExamID = question.ExamID;
            return View(question);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuestion(Question model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            await this._question.UpdateQuestion(model);

            return RedirectToAction("Index", "Question");
        }


        [HttpGet]
        public IActionResult BulkQuestion()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ImportExcelFile(IFormFile FormFile)
        {
            
            //get file name
            var filename = ContentDispositionHeaderValue.Parse(FormFile.ContentDisposition).FileName.Trim('"');
            
            //get path
            var MainPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads");

            //create directory "Uploads" if it doesn't exists
            if (!Directory.Exists(MainPath))
            {
                Directory.CreateDirectory(MainPath);
            }

            //get file path 
            var filePath = Path.Combine(MainPath, FormFile.FileName);
            using (System.IO.Stream stream = new FileStream(filePath, FileMode.Create))
            {
                await FormFile.CopyToAsync(stream);
            }

            //get extension
            string extension = Path.GetExtension(filename);


            string conString = string.Empty;

            switch (extension)
            {
                case ".xls": //Excel 97-03.
                    conString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath + ";Extended Properties='Excel 8.0;HDR=YES'";
                    break;
                case ".xlsx": //Excel 07 and above.
                    conString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties='Excel 8.0;HDR=YES'";
                    break;
            }

            DataTable dt = new DataTable();
            conString = string.Format(conString, filePath);

            using (OleDbConnection connExcel = new OleDbConnection(conString))
            {
                using (OleDbCommand cmdExcel = new OleDbCommand())
                {
                    using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                    {
                        cmdExcel.Connection = connExcel;

                        //Get the name of First Sheet.
                        connExcel.Open();
                        DataTable dtExcelSchema;
                        dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                        connExcel.Close();

                        //Read Data from First Sheet.
                        connExcel.Open();
                        cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                        odaExcel.SelectCommand = cmdExcel;
                        odaExcel.Fill(dt);
                        connExcel.Close();
                    }
                }
            }
            //your database connection string
            conString = @"Server=DESKTOP-CSC01;Database=rblexamdb;Trusted_Connection=True;";

            using (SqlConnection con = new SqlConnection(conString))
            {
                using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                {
                    //Set the database table name.
                    sqlBulkCopy.DestinationTableName = "dbo.QuestionBulk";

                    // Map the Excel columns with that of the database table, this is optional but good if you do
                    sqlBulkCopy.ColumnMappings.Add("Exam", "Exam");
                    sqlBulkCopy.ColumnMappings.Add("DisplayText", "DisplayText");
                    sqlBulkCopy.ColumnMappings.Add("Choice1", "Choice1");
                    sqlBulkCopy.ColumnMappings.Add("Choice2", "Choice2");
                    sqlBulkCopy.ColumnMappings.Add("Choice3", "Choice3");
                    sqlBulkCopy.ColumnMappings.Add("Choice4", "Choice4");
                    sqlBulkCopy.ColumnMappings.Add("Answer", "Answer");

                    con.Open();
                    sqlBulkCopy.WriteToServer(dt);
                    con.Close();
                }
            }

            _question.BulkQuestionEntry();

            return RedirectToAction("Index", "Question");

        }
    }
}
