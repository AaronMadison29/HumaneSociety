using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HumaneSociety
{
    static class CSVReader
    {
        public static List<List<string>> ReadFile()
        {
            StreamReader sr = new StreamReader("C:\\Users\\aaron\\Desktop\\DevCodeCamp\\Projects\\HumaneSociety\\animals.csv");
            Regex input = new Regex(@"\d+|\w+");

            var lines = File.ReadAllLines("C:\\Users\\aaron\\Desktop\\DevCodeCamp\\Projects\\HumaneSociety\\animals.csv").ToList();
            var result = lines.Select(l => l.Split(',')).ToList();
            var output = result.Select(x => x.Select(y => input.Match(y).ToString()).ToList()).ToList();

            return output;
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
                            newAnimal.KidFriendly = animalList[i][j] == "0" ? false : true;
                            break;
                        case 6:
                            newAnimal.PetFriendly = animalList[i][j] == "0" ? false : true;
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

                Query.AddAnimal(newAnimal);
            }
        }

    }
}
