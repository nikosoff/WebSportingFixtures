//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Moq;
//using NUnit.Framework;
//using WebSportingFixtures.Core.Interfaces;
//using WebSportingFixtures.Core.Models;

//namespace WebSportingFixtures.Services.Tests
//{
//    [TestFixture]
//    class SQLiteStoreTests
//    {
//        [Test]
//        public void CreateTeam()
//        {
//            //Arrange
//            var mockSqliteStore = new Mock<IStore>();
//            mockSqliteStore.Setup(sqliteStore => sqliteStore.CreateTeam(It.Is<Team>(t => t.Name == "RandomName" && t.KnownName == "RandomKnownName"))).Returns(true);
//            bool expectedResult = true;

//            //Act
//            var actualResult = mockSqliteStore.Object.CreateTeam(new Team {Name = "RandomName", KnownName = "RandomKnownName"});

//            //Assert
//            Assert.AreEqual(expectedResult, actualResult);
//            mockSqliteStore.Verify(store => store.CreateTeam(It.Is<Team>(t => t.Name == "RandomName" && t.KnownName == "RandomKnownName")), Times.Once);
            

//        }
//    }
//}
