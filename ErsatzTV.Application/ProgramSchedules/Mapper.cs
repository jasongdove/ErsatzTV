﻿using System;
using ErsatzTV.Core.Domain;

namespace ErsatzTV.Application.ProgramSchedules
{
    internal static class Mapper
    {
        internal static ProgramScheduleViewModel ProjectToViewModel(ProgramSchedule programSchedule) =>
            new(programSchedule.Id, programSchedule.Name, programSchedule.MediaCollectionPlaybackOrder);

        internal static ProgramScheduleItemViewModel ProjectToViewModel(ProgramScheduleItem programScheduleItem) =>
            programScheduleItem switch
            {
                ProgramScheduleItemDuration duration =>
                    new ProgramScheduleItemDurationViewModel(
                        duration.Id,
                        duration.Index,
                        duration.StartType,
                        duration.StartTime,
                        duration.CollectionType,
                        duration.Collection != null
                            ? MediaCollections.Mapper.ProjectToViewModel(duration.Collection)
                            : null,
                        duration.MediaItem switch
                        {
                            Show show => MediaItems.Mapper.ProjectToViewModel(show),
                            Season season => MediaItems.Mapper.ProjectToViewModel(season),
                            _ => null
                        },
                        duration.PlayoutDuration,
                        duration.OfflineTail),
                ProgramScheduleItemFlood flood =>
                    new ProgramScheduleItemFloodViewModel(
                        flood.Id,
                        flood.Index,
                        flood.StartType,
                        flood.StartTime,
                        flood.CollectionType,
                        flood.Collection != null
                            ? MediaCollections.Mapper.ProjectToViewModel(flood.Collection)
                            : null,
                        flood.MediaItem switch
                        {
                            Show show => MediaItems.Mapper.ProjectToViewModel(show),
                            Season season => MediaItems.Mapper.ProjectToViewModel(season),
                            _ => null
                        }),
                ProgramScheduleItemMultiple multiple =>
                    new ProgramScheduleItemMultipleViewModel(
                        multiple.Id,
                        multiple.Index,
                        multiple.StartType,
                        multiple.StartTime,
                        multiple.CollectionType,
                        multiple.Collection != null
                            ? MediaCollections.Mapper.ProjectToViewModel(multiple.Collection)
                            : null,
                        multiple.MediaItem switch
                        {
                            Show show => MediaItems.Mapper.ProjectToViewModel(show),
                            Season season => MediaItems.Mapper.ProjectToViewModel(season),
                            _ => null
                        },
                        multiple.Count),
                ProgramScheduleItemOne one =>
                    new ProgramScheduleItemOneViewModel(
                        one.Id,
                        one.Index,
                        one.StartType,
                        one.StartTime,
                        one.CollectionType,
                        one.Collection != null
                            ? MediaCollections.Mapper.ProjectToViewModel(one.Collection)
                            : null,
                        one.MediaItem switch
                        {
                            Show show => MediaItems.Mapper.ProjectToViewModel(show),
                            Season season => MediaItems.Mapper.ProjectToViewModel(season),
                            _ => null
                        }),
                _ => throw new NotSupportedException(
                    $"Unsupported program schedule item type {programScheduleItem.GetType().Name}")
            };
    }
}
