using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using ShireBank.Server.Database;
using ShireBank.Server.Database.Queries;
using ShireBank.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Xunit;

namespace ShireBank.UnitTests
{
    public class AccountQueriesTests
    {
        private readonly AccountQueries Subject;
        private readonly Mock<DataContext> DbContextMock;

        public AccountQueriesTests()
        {
            DbContextMock = new Mock<DataContext>();
            Subject = new AccountQueries(DbContextMock.Object);
        }

        [Fact]
        public async void Given_new_firstName_and_lastName_When_OpenAccount_is_called_Then_a_new_account_is_created()
        {
            // Arrange
            var firstName = "firstName";
            var lastName = "lastName";
            var debitLimit = 100;

            var accounts = new List<Account> { };
            DbContextMock.Setup(x => x.Accounts).ReturnsDbSet(accounts);

            // Act
            var result = await Subject.OpenAccount(firstName, lastName, debitLimit);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(firstName, result.FirstName);
            Assert.Equal(lastName, result.LastName);
            Assert.Equal(debitLimit, result.DebtLimit);

            DbContextMock.Verify(x => x.Accounts.AddAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()), Times.Once);
            DbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void Given_debitLimit_less_than_0_When_OpenAccount_is_called_Then_ArgumentOutOfRangeException_should_be_thrown()
        {
            // Arrange

            // Act
            var act = () => Subject.OpenAccount("firstName", "lastName", -10);

            // Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(act);
        }

        [Theory]
        [InlineData(null, "lastName")]
        [InlineData("firstName", null)]
        [InlineData(null, null)]
        public async void Given_null_parameter_When_OpenAccount_is_called_Then_ArgumentNullException_should_be_thrown(string firstName, string lastName)
        {
            // Arrange

            // Act
            var act = () => Subject.OpenAccount(firstName, lastName, 100);

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(act);
        }

        [Fact]
        public async void Given_the_account_exists_and_is_open_When_OpenAccount_is_called_for_this_account_Then_Exception_should_be_thrown()
        {
            // Arrange
            var firstName = "firstName";
            var lastName = "lastName";

            var accounts = new List<Account>
            {
                new Account {
                    FirstName = firstName,
                    LastName = lastName,
                    IsClosed = false
                }
            };
            DbContextMock.Setup(x => x.Accounts).ReturnsDbSet(accounts);

            // Act
            var act = () => Subject.OpenAccount(firstName, lastName, 100);

            // Assert
            await Assert.ThrowsAsync<Exception>(act);
        }

        [Fact]
        public async void Given_the_account_exists_and_is_closed_When_OpenAccount_is_called_for_this_account_Then_the_account_is_opened_again()
        {
            // Arrange
            var firstName = "firstName";
            var lastName = "lastName";

            var accounts = new List<Account>
            {
                new Account {
                    FirstName = firstName,
                    LastName = lastName,
                    IsClosed = true
                }
            };
            DbContextMock.Setup(x => x.Accounts).ReturnsDbSet(accounts);

            // Act
            var result = await Subject.OpenAccount(firstName, lastName, 100);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsClosed);

            DbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}