using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Quiz_Application.Web.Authentication;
using Quiz_Application.Services.Entities;
using Quiz_Application.Services.Repository.Interfaces;
using System.Net.Http.Headers;
using System.IO;

namespace Quiz_Application.Web.Controllers
{    
    [BasicAuthentication]   
    public class ExamController : Controller
    {
        private readonly ILogger<ExamController> _logger;
        private readonly IExam<Services.Entities.Exam> _exam;
        private readonly IQuestion<Services.Entities.Question> _question;
        private readonly IResult<Services.Entities.Result> _result;
        public ExamController(ILogger<ExamController> logger, IExam<Services.Entities.Exam> exam, IQuestion<Services.Entities.Question> question, IResult<Services.Entities.Result> result)
        {
            _logger = logger;
            _exam = exam;
            _question = question;
            _result = result;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IEnumerable<Exam> lst = await _exam.GetExamList();
            return View(lst.OrderByDescending(e=>e.ExamID));
        }
             
        [HttpGet]
        [Route("~/api/Exams")]
        public async Task<IActionResult> Exams()
        {           
            try
            {
                IEnumerable<Exam> lst = await _exam.GetExamList();               
                return Ok(lst.ToList().OrderByDescending(e=>e.ExamID));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
            finally 
            {
            }
        }

        [HttpGet]
        [Route("~/api/Exam/{ExamID?}")]
        public async Task<IActionResult> Exam(int ExamID)
        {
            try
            {
                Exam exm = await _exam.GetExam(ExamID);
                return Ok(exm);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
            finally 
            {
            }
        }

        [HttpGet]
        [Route("~/api/Questions/{ExamID?}")]
        public async Task<IActionResult> Questions(int ExamID)
        {
            try
            {
                QnA _obj = await _question.GetQuestionList(ExamID);
                return Ok(_obj);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
            finally 
            {
            }
        }

        [HttpPost]
        [Route("~/api/Score")]       
        public async Task<IActionResult> Score(List<Option> objRequest)
        {
            int i = 0;
            bool IsCorrect = false;
            List<Result> objList = null;
            string _SessionID = null;
            try
            {               
                if (objRequest.Count > 0)
                {
                    _SessionID = Guid.NewGuid().ToString() + "-" + DateTime.Now;
                    objList = new List<Result>();
                    foreach (var item in objRequest)
                    {
                        if (item.AnswerID == item.SelectedOption)
                            IsCorrect = true;
                        else
                            IsCorrect = false;

                        Result obj = new Result()
                        {
                            CandidateID = item.CandidateID,
                            ExamID = item.ExamID,
                            QuestionID = item.QuestionID,
                            AnswerID = item.AnswerID,
                            SelectedOptionID = item.SelectedOption,
                            IsCorrent = IsCorrect,
                            SessionID= _SessionID,
                            CreatedBy = "SYSTEM",
                            CreatedOn = DateTime.Now
                        };
                        objList.Add(obj);
                    }
                    i = await _result.AddResult(objList);
                }
               
            }
            catch (Exception ex)
            {
                i = 0;
                throw new Exception(ex.Message, ex.InnerException);           
            }
            finally
            {                
            }
            return Ok(i);
        }

        [HttpGet]
        public IActionResult AddExam()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddExam(Exam model, Microsoft.AspNetCore.Http.IFormFile FormFile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            //get file name
            var filename = ContentDispositionHeaderValue.Parse(FormFile.ContentDisposition).FileName.Trim('"');

            //get path
            var MainPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads");

            //create directory "Uploads" if it doesn't exists
            if (!Directory.Exists(MainPath))
            {
                Directory.CreateDirectory(MainPath);
            }

            // get file path
            var filePath = Path.Combine(MainPath, FormFile.FileName);
            using (System.IO.Stream stream = new FileStream(filePath, FileMode.Create))
            {
                await FormFile.CopyToAsync(stream);
            }

            model.PDFLink = filePath;


            await this._exam.AddExam(model);

            return RedirectToAction("Index", "Home");
        }

        // Confirm Delete
        public async Task<ActionResult> DeleteConfirm(int id)
        {
            Exam exam = await _exam.GetExam(id);
            return View(exam);
        }


        public async Task<ActionResult> Delete(int id)
        {
            Exam exam = await _exam.GetExam(id);
            int output = await _exam.DeleteExam(exam);
            return RedirectToAction("Index", "Exam");
        }

        [HttpGet]
        public async Task<ActionResult> Details(int id)
        {
            Exam exam = await _exam.GetExam(id);
            return View(exam);
        }

        [HttpGet]
        public async Task<ActionResult> WatchTutorial(int id)
        {
            Exam exam = await _exam.GetExam(id);
            return View(exam);
        }

        [HttpGet]
        public async Task<ActionResult> UpdateExam(int id)
        {
            Exam exam = await _exam.GetExam(id);
            return View(exam);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateExam(Exam model, Microsoft.AspNetCore.Http.IFormFile FormFile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            //get file name
            var filename = ContentDispositionHeaderValue.Parse(FormFile.ContentDisposition).FileName.Trim('"');

            //get path
            var MainPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads");

            //create directory "Uploads" if it doesn't exists
            if (!Directory.Exists(MainPath))
            {
                Directory.CreateDirectory(MainPath);
            }

            // get file path
            var filePath = Path.Combine(MainPath, FormFile.FileName);
            using (System.IO.Stream stream = new FileStream(filePath, FileMode.Create))
            {
                await FormFile.CopyToAsync(stream);
            }

            model.PDFLink = filePath;

            await this._exam.UpdateExam(model);

            return RedirectToAction("Index", "Exam");
        }

    }
}
