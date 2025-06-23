using Documentt.Data;
using Documentt.Models;
using Documentt.Models.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Documentt.Controllers
{
    public class ContactController : Controller
    {
        private TailieuContextDB db;
        public ContactController( TailieuContextDB db)
        {
            this.db = db;
        }
        public IActionResult Contact()
        {
            return View(new Contact());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Send(Contact model)
        {
            if (ModelState.IsValid)
            {
                var contactMessage = new Contact
                {
                    Name = model.Name,
                    Email = model.Email,
                    Message = model.Message,
                };

                db.Contacts.Add(contactMessage);
                await db.SaveChangesAsync();


                ViewBag.SuccessMessage = "Tin nhắn của bạn đã được gửi thành công. Chúng tôi sẽ liên hệ lại với bạn sớm nhất!";

                ModelState.Clear();
                return View("Contact", new Contact());

            }


            ViewBag.ErrorMessage = "Vui lòng kiểm tra lại thông tin bạn đã nhập.";
            return View("Contact", model);
        }
        public IActionResult Confirmation()
        {
            return View();
        }
    }
}
