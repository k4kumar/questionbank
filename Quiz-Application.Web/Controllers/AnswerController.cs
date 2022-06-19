using Microsoft.AspNetCore.Mvc;
using Quiz_Application.Services.Entities;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quiz_Application.Services.Repository.Interfaces;
using Quiz_Application.Web.Authentication;
using System.Collections.Generic;
using System.Linq;

namespace Quiz_Application.Web.Controllers
{
    [BasicAuthentication]
    public class AnswerController : Controller
    {
        private readonly IAnswer<Services.Entities.Answer> _answer;
        public AnswerController(IAnswer<Services.Entities.Answer> answer)
        {
            _answer = answer;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IEnumerable<Answer> lst = await _answer.GetAnswerList();
            return View(lst.OrderByDescending(e => e.Sl_No));
        }

        [HttpGet]
        public IActionResult AddAnswer()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddAnswer(Answer model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            await this._answer.AddAnswer(model);

            return RedirectToAction("Index", "Answer");
        }

        // Confirm Delete
        public async Task<ActionResult> DeleteConfirm(int id)
        {
            Answer answer = await _answer.GetAnswer(id);
            return View(answer);
        }


        public async Task<ActionResult> Delete(int id)
        {
            Answer answer = await _answer.GetAnswer(id);
            int output = await _answer.DeleteAnswer(answer);
            return RedirectToAction("Index", "Answer");
        }


        [HttpGet]
        public async Task<ActionResult> UpdateAnswer(int id)
        {
            Answer answer = await _answer.GetAnswer(id);
            ViewBag.ChoiceID = answer.ChoiceID;
            ViewBag.QuestionID = answer.QuestionID;
            return View(answer);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAnswer(Answer model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            await this._answer.UpdateAnswer(model);

            return RedirectToAction("Index", "Answer");
        }
    }
}
