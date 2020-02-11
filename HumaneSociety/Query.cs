using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

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

            return employeeWithUserName != null;
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
                    GetEmployeeByEmployeeNumber(employee.EmployeeNumber);
                    break;
                case "update":
                    UpdateEmployee(employee);
                    break;
                case "delete":
                    RemoveEmployee(employee);
                    break;
                default:
                    break;
            }
        }

        internal static void AddEmployee(Employee employee)
        {
            db.Employees.InsertOnSubmit(employee);
            db.SubmitChanges();
        }

        internal static void GetEmployeeByEmployeeNumber(int? employeeNumber)
        {
            UserInterface.DisplayEmployeeInfo(db.Employees.Where(x => x.EmployeeNumber == employeeNumber).SingleOrDefault());
        }
        internal static void UpdateEmployee(Employee employee)
        {
            var result = db.Employees.Where(x => x.EmployeeNumber == employee.EmployeeNumber).SingleOrDefault();
            result.Email = employee.Email;
            result.FirstName = employee.FirstName;
            result.LastName = employee.LastName;

            db.SubmitChanges();
        }
        internal static void RemoveEmployee(Employee employee)
        {
            db.Employees.DeleteOnSubmit(db.Employees.Where(x => x.EmployeeNumber == employee.EmployeeNumber).SingleOrDefault());
            db.SubmitChanges();
        }

        // TODO: Animal CRUD Operations
        internal static void AddAnimal(Animal animal)
        {
            db.Animals.InsertOnSubmit(animal);
            db.SubmitChanges();
        }

        internal static Animal GetAnimalByID(int id)
        {
            var animal = db.Animals.Where(x => x.AnimalId == id).SingleOrDefault();
            return animal;
        }

        internal static void UpdateAnimal(int animalId, Dictionary<int, string> updates)
        {
            var animal = db.Animals.Where(x => x.AnimalId == animalId).SingleOrDefault();

            foreach (KeyValuePair<int, string> element in updates)
            {
                switch (element.Key)
                {
                    case 1:
                        animal.Category.Name = element.Value;
                        break;
                    case 2:
                        animal.Name = element.Value;
                        break;
                    case 3:
                        animal.Age = Convert.ToInt32(element.Value);
                        break;
                    case 4:
                        animal.Demeanor = element.Value;
                        break;
                    case 5:
                        animal.KidFriendly = element.Value == "true" ? true: false;
                        break;
                    case 6:
                        animal.PetFriendly = element.Value == "true" ? true : false;
                        break;
                    case 7:
                        animal.Weight = Convert.ToInt32(element.Value);
                        break;
                    case 8:
                        animal.AnimalId = Convert.ToInt32(element.Value);
                        break;
                    default:
                        break;
                }
            }
            db.SubmitChanges();
        }

        internal static void RemoveAnimal(Animal animal)
        {
            db.Animals.DeleteOnSubmit(db.Animals.Where(x => x.AnimalId == animal.AnimalId).SingleOrDefault());
            db.SubmitChanges();
        }
        
        // TODO: Animal Multi-Trait Search
        internal static IQueryable<Animal> SearchForAnimalsByMultipleTraits(Dictionary<int, string> updates) // parameter(s)?
        {

            IQueryable<Animal> animals = db.Animals;

            foreach (KeyValuePair<int, string> element in updates)
            {
                switch (element.Key)
                {
                    case 1:
                        animals = animals.Where(x => x.Category.Name == element.Value);
                        break;
                    case 2:
                        animals = animals.Where(x => x.Name == element.Value);
                        break;
                    case 3:
                        animals = animals.Where(x => x.Age == Convert.ToInt32(element.Value));
                        break;
                    case 4:
                        animals = animals.Where(x => x.Demeanor == element.Value);
                        break;
                    case 5:
                        animals = animals.Where(x => x.KidFriendly.ToString() == element.Value);
                        break;
                    case 6:
                        animals = animals.Where(x => x.PetFriendly.ToString() == element.Value);
                        break;
                    case 7:
                        animals = animals.Where(x => x.Weight == Convert.ToInt32(element.Value));
                        break;
                    case 8:
                        animals = animals.Where(x => x.AnimalId == Convert.ToInt32(element.Value));
                        break;
                    default:
                        break;
                }
            }
            return animals;
        }

        // TODO: Misc Animal Things
        internal static int GetCategoryId(string categoryName)
        {
            var category = db.Categories.Where(x => x.Name == categoryName).SingleOrDefault();  // This gets it to of class Category
            var categoryId = category.CategoryId; // Need to access the category id of that category 
            return categoryId;
        }
        
        internal static Room GetRoom(int animalId)
        {
            var room = db.Rooms.Where(x => x.AnimalId == animalId).SingleOrDefault();
            return room;
        }
        
        internal static int GetDietPlanId(string dietPlanName)
        {
            var dietPlan = db.DietPlans.Where(x => x.Name == dietPlanName).SingleOrDefault();
            var dietPlanID = dietPlan.DietPlanId;
            return dietPlanID;
        }

        // TODO: Adoption CRUD Operations
        internal static void Adopt(Animal animal, Client client)
        {
            Adoption newAdoption = new Adoption();
            newAdoption.AnimalId = animal.AnimalId;
            newAdoption.ClientId = client.ClientId;
            newAdoption.Animal = animal;
            newAdoption.Client = client;
            newAdoption.AdoptionFee = 75;
            newAdoption.ApprovalStatus = "Pending";
            newAdoption.PaymentCollected = false;
            ClientAdoptionSetup(client, newAdoption);
            AnimalAdoptionSetup(animal, newAdoption);
            db.SubmitChanges();
        }

        internal static void ClientAdoptionSetup(Client client, Adoption newAdoption)
        {
            client.Adoptions.Add(newAdoption);
        }

        internal static void AnimalAdoptionSetup(Animal animal, Adoption newAdoption)
        {
            animal.AdoptionStatus = "Pending";
            animal.Adoptions.Add(newAdoption);
            db.SubmitChanges();
        }

        internal static IQueryable<Adoption> GetPendingAdoptions()
        {
            return db.Adoptions.Where(x => x.ApprovalStatus == "Pending");
        }

        internal static void UpdateAdoption(bool isAdopted, Adoption adoption)
        {
            var result = db.Adoptions.Where(x => x.AnimalId == adoption.AnimalId).SingleOrDefault();
            if(isAdopted)
            {
                result.ApprovalStatus = "Approved";
                adoption.Animal.AdoptionStatus = "Approved";
                adoption.PaymentCollected = true;
                db.SubmitChanges();
            }
            else
            {
                result.ApprovalStatus = "Denied";
                adoption.Animal.AdoptionStatus = "Not Adopted";
                db.SubmitChanges();
            }
        }

        internal static void RemoveAdoption(int animalId, int clientId)
        {
            db.Adoptions.DeleteOnSubmit(db.Adoptions.Where(x => x.AnimalId == animalId && x.ClientId == clientId).SingleOrDefault());
            db.SubmitChanges();
        }

        // TODO: Shots Stuff
        internal static IQueryable<AnimalShot> GetShots(Animal animal)
        {
            var shots = db.AnimalShots.Where(x => x.Animal.AnimalId == animal.AnimalId);
            return shots;
        }

        internal static void UpdateShot(string shotName, Animal animal)
        {
            Shot shot = new Shot();
            shot.Name = shotName;
            db.Shots.InsertOnSubmit(shot);

            db.SubmitChanges();

            var animalShot = new AnimalShot();

            animalShot.DateReceived = DateTime.Now;
            animalShot.AnimalId = animal.AnimalId;
            animalShot.ShotId = shot.ShotId;
            db.AnimalShots.InsertOnSubmit(animalShot);

            db.SubmitChanges();
        }




        
        public static List<List<string>> ReadFile()
        {
            StreamReader sr = new StreamReader("C:\\Users\\aaron\\Desktop\\DevCodeCamp\\Projects\\HumaneSociety\\animals.csv");
            Regex input = new Regex(@"\d+|\w+");


            string line;
            List<List<string>> list = new List<List<string>>();

            while ((line = sr.ReadLine()) != null)
            {
                list.Add(line.Split(',').ToList());
            }
            for (int j = 0; j < list.Count; j++)
            {
                for (int i = 0; i < list[j].Count; i++)
                {
                    var match = input.Match(list[j][i]);
                    list[j][i] = match.ToString();
                }
            }
            sr.Close();

            return list;
        }

        public static void AddAnimalsFromFile(List<List<string>> animalList)
        {

            for (int i = 0; i < animalList.Count; i++)
            {
                Animal newAnimal = new Animal();

                for (int j = 0; j < animalList[i].Count; j++)
                {
                    switch (j + 1)
                    {
                        case 1:
                            newAnimal.Name = animalList[i][j];
                            break;
                        case 2:
                            newAnimal.Weight = Convert.ToInt32(animalList[i][j]);
                            break;
                        case 3:
                            newAnimal.Age = Convert.ToInt32(animalList[i][j]);
                            break;
                        case 4:
                            newAnimal.Demeanor = animalList[i][j];
                            break;
                        case 5:
                            newAnimal.KidFriendly = animalList[i][j] == "0" ? false: true;
                            break;
                        case 6:
                            newAnimal.PetFriendly = animalList[i][j] == "0" ? false: true;
                            break;
                        case 7:
                            newAnimal.Gender = animalList[i][j];
                            break;
                        case 8:
                            newAnimal.AdoptionStatus = animalList[i][j];
                            break;
                        case 9:
                            newAnimal.CategoryId = animalList[i][j] == "null" ? (int?)null : Convert.ToInt32(animalList[i][j]);
                            break;
                        case 10:
                            newAnimal.DietPlanId = animalList[i][j] == "null" ? (int?)null : Convert.ToInt32(animalList[i][j]);
                            break;
                        case 11:
                            newAnimal.EmployeeId = animalList[i][j] == "null" ? (int?)null : Convert.ToInt32(animalList[i][j]);
                            break;

                    }
                }

                db.Animals.InsertOnSubmit(newAnimal);
                db.SubmitChanges();
            }
        }
    }
}