using Microsoft.VisualStudio.TestTools.UnitTesting;
using FlightControlWeb;
using FlightControlWeb.Controllers;
using FlightControlWeb.Data;
using FlightControlWeb.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace FlightPlanControllerUnitTest
{
    [TestClass]
    public class FlightPlanControllerTests
    {
        // _sut == system under test
        private readonly FlightPlanController _sut;
        private readonly FlightDbContext _FlightDBContextMock;
        public FlightPlanControllerTests()
        {
            string[] args = {};
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
            // Assert
            // check some attrubutes of the FlightPlan feilds
            Assert.IsTrue("NL145289" == flightPlan.Id);
            Assert.IsTrue(257 == flightPlan.Passengers);
            Assert.IsTrue("el al" == flightPlan.Company_name);
            Assert.IsTrue("DF562344" == flightPlan.Initial_location.Id);
        }

        [TestMethod]
        public async Task FirstLoc_ShouldReturnLocation_WhenLocationExists()
        {
            // Arrange
            var locDto = new Location
            {
                key = 15,
                Id = "WR822321",
                Longitude = 15.46,
                Latitude = 21.78,
                Date_time = "2020-05-23T20:50:22Z"
            };
            _FlightDBContextMock.firstLoc.Add(locDto);
            _FlightDBContextMock.SaveChanges();
            // Act
            var location = await _FlightDBContextMock.firstLoc.FindAsync(15);
            // Assert
            // check some attrubutes of the Location feilds
            Assert.IsTrue(location.key == 15);
            Assert.IsTrue(location.Id == "WR822321");
            // this is false
            Assert.IsFalse(location.Longitude == 12.85);
            Assert.IsTrue(location.Latitude == 21.78);
            Assert.IsTrue(location.Date_time == "2020-05-23T20:50:22Z");
        }

        [TestMethod]
        public async Task Segments_ShouldReturnSegmant_WhenSegmentExists()
        {
            // Arrange
            var segDto = new Segment
            {
                key = 6,
                Id = "WR822321",
                Longitude = 17.1,
                Latitude = 20.19,
                timespan_seconds = 20650
            };
            _FlightDBContextMock.segments.Add(segDto);
            _FlightDBContextMock.SaveChanges();
            // Act
            var segment = await _FlightDBContextMock.segments.FindAsync(6);
            // Assert
            Assert.IsTrue(6 == segment.key);
            // this is false
            Assert.IsFalse("ER822321" == segment.Id);
            Assert.IsFalse(11.7 == segment.Longitude);
            Assert.IsFalse(19.20 == segment.Latitude);
            Assert.IsTrue(20650 == segment.timespan_seconds);
        }

        [TestMethod]
        public async Task Server_ShouldReturnServer_WhenServerExists()
        {
            // Arrange
            var serverDto = new Server
            {
                ServerId = "TY762113",
                ServerURL = "http://localhost:5554"
            };
            _FlightDBContextMock.Server.Add(serverDto);
            _FlightDBContextMock.SaveChanges();
            // Act
            var server = await _FlightDBContextMock.Server.FindAsync("TY762113");
            // Assert
            Assert.IsTrue("TY762113" == server.ServerId);
            Assert.IsTrue("http://localhost:5554" == server.ServerURL);
        }
    }
}
