using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebStore.Domain.Entities;
using WebStore.Interfaces.Service;

namespace WebStore.ServiceHosting.Controllers
{
    //[Route("api/[controller]")] // http://localhost:5001/api/EmployeesApi
    [Route("api/employees")]      // http://localhost:5001/api/employees - customized path to controller
    [Produces("application/json")]   // output format of the result
    [ApiController]
    public class EmployeesApiController : ControllerBase, IEmployeesData
    {
        private readonly IEmployeesData _EmployeesData;

        public EmployeesApiController(IEmployeesData EmployeesData)
        {
            _EmployeesData = EmployeesData;
        }

        [HttpGet]        // GET http://localhost:5001/api/employees
        //[HttpGet("all")] // GET http://localhost:5001/api/employees/all
        public IEnumerable<Employee> Get()
        {
            return _EmployeesData.Get();
        }

        [HttpGet("{id}")]        // GET http://localhost:5001/api/employees/5
        public Employee GetById(int id)
        {
            return _EmployeesData.GetById(id);
        }

        [HttpPost] // Post http://localhost:5001/api/employees ???
        public int Add(Employee employee)
        {
            var id = _EmployeesData.Add(employee);
            SaveChanges();
            return id;
        }

        [HttpPut] //  Put http://localhost:5001/api/employees ???
        public void Edit(Employee employee)
        {
            _EmployeesData.Edit(employee);
            SaveChanges();
        }

        [HttpDelete("{id}")] // DELETE http://localhost:5001/api/employees/5
        //[HttpDelete("delete/{id}")]    // DELETE http://localhost:5001/api/employees/delete/5
        //[HttpDelete("delete({id})")]    // DELETE http://localhost:5001/api/employees/delete(5)
        public bool Delete(int id)
        {
            var result = _EmployeesData.Delete(id);
            SaveChanges();
            return result;
        }

        // Будет ошибка при автоматизированной генерации документации по WebAPI
        //[NonAction]
        public void SaveChanges()
        {
            _EmployeesData.SaveChanges();
        }
    }
}