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
        public SportingFixturesServiceTests()
        {

        }

        [Test]
        public void TryCreateTeam_ProvideValidTeam_ReturnsTrueAndReturnsTeamErrorNone()
        {
            //Arrange
            Mock<IStore> mockStore = new Mock<IStore>();
            Mock<ITextSimilarityAlgorithm> mockTextSimilarityAlgorithm =
                new Mock<ITextSimilarityAlgorithm>();

            var sut = new SportingFixturesService(mockStore.Object, mockTextSimilarityAlgorithm.Object);

            var validTeam = new Team { Name = "Random", KnownName = "Random" };
            bool expectedResult = true;
            TeamError actualTeamError;

            mockStore
                .Setup(store => store.CreateTeam(It.Is<Team>(t => t.Name == validTeam.Name && t.KnownName == validTeam.KnownName)))
                .Returns(validTeam);

            //Arrange
            bool actualResult = sut.TryCreateTeam(new Team { Name = "Random", KnownName = "Random"}, out actualTeamError);

            //Assert
            Assert.AreEqual(expectedResult, actualResult);
            Assert.AreEqual(TeamError.None, actualTeamError);
        }

        [TestCase(new object[] { "" })]
        [TestCase(new object[] { null })]
        public void TryCreateTeam_ProvideTeamWithInvalidName_ReturnsFalseAndReturnsTeamErrorInvalidName(string teamName)
        {
            //Arrange
            Mock<IStore> mockStore = new Mock<IStore>();
            Mock<ITextSimilarityAlgorithm> mockTextSimilarityAlgorithm =
                new Mock<ITextSimilarityAlgorithm>();

            var sut = new SportingFixturesService(mockStore.Object, mockTextSimilarityAlgorithm.Object);

            var randomTeam = new Team { Name = teamName, KnownName = "Random" };
            bool expectedResult = false;
            TeamError actualTeamError;

            mockStore
                .Setup(store => store.CreateTeam(It.Is<Team>(t => t.Name == randomTeam.Name && t.KnownName == randomTeam.KnownName)))
                .Returns(randomTeam);

            //Arrange
            bool actualResult = sut.TryCreateTeam(new Team { Name = teamName, KnownName = "Random" }, out actualTeamError);

            //Assert
            Assert.AreEqual(expectedResult, actualResult);
            Assert.AreEqual(TeamError.InvalidName, actualTeamError);
        }

        [TestCase(new object[] { "" })]
        [TestCase(new object[] { null })]
        public void TryCreateTeam_ProvideTeamWithInvalidKnownName_ReturnsFalseAndReturnsTeamErrorInvalidKnownName(string teamKnownName)
        {
            //Arrange
            Mock<IStore> mockStore = new Mock<IStore>();
            Mock<ITextSimilarityAlgorithm> mockTextSimilarityAlgorithm =
                new Mock<ITextSimilarityAlgorithm>();

            var sut = new SportingFixturesService(mockStore.Object, mockTextSimilarityAlgorithm.Object);

            var randomTeam = new Team { Name = "Random", KnownName = teamKnownName };
            bool expectedResult = false;
            TeamError actualTeamError;

            mockStore
                .Setup(store => store.CreateTeam(It.Is<Team>(t => t.Name == randomTeam.Name && t.KnownName == randomTeam.KnownName)))
                .Returns(randomTeam);

            //Arrange
            bool actualResult = sut.TryCreateTeam(new Team { Name = "Random", KnownName = teamKnownName }, out actualTeamError);

            //Assert
            Assert.AreEqual(expectedResult, actualResult);
            Assert.AreEqual(TeamError.InvalidKnownName, actualTeamError);
        }

    }
}
