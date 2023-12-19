﻿namespace Vote.Monitor.Api.Feature.Observer.Services;
internal class ObserverImportModelValidator : Validator<ObserverImportModel>
{
    public ObserverImportModelValidator()
    {

        RuleFor(x => x.Name)
          .MinimumLength(3);

        RuleFor(x => x.Email)
            .EmailAddress();

        RuleFor(x => x.Password)
            .MinimumLength(3);

        RuleFor(x => x.PhoneNumber)
           .MinimumLength(8); //lenhth of phone number can be 8 or 9


    }
}
