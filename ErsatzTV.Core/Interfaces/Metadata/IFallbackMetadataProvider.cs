﻿using System;
using ErsatzTV.Core.Domain;

namespace ErsatzTV.Core.Interfaces.Metadata
{
    public interface IFallbackMetadataProvider
    {
        ShowMetadata GetFallbackMetadataForShow(string showFolder);
        Tuple<EpisodeMetadata, int> GetFallbackMetadata(Episode episode);
        MovieMetadata GetFallbackMetadata(Movie movie);
        string GetSortTitle(string title);
    }
}
