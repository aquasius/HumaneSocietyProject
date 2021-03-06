﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    public static class Query
    {        

        static HumaneSocietyDataContext db;

        static Query()
        {
            db = new HumaneSocietyDataContext();
        }

        internal static List<USState> GetStates()
        {
            List<USState> allStates = db.USStates.ToList();       

            return allStates;
        }
            
        internal static Client GetClient(string userName, string password)
        {
            Client client = db.Clients.Where(c => c.UserName == userName && c.Password == password).Single();

            return client;
        }

        internal static List<Client> GetClients()
        {
            List<Client> allClients = db.Clients.ToList();

            return allClients;
        }

        internal static void AddNewClient(string firstName, string lastName, string username, string password, string email, string streetAddress, int zipCode, int stateId)
        {
            Client newClient = new Client();

            newClient.FirstName = firstName;
            newClient.LastName = lastName;
            newClient.UserName = username;
            newClient.Password = password;
            newClient.Email = email;

            Address addressFromDb = db.Addresses.Where(a => a.AddressLine1 == streetAddress && a.Zipcode == zipCode && a.USStateId == stateId).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if (addressFromDb == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = streetAddress;
                newAddress.City = null;
                newAddress.USStateId = stateId;
                newAddress.Zipcode = zipCode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                addressFromDb = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            newClient.AddressId = addressFromDb.AddressId;

            db.Clients.InsertOnSubmit(newClient);

            db.SubmitChanges();
        }

        internal static void UpdateClient(Client clientWithUpdates)
        {
            // find corresponding Client from Db
            Client clientFromDb = null;

            try
            {
                clientFromDb = db.Clients.Where(c => c.ClientId == clientWithUpdates.ClientId).Single();
            }
            catch(InvalidOperationException e)
            {
                Console.WriteLine("No clients have a ClientId that matches the Client passed in.");
                Console.WriteLine("No update have been made.");
                return;
            }
            
            // update clientFromDb information with the values on clientWithUpdates (aside from address)
            clientFromDb.FirstName = clientWithUpdates.FirstName;
            clientFromDb.LastName = clientWithUpdates.LastName;
            clientFromDb.UserName = clientWithUpdates.UserName;
            clientFromDb.Password = clientWithUpdates.Password;
            clientFromDb.Email = clientWithUpdates.Email;

            // get address object from clientWithUpdates
            Address clientAddress = clientWithUpdates.Address;

            // look for existing Address in Db (null will be returned if the address isn't already in the Db
            Address updatedAddress = db.Addresses.Where(a => a.AddressLine1 == clientAddress.AddressLine1 && a.USStateId == clientAddress.USStateId && a.Zipcode == clientAddress.Zipcode).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if(updatedAddress == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = clientAddress.AddressLine1;
                newAddress.City = null;
                newAddress.USStateId = clientAddress.USStateId;
                newAddress.Zipcode = clientAddress.Zipcode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                updatedAddress = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            clientFromDb.AddressId = updatedAddress.AddressId;
            
            // submit changes
            db.SubmitChanges();
        }
        
        internal static void AddUsernameAndPassword(Employee employee)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).FirstOrDefault();

            employeeFromDb.UserName = employee.UserName;
            employeeFromDb.Password = employee.Password;

            db.SubmitChanges();
        }

        internal static Employee RetrieveEmployeeUser(string email, int employeeNumber)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.Email == email && e.EmployeeNumber == employeeNumber).FirstOrDefault();

            if (employeeFromDb == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return employeeFromDb;
            }
        }

        internal static Employee EmployeeLogin(string userName, string password)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.UserName == userName && e.Password == password).FirstOrDefault();

            return employeeFromDb;
        }

        internal static bool CheckEmployeeUserNameExist(string userName)
        {
            Employee employeeWithUserName = db.Employees.Where(e => e.UserName == userName).FirstOrDefault();

            return employeeWithUserName == null;
        }


        //// TODO Items: ////
        
        // TODO: Allow any of the CRUD operations to occur here
        internal static void RunEmployeeQueries(Employee employee, string crudOperation)
        {
            switch(crudOperation)
            {
                case "create":
                    AddEmployee(employee);
                    break;
                case "read":
                    ReadEmployee(employee);
                    break;
                case "update":
                    UpdateEmployee(employee);
                        break;
                case "delete":
                    DeleteEmployee(employee);
                    break;
            }
        }

        // TODO: Animal CRUD Operations
        public static void AddEmployee(Employee employee)
        {
            db.Employees.InsertOnSubmit(employee);
            db.SubmitChanges();
        }
        
        public static void ReadEmployee(Employee employee)
        {
            var employeeToRead = db.Employees.FirstOrDefault(e => e.EmployeeNumber == employee.EmployeeNumber);
            List<string> info = new List<string>() { "Employee Number : " + employeeToRead.EmployeeNumber + "Name : " + employeeToRead.FirstName + employeeToRead.LastName, "Email : " + employeeToRead.Email };
            UserInterface.DisplayUserOptions(info);
            Console.ReadLine();
        }
        
        public static void UpdateEmployee(Employee employee)
        {
            var employeeToUpdate = db.Employees.Where(r => r.EmployeeId == employee.EmployeeId).FirstOrDefault();
            employeeToUpdate.FirstName = employee.FirstName;
            employeeToUpdate.LastName = employee.LastName;
            employeeToUpdate.EmployeeNumber = employee.EmployeeNumber;
            employeeToUpdate.Email = employee.Email;
            db.SubmitChanges();
        }

        public static void DeleteEmployee(Employee employee)
        {
            db.Employees.DeleteOnSubmit(employee);
            db.SubmitChanges();
            Console.WriteLine("employee deleted");
        }

        internal static void AddAnimal(Animal animal)
        {
            db.Animals.InsertOnSubmit(animal);
            db.SubmitChanges();
            Console.WriteLine("Animal Added.");
        }

        internal static Animal GetAnimalByID(int id)
        {
            return db.Animals.Where(a => a.AnimalId == id).Single();
        }

        public static void UpdateAnimal(int animalId, Dictionary<int, string> updates)
        {
            Animal dbAnimal = db.Animals.Where(a => a.AnimalId == animalId).Single();
            foreach(KeyValuePair<int,string> update in updates)
            {
                switch (update.Key)
                {
                    case 1:
                        dbAnimal.Category.Name = update.Value;
                        break;
                    case 2:
                        dbAnimal.Name = update.Value;
                        break;
                    case 3:
                        dbAnimal.Age = Convert.ToInt32(update.Value);
                        break;
                    case 4:
                        dbAnimal.Demeanor = update.Value;
                        break;
                    case 5:
                        dbAnimal.KidFriendly = Convert.ToBoolean(update.Value);
                        break;
                    case 6:
                        dbAnimal.PetFriendly = Convert.ToBoolean(update.Value);
                        break;
                    case 7:
                        dbAnimal.Weight = Convert.ToInt32(update.Value);
                        break;
                    default:
                        break;
                }
            }  
            db.SubmitChanges();
        }

        internal static void RemoveAnimal(Animal animal)
        {
            db.Animals.DeleteOnSubmit(animal);
            db.SubmitChanges();

            Console.WriteLine("Animal Removed.");
        }
        
        // TODO: Animal Multi-Trait Search
        internal static IQueryable<Animal> SearchForAnimalsByMultipleTraits(Dictionary<int, string> updates) // parameter(s)?
        {
            var results = db.Animals.AsQueryable();

            foreach (KeyValuePair<int,string> update in updates)
            { 
                    switch (update.Key)
                    {
                        case 1:
                            var dbCatagory = db.Categories.Where(a => a.Name == update.Value).FirstOrDefault();
                            var dbCatagoryId = dbCatagory.CategoryId;
                            var dbCatagoryResult = results.Where(a => a.CategoryId == dbCatagoryId);
                            results = dbCatagoryResult;
                            break;
                        case 2:
                            var dbName = results.Where(a => a.Name == update.Value);
                            results = dbName;
                            break;
                        case 3:
                            var dbAge = results.Where(a => a.Age == Convert.ToInt32(update.Value));
                            results = dbAge;
                            break;
                        case 4:
                            var dbDemeanor = results.Where(a => a.Demeanor == update.Value);
                            results = dbDemeanor;
                            break;
                        case 5:
                            var dbKidFriendly = results.Where(a => a.KidFriendly == Convert.ToBoolean(update.Value));
                            results = dbKidFriendly;    
                            break;
                        case 6:
                            var dbPetFriendly = results.Where(a => a.PetFriendly == Convert.ToBoolean(update.Value));
                            results = dbPetFriendly;
                            break;
                        case 7:
                            var dbWeight = results.Where(a => a.Weight == Convert.ToInt32(update.Value));
                            results = dbWeight;
                            break;
                        case 8:
                            var dbAnimalId = results.Where(a => a.AnimalId == Convert.ToInt32(update.Value));
                            results = dbAnimalId;
                            break;
                        default:
                            UserInterface.DisplayUserOptions("Input not recognized please try again."); //this the function we want?
                            break;
                    }
            }
            return results;
        }
         
        // TODO: Misc Animal Things
        internal static int GetCategoryId(string categoryName)
        {
            try
            {
                Category foundCategoryId = db.Categories.Where(a => a.Name == categoryName).Single();
                return foundCategoryId.CategoryId;
            }
            catch(NullReferenceException)
            {
                AddCategoryId(categoryName);
                Category foundCategoryId = db.Categories.Where(a => a.Name == categoryName).Single();
                return foundCategoryId.CategoryId;
            }

        }
        
        internal static void AddCategoryId(string categoryName)
        {
            Category createCategory = new Category();
            createCategory.Name = categoryName;
            db.Categories.InsertOnSubmit(createCategory);
            db.SubmitChanges();
           
        }

        internal static Room GetRoom(int animalId)
        {
            var foundAnimalRoom = db.Rooms.Where(a => a.AnimalId == animalId).FirstOrDefault();
            return foundAnimalRoom;
        }
        
        internal static int GetDietPlanId(string dietPlanName)
        {
            try
            {
                DietPlan foundDietPlan = db.DietPlans.Where(a => a.Name == dietPlanName).Single();
                return foundDietPlan.DietPlanId;
            }
            catch(NullReferenceException)
            {
                AddDietPlanId(dietPlanName);
                DietPlan foundDietPlan = db.DietPlans.Where(a => a.Name == dietPlanName).Single();
                return foundDietPlan.DietPlanId;
            }
        }

        internal static void AddDietPlanId(string dietPlanName)
        {
            DietPlan newDietPlan = new DietPlan();
            newDietPlan.Name = dietPlanName;
            db.DietPlans.InsertOnSubmit(newDietPlan);
            db.SubmitChanges();
        }
     
        // TODO: Adoption CRUD Operations
        internal static void Adopt(Animal animal, Client client)
        {
            Adoption adoption = new Adoption();
            adoption.AnimalId = animal.AnimalId;
            adoption.ClientId = client.ClientId;
            adoption.AdoptionFee = 80;
            adoption.ApprovalStatus = "pending";
            db.Adoptions.InsertOnSubmit(adoption);
            db.SubmitChanges();
        }

        internal static IQueryable<Adoption> GetPendingAdoptions()
        {

            return db.Adoptions.Where(a => a.ApprovalStatus == "pending");
            
        }

        internal static void UpdateAdoption(bool isAdopted, Adoption adoption)
        {
           
            if(isAdopted == true)
            {
                Adoption adoptionUpdate = db.Adoptions.Where(a => a.AnimalId == adoption.AnimalId && a.ClientId == adoption.ClientId).Select(a => a).Single();
                adoptionUpdate.ApprovalStatus = "Approved";
                adoptionUpdate.PaymentCollected = true;
                db.SubmitChanges();
            }
        }

        internal static void RemoveAdoption(int animalId, int clientId)
        {
            Adoption searchAndRemove = db.Adoptions.Where(a => a.AnimalId == animalId && a.ClientId == clientId).Single();
            db.Adoptions.DeleteOnSubmit(searchAndRemove);
            db.SubmitChanges();
        }

        // TODO: Shots Stuff
        internal static IQueryable<AnimalShot> GetShots(Animal animal)
        {
             return db.AnimalShots.Where(a => a.AnimalId == animal.AnimalId).Select(a => a);

        }

        internal static void UpdateShot(string shotName, Animal animal)
        {
            AnimalShot updateNewShot = new AnimalShot();
            updateNewShot.AnimalId = animal.AnimalId;

            var shotGivenToAnimal = db.Shots.Where(s => s.Name == shotName).Single();

            updateNewShot.ShotId = shotGivenToAnimal.ShotId;
            updateNewShot.DateReceived = DateTime.Now;
            db.AnimalShots.Where(a => a.AnimalId == shotGivenToAnimal.ShotId);

            db.AnimalShots.InsertOnSubmit(updateNewShot);
            db.SubmitChanges();
            
        }
    }
}