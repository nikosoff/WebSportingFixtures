using NUnit.Framework;
using Moq;
using WebSportingFixtures.Core.Interfaces;
using WebSportingFixtures.Core.Models;
using System.Linq;

namespace WebSportingFixtures.Services.Tests
{
    [TestFixture]
    class SportingFixturesServiceTests
    {
        private SportingFixturesService _sut;

        public SportingFixturesServiceTests()
        {

            Mock<IStore> mockStore = new Mock<IStore>();
            Mock<ITextSimilarityAlgorithm> mockTextSimilarityAlgorithm =
                new Mock<ITextSimilarityAlgorithm>();

            mockStore
                .Setup(store => store.CreateTeam(It.Is<Team>(t => t.Name == "Random" && t.KnownName == "Random")))
                .Returns(true);

            mockStore
                .Setup(store => store.CreateTeam(It.Is<Team>(t => t == null || t.Name == "" || t.Name == null || t.KnownName == "" || t.KnownName == null)))
                .Returns(false);

            mockStore
                .Setup(store => store.EditTeam(It.Is<Team>(t => t.Id == 1 && t.Name == "Random" && t.KnownName == "Random")))
                .Returns(true);

            mockStore.
                Setup(store => store.GetAllTeams())
                .Returns(() => new Team[]
                {
                    new Team { Id = 1, Name = "Manchester United F.C.", KnownName = "Manchester United" },
                    new Team { Id = 2, Name = "FC Barcelona", KnownName = "Barcelona" },
                    new Team { Id = 3, Name = "Real Madrid C.F.", KnownName = "Real Madrid"}
                });

            _sut = new SportingFixturesService(mockStore.Object, mockTextSimilarityAlgorithm.Object);
        }

        [Test]
        public void CreateTeam_CreateNewTeamWithoutId_ReturnsTrue()
        {
            //Arrange
            var randomTeam = new Team { Name = "Random", KnownName = "Random" };
            bool expectedResult = true;

            //Act
            bool actualResult = _sut.CreateTeam(randomTeam);

            //Assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void CreateTeam_CreateNewTeamWithId_ReturnsTrue()
        {
            //Arrange
            var randomTeam = new Team { Id = 1, Name = "Random", KnownName = "Random" };
            bool expectedResult = true;

            //Act
            bool actualResult = _sut.CreateTeam(randomTeam);

            //Assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void CreateTeam_CreateNullTeam_ReturnsFalse()
        {
            //Arrange
            Team randomTeam = null;
            bool expectedResult = false;

            //Act
            bool actualResult = _sut.CreateTeam(randomTeam);

            //Assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void CreateTeam_CreateNewTeamWithEmptyName_ReturnsFalse()
        {
            //Arrange
            var randomTeam = new Team { Name = "", KnownName = "Random"};
            bool expectedResult = false;

            //Act
            bool actualResult = _sut.CreateTeam(randomTeam);

            //Assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void CreateTeam_CreateNewTeamWithNullName_ReturnsFalse()
        {
            //Arrange
            var randomTeam = new Team { KnownName = "Random" };
            bool expectedResult = false;

            //Act
            bool actualResult = _sut.CreateTeam(randomTeam);

            //Assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void CreateTeam_CreateNewTeamWithEmptyKnownName_ReutrnsFalse()
        {
            //Arrange
            var randomTeam = new Team { Name = "Random", KnownName = "" };
            bool expectedResult = false;

            //Act
            bool actualResult = _sut.CreateTeam(randomTeam);

            //Assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void CreateTeam_CreateNewTeamWithNullKnownName_ReturnsFalse()
        {
            //Arrange
            var randomTeam = new Team { Name = "Random" };
            bool expectedResult = false;

            //Act
            bool actualResult = _sut.CreateTeam(randomTeam);

            //Assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        //[Test]
        //public void CreateTeam_CreateNewTeamThatAlreadyExists_ReturnsFalse()
        //{

        //}

        //[Test]
        //public void CreateTeam_CreateNewTeamWithNameThatAlreadyExists_ReturnsFalse()
        //{

        //}

        //[Test]
        //public void CreateTeam_CreateNewTeamWithKnownNameThatAlreadyExists_ReturnsFalse()
        //{

        //}

        [Test]
        public void EditTeam_EditExistingTeamWithId_ReturnsTrue()
        {
            //Arrange
            var existingTeam = new Team { Id = 1, Name = "Random", KnownName = "Random" };
            bool expectedResult = true;

            //Act
            bool actualResult = _sut.EditTeam(existingTeam);

            //Assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void EditTeam_EditExistingTeamWithoutId_ReturnsFalse()
        {
            //Arrange
            var existingTeam = new Team { Name = "Random", KnownName = "Random" };
            bool expectedResult = false;

            //Act
            bool actualResult = _sut.EditTeam(existingTeam);

            //Assert
            Assert.AreEqual(expectedResult, actualResult);
        }


        [Test]
        public void GetAllTeams_GetAllStoredTeams_ReturnNonEmptyResult()
        {
            //Arrange

            //Act
            var storedTeams = _sut.GetAllTeams();
            
            //Assert
            Assert.AreNotEqual(0, storedTeams.Count());
        }

        [Test]
        public void GetAllTeams_GetFirstStoredTeam_ReturnNonNullResult()
        {
            //Arrange
            
            //Act
            var actualTeam = _sut.GetAllTeams().FirstOrDefault();

            //Assert
            Assert.IsNotNull(actualTeam);
        }

    }
}
