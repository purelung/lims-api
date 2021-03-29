using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Data.SqlClient;
using System.Text;
using System.Collections.Generic;
using ZeeReportingApi.Model;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ZeeReportingApi.Data
{
    public class UsersRepo
    {
        private readonly ODSContext _context;
        public UsersRepo(ODSContext context)
        {
            _context = context;
        }

        public int AddUser(User user)
        {
            var newUser = _context.User.Add(user);
            _context.SaveChanges();
            return newUser.Entity.Id;
        }

        public void UpdateUser(User user)
        {
            _context.User.Update(user);
            _context.SaveChanges();
        }

        public void AddUserSalons(List<UserXSalon> userSalons)
        {
            _context.UserXSalon.AddRange(userSalons);
            _context.SaveChanges();
        }

        public void RemoveUserSalons(int userId, List<int> salonIds)
        {
            var records = _context.UserXSalon.Where(u => u.UserId == userId && salonIds.Contains(u.SalonId)).ToList();

            if (records.Count > 0)
                _context.UserXSalon.RemoveRange(records);

            _context.SaveChanges();
        }

        public void UpdateUserSalons(List<UserXSalon> newUserSalons)
        {
            var userId = newUserSalons.Select(u => u.UserId).FirstOrDefault();
            var oldUserSalonIds = _context.UserXSalon.Where(u => u.UserId == userId).Select(u => u.SalonId).ToList();
            var newUserSalonIds = newUserSalons.Select(u => u.SalonId).ToList();

            var salonIdsToAdd = newUserSalonIds.Except(oldUserSalonIds).ToList();

            if (salonIdsToAdd.Count() > 0)
            {
                AddUserSalons(salonIdsToAdd.Select(sid => new UserXSalon() { UserId = userId, SalonId = sid }).ToList());
            }

            var salonIdsToRemove = oldUserSalonIds.Except(newUserSalonIds).ToList();

            if (salonIdsToRemove.Count() > 0)
            {
                RemoveUserSalons(userId, salonIdsToRemove);
            }
        }

        public List<User> GetUser(string email)
        {
            return _context.User.Where(u => u.Email == email).ToList();
        }
        public List<User> GetUser(int id)
        {
            return _context.User.Where(u => u.Id == id).ToList();
        }

        public List<User> GetUsers(int franchiseId)
        {
            return _context.User.Where(u => u.FranchiseId == franchiseId).ToList();
        }
    }
}
