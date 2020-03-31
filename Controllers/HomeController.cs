using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FlashCards.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;




namespace FlashCards.Controllers
{
    public class HomeController : Controller
    {
        private Context dbContext;
        public HomeController(Context context)
        {
            dbContext = context;
        }

    // HANDLING REGISTER PAGE ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

    // HANDLING HOME PAGE ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [HttpGet("home")]
        public IActionResult Home()
        {
            int? Session = HttpContext.Session.GetInt32("UserInSession");
            if(Session == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.CurrentUser = dbContext.User
                .FirstOrDefault(u => u.UserId == (int)HttpContext.Session.GetInt32("UserInSession"));

            return View();
        }
    
    // HANDLING NEW GROUP ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [HttpGet("newgroup")]
        public IActionResult Form_NewGroup()
        {
           int? Session = HttpContext.Session.GetInt32("UserInSession");
            if(Session == null)
            {
                return RedirectToAction("Index");
            }
            return View(); 
        }

    // HANDLING GROUP HOME ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [HttpGet("grouphome/{IdGroup}")]
        public IActionResult GroupHome(int IdGroup)
        {
           int? Session = HttpContext.Session.GetInt32("UserInSession");
            if(Session == null)
            {
                return RedirectToAction("Index");
            }
            Group ThisGroup = dbContext.Group
                .Include(C => C.FlashCards)
                .FirstOrDefault(G => G.GroupId == IdGroup);

            return View(ThisGroup); 
        }

    // HANDLING ALL GROUPS PAGE ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [HttpGet("allgroups")]
        public IActionResult AllGroups()
        {
          int? Session = HttpContext.Session.GetInt32("UserInSession");
            if(Session == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.EveryGroup = dbContext.Group
                .ToList();
            return View();   
        }
    
    // HANDLING LOGOUT ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [HttpGet("logout")]
        public IActionResult logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Index");
        }

    // HANDLING NEW CARD FORM ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [HttpGet("newcard/{IdGroup}")]
        public IActionResult Form_NewCard(int IdGroup)
        {
            int? Session = HttpContext.Session.GetInt32("UserInSession");
            if(Session == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.ThisGroup = dbContext.Group
                .FirstOrDefault(G => G.GroupId == IdGroup);
            
            return View();
        }

    // HANDLING ALL CARDS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [HttpGet("allcards/{IdGroup}")]
        public IActionResult AllCards(int IdGroup)
        {
            int? Session = HttpContext.Session.GetInt32("UserInSession");
            if(Session == null)
            {
                return RedirectToAction("Index");
            }

            List<Card> EveryCard = dbContext.Card
                .Where(G => G.GroupId == IdGroup)
                .ToList();
            ViewBag.ThisGroup = dbContext.Group
                .FirstOrDefault(G => G.GroupId == IdGroup);

            return View(EveryCard);
        }

    // HANDLING CARD DELETION ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [HttpGet("deletecard/{IdCard}/{IdGroup}")]
        public IActionResult deletecard(int IdCard, int IdGroup)
        {
            Card CardToDelete = dbContext.Card
                .FirstOrDefault(C => C.CardId == IdCard);
            dbContext.Remove(CardToDelete);
            dbContext.SaveChanges();
            return Redirect("/allcards/" + IdGroup);
        }

    // HANDLING EDIT PAGE ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [HttpGet("editcard/{IdCard}/{IdGroup}")]
        public IActionResult Form_EditCard(int IdCard, int IdGroup)
        {
            int? Session = HttpContext.Session.GetInt32("UserInSession");
            if(Session == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.ThisCard = dbContext.Card
                .FirstOrDefault(C => C.CardId == IdCard);
            ViewBag.ThisGroup = dbContext.Group
                .FirstOrDefault(G => G.GroupId == IdGroup);
            return View();
        }

    // HANDLING QUIZ PAGE ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // [HttpGet("quiz/{IdGroup}")]
        // public IActionResult Quiz(int IdGroup)
        // {
            
        // }
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~POST~~~~~REQUESTS~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    // HANDLING REGISTRATION ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [HttpPost("/register")]
        public IActionResult register(User NewUser)
        {
            if(ModelState.IsValid)
            {
                if(dbContext.User.Any(u => u.Email == NewUser.Email))
                {
                    ModelState.AddModelError("Email", "Email is already in use.");
                    return View("Index");
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                NewUser.Password = Hasher.HashPassword(NewUser, NewUser.Password);

                dbContext.Add(NewUser);
                dbContext.SaveChanges();
                HttpContext.Session.SetInt32("UserInSession", NewUser.UserId);
                return RedirectToAction("Home");
            }
            return View("Index");
        }
     // HANDLING LOGIN ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [HttpPost("/login")]
        public IActionResult login(LoginUser AccessUser)
        {
            if(ModelState.IsValid)
            {
                var UserInDb = dbContext.User.FirstOrDefault(u => u.Email == AccessUser.LEmail); 
                if(UserInDb == null)
                {
                    ModelState.AddModelError("LEmail", "Invalid Email/Password");
                    return View("Index");
                }
                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(AccessUser, UserInDb.Password, AccessUser.LPassword);
                if(result == 0)
                {
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("Index");
                }
                HttpContext.Session.SetInt32("UserInSession", UserInDb.UserId);
                return RedirectToAction("Home");
            }
            return View("Index");
        }
        
    // HANDLING NEW GROUP ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [HttpPost("newgroup")]
        public IActionResult newgroup(Group NewGroup)
        {
            if(ModelState.IsValid)
            {
                NewGroup.UserId = (int)HttpContext.Session.GetInt32("UserInSession");
                dbContext.Add(NewGroup);
                dbContext.SaveChanges();
                return Redirect("/grouphome/"+ NewGroup.GroupId);
            }
            return View("Form_NewGroup");
        }
    
    // HANDLING CARD CREATOR ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [HttpPost("cardcreator/{IdGroup}")]
        public IActionResult cardcreator(int IdGroup, Card NewCard)
        {
            NewCard.GroupId = IdGroup;
            dbContext.Add(NewCard);
            dbContext.SaveChanges();
            return Redirect("/newcard/" + IdGroup);
        }

    // HANDLING CARD EDIT ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [HttpPost("cardeditor/{IdCard}/{IdGroup}")]
        public IActionResult cardeditor(int IdCard, int IdGroup, Card UpdatedCard)
        {
            Card ThisCard = dbContext.Card
                .FirstOrDefault(C => C.CardId == IdCard);
            ThisCard.Question = UpdatedCard.Question;
            ThisCard.Answer = UpdatedCard.Answer;
            dbContext.SaveChanges();
            return Redirect("/allcards/" + IdGroup);
        }


    }
}
