using FlightControlWeb.Controllers;
using FlightControlWeb.Data;
using FlightControlWeb.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace XUnitTestProject
{
    public class FlightPlanControllerTests
    {
        // _sut == system under test
        public static readonly DbContextOptionsBuilder<FlightDbContext>  d = new DbContextOptionsBuilder<FlightDbContext>();
        public readonly Mock<FlightDbContext> _FlightDBContextMock = new Mock<FlightDbContext>(d.Options);
        private readonly FlightPlanController _sut;
        public FlightPlanControllerTests()
        {
            _sut = new FlightPlanController(_FlightDBContextMock.Object);
        }

        [Fact]
        public async Task GetFlightPlan_ShouldReturnFlightPlan_WhenFlightPlanExists()
        {
            // Arrange
            var flightPlanId = Guid.NewGuid();
            var flightPlanDto = new FlightPlan
            {
                Id = flightPlanId.ToString()
            };
            _FlightDBContextMock.Setup(x => x.flightPlan.FindAsync(flightPlanId.ToString()))
                .ReturnsAsync(flightPlanDto);
            // Act
            var flightPlan = await _sut.GetFlightPlan(flightPlanId.ToString());
            // Assert
            Assert.Equal(flightPlanId.ToString(), flightPlan.Value.Id);
        }
    }
}