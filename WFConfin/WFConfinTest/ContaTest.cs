using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WFConfin.Controllers;
using WFConfin.Data;
using WFConfin.Models;
using Xunit;
using Microsoft.EntityFrameworkCore;

namespace WFConfin.Tests
{
    public class ContaControllerTests
    {
        private readonly Mock<WFConfinDbContext> _mockContext;
        private readonly ContaController _controller;

        public ContaControllerTests()
        {
            _mockContext = new Mock<WFConfinDbContext>();
            _controller = new ContaController(_mockContext.Object);
        }

        [Fact]
        public async Task GetContas_ReturnsOkResult_WithListOfContas()
        {
            // Arrange
            var contas = new List<Conta> {
                new Conta { Id = Guid.NewGuid(), Descricao = "Conta 1" },
                new Conta { Id = Guid.NewGuid(), Descricao = "Conta 2" }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Conta>>();
            mockSet.As<IQueryable<Conta>>().Setup(m => m.Provider).Returns(contas.Provider);
            mockSet.As<IQueryable<Conta>>().Setup(m => m.Expression).Returns(contas.Expression);
            mockSet.As<IQueryable<Conta>>().Setup(m => m.ElementType).Returns(contas.ElementType);
            mockSet.As<IQueryable<Conta>>().Setup(m => m.GetEnumerator()).Returns(contas.GetEnumerator());

            _mockContext.Setup(c => c.Conta).Returns(mockSet.Object);

            // Act
            var result = await _controller.GetContas();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<Conta>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task PostConta_ReturnsOkResult_WhenContaIsAdded()
        {
            // Arrange
            var conta = new Conta { Id = Guid.NewGuid(), Descricao = "Nova Conta" };

            var mockSet = new Mock<DbSet<Conta>>();
            _mockContext.Setup(m => m.Conta).Returns(mockSet.Object);
            _mockContext.Setup(m => m.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _controller.PostConta(conta);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Sucesso, conta Incluida", okResult.Value);
        }

        [Fact]
        public async Task DeleteConta_ReturnsOkResult_WhenContaIsDeleted()
        {
            // Arrange
            var conta = new Conta { Id = Guid.NewGuid(), Descricao = "Conta a ser excluída" };

            var mockSet = new Mock<DbSet<Conta>>();
            mockSet.Setup(m => m.FindAsync(It.IsAny<Guid>())).ReturnsAsync(conta);
            _mockContext.Setup(m => m.Conta).Returns(mockSet.Object);
            _mockContext.Setup(m => m.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _controller.DeleteConta(conta.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Sucesso, conta excluída", okResult.Value);
        }
    }
}
