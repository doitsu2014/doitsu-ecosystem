﻿using System.Collections.Generic;
using LanguageExt;
using Shared.Abstraction.Models.Types;

namespace FileConversion.Core.Interface
{
    public interface IBeanMapper
    {
        Either<Error, IEnumerable<object>> Map(IEnumerable<object> data);
    }
}
