﻿

namespace FileConversion.Api.ModelBinders
{
    // public class OptionModelBinder<T> : IModelBinder
    // {
    //     public Task BindModelAsync(ModelBindingContext bindingContext)
    //     {
    //         var value = bindingContext.ValueProvider.GetValue(bindingContext.FieldName).FirstValue.SomeNotNull();
    //
    //         value.MatchSome(
    //             val =>
    //             {
    //                 try
    //                 {
    //                     if (bindingContext.ModelType.GenericTypeArguments.First() != typeof(string))
    //                     {
    //                         var deserialized = JsonConvert.DeserializeObject<T>(val).SomeNotNull();
    //                         bindingContext.Result = ModelBindingResult.Success(deserialized);
    //                     }
    //                     else
    //                     {
    //                         bindingContext.Result = ModelBindingResult.Success(val.Some());
    //                     }
    //                 }
    //                 catch (Exception)
    //                 {
    //                     bindingContext.ModelState.AddModelError(bindingContext.FieldName, $"Unable to parse field {bindingContext.FieldName}.");
    //                     bindingContext.Result = ModelBindingResult.Failed();
    //                 }
    //             });
    //
    //         return Task.CompletedTask;
    //     }
    // }
}
