﻿namespace Vote.Monitor.Api.Feature.FormTemplate.Models;

public class SectionModel
{
    public Guid Id { get; init; }
    public string Code { get; init; }
    public TranslatedString Title { get; init; }
    public List<BaseQuestionModel> Questions { get; init; }
}