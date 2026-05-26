using MediatR;
using Microsoft.AspNetCore.Mvc;
using NaarNoor.Application.Reviews.Queries.GetApprovedReviews;

namespace NaarNoor.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReviewsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<ReviewDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetApprovedReviewsQuery(), cancellationToken);
        return Ok(result);
    }
}
