using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using APIContatoNasser;
using Domain.Models;
using Infraestructure.Context;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit;

namespace ApplicationTest
{
    public class ContatoTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private const string ContatoGetEndpoint = "api/Contato";
        private const string ContatoPostEndpoint = "api/Contato";
        private const string dbName = "ContatoDbTest";

        
        private readonly WebApplicationFactory<Startup> _factory;

        public ContatoTest(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        #region Setup Testes API

        private HttpClient CreateClient() => _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(d =>
                        d.ServiceType == typeof(DbContextOptions<ApplicationDBContext>));
                    services.Remove(descriptor);
                    services.AddDbContext<ApplicationDBContext>(options =>
                        options.UseInMemoryDatabase(dbName));

                    using var scope = services.BuildServiceProvider().CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

                    db.Database.EnsureCreated();
                });
            })
            .CreateClient(new WebApplicationFactoryClientOptions {AllowAutoRedirect = false});
        
        private ApplicationDBContext GetDbContext()
        {
            var builder = new DbContextOptionsBuilder<ApplicationDBContext>();
            builder.UseInMemoryDatabase(dbName);
            var context = new ApplicationDBContext(builder.Options);

            if (context.Contatos.Count() == 0)
            {
                context.Contatos.Add(new Contato() { Id = 1, Nome = "Primeiro", Sexo = "M", IsAtivo = true });
                context.Contatos.Add(new Contato() { Id = 2, Nome = "Segundo", Sexo = "F", IsAtivo = true });
                context.Contatos.Add(new Contato() { Id = 3, Nome = "Terceiro", Sexo = "M", IsAtivo = true });

                context.SaveChanges();
            }
            return context;
        }

        #endregion
        
        [Fact]
        public async Task ContatoGet_Should_Return_Data()
        {
            var client = CreateClient();
            await using var context = GetDbContext();
            
            var response = await client.GetAsync(ContatoGetEndpoint);
            var content = await response.Content.ReadAsStringAsync();
            var list = JsonConvert.DeserializeObject<List<Contato>>(content);

            response.EnsureSuccessStatusCode();
            Assert.Equal(3, list.Count());
        }

        [Fact]
        public async Task ContatoActivate_Should_Activate_Contact()
        {
            var client = CreateClient();
            await using var context = GetDbContext();
            var response = await client.GetAsync("api/Contato/changeState/1?isActive=true");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        
        [Fact]
        public async Task ContatoCreate_Should_PersistData_In_Database()
        {
            var client = CreateClient();
            
            var contato = new Contato
            {
                Nome = "TesteSemErro",
                DataNascimento = DateTime.Now.AddYears(-22),
                IsAtivo = true,
                Sexo = "M",
                Idade = 0
            };
            var json = JsonConvert.SerializeObject(contato);

            var response = await client.PostAsync(ContatoPostEndpoint,
                new StringContent(json, Encoding.UTF8, "application/json"));
            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        
        
        [Fact]
        public async Task ContatoCreate_Should_Give_Error()
        {
            var client = CreateClient();
            
            var contato = new Contato
            {
                Nome = "TesteErro",
                DataNascimento = DateTime.Now.AddYears(-10),
                IsAtivo = true,
                Sexo = "M",
                Idade = 0
            };
            var json = JsonConvert.SerializeObject(contato);

            var response = await client.PostAsync(ContatoPostEndpoint,
                new StringContent(json, Encoding.UTF8, "application/json"));
            
            Assert.Equal(HttpStatusCode.BadRequest,response.StatusCode);
        }
    }
}