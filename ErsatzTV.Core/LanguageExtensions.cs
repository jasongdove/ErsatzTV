﻿using System.Threading.Tasks;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace ErsatzTV.Core
{
    public static class LanguageExtensions
    {
        public static Either<BaseError, TR> ToEither<TR>(this Validation<BaseError, TR> validation) =>
            validation.ToEither().MapLeft(errors => errors.Join());

        public static Task<Either<BaseError, TR>> ToEitherAsync<TR>(this Validation<BaseError, Task<TR>> validation) =>
            validation.ToEither()
                .MapLeft(errors => errors.Join())
                .MapAsync<BaseError, Task<TR>, TR>(e => e);

        public static Task<T> LogFailure<T>(
            this TryAsync<T> tryAsync,
            T defaultValue,
            ILogger logger,
            string message,
            params object[] args) =>
            tryAsync.IfFail(
                ex =>
                {
                    logger.LogError(ex, message, args);
                    return defaultValue;
                });

        public static Task<Unit> LogFailure(
            this TryAsync<Unit> tryAsync,
            ILogger logger,
            string message,
            params object[] args) => LogFailure(tryAsync, Unit.Default, logger, message, args);
    }
}
