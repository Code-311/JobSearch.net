namespace SentinelCareer.Application.DTOs;

public record OpportunityDto(Guid Id, string Title, string Company, string Location, int Score, string ScoreBand, string NextAction);
