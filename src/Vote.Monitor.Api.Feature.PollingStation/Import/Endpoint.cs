﻿using Vote.Monitor.Api.Feature.PollingStation.Helpers;
using Vote.Monitor.Api.Feature.PollingStation.Services;
using Vote.Monitor.Core.Services.Time;

namespace Vote.Monitor.Api.Feature.PollingStation.Import;
public class Endpoint(
    IRepository<ElectionRoundAggregate> electionRoundRepository,
    VoteMonitorContext context,
    IPollingStationParser parser,
    ITimeProvider timeProvider)
    : Endpoint<Request, Results<Ok<Response>, NotFound<ProblemDetails>, ProblemDetails>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/polling-stations:import");
        DontAutoTag();
        Options(x => x.WithTags("polling-stations"));
        AllowFileUploads();
    }

    public override async Task<Results<Ok<Response>, NotFound<ProblemDetails>, ProblemDetails>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var electionRound = await electionRoundRepository.GetByIdAsync(req.ElectionRoundId, ct);
        if (electionRound is null)
        {
            AddError(r => r.ElectionRoundId, "A polling station with same address and tags exists");
            return TypedResults.NotFound(new ProblemDetails(ValidationFailures));
        }

        var parsingResult = parser.Parse(req.File.OpenReadStream());
        if (parsingResult is PollingStationParsingResult.Fail failedResult)
        {
            foreach (var validationFailure in failedResult.ValidationErrors.SelectMany(x => x.Errors))
            {
                AddError(validationFailure);
            }

            ThrowIfAnyErrors();
        }

        var successResult = parsingResult as PollingStationParsingResult.Success;

        var entities = successResult!
        .PollingStations
            .Select(x => new PollingStationAggregate(electionRound, x.Address, x.DisplayOrder, x.Tags.ToTagsObject(), timeProvider))
            .ToList();

        await context.PollingStations.BatchDeleteAsync(cancellationToken: ct);
        await context.BulkInsertAsync(entities, cancellationToken: ct);
        await context.BulkSaveChangesAsync(cancellationToken: ct);

        return TypedResults.Ok(new Response { RowsImported = entities.Count });
    }
}
