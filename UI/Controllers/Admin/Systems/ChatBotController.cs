using Azure;
using DataAccess.Entities.Systems;
using Microsoft.AspNetCore.Mvc;
using Services.Systems;
using System.Threading.Tasks;
using UI.Helper;
using UI.Security;

namespace UI.Controllers.Admin.Systems
{
    [SetViewDataFilter]
    public class ChatBotController : Controller
    {
        private readonly IChatBotService _chatBotService;

        public ChatBotController(IChatBotService chatBotService)
        {
            _chatBotService = chatBotService;
        }


        [HttpGet("Systems/ChatBot")]
        [AuthorizeAction("Index")]
        public async Task<IActionResult> Index(string TimKiem = "", int PageSize = 5, int PageCurrent = 1)
        {
            PageSize = PageSize < 5 ? 5 : PageSize > 100 ? 100 : PageSize;
            PageCurrent = PageCurrent < 1 ? 1 : PageCurrent;

            var model = await _chatBotService.GetChatBotAsync(TimKiem, PageSize, PageCurrent);

            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "Home";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            ViewData["Title"] = "Quản lý ChatBot";
            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(model.TotalRecord, TimKiem, PageSize, PageCurrent);
            return View("Views/Admin/Systems/ChatBot/Index.cshtml", model.Data);
        }

        [HttpPost("Systems/ChatBot/Create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create()
        {
            var model = new QuestionAnswer(); // Không cần new Guid ở đây
            return PartialView("~/Views/Admin/Systems/ChatBot/_FormFields.cshtml", model);
        }

        [HttpPost("Systems/ChatBot/Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id)
        {
            var result = await _chatBotService.EditAsync(id);
            if (result.Status != "success" || result.Data == null)
            {
                ViewData["Messages"] = "Không tìm thấy câu hỏi!";
                ViewData["Controller"] = "ChatBot";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            var model = result.Data as QuestionAnswer;
            return PartialView("~/Views/Admin/Systems/ChatBot/_FormFields.cshtml", model);
        }

        [HttpGet("Systems/ChatBot/GetQuestions")]
        public async Task<IActionResult> GetQuestions(string moTa)
        {
            var questions = await _chatBotService.GetQuestionsAsync(moTa);
            return Json(questions);
        }

        [HttpGet("Systems/ChatBot/GetAnswer/{guid}")]
        public async Task<IActionResult> GetAnswer(Guid guid)
        {
            var answer = await _chatBotService.GetAnswerAsync(guid);
            return Json(answer);
        }

        [HttpPost("Systems/ChatBot/Store")]
        [AuthorizeAction("Store")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Store(string question, string answer, string description)
        {
            var model = await _chatBotService.StoreAsync(question, answer, description);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "VanBanPhapLuat";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            return Json(model);
        }

        [HttpPost("Systems/ChatBot/Update")]
        [AuthorizeAction("Update")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Guid id, string question, string answer, string description)
        {
            var model = await _chatBotService.UpdateAsync(id, question, answer, description);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "VanBanPhapLuat";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            return Json(model);
        }

        [HttpPost("Systems/ChatBot/Delete")]
        [AuthorizeAction("Delete")]
        public async Task<IActionResult> Delete(Guid id_delete)
        {
            var model = await _chatBotService.DeleteAsync(id_delete);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                return View("Views/Shared/Error.cshtml");
            }
            return RedirectToAction("Index", "ChatBot");
        }
    }
}
