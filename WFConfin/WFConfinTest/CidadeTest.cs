using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using WFConfin.Controllers;
using WFConfin.Models;
using WFConfin.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace WFConfinTest
{
    public class CidadeTest
    {
        public class CidadeControllerTests
        {
            private readonly WFConfinDbContext _context;
            private readonly CidadeController _controller;

            public CidadeControllerTests()
            {
                var options = new DbContextOptionsBuilder<WFConfinDbContext>()
                    .UseInMemoryDatabase(databaseName: "TestDatabase")
                    .Options;

                _context = new WFConfinDbContext(options);
                _controller = new CidadeController(_context);
            }

            [Fact]
            public async Task GetCidades_ReturnsOkResult_WithListOfCidades()
            {
                var cidadeData = new List<Cidade>
                {
                    new Cidade { Id = Guid.NewGuid(), Nome = "Cidade1", EstadoSigla = "SP" },
                    new Cidade { Id = Guid.NewGuid(), Nome = "Cidade2", EstadoSigla = "SC" }
                };

                await _context.Cidade.AddRangeAsync(cidadeData);
                await _context.SaveChangesAsync();

                var result = await _controller.GetCidades();

                var okResult = Assert.IsType<OkObjectResult>(result);
                var returnValue = Assert.IsType<List<Cidade>>(okResult.Value);
                Assert.Equal(2, returnValue.Count);
            }

            [Fact]
            public async Task PostCidade_ReturnsOkResult_WhenCidadeIsAdded()
            {
                var newCidade = new Cidade { Id = Guid.NewGuid(), Nome = "Nova Cidade", EstadoSigla = "SP" };

                var result = await _controller.PostCidade(newCidade);

                var okResult = Assert.IsType<OkObjectResult>(result);
                Assert.Equal("Sucesso, cidade Incluido", okResult.Value);

                var cidadeInDb = await _context.Cidade.FindAsync(newCidade.Id);
                Assert.NotNull(cidadeInDb);
                Assert.Equal(newCidade.Nome, cidadeInDb.Nome);
                Assert.Equal(newCidade.EstadoSigla, cidadeInDb.EstadoSigla);
            }

            [Fact]
            public async Task PostCidade_ReturnsBadRequest_WhenCidadeIsNotAdded()
            {
                var newCidade = new Cidade { Id = Guid.NewGuid(), Nome = "Nova Cidade", EstadoSigla = "SP" };

                // Simula uma falha na adição da cidade (duplicação de ID)
                _context.Cidade.Add(newCidade);
                await _context.SaveChangesAsync();
                // Arrange
                var anotherCidade = new Cidade { Id = newCidade.Id, Nome = "Outra Cidade", EstadoSigla = "RJ" };
                // Act
                var result = await _controller.PostCidade(anotherCidade);
                // Assert
                var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
                Assert.Contains("Erro, cidade não incluido", badRequestResult.Value.ToString());
            }
            [Fact]
            public async Task PutCidade_ReturnsOkResult_WhenCidadeIsUpdated()
            {
                // Arrange
                var existingCidade = new Cidade { Id = Guid.NewGuid(), Nome = "Cidade Existente", EstadoSigla = "SP" };
                await _context.Cidade.AddAsync(existingCidade);
                await _context.SaveChangesAsync();

                var updatedCidade = new Cidade { Id = existingCidade.Id, Nome = "Cidade Atualizada", EstadoSigla = "RJ" };
                //Limpa os rastreamento dos dados locais
                _context.ChangeTracker.Clear();
                // Act
                var result = await _controller.PutCidade(updatedCidade);
                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result);
                Assert.Equal("Sucesso, cidade alterada", okResult.Value);

                var cidadeInDb = await _context.Cidade.FindAsync(existingCidade.Id);
                Assert.NotNull(cidadeInDb);
                Assert.Equal(updatedCidade.Nome, cidadeInDb.Nome);
                Assert.Equal(updatedCidade.EstadoSigla, cidadeInDb.EstadoSigla);
            }

            [Fact]
            public async Task PutCidade_ReturnsBadRequest_WhenCidadeIsNotFound()
            {
                // Arrange
                var updatedCidade = new Cidade { Id = Guid.NewGuid(), Nome = "Cidade Não Existente", EstadoSigla = "SP" };

                // Act
                var result = await _controller.PutCidade(updatedCidade);

                // Assert
                var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
                Assert.Contains("Erro, Cidade não alterada", badRequestResult.Value.ToString());
            }

            [Fact]
            public async Task DeleteCidade_ReturnsOkResult_WhenCidadeIsDeleted()
            {
                // Arrange
                var options = new DbContextOptionsBuilder<WFConfinDbContext>()
                    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                    .Options;

                using var context = new WFConfinDbContext(options);
                var controller = new CidadeController(context);

                var cidade = new Cidade { Id = Guid.NewGuid(), Nome = "Cidade para excluir", EstadoSigla = "SP" };
                context.Cidade.Add(cidade);
                context.SaveChanges();

                // Act
                var result = await controller.DeleteCidade(cidade.Id);

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result);
                Assert.Equal("Sucesso, cidade Excluída", okResult.Value);
            }

            [Fact]
            public async Task DeleteCidade_ReturnsBadRequest_WhenCidadeNotFound()
            {

                // Arrange
                var options = new DbContextOptionsBuilder<WFConfinDbContext>()
                    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                    .Options;

                using var context = new WFConfinDbContext(options);
                var controller = new CidadeController(context);

                // Act
                var result = await controller.DeleteCidade(Guid.NewGuid());

                // Assert
                var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
                Assert.Contains("Erro, cidade não Excluída", badRequestResult.Value.ToString());
            }

            [Fact]
            public async Task DeleteCidade_ReturnsBadRequest_WhenExceptionOccurs()
            {
                // Arrange
                var mockContext = new Mock<WFConfinDbContext>();
                var controller = new CidadeController(mockContext.Object);

                // Configura o mock para lançar uma exceção ao tentar salvar
                mockContext.Setup(c => c.Cidade.FindAsync(It.IsAny<Guid>()))
                           .ThrowsAsync(new Exception("Erro ao buscar cidade"));

                // Act
                var result = await controller.DeleteCidade(Guid.NewGuid());

                // Assert
                var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
                Assert.Contains("Erro, cidade não alterada. Exeção: Erro ao buscar cidade", badRequestResult.Value.ToString());
            }



        }
    }
}