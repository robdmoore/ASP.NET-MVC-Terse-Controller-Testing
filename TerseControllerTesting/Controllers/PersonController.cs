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

        // Needed for the dynamic proxy generation for some reason :S
        public PersonController() {}

        //
        // GET: /Person/

        public virtual ActionResult Index()
        {
            return View(_personRepository.GetAll());
        }

        //
        // GET: /Person/Create

        public virtual ActionResult Create()
        {
            return View("Edit");
        } 

        //
        // POST: /Person/Create

        [HttpPost]
        public virtual ActionResult Create(Person person)
        {
            if (ModelState.IsValid)
            {
                if (!_personRepository.EmailBelongsToSomeoneElse(person.EmailAddress))
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
 
        public virtual ActionResult Edit(int id)
        {
            var person = _personRepository.GetById(id);
            if (person == null)
                return new HttpStatusCodeResult(404);

            return View(person);
        }

        //
        // POST: /Person/Edit/5

        [HttpPost]
        public virtual ActionResult Edit(int id, Person person)
        {
            if (ModelState.IsValid)
            {
                if (!_personRepository.EmailBelongsToSomeoneElse(person.EmailAddress, id))
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
