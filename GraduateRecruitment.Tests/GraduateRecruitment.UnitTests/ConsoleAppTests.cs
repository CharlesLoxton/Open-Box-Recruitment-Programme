using System;
using System.Linq;
using FluentAssertions;
using GraduateRecruitment.ConsoleApp;
using GraduateRecruitment.ConsoleApp.Data;
using NUnit.Framework;

namespace GraduateRecruitment.UnitTests
{
    [TestFixture]
    public class ConsoleAppTests
    {
        [Test]
        public void GetHighestAverage_When_Wednesday_Then_ReturnsSavannaDry()
        {
            var repo = new OpenBarRepository();

            var result = Program.GetHighestAverage("Wednesday", repo);

            result.Keys.First().Should().Be("Savanna Dry");
            result.Values.First().Should().Be(6);
        }

        [Test]
        public void DrinkQuantity()
        {
            var repo = new OpenBarRepository();

            var result = Program.DrinkQuantity("Fanta Orange", new DateTime(2022, 04, 20), repo);

            result.Should().Be(3);
        }

        [Test]
        public void GetDrinkPrice()
        {
            var repo = new OpenBarRepository();

            var result = Program.GetDrinkPrice("Ceres Orange Juice", repo);

            result.Should().Be(8.99M);
        }

        [Test]
        public void latestRecord()
        {
            var repo = new OpenBarRepository();

            var result = Program.latestRecord(repo);

            result.Should().Be(repo.AllOpenBarRecords[repo.AllOpenBarRecords.Count - 1].Date);
        }
        [Test]
        public void CalculatePopularity()
        {
            var repo = new OpenBarRepository();

            var result = Program.CalculatePopularity("Savanna Dry", repo);

            result.Should().Be(13);
        }
    }
}
