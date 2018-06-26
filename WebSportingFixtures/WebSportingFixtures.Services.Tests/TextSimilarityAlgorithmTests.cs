using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using WebSportingFixtures.Services;
using FluentAssertions;

namespace SportingFixtures.Services.Tests
{
    [TestFixture]
    class TextSimilarityAlgorithmTests
    {
        IEnumerable<string> _availableItems;
        TextSimilarityAlgorithm _sut;


        [SetUp]
        public void SetUp()
        {
            _sut = new TextSimilarityAlgorithm();

            _availableItems = new string[]
            {
                "Real Madrid C.F.",
                "FC Barcelona",
                "Deportivo La Coruña",
                "Real Betis",
                "Atlético Madrid"
            };
        }

        [TearDown]
        public void TearDown()
        {
            _availableItems = Enumerable.Empty<string>();
        }


        [TestCase(new object[] { "Real Madrid C.F.", true, "Real Madrid C.F." })]
        [TestCase(new object[] { "FC Barcelona", true, "FC Barcelona" })]
        [TestCase(new object[] { "Deportivo La Coruña", true, "Deportivo La Coruña" })]
        [TestCase(new object[] { "Real Betis", true, "Real Betis" })]
        [TestCase(new object[] { "Atlético Madrid", true, "Atlético Madrid" })]
        public void TryFindBestMatches_WithExactTeamNames_ReturnsTrueAndAlwaysTheExactResultAsAFirstElement(string exactTeamNameInput, bool expectedResult, string expectedTeamName )
        {
            // Arrange
            bool actualResult;
            IEnumerable<string> actualTeamNames;

            // Act
            actualResult = _sut.TryFindBestMatches(exactTeamNameInput, _availableItems, 1, out actualTeamNames);

            // Assert
            Assert.AreEqual(expectedResult, actualResult);
            Assert.AreEqual(expectedTeamName, actualTeamNames.FirstOrDefault());

            actualResult.Should().Be(expectedResult);
            expectedTeamName.Should().Be(actualTeamNames.FirstOrDefault());

            //actualResult.Should().BeTrue();

            //actualTeamNames.Should().NotBeEmpty();
            //actualTeamNames.Should().NotBeNullOrEmpty();

        }

        [Test]
        public void TryFindBestMatches_SimilarToExistingTeamName_ReturnsTrueAndNonEmptyResult()
        {
            // Arrange
            string teamName = "asdfghjkl";
            IEnumerable<string> actualMatches;
            bool expectedResult = true;

            // Act
            bool actualResult = _sut.TryFindBestMatches(teamName, _availableItems, 1, out actualMatches);

            // Assert
            Assert.AreEqual(expectedResult, actualResult);
            Assert.AreNotEqual(0, actualMatches.Count());

        }

        [Test]
        public void TryFindBestMatches_NonSimilarToExistingTeamName_ReturnsTrueAndNonEmptyResult()
        {
            //Arrange
            string teamName = "Betis";
            int numberOfMatches = 5;
            bool expectedResult = true;
            IEnumerable<string> actualMatches;

            //Act
            bool actualResult = _sut.TryFindBestMatches(teamName, _availableItems, numberOfMatches, out actualMatches);

            //Assert
            Assert.AreEqual(expectedResult, actualResult);
            Assert.AreEqual(5, actualMatches.Count());
        }

        [Test]
        public void TryFindBestMatches_ZeroNumberOFMatches_ReturnsFalseAndEmptyResult()
        {
            // Arrange
            string teamName = "Betis";
            IEnumerable<string> actualMatches;
            bool expectedResult = false;

            // Act
            bool actualResult = _sut.TryFindBestMatches(teamName, _availableItems, 0, out actualMatches);

            // Assert
            Assert.AreEqual(expectedResult, actualResult);
            Assert.AreEqual(0, actualMatches.Count());

        }

        [Test]
        public void TryFindBestMatches_EmptyInput_ReturnsFalseAndEmptyResult()
        {
            //Arrange
            TextSimilarityAlgorithm textSimilarityAlgorithm = new TextSimilarityAlgorithm();
            string teamName = "";
            IEnumerable<string> actualMatches;
            bool expectedResult = false;

            // Act
            bool actualResult = textSimilarityAlgorithm.TryFindBestMatches(teamName, _availableItems, 1, out actualMatches);

            // Assert
            Assert.AreEqual(expectedResult, actualResult);
            Assert.AreEqual(0, actualMatches.Count());
        }

        [Test]
        public void TryFindBestMatches_InputIsNull_ReturnsFalseAndEmptyResult()
        {
            //Arrange
            string teamName = null;
            IEnumerable<string> actualMatches;
            bool expectedResult = false;

            // Act
            bool actualResult = _sut.TryFindBestMatches(teamName, _availableItems, 1, out actualMatches);

            // Assert
            Assert.AreEqual(expectedResult, actualResult);
            Assert.AreEqual(0, actualMatches.Count());
        }

        [Test]
        public void TryFindBestMatches_NumberOfMatchesGreaterThanTheNumberOfAvailableItems_ReturnsFalseAndEmptyResult()
        {
            //Arrange
            string teamName = "Betis";
            IEnumerable<string> actualMatches;
            bool expectedResult = false;
            int numberOfMatches = _availableItems.Count() + 1;

            // Act
            bool actualResult = _sut.TryFindBestMatches(teamName, _availableItems, numberOfMatches, out actualMatches);

            // Assert
            Assert.AreEqual(expectedResult, actualResult);
            Assert.AreEqual(0, actualMatches.Count());
        }

        [Test]
        public void TryFindBestMatches_AvailableItemsAreNull_ReturnsFalseAndEmptyResult()
        {
            // Arrange
            string teamName = "Betis";
            IEnumerable<string> actualMatches;
            bool expectedResult = false;
            IEnumerable<string> availableItems = null;

            // Act
            bool actualResult = _sut.TryFindBestMatches(teamName, availableItems, 1, out actualMatches);


            // Assert
            Assert.AreEqual(expectedResult, actualResult);
            Assert.AreEqual(0, actualMatches.Count());
        }

        [Test]
        public void TryFindBestMatches_AvailableItemsAreEmpty_ReturnsFalseAndEmptyResult()
        {
            //Arrange
            string teamName = "Betis";
            IEnumerable<string> actualMatches;
            IEnumerable<string> availableItems = Enumerable.Empty<string>();
            bool expectedResult = false;

            //Act
            bool actualResult = _sut.TryFindBestMatches(teamName, availableItems, 1, out actualMatches);

            //Assert
            Assert.AreEqual(expectedResult, actualResult);
            Assert.AreEqual(0, actualMatches.Count());

        }

        [TestCase(new object[] { 1 , 1 })]
        [TestCase(new object[] { 2 , 2})]
        [TestCase(new object[] { 5 , 5})]
        public void TryFindBestMatches_NumberOfMatchesGreaterThanZeroAndLessThanTheNumberOfAvailableItems_ReturnsTrueAndResultHasLengthAsTheNumberOfMatches(int providedNumberOfMatches, int expectedNumberOfMatches)
        {
            //Arrange
            string teamName = "Betis";
            bool expectedResult = true;
            IEnumerable<string> actualMatches;

            //Act
            bool actualResult = _sut.TryFindBestMatches(teamName, _availableItems, providedNumberOfMatches, out actualMatches);

            //Assert
            Assert.AreEqual(expectedResult, actualResult);
            Assert.AreEqual(expectedNumberOfMatches, actualMatches.Count());
       }


    }
}

