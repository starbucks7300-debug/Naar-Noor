using FluentAssertions;
using NaarNoor.Application.Reviews.Queries.GetApprovedReviews;
using Xunit;

namespace NaarNoor.Application.Tests.Reviews;

public class GetApprovedReviewsQueryTests
{
    [Fact]
    public void GetApprovedReviewsQuery_CanBeInstantiated()
    {
        var query = new GetApprovedReviewsQuery();

        query.Should().NotBeNull();
    }

    [Fact]
    public void GetApprovedReviewsQuery_TwoInstances_AreEqual()
    {
        var q1 = new GetApprovedReviewsQuery();
        var q2 = new GetApprovedReviewsQuery();

        q1.Should().Be(q2, "parameterless records with same type are equal");
    }

    [Fact]
    public void GetApprovedReviewsQuery_GetHashCode_IsConsistent()
    {
        var query = new GetApprovedReviewsQuery();

        query.GetHashCode().Should().Be(query.GetHashCode());
    }

    [Fact]
    public void GetApprovedReviewsQuery_ToString_ReturnsNonEmpty()
    {
        var query = new GetApprovedReviewsQuery();

        query.ToString().Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void GetApprovedReviewsQuery_Equals_SameInstance_IsTrue()
    {
        var query = new GetApprovedReviewsQuery();

        query.Equals(query).Should().BeTrue();
    }

    [Fact]
    public void GetApprovedReviewsQuery_Equals_Null_IsFalse()
    {
        var query = new GetApprovedReviewsQuery();

        query.Equals(null).Should().BeFalse();
    }

    [Fact]
    public void ReviewDto_CanBeConstructed_WithAllFields()
    {
        var id = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;

        var dto = new ReviewDto(id, "John Doe", 5, "Excellent food!", "Google", createdAt);

        dto.Id.Should().Be(id);
        dto.CustomerName.Should().Be("John Doe");
        dto.Rating.Should().Be(5);
        dto.Comment.Should().Be("Excellent food!");
        dto.Source.Should().Be("Google");
        dto.CreatedAt.Should().Be(createdAt);
    }

    [Fact]
    public void ReviewDto_Source_CanBeNull()
    {
        var dto = new ReviewDto(Guid.NewGuid(), "Jane", 4, "Very good", null, DateTime.UtcNow);

        dto.Source.Should().BeNull();
    }

    [Fact]
    public void ReviewDto_TwoIdenticalInstances_AreEqual()
    {
        var id = Guid.NewGuid();
        var createdAt = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        var dto1 = new ReviewDto(id, "Alice", 5, "Perfect", "TripAdvisor", createdAt);
        var dto2 = new ReviewDto(id, "Alice", 5, "Perfect", "TripAdvisor", createdAt);

        dto1.Should().Be(dto2);
    }

    [Fact]
    public void ReviewDto_DifferentRatings_AreNotEqual()
    {
        var id = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;

        var dto1 = new ReviewDto(id, "Alice", 5, "Perfect", "Google", createdAt);
        var dto2 = new ReviewDto(id, "Alice", 4, "Good", "Google", createdAt);

        dto1.Should().NotBe(dto2);
    }

    [Fact]
    public void ReviewDto_ToString_ContainsTypeName()
    {
        var dto = new ReviewDto(Guid.NewGuid(), "Bob", 3, "Average", "Direct", DateTime.UtcNow);

        dto.ToString().Should().Contain("ReviewDto");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void ReviewDto_RatingRange_IsStoredCorrectly(int rating)
    {
        var dto = new ReviewDto(Guid.NewGuid(), "Customer", rating, "Comment", "Source", DateTime.UtcNow);

        dto.Rating.Should().Be(rating);
    }
}
