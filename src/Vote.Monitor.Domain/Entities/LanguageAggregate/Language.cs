﻿using Vote.Monitor.Core.Extensions;

namespace Vote.Monitor.Domain.Entities.LanguageAggregate;

public class Language : BaseEntity, IAggregateRoot
{
#pragma warning disable CS8618 // Required by Entity Framework
    private Language()
    {
    }
#pragma warning restore CS8618

    /// <summary>
    /// English language name
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Native language name
    /// </summary>
    public string NativeName { get; }

    /// <summary>
    /// Two-letter language code (ISO 639-1)
    /// </summary>
    public string Iso1 { get; }

    public Language(string name,
        string nativeName,
        string iso1,
        ITimeProvider timeProvider) : base(iso1.ToGuid(), timeProvider)
    {
        Id = iso1.ToGuid();
        Name = name;
        NativeName = nativeName;
        Iso1 = iso1;
    }
}
