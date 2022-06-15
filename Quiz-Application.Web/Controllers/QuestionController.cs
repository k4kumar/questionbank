using Microsoft.AspNetCore.Mvc;
using Quiz_Application.Services.Entities;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quiz_Application.Services.Repository.Interfaces;
using Quiz_Application.Web.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;

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
                return Ok(lst);
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
    }
}
