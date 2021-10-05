using BCryptNet = BCrypt.Net.BCrypt;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Authorization;
using WebApi.Data;
using WebApi.Data.Entities;
using WebApi.Helpers;
using WebApi.Models.Users;

namespace WebApi.Services
{
    public interface ITrainingService
    {
        //AuthenticateResponse Authenticate(AuthenticateRequest model, string ipAddress);
        //AuthenticateResponse RefreshToken(string token, string ipAddress);
        //void RevokeToken(string token, string ipAddress);
        
        IEnumerable<Training> GetAll();
        Training GetById(int id);
        Task<Training> CreateTraining(Training training);
    }
    public class TrainingService : ITrainingService
    {
        private WebApiDbContext _context;
        private IJwtUtils _jwtUtils;
        private readonly AppSettings _appSettings;
        private UserService _userService;
        public TrainingService(
            WebApiDbContext context,
            IJwtUtils jwtUtils,
            IOptions<AppSettings> appSettings,
            UserService userService)
        {
            _context = context;
            _jwtUtils = jwtUtils;
            _appSettings = appSettings.Value;
            _userService = userService;
        }
        //métodos de autenticacion de usuario 
        //Userservice.Authenticate(AuthenticateRequest model, string ipAddress)
        
        public IEnumerable<Training> GetAll()
        {
            return _context.Training;
        }

        public Training GetById(int id)
        {
            var training = _context.Training.Find(id);
            if (training == null) throw new KeyNotFoundException("Training not found");
            return training;
        }
        public async Task<Training> CreateTraining(Training training)
        {
            var _training = await _context.Training.AddAsync(training);
            await _context.SaveChangesAsync();
            return _training.Entity;
        }
        public async Task<Training> UpdateTraining(Training training)
        {
            var _training = _context.Training.Update(training);
            await _context.SaveChangesAsync();
            return _training.Entity;
        }
        public async Task<Training> DeleteTraining(Training training)
        {
            var _training = _context.Training.Remove(training);
            await _context.SaveChangesAsync();
            return _training.Entity;
        }
        
    }
}
