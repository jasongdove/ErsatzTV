﻿using System;
using ErsatzTV.Application.MediaCollections;
using ErsatzTV.Application.MediaItems;
using ErsatzTV.Core.Domain;

namespace ErsatzTV.Application.ProgramSchedules
{
    public record ProgramScheduleItemOneViewModel : ProgramScheduleItemViewModel
    {
        public ProgramScheduleItemOneViewModel(
            int id,
            int index,
            StartType startType,
            TimeSpan? startTime,
            ProgramScheduleItemCollectionType collectionType,
            MediaCollectionViewModel collection,
            NamedMediaItemViewModel mediaItem) : base(
            id,
            index,
            startType,
            startTime,
            PlayoutMode.One,
            collectionType,
            collection,
            mediaItem)
        {
        }
    }
}
