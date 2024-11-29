using CityApiServer.Controllers; // Підключаємо контролер
using CityApiServer.Models;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CityApiServer.Tests.Controllers
{
    public class CitiesControllerTests
    {
        [Fact] // Атрибут xUnit для позначення тестового методу
        public async Task GetCities_ReturnsOkResult()
        {
            // Arrange: створюємо екземпляр контролера
            var controller = new CitiesController();

            var result = await controller.GetCities();

            // Assert: перевіряємо результат
            var okResult = Assert.IsType<OkObjectResult>(result); // Очікуємо 200 OK
            
            var cities = Assert.IsType<List<City>>(okResult.Value); // Перевіряємо, чи повертається список міст

            Assert.NotEmpty(cities); // Перевіряємо, чи список не порожній
        }
    }
}
