using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using GraduateRecruitment.ConsoleApp.Data;
using GraduateRecruitment.ConsoleApp.Extensions;

[assembly: InternalsVisibleTo("GraduateRecruitment.UnitTests")]

namespace GraduateRecruitment.ConsoleApp
{
    internal class Program
    {
        static Dictionary<string, int> maxDrinks = new Dictionary<string, int>
        {
                { "Savanna Dry", 50 }, 
                { "Savanna Light", 30 },
                { "Castle Lite", 40 },
                { "Castle Larger", 40 },
                { "Tafel Larger", 20 },
                { "Coca Cola", 30 },
                { "Fanta Orange", 10 },
                { "Sprite", 10 },
                { "Ceres Orange Juice", 15 },
                { "Ceres Apple Juice", 15 }
        };

        internal static void Main(string[] args)
        {
            var repo = new OpenBarRepository();

            Question1(repo);
            Console.WriteLine(Environment.NewLine);
            Question2(repo);
            Console.WriteLine(Environment.NewLine);
            Question3(repo);
            Console.WriteLine(Environment.NewLine);
            Question4(repo);
            Console.WriteLine(Environment.NewLine);
            Question5(repo);
            Console.WriteLine(Environment.NewLine);
            Question6(repo);
            Console.WriteLine(Environment.NewLine);
            Question7(repo);
            Console.WriteLine(Environment.NewLine);
        }

        private static void Question1(OpenBarRepository repo)
        {
            Console.WriteLine("Question 1: What is the most popular drink, including the quantity, on a Wednesday?");

            var result = GetHighestAverage("Wednesday", repo);

            Console.WriteLine("{0}: {1}", result.Keys.First(), result.Values.First());
        }

        private static void Question2(OpenBarRepository repo)
        {
            Console.WriteLine("Question 2: What is the most popular drink, including the quantities, per day?");

            List<string> daysOfWeek = new List<string>() { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };

            foreach (var day in daysOfWeek)
            {
                Console.WriteLine(day);
                var result = GetHighestAverage(day, repo);

                Console.WriteLine("{0}: {1}", result.Keys.First(), result.Values.First());
                Console.WriteLine(Environment.NewLine);
            }
        }

        private static void Question3(OpenBarRepository repo)
        {
            Console.WriteLine("Question 3: Which dates did we run out of Savanna Dry for the last recorded month?");

            int quantity = 0;

            DateTime lastMonth = latestRecord(repo);

            foreach (var item in repo.AllOpenBarRecords)
            {
                foreach (var drink in item.FridgeStockTakeList)
                {
                    if (drink.Inventory.Name.Equals("Savanna Dry"))
                    {
                        quantity += drink.Quantity.Added - drink.Quantity.Taken;

                        if (item.Date.Month == lastMonth.Month && item.Date.Year == lastMonth.Year)
                        {
                            if (quantity <= 0)
                            {
                                Console.WriteLine(item.Date.ToString("yyyy-MM-dd"));
                            }
                        }
                    }
                }
            }
        }

        private static void Question4(OpenBarRepository repo)
        {
            Console.WriteLine("Question 4: How many Fanta Oranges do we need to order next week?");

            DateTime lastMonth = latestRecord(repo);
            int maxFantas = maxDrinks["Fanta Orange"];
            int missingFantas = maxFantas - DrinkQuantity("Fanta Orange", lastMonth, repo);
            Console.WriteLine(missingFantas);
        }

        private static void Question5(OpenBarRepository repo)
        {
            Console.WriteLine("Question 5: How much do we need to budget next month for Ceres Orange Juice?");

            int maxOrangeJuice = 15;

            DateTime lastMonth = latestRecord(repo);

            decimal price = GetDrinkPrice("Ceres Orange Juice", repo);

            decimal amount = (maxOrangeJuice - DrinkQuantity("Ceres Orange Juice", lastMonth, repo)) * price;

            Console.WriteLine("R{0}", amount.RoundToInt());
        }

        private static void Question6(OpenBarRepository repo)
        {
            Console.WriteLine("Question 6: How much do we need to budget for next month to restock the fridge?");

            DateTime lastMonth = latestRecord(repo);

            decimal budget = 0;

            foreach (KeyValuePair<string, int> item in maxDrinks)
            {
                budget += GetDrinkPrice(item.Key, repo) * (item.Value - DrinkQuantity(item.Key, lastMonth, repo));
            }

            Console.WriteLine("R{0}", budget.RoundToInt());
        }

        private static void Question7(OpenBarRepository repo)
        {
            Console.WriteLine("Question 7: We're planning a braai and expecting 100 people, how many of each drink should we order based on historical popularity of drinks?");

            foreach (KeyValuePair<string, int> item in maxDrinks)
            {
                Console.WriteLine("{0}: {1}", item.Key, CalculatePopularity(item.Key, repo));
            }
        }

        //This function calculates the average amount that every drink gets taken on the specified day of the week and returns the highest average
        public static Dictionary<string, int> GetHighestAverage(string dayOfWeek, OpenBarRepository repo)
        {
            Dictionary<string, decimal> drinks = new Dictionary<string, decimal>();

            decimal highestQuantity = 0;
            string nameOfDrink = "";

            foreach (KeyValuePair<string, int> item in maxDrinks)
            {
                decimal quantity = 0;
                decimal days = 0;

                foreach (var record in repo.AllOpenBarRecords)
                {
                    if (record.DayOfWeek.ToString().Equals(dayOfWeek))
                    {
                        foreach (var drink in record.FridgeStockTakeList)
                        {
                            if(drink.Inventory.Name.Equals(item.Key))
                            {
                                quantity += drink.Quantity.Taken;
                            }
                        }
                        days++;
                    }
                }

                drinks.Add(item.Key, quantity / days);
            }

            foreach (KeyValuePair<string, decimal> item in drinks)
            {
                decimal quantity = item.Value;

                if(quantity > highestQuantity)
                {
                    highestQuantity = quantity;
                    nameOfDrink = item.Key;
                }
            }

            var results = new Dictionary<string, int>();
            results.Add(nameOfDrink, highestQuantity.RoundToInt());

            return results;
        }

        //This function returns the amount of a specific drink that are left in the fridge at a certain date
        public static int DrinkQuantity(string name, DateTime date, OpenBarRepository repo)
        {
            int quantity = 0;

            foreach (var item in repo.AllOpenBarRecords)
            {
                foreach (var drink in item.FridgeStockTakeList)
                {
                    if (drink.Inventory.Name.Equals(name))
                    {
                        quantity += drink.Quantity.Added - drink.Quantity.Taken;

                        if (DateTime.Compare(item.Date, date) == 0)
                        {
                            return quantity;
                        }
                    }
                }
            }
            return quantity;
        }

        //This function returns the latest entry date in the OpenBarRecords
        public static DateTime latestRecord(OpenBarRepository repo)
        {
            return repo.AllOpenBarRecords[repo.AllOpenBarRecords.Count - 1].Date;
        }

        //This function will return the price of a specified drink
        public static decimal GetDrinkPrice(string name, OpenBarRepository repo)
        {
            decimal price = 0;

            foreach (var item in repo.AllOpenBarRecords[0].FridgeStockTakeList)
            {
                if (item.Inventory.Name.Equals(name))
                {
                    price = item.Inventory.Price;
                }
            }

            return price;
        }

        //This function returns the average amount a drink has been bought since the beginning based on how many people were in the bar at that time
        public static int CalculatePopularity(string name, OpenBarRepository repo)
        {
            DateTime firstDate = repo.AllOpenBarRecords[0].Date;
            DateTime lastDate = latestRecord(repo);
            int months = ((lastDate.Year - firstDate.Year) * 12) + lastDate.Month - firstDate.Month;

            decimal amount = 0;

            foreach (var item in repo.AllOpenBarRecords)
            {
                foreach (var drink in item.FridgeStockTakeList)
                {
                    if (drink.Inventory.Name.Equals(name))
                    {
                        if (item.NumberOfPeopleInBar > 0)
                        {
                            amount += drink.Quantity.Taken / item.NumberOfPeopleInBar;
                        }
                        else
                        {
                            amount += drink.Quantity.Taken;
                        }
                    }
                }
            }

            return (amount / months).RoundToInt();
        }
    }
}