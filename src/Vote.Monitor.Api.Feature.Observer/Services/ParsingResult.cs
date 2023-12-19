﻿namespace Vote.Monitor.Api.Feature.Observer.Services;

public abstract record ParsingResult<T> where T : class
{
    public sealed record Success(IEnumerable<T> Items) : ParsingResult<T>;
    public sealed record Fail(IEnumerable<CsvRowParsed<T>> Items) : ParsingResult<T>
    {
        public Fail(CsvRowParsed<T> item) : this(new List<CsvRowParsed<T>>() { item })
        {
        }
    }


    private ParsingResult() { }
}
