﻿using System.Collections.Generic;
using LanguageExt;
using MediatR;

namespace ErsatzTV.Application.ProgramSchedules.Queries
{
    public record GetProgramScheduleItems(int Id) : IRequest<Option<IEnumerable<ProgramScheduleItemViewModel>>>;
}
