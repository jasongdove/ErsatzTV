﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ErsatzTV.Core;
using ErsatzTV.Core.Domain;
using ErsatzTV.Core.Interfaces.Plex;
using ErsatzTV.Core.Interfaces.Repositories;
using LanguageExt;
using MediatR;

namespace ErsatzTV.Application.Plex.Commands
{
    public class
        SynchronizePlexMediaSourcesHandler : IRequestHandler<SynchronizePlexMediaSources,
            Either<BaseError, List<PlexMediaSource>>>
    {
        private readonly IMediaSourceRepository _mediaSourceRepository;
        private readonly IPlexTvApiClient _plexTvApiClient;

        public SynchronizePlexMediaSourcesHandler(
            IMediaSourceRepository mediaSourceRepository,
            IPlexTvApiClient plexTvApiClient)
        {
            _mediaSourceRepository = mediaSourceRepository;
            _plexTvApiClient = plexTvApiClient;
        }

        public Task<Either<BaseError, List<PlexMediaSource>>> Handle(
            SynchronizePlexMediaSources request,
            CancellationToken cancellationToken) => _plexTvApiClient.GetServers().BindAsync(SynchronizeAllServers);

        private async Task<Either<BaseError, List<PlexMediaSource>>> SynchronizeAllServers(
            List<PlexMediaSource> servers)
        {
            List<PlexMediaSource> allExisting = await _mediaSourceRepository.GetAllPlex();
            foreach (PlexMediaSource server in servers)
            {
                await SynchronizeServer(allExisting, server);
            }

            return allExisting;
        }

        private async Task SynchronizeServer(List<PlexMediaSource> allExisting, PlexMediaSource server)
        {
            Option<PlexMediaSource> maybeExisting =
                allExisting.Find(s => s.ClientIdentifier == server.ClientIdentifier);
            await maybeExisting.Match(
                existing =>
                {
                    existing.ProductVersion = server.ProductVersion;
                    existing.ServerName = server.ServerName;
                    MergeConnections(existing.Connections, server.Connections);
                    if (existing.Connections.Any() && existing.Connections.All(c => !c.IsActive))
                    {
                        existing.Connections.Head().IsActive = true;
                    }

                    return _mediaSourceRepository.Update(existing);
                },
                async () =>
                {
                    await _mediaSourceRepository.Add(server);
                    if (server.Connections.Any())
                    {
                        server.Connections.Head().IsActive = true;
                    }

                    await _mediaSourceRepository.Update(server);
                });
        }

        private void MergeConnections(
            List<PlexConnection> existing,
            List<PlexConnection> incoming)
        {
            var toAdd = incoming.Filter(connection => existing.All(c => c.Uri != connection.Uri)).ToList();
            var toRemove = existing.Filter(connection => incoming.All(c => c.Uri != connection.Uri)).ToList();
            existing.AddRange(toAdd);
            toRemove.ForEach(c => existing.Remove(c));
        }
    }
}
