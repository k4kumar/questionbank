using Microsoft.AspNetCore.Mvc;
using Quiz_Application.Services.Entities;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quiz_Application.Services.Repository.Interfaces;
using Quiz_Application.Web.Authentication;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Quiz_Application.Web.Controllers
{
    public class ChoiceController : Controller
    {
        private readonly IChoice<Services.Entities.Choice> _choice;
        public ChoiceController(IChoice<Services.Entities.Choice> choice)
        {
            _choice = choice;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IEnumerable<Choice> lst = await _choice.GetChoiceList();
            return View(lst.OrderByDescending(e => e.ChoiceID));
        }

        [HttpGet]
        public IActionResult AddChoice()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddChoice(Choice model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            await this._choice.AddChoice(model);

            return RedirectToAction("Index", "Choice");
        }

        [HttpGet]
        [Route("~/api/Choice/{QuestionID?}")]
        public async Task<IActionResult> Choice(int QuestionID)
        {
            try
            {
                IEnumerable<Choice> choices = await _choice.GetChoiceList(QuestionID);
                return Ok(choices);
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
            Choice choice = await _choice.GetChoice(id);
            return View(choice);
        }


        public async Task<ActionResult> Delete(int id)
        {
            Choice choice = await _choice.GetChoice(id);
            int output = await _choice.DeleteChoice(choice);
            return RedirectToAction("Index", "Choice");
        }
    }
}
