﻿using FastEndpoints;
using FluentValidation;
using Vote.Monitor.Domain.Entities.FormTemplateAggregate.Questions;

namespace Vote.Monitor.Domain.Entities.FormTemplateAggregate.Validation;

public class TextInputQuestionValidator : Validator<TextInputQuestion>
{
    public TextInputQuestionValidator()
    {
        RuleFor(x => x.Text)
            .SetValidator(new TranslatedStringValidator());

        RuleFor(x => x.Helptext)
            .SetValidator(new TranslatedStringValidator())
            .When(x => x.Helptext != null);

        RuleFor(x => x.InputPlaceholder)
            .SetValidator(new TranslatedStringValidator())
            .When(x => x.InputPlaceholder != null);
    }
}