using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Data;
using WebApi.Data.Entities;
using WebApi.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApi.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class TrainingController : ControllerBase
    {
        private WebApiDbContext _context;
        private IUserService _userService;
        private ITrainingService _trainingService;

        public TrainingController(
            IUserService userService, 
            ITrainingService trainingService, 
            WebApiDbContext context)
        {
            _trainingService = trainingService;
            _userService = userService;
            _context = context;
        }
        
        // GET: api/<TrainingController>
        [HttpGet]
        public IActionResult GetAll()
        {
            var training = _trainingService.GetAll();
            return Ok(training);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var training = _trainingService.GetById(id);
            return Ok(training);
        }
        // POST api/<TrainingController>
        [HttpPost]
        public IActionResult CreateTraining (Training training)
        {
            var _training = _trainingService.CreateTraining(training);
            return Ok();
        }

        // PUT api/<TrainingController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTraining(Training training, int id)
        {
            if (training.Id != id)
            {
                return BadRequest();
            }
            
            _context.Entry(training).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TrainingExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }

            }
            return NoContent();
        }

        // DELETE api/<TrainingController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTraining(int id)
        {
            var training = await _context.Training.FindAsync(id);
            if (training == null)
            {
                return NotFound();
            }
            _context.Training.Remove(training);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        private bool TrainingExists(int id)
        {
            return _context.Training.Any(t => t.Id == id);
        }
    }
}
