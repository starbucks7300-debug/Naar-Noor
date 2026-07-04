using Xunit;
using NaarNoor.Desktop.Common.Utilities;

namespace NaarNoor.Desktop.Tests.Utilities
{
    /// <summary>
    /// Unit tests for Result<T> pattern implementation
    /// Validates error handling and functional composition
    /// </summary>
    public class ResultPatternTests
    {
        [Fact]
        public void Success_CreatesSuccessfulResult()
        {
            // Arrange
            const int expectedValue = 42;

            // Act
            var result = Result<int>.Success(expectedValue);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedValue, result.Value);
            Assert.Null(result.Error);
        }

        [Fact]
        public void Failure_CreatesFailedResult()
        {
            // Arrange
            const string expectedError = "Operation failed";

            // Act
            var result = Result<string>.Failure(expectedError);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Value);
            Assert.Equal(expectedError, result.Error);
        }

        [Fact]
        public void Map_TransformsSuccessfulResult()
        {
            // Arrange
            var result = Result<int>.Success(10);

            // Act
            var mappedResult = result.Map(x => x * 2);

            // Assert
            Assert.True(mappedResult.IsSuccess);
            Assert.Equal(20, mappedResult.Value);
        }

        [Fact]
        public void Map_PropagatesFailure()
        {
            // Arrange
            const string error = "Original error";
            var result = Result<int>.Failure(error);

            // Act
            var mappedResult = result.Map(x => x * 2);

            // Assert
            Assert.False(mappedResult.IsSuccess);
            Assert.Equal(error, mappedResult.Error);
        }

        [Fact]
        public void Bind_ChainsSuccessfulResults()
        {
            // Arrange
            var result = Result<int>.Success(5);

            // Act
            var chainedResult = result.Bind(x => Result<int>.Success(x + 10));

            // Assert
            Assert.True(chainedResult.IsSuccess);
            Assert.Equal(15, chainedResult.Value);
        }

        [Fact]
        public void Bind_PropagatesFailureFromOriginal()
        {
            // Arrange
            const string error = "Initial failure";
            var result = Result<int>.Failure(error);

            // Act
            var chainedResult = result.Bind(x => Result<int>.Success(x + 10));

            // Assert
            Assert.False(chainedResult.IsSuccess);
            Assert.Equal(error, chainedResult.Error);
        }

        [Fact]
        public void Bind_PropagatesFailureFromChain()
        {
            // Arrange
            var result = Result<int>.Success(5);

            // Act
            var chainedResult = result.Bind(x => Result<int>.Failure("Chain failed"));

            // Assert
            Assert.False(chainedResult.IsSuccess);
            Assert.Equal("Chain failed", chainedResult.Error);
        }

        [Fact]
        public void Map_WithTypeConversion()
        {
            // Arrange
            var result = Result<int>.Success(100);

            // Act
            var stringResult = result.Map(x => x.ToString());

            // Assert
            Assert.True(stringResult.IsSuccess);
            Assert.Equal("100", stringResult.Value);
        }

        [Fact]
        public void MultipleMapCalls_CreatesComposedTransformation()
        {
            // Arrange
            var result = Result<int>.Success(5);

            // Act
            var composed = result
                .Map(x => x * 2)
                .Map(x => x + 10)
                .Map(x => x.ToString());

            // Assert
            Assert.True(composed.IsSuccess);
            Assert.Equal("20", composed.Value);
        }

        [Fact]
        public void VoidResult_Success()
        {
            // Act
            var result = Result.Success();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.Error);
        }

        [Fact]
        public void VoidResult_Failure()
        {
            // Arrange
            const string error = "Operation error";

            // Act
            var result = Result.Failure(error);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(error, result.Error);
        }

        [Fact]
        public void Result_WithNullValue_Succeeds()
        {
            // Act
            var result = Result<string?>.Success(null);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.Value);
        }

        [Fact]
        public void Result_WithEmptyStringError_CreatesFailure()
        {
            // Arrange - empty string is allowed as error message
            const string error = "";

            // Act
            var result = Result<int>.Failure(error);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(error, result.Error);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(-50)]
        public void Success_WithVariousValues(int value)
        {
            // Act
            var result = Result<int>.Success(value);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(value, result.Value);
        }

        [Theory]
        [InlineData("Error 1")]
        [InlineData("Error with numbers 123")]
        [InlineData("Unicode: 你好")]
        public void Failure_WithVariousErrorMessages(string error)
        {
            // Act
            var result = Result<object>.Failure(error);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(error, result.Error);
        }
    }
}
