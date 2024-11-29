using CityApiServer.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace CityApiServer.Tests.Controllers
{
    public class GetCityDetails
    {
        [Fact]
        public async Task GetCityDetails_ReturnsNotFound_ForInvalidId()
        {
            var controller = new CitiesController();
            int invalidId = 9999;  

            var result = await controller.GetCityDetails(invalidId);

            Assert.IsType<NotFoundObjectResult>(result); // Î÷³êóºìî 404 NotFound
        }
    }
}