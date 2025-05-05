using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using ClinicLogic.Models;
using Services.GiftServices.Models;
using System.Text.Json;

namespace Services.GiftServices.Managers
{
    public class GiftManager
    {
        private readonly AppConfig _config;

        public GiftManager(IOptions<AppConfig> config)
        {
            _config = config.Value;
        }

        public string GetUrl() => _config.GiftsApiUrl;
        public string GetPathUsers() => _config.UsersFilePath;

        private List<User> ReadAllUsers()
        {
            var users = new List<User>();

            using (var reader = new StreamReader(GetPathUsers()))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    //the users are stored as: CI,gift1;gift2;gift3...
                    //using the injected patients list to add to the corresponding CI the patient to the list
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    //a line looks like this 1,{Gift Object},{Gift Object} with the possilibity of having no gifts and a lot of gifts
                    // the first part is the patient ID, and the second part is the gift list
                    //inside the each gift object the data is separated by '|' so we can use ';' to separate the gifts
                    var parts = line.Split('|');
                        var patient = ReadAllPatients().FirstOrDefault(p => p.CI == parts[0]);
                    if (patient == null) continue;
                        var user = new User(patient);
                        users.Add(user);
                        var gifts = parts[1].Split(';');
                    foreach (var gift in gifts)
                    {
                        if (string.IsNullOrWhiteSpace(gift)) continue;
                        //a gift object looks like {"id":"1","name":"Google Pixel 6 Pro","data":{JsonElement}}
                        Gift giftObj = JsonSerializer.Deserialize<Gift>(gift);
                        if (giftObj != null)
                        {
                            user.gifts.Add(giftObj);
                        }
                    }

                }
            }

            return users;
        }

        private List<Patient> ReadAllPatients() //no quise colocar un manager en esta clase, sin esto, no se podría hacer una lista de gifts
        {
            var patients = new List<Patient>();

            using (var reader = new StreamReader(_config.PatientsFilePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var parts = line.Split(',');
                    patients.Add(new Patient(parts[2], parts[0], parts[1])
                    {
                        BloodType = parts[3]
                    });
                }
            }

            return patients;
        }


        private void SaveAllUsers(List<User> users)
        {
            using (var writer = new StreamWriter(GetPathUsers()))
            {
                foreach (var patient in users)
                {
                    //it stores the patient ID, and the gift list, which is stored as the Gift objects
                    //a gift object looks like {"id":"1","name":"Google Pixel 6 Pro","data":{JsonElement}}
                    writer.WriteLine($"{patient.Id}|{string.Join(";", patient.gifts.Select(g => JsonSerializer.Serialize(g)))}");
                }
            }
        }

        public void AddUser(User user)
        {
            var users = ReadAllUsers();

            if (users.Any(u => u.Id == user.Id))
                throw new Exception($"User with ID {user.Id} already exists");

            users.Add(user);
            SaveAllUsers(users);
        }

        public User GetUser(string id)
        {
            var users = ReadAllUsers();
            return users.FirstOrDefault(u => u.Id == id)
                   ?? throw new Exception($"User {id} not found");
        }

        public void UpdateUser(string id, User updatedUser)
        {
            var users = ReadAllUsers();
            var index = users.FindIndex(u => u.Id == id);

            if (index == -1)
                throw new Exception($"User {id} not found");

            users[index] = updatedUser;
            SaveAllUsers(users);
        }

        public void DeleteUser(string id)
        {
            var users = ReadAllUsers();
            var user = users.FirstOrDefault(u => u.Id == id);

            if (user == null)
                throw new Exception($"User {id} not found");

            users.Remove(user);
            SaveAllUsers(users);
        }

        public void AddGiftToUser(string userId, Gift gift)
        {
            var users = ReadAllUsers();
            //after searching, if there is no user assigned to the patient, make and add one for it
            var user = users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                var patient = ReadAllPatients().FirstOrDefault(p => p.CI == userId);
                if (patient == null)
                    throw new Exception($"Patient {userId} not found");
                user = new User(patient);
                users.Add(user);
            }
            user.gifts.Add(gift);
            SaveAllUsers(users);
        }

        public List<Gift> GetUserGifts(string userId)
        {
            return GetUser(userId).gifts;
        }

        public List<User> GetUsers()
        {
            return ReadAllUsers();
        }
    }
}
