﻿using System.Threading.Tasks;
using ErsatzTV.Core;
using LanguageExt;
using MediatR;

namespace ErsatzTV.Application.ProgramSchedules.Commands
{
    public record DeleteProgramSchedule(int ProgramScheduleId) : IRequest<Either<BaseError, Task>>;
}
