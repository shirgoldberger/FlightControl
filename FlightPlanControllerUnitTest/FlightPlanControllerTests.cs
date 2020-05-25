using Microsoft.VisualStudio.TestTools.UnitTesting;
using FlightControlWeb;
using FlightControlWeb.Controllers;
using FlightControlWeb.Data;
using FlightControlWeb.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Net;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace FlightPlanControllerTest
{
    [TestClass]
    public class FlightPlanControllerTests
    {
        // _sut == system under test
        private readonly FlightPlanController _sut;
        private readonly FlightDbContext _FlightDBContextMock;
        public FlightPlanControllerTests()
        {
            string[] args = { };
            var h = Program.CreateHostBuilder(args);
            var d = new DbContextOptionsBuilder<FlightDbContext>();
            d.UseInMemoryDatabase("DBName");
            _FlightDBContextMock = new FlightDbContext(d.Options);
            _sut = new FlightPlanController(_FlightDBContextMock);
        }

        [TestMethod]
        public async Task GetFlightPlan_ShouldReturnFlightPlan_WhenFlightPlanExists()
        {
            Location location = new Location();
            location.Id = "DF562344";
            // Arrange
            var flightPlanDto = new FlightPlan
            {
                Id = "NL145289",
                Passengers = 257,
                Company_name = "el al",
                Initial_location = location
            };
            _FlightDBContextMock.flightPlan.Add(flightPlanDto);
            _FlightDBContextMock.SaveChanges();
            // Act
            var flightPlan = await _FlightDBContextMock.flightPlan.FindAsync("NL145289");
            Task<ActionResult<FlightPlan>> fp1 = _sut.GetFlightPlan("NL145289");
            // Assert
            /** check some attrubutes of the FlightPlan feilds in DB
             * */
            Assert.IsTrue("NL145289" == flightPlan.Id);
            Assert.IsTrue(257 == flightPlan.Passengers);
            Assert.IsTrue("el al" == flightPlan.Company_name);
            Assert.IsTrue("DF562344" == flightPlan.Initial_location.Id);
            /** check GetFlightPlan method
             */
            // true because we added a flight plan with that id to the DB
            Assert.IsNotNull(fp1);
        }
        [TestMethod]
        [DataRow("DF562344")]
        [DataRow("JF347781")]

        public async Task GetFlightPlan_ShouldReturnNotFound_WhenFlightPlanNotExist(string id)
        {
            // Act
            ActionResult<FlightPlan> fp2 = await _sut.GetFlightPlan(id);
            string result = fp2.Result.ToString();
            // Assert
            Assert.IsTrue(result == "Microsoft.AspNetCore.Mvc.NotFoundObjectResult");
        }
    }
}
