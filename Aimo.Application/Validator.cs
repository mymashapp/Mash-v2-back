#nullable disable
using Aimo.Domain.Infrastructure;
using Aimo.Domain.Labels;
using FluentValidation;
using FluentValidation.Results;

namespace Aimo.Application;

public abstract class Validator<TInstance> : AbstractValidator<TInstance>
{
    private ILocalizer<Validation> _l;
    public ILocalizer<Validation> L => _l ??= EngineContext.Current.Resolve<ILocalizer<Validation>>();

    protected Validator() /*:this(EngineContext.Current.Resolve<ILocalizer<Validation>>())*/
    {
        PostInitialize();
    }

    protected Validator(ILocalizer<Validation> validationLocalizer)
    {
        _l = validationLocalizer;
        PostInitialize();
    }

    private static bool BlankInstancePreValidate(ValidationContext<TInstance> context, ValidationResult result,
        ILocalizer<Validation> L)
    {
        if (context.InstanceToValidate is null)
        {
            result.Errors.Add(new ValidationFailure("", L["Validation.NullOrEmptyObject"]));
            return false;
        }

        if (context.InstanceToValidate.GetType() != typeof(List<int>)) return true;

        var intList = (IList<int>)context.InstanceToValidate;
        if (intList.Any()) return true;


        result.Errors.Add(new ValidationFailure("", L["Validation.SelectAtLeastOne"]));
        return false;
    }


    public override ValidationResult Validate(ValidationContext<TInstance> context)
    {
        var result = new ValidationResult();
        if (!BlankInstancePreValidate(context, result, L))
            return result;

        result = base.Validate(context);
        if (!result.IsValid) return result;

        PostValidate(context, result);

        return result;
    }

    protected virtual void PostInitialize()
    {
    }

    protected virtual void PostValidate(ValidationContext<TInstance> context, ValidationResult result)
    {
    }
}

public static class ValidatorExtension
{
    //TODO: change string to lambda expression
    public static async Task<Result<TInstance>> GetValidationResultFor<TInstance>(this Task<Result<TInstance>> resultTask, string key) where TInstance : new()
    {
        var result = await resultTask;
        var pairs = result.Errors.Where(x =>
            !string.Equals(x.Key, key, StringComparison.InvariantCultureIgnoreCase));
        
        foreach (var pair in pairs) result.Errors.TryRemove(pair);

        result.IsSucceeded = !result.Errors.Any();
        
        return result;
    }

    public static async Task<Result<TInstance>> ValidateResultAsync<TInstance>(this Validator<TInstance> validator,
        TInstance instance) where TInstance : new()
    {
        var results = await validator.ValidateAsync(instance);
        var ret = Result.Create(instance);
        if (results.IsValid) return ret.Success();

        if (results.Errors.Count == 1 && results.Errors.First().PropertyName == "")
            return ret.Failure(results.Errors.First().ErrorMessage);

        //var L = EngineContext.Current.Resolve<ILocalizer<Validation>>();
        var result = ret.Failure(validator.L["Validation.CorrectAllValidationErrors"]);

        foreach (var validationFailure in results.Errors)
            result.Errors.AddOrUpdate(validationFailure.PropertyName, validationFailure.ErrorMessage,
                (_, _) => validationFailure.ErrorMessage);
        //result.Errors.Add(validationFailure.PropertyName, validationFailure.ErrorMessage);

        return result.SetData(instance);
    }
}