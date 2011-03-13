using System.Web.Mvc;
using TerseControllerTesting.Data;
using TerseControllerTesting.Models;

namespace TerseControllerTesting.Controllers
{
    public class PersonController : Controller
    {
        private readonly IPersonRepository _personRepository;

        public PersonController(IPersonRepository personRepository)
        {
            _personRepository = personRepository;
        }

        //
        // GET: /Person/

        public ActionResult Index()
        {
            return View(_personRepository.GetAll());
        }

        //
        // GET: /Person/Create

        public ActionResult Create()
        {
            return View("Edit");
        } 

        //
        // POST: /Person/Create

        [HttpPost]
        public ActionResult Create(Person person)
        {
            if (ModelState.IsValid)
            {
                if (!_personRepository.EmailExists(person.EmailAddress))
                {
                    _personRepository.Save(person);
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("EmailAddress", "The Email address must be unique; that email address already exists in the system.");
                }
            }

            return View("Edit", person);
        }
        
        //
        // GET: /Person/Edit/5
 
        public ActionResult Edit(int id)
        {
            return View(_personRepository.GetById(id));
        }

        //
        // POST: /Person/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, Person person)
        {
            if (ModelState.IsValid)
            {
                if (!_personRepository.EmailExists(person.EmailAddress, id))
                {
                    person.Id = id;
                    _personRepository.Save(person);
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("EmailAddress", "The Email address must be unique; that email address already exists in the system.");
                }
            }

            return View(person);
        }
    }
}
