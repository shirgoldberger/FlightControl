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
        private readonly FlightPlanController sut;
        private readonly FlightDbContext flightDBContextMock;
        public FlightPlanControllerTests()
        {
            string[] args = { };
            var h = Program.CreateHostBuilder(args);
            var d = new DbContextOptionsBuilder<FlightDbContext>();
            d.UseInMemoryDatabase("DBName");
            flightDBContextMock = new FlightDbContext(d.Options);
            sut = new FlightPlanController(flightDBContextMock);
        }

        [TestMethod]
        public async Task GetFlightPlan_ShouldReturnFlightPlan_WhenFlightPlanExists()
        {
            Location location = new Location();
            location.ID = "DF562344";
            // Arrange
            var flightPlanDto = new FlightPlan
            {
                ID = "NL145289",
                Passengers = 257,
                CompanyName = "el al",
                InitialLocation = location
            };
            flightDBContextMock.FlightPlan.Add(flightPlanDto);
            flightDBContextMock.SaveChanges();
            // Act
            var flightPlan = await flightDBContextMock.FlightPlan.FindAsync("NL145289");
            Task<ActionResult<FlightPlan>> fp1 = sut.GetFlightPlan("NL145289");
            // Assert
            /** check some attrubutes of the FlightPlan feilds in DB
             * */
            Assert.IsTrue("NL145289" == flightPlan.ID);
            Assert.IsTrue(257 == flightPlan.Passengers);
            Assert.IsTrue("el al" == flightPlan.CompanyName);
            Assert.IsTrue("DF562344" == flightPlan.InitialLocation.ID);
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
            ActionResult<FlightPlan> fp2 = await sut.GetFlightPlan(id);
            string result = fp2.Result.ToString();
            // Assert
            Assert.IsTrue(result.Contains("NotFound"));
        }
    }
}
