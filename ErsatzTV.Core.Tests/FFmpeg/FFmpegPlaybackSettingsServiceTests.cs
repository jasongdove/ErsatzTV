﻿using System;
using ErsatzTV.Core.Domain;
using ErsatzTV.Core.FFmpeg;
using FluentAssertions;
using NUnit.Framework;

namespace ErsatzTV.Core.Tests.FFmpeg
{
    [TestFixture]
    public class FFmpegPlaybackSettingsCalculatorTests
    {
        [TestFixture]
        public class CalculateSettings
        {
            private readonly FFmpegPlaybackSettingsCalculator _calculator;

            public CalculateSettings() => _calculator = new FFmpegPlaybackSettingsCalculator();

            [Test]
            public void Should_UseSpecifiedThreadCount_ForTransportStream()
            {
                FFmpegProfile ffmpegProfile = TestProfile() with { ThreadCount = 7 };

                FFmpegPlaybackSettings actual = _calculator.CalculateSettings(
                    StreamingMode.TransportStream,
                    ffmpegProfile,
                    new MediaVersion(),
                    DateTimeOffset.Now,
                    DateTimeOffset.Now);

                actual.ThreadCount.Should().Be(7);
            }

            [Test]
            public void Should_UseSpecifiedThreadCount_ForHttpLiveStreaming()
            {
                FFmpegProfile ffmpegProfile = TestProfile() with { ThreadCount = 7 };

                FFmpegPlaybackSettings actual = _calculator.CalculateSettings(
                    StreamingMode.HttpLiveStreaming,
                    ffmpegProfile,
                    new MediaVersion(),
                    DateTimeOffset.Now,
                    DateTimeOffset.Now);

                actual.ThreadCount.Should().Be(7);
            }

            [Test]
            public void Should_SetFormatFlags_ForTransportStream()
            {
                FFmpegProfile ffmpegProfile = TestProfile();

                FFmpegPlaybackSettings actual = _calculator.CalculateSettings(
                    StreamingMode.TransportStream,
                    ffmpegProfile,
                    new MediaVersion(),
                    DateTimeOffset.Now,
                    DateTimeOffset.Now);

                string[] expected = { "+genpts", "+discardcorrupt", "+igndts" };
                actual.FormatFlags.Count.Should().Be(expected.Length);
                actual.FormatFlags.Should().Contain(expected);
            }

            [Test]
            public void Should_SetFormatFlags_ForHttpLiveStreaming()
            {
                FFmpegProfile ffmpegProfile = TestProfile();

                FFmpegPlaybackSettings actual = _calculator.CalculateSettings(
                    StreamingMode.HttpLiveStreaming,
                    ffmpegProfile,
                    new MediaVersion(),
                    DateTimeOffset.Now,
                    DateTimeOffset.Now);

                string[] expected = { "+genpts", "+discardcorrupt", "+igndts" };
                actual.FormatFlags.Count.Should().Be(expected.Length);
                actual.FormatFlags.Should().Contain(expected);
            }

            [Test]
            public void Should_SetRealtime_ForTransportStream()
            {
                FFmpegProfile ffmpegProfile = TestProfile();

                FFmpegPlaybackSettings actual = _calculator.CalculateSettings(
                    StreamingMode.TransportStream,
                    ffmpegProfile,
                    new MediaVersion(),
                    DateTimeOffset.Now,
                    DateTimeOffset.Now);

                actual.RealtimeOutput.Should().BeTrue();
            }

            [Test]
            public void Should_SetRealtime_ForHttpLiveStreaming()
            {
                FFmpegProfile ffmpegProfile = TestProfile();

                FFmpegPlaybackSettings actual = _calculator.CalculateSettings(
                    StreamingMode.HttpLiveStreaming,
                    ffmpegProfile,
                    new MediaVersion(),
                    DateTimeOffset.Now,
                    DateTimeOffset.Now);

                actual.RealtimeOutput.Should().BeTrue();
            }

            [Test]
            public void Should_SetStreamSeek_When_PlaybackIsLate_ForTransportStream()
            {
                DateTimeOffset now = DateTimeOffset.Now;

                FFmpegProfile ffmpegProfile = TestProfile();

                FFmpegPlaybackSettings actual = _calculator.CalculateSettings(
                    StreamingMode.TransportStream,
                    ffmpegProfile,
                    new MediaVersion(),
                    now,
                    now.AddMinutes(5));

                actual.StreamSeek.IsSome.Should().BeTrue();
                actual.StreamSeek.IfNone(TimeSpan.Zero).Should().Be(TimeSpan.FromMinutes(5));
            }

            [Test]
            public void Should_SetStreamSeek_When_PlaybackIsLate_ForHttpLiveStreaming()
            {
                DateTimeOffset now = DateTimeOffset.Now;

                FFmpegProfile ffmpegProfile = TestProfile();

                FFmpegPlaybackSettings actual = _calculator.CalculateSettings(
                    StreamingMode.HttpLiveStreaming,
                    ffmpegProfile,
                    new MediaVersion(),
                    now,
                    now.AddMinutes(5));

                actual.StreamSeek.IsSome.Should().BeTrue();
                actual.StreamSeek.IfNone(TimeSpan.Zero).Should().Be(TimeSpan.FromMinutes(5));
            }

            [Test]
            public void ShouldNot_SetScaledSize_When_NotNormalizingResolution_ForTransportStream()
            {
                FFmpegProfile ffmpegProfile = TestProfile() with { NormalizeResolution = false };

                FFmpegPlaybackSettings actual = _calculator.CalculateSettings(
                    StreamingMode.TransportStream,
                    ffmpegProfile,
                    new MediaVersion(),
                    DateTimeOffset.Now,
                    DateTimeOffset.Now);

                actual.ScaledSize.IsNone.Should().BeTrue();
            }

            [Test]
            public void ShouldNot_SetScaledSize_When_ContentIsCorrectSize_ForTransportStream()
            {
                FFmpegProfile ffmpegProfile = TestProfile() with
                {
                    NormalizeResolution = true,
                    Resolution = new Resolution { Width = 1920, Height = 1080 }
                };

                // not anamorphic
                var version = new MediaVersion { Width = 1920, Height = 1080, SampleAspectRatio = "1:1" };

                FFmpegPlaybackSettings actual = _calculator.CalculateSettings(
                    StreamingMode.TransportStream,
                    ffmpegProfile,
                    version,
                    DateTimeOffset.Now,
                    DateTimeOffset.Now);

                actual.ScaledSize.IsNone.Should().BeTrue();
            }

            [Test]
            public void ShouldNot_SetScaledSize_When_ScaledSizeWouldEqualContentSize_ForTransportStream()
            {
                FFmpegProfile ffmpegProfile = TestProfile() with
                {
                    NormalizeResolution = true,
                    Resolution = new Resolution { Width = 1920, Height = 1080 }
                };

                // not anamorphic
                var version = new MediaVersion { Width = 1918, Height = 1080, SampleAspectRatio = "1:1" };

                FFmpegPlaybackSettings actual = _calculator.CalculateSettings(
                    StreamingMode.TransportStream,
                    ffmpegProfile,
                    version,
                    DateTimeOffset.Now,
                    DateTimeOffset.Now);

                actual.ScaledSize.IsNone.Should().BeTrue();
            }

            [Test]
            public void ShouldNot_PadToDesiredResolution_When_ContentIsCorrectSize_ForTransportStream()
            {
                FFmpegProfile ffmpegProfile = TestProfile() with
                {
                    NormalizeResolution = true,
                    Resolution = new Resolution { Width = 1920, Height = 1080 }
                };

                // not anamorphic
                var version = new MediaVersion { Width = 1920, Height = 1080, SampleAspectRatio = "1:1" };

                FFmpegPlaybackSettings actual = _calculator.CalculateSettings(
                    StreamingMode.TransportStream,
                    ffmpegProfile,
                    version,
                    DateTimeOffset.Now,
                    DateTimeOffset.Now);

                actual.ScaledSize.IsNone.Should().BeTrue();
                actual.PadToDesiredResolution.Should().BeFalse();
            }

            [Test]
            public void Should_PadToDesiredResolution_When_UnscaledContentIsUnderSized_ForTransportStream()
            {
                FFmpegProfile ffmpegProfile = TestProfile() with
                {
                    NormalizeResolution = true,
                    Resolution = new Resolution { Width = 1920, Height = 1080 }
                };

                // not anamorphic
                var version = new MediaVersion { Width = 1918, Height = 1080, SampleAspectRatio = "1:1" };

                FFmpegPlaybackSettings actual = _calculator.CalculateSettings(
                    StreamingMode.TransportStream,
                    ffmpegProfile,
                    version,
                    DateTimeOffset.Now,
                    DateTimeOffset.Now);

                actual.ScaledSize.IsNone.Should().BeTrue();
                actual.PadToDesiredResolution.Should().BeTrue();
            }

            [Test]
            public void Should_NotPadToDesiredResolution_When_UnscaledContentIsUnderSized_ForHttpLiveStreaming()
            {
                FFmpegProfile ffmpegProfile = TestProfile() with
                {
                    NormalizeResolution = true,
                    Resolution = new Resolution { Width = 1920, Height = 1080 }
                };

                // not anamorphic
                var version = new MediaVersion { Width = 1918, Height = 1080, SampleAspectRatio = "1:1" };

                FFmpegPlaybackSettings actual = _calculator.CalculateSettings(
                    StreamingMode.HttpLiveStreaming,
                    ffmpegProfile,
                    version,
                    DateTimeOffset.Now,
                    DateTimeOffset.Now);

                actual.ScaledSize.IsNone.Should().BeTrue();
                actual.PadToDesiredResolution.Should().BeFalse();
            }

            [Test]
            public void Should_NotPadToDesiredResolution_When_NotNormalizingResolution()
            {
                FFmpegProfile ffmpegProfile = TestProfile() with
                {
                    NormalizeResolution = false,
                    Resolution = new Resolution { Width = 1920, Height = 1080 }
                };

                // not anamorphic
                var version = new MediaVersion { Width = 1918, Height = 1080, SampleAspectRatio = "1:1" };

                FFmpegPlaybackSettings actual = _calculator.CalculateSettings(
                    StreamingMode.TransportStream,
                    ffmpegProfile,
                    version,
                    DateTimeOffset.Now,
                    DateTimeOffset.Now);

                actual.ScaledSize.IsNone.Should().BeTrue();
                actual.PadToDesiredResolution.Should().BeFalse();
            }

            [Test]
            public void Should_SetDesiredVideoCodec_When_ContentIsPadded_ForTransportStream()
            {
                var ffmpegProfile = new FFmpegProfile
                {
                    NormalizeResolution = true,
                    Resolution = new Resolution { Width = 1920, Height = 1080 },
                    NormalizeVideoCodec = false,
                    VideoCodec = "testCodec"
                };

                // not anamorphic
                var version = new MediaVersion { Width = 1918, Height = 1080, SampleAspectRatio = "1:1" };

                FFmpegPlaybackSettings actual = _calculator.CalculateSettings(
                    StreamingMode.TransportStream,
                    ffmpegProfile,
                    version,
                    DateTimeOffset.Now,
                    DateTimeOffset.Now);

                actual.ScaledSize.IsNone.Should().BeTrue();
                actual.PadToDesiredResolution.Should().BeTrue();
                actual.VideoCodec.Should().Be("testCodec");
            }

            [Test]
            public void
                Should_SetDesiredVideoCodec_When_ContentIsCorrectSize_And_NormalizingWrongCodec_ForTransportStream()
            {
                var ffmpegProfile = new FFmpegProfile
                {
                    NormalizeResolution = true,
                    Resolution = new Resolution { Width = 1920, Height = 1080 },
                    NormalizeVideoCodec = true,
                    VideoCodec = "testCodec"
                };

                // not anamorphic
                var version = new MediaVersion
                    { Width = 1920, Height = 1080, SampleAspectRatio = "1:1", VideoCodec = "mpeg2video" };

                FFmpegPlaybackSettings actual = _calculator.CalculateSettings(
                    StreamingMode.TransportStream,
                    ffmpegProfile,
                    version,
                    DateTimeOffset.Now,
                    DateTimeOffset.Now);

                actual.ScaledSize.IsNone.Should().BeTrue();
                actual.PadToDesiredResolution.Should().BeFalse();
                actual.VideoCodec.Should().Be("testCodec");
            }

            [Test]
            public void
                Should_SetCopyVideoCodec_When_ContentIsCorrectSize_And_NormalizingWrongCodec_ForHttpLiveStreaming()
            {
                var ffmpegProfile = new FFmpegProfile
                {
                    NormalizeResolution = true,
                    Resolution = new Resolution { Width = 1920, Height = 1080 },
                    NormalizeVideoCodec = true,
                    VideoCodec = "testCodec"
                };

                // not anamorphic
                var version = new MediaVersion
                    { Width = 1920, Height = 1080, SampleAspectRatio = "1:1", VideoCodec = "mpeg2video" };

                FFmpegPlaybackSettings actual = _calculator.CalculateSettings(
                    StreamingMode.HttpLiveStreaming,
                    ffmpegProfile,
                    version,
                    DateTimeOffset.Now,
                    DateTimeOffset.Now);

                actual.ScaledSize.IsNone.Should().BeTrue();
                actual.PadToDesiredResolution.Should().BeFalse();
                actual.VideoCodec.Should().Be("copy");
            }

            [Test]
            public void Should_SetCopyVideoCodec_When_ContentIsCorrectSize_And_CorrectCodec_ForTransportStream()
            {
                var ffmpegProfile = new FFmpegProfile
                {
                    NormalizeResolution = true,
                    Resolution = new Resolution { Width = 1920, Height = 1080 },
                    NormalizeVideoCodec = true,
                    VideoCodec = "libx264"
                };

                // not anamorphic
                var version = new MediaVersion
                    { Width = 1920, Height = 1080, SampleAspectRatio = "1:1", VideoCodec = "libx264" };

                FFmpegPlaybackSettings actual = _calculator.CalculateSettings(
                    StreamingMode.TransportStream,
                    ffmpegProfile,
                    version,
                    DateTimeOffset.Now,
                    DateTimeOffset.Now);

                actual.ScaledSize.IsNone.Should().BeTrue();
                actual.PadToDesiredResolution.Should().BeFalse();
                actual.VideoCodec.Should().Be("copy");
            }

            [Test]
            public void
                Should_SetCopyVideoCodec_When_ContentIsCorrectSize_And_NotNormalizingWrongCodec_ForTransportStream()
            {
                var ffmpegProfile = new FFmpegProfile
                {
                    NormalizeResolution = true,
                    Resolution = new Resolution { Width = 1920, Height = 1080 },
                    NormalizeVideoCodec = false,
                    VideoCodec = "libx264"
                };

                // not anamorphic
                var version = new MediaVersion
                    { Width = 1920, Height = 1080, SampleAspectRatio = "1:1", VideoCodec = "mpeg2video" };

                FFmpegPlaybackSettings actual = _calculator.CalculateSettings(
                    StreamingMode.TransportStream,
                    ffmpegProfile,
                    version,
                    DateTimeOffset.Now,
                    DateTimeOffset.Now);

                actual.ScaledSize.IsNone.Should().BeTrue();
                actual.PadToDesiredResolution.Should().BeFalse();
                actual.VideoCodec.Should().Be("copy");
            }

            [Test]
            public void Should_SetVideoBitrate_When_ContentIsPadded_ForTransportStream()
            {
                var ffmpegProfile = new FFmpegProfile
                {
                    NormalizeResolution = true,
                    Resolution = new Resolution { Width = 1920, Height = 1080 },
                    NormalizeVideoCodec = false,
                    VideoBitrate = 2525
                };

                // not anamorphic
                var version = new MediaVersion { Width = 1918, Height = 1080, SampleAspectRatio = "1:1" };

                FFmpegPlaybackSettings actual = _calculator.CalculateSettings(
                    StreamingMode.TransportStream,
                    ffmpegProfile,
                    version,
                    DateTimeOffset.Now,
                    DateTimeOffset.Now);

                actual.ScaledSize.IsNone.Should().BeTrue();
                actual.PadToDesiredResolution.Should().BeTrue();
                actual.VideoBitrate.IfNone(0).Should().Be(2525);
            }

            [Test]
            public void Should_SetVideoBitrate_When_ContentIsCorrectSize_And_NormalizingWrongCodec_ForTransportStream()
            {
                var ffmpegProfile = new FFmpegProfile
                {
                    NormalizeResolution = true,
                    Resolution = new Resolution { Width = 1920, Height = 1080 },
                    NormalizeVideoCodec = true,
                    VideoBitrate = 2525
                };

                // not anamorphic
                var version = new MediaVersion
                    { Width = 1920, Height = 1080, SampleAspectRatio = "1:1", VideoCodec = "mpeg2video" };

                FFmpegPlaybackSettings actual = _calculator.CalculateSettings(
                    StreamingMode.TransportStream,
                    ffmpegProfile,
                    version,
                    DateTimeOffset.Now,
                    DateTimeOffset.Now);

                actual.ScaledSize.IsNone.Should().BeTrue();
                actual.PadToDesiredResolution.Should().BeFalse();
                actual.VideoBitrate.IfNone(0).Should().Be(2525);
            }

            [Test]
            public void Should_SetVideoBufferSize_When_ContentIsPadded_ForTransportStream()
            {
                var ffmpegProfile = new FFmpegProfile
                {
                    NormalizeResolution = true,
                    Resolution = new Resolution { Width = 1920, Height = 1080 },
                    NormalizeVideoCodec = false,
                    VideoBufferSize = 2525
                };

                // not anamorphic
                var version = new MediaVersion { Width = 1918, Height = 1080, SampleAspectRatio = "1:1" };

                FFmpegPlaybackSettings actual = _calculator.CalculateSettings(
                    StreamingMode.TransportStream,
                    ffmpegProfile,
                    version,
                    DateTimeOffset.Now,
                    DateTimeOffset.Now);

                actual.ScaledSize.IsNone.Should().BeTrue();
                actual.PadToDesiredResolution.Should().BeTrue();
                actual.VideoBufferSize.IfNone(0).Should().Be(2525);
            }

            [Test]
            public void
                Should_SetVideoBufferSize_When_ContentIsCorrectSize_And_NormalizingWrongCodec_ForTransportStream()
            {
                var ffmpegProfile = new FFmpegProfile
                {
                    NormalizeResolution = true,
                    Resolution = new Resolution { Width = 1920, Height = 1080 },
                    NormalizeVideoCodec = true,
                    VideoBufferSize = 2525
                };

                // not anamorphic
                var version = new MediaVersion
                    { Width = 1920, Height = 1080, SampleAspectRatio = "1:1", VideoCodec = "mpeg2video" };

                FFmpegPlaybackSettings actual = _calculator.CalculateSettings(
                    StreamingMode.TransportStream,
                    ffmpegProfile,
                    version,
                    DateTimeOffset.Now,
                    DateTimeOffset.Now);

                actual.ScaledSize.IsNone.Should().BeTrue();
                actual.PadToDesiredResolution.Should().BeFalse();
                actual.VideoBufferSize.IfNone(0).Should().Be(2525);
            }

            [Test]
            public void Should_SetCopyAudioCodec_When_CorrectCodec_ForTransportStream()
            {
                FFmpegProfile ffmpegProfile = TestProfile() with
                {
                    NormalizeAudioCodec = true,
                    AudioCodec = "aac"
                };

                var version = new MediaVersion { AudioCodec = "aac" };

                FFmpegPlaybackSettings actual = _calculator.CalculateSettings(
                    StreamingMode.TransportStream,
                    ffmpegProfile,
                    version,
                    DateTimeOffset.Now,
                    DateTimeOffset.Now);

                actual.AudioCodec.Should().Be("copy");
            }

            [Test]
            public void Should_SetCopyAudioCodec_When_NotNormalizingWrongCodec_ForTransportStream()
            {
                FFmpegProfile ffmpegProfile = TestProfile() with
                {
                    NormalizeAudioCodec = false,
                    AudioCodec = "aac"
                };

                var version = new MediaVersion { AudioCodec = "ac3" };

                FFmpegPlaybackSettings actual = _calculator.CalculateSettings(
                    StreamingMode.TransportStream,
                    ffmpegProfile,
                    version,
                    DateTimeOffset.Now,
                    DateTimeOffset.Now);

                actual.AudioCodec.Should().Be("copy");
            }

            [Test]
            public void Should_SetDesiredAudioCodec_When_NormalizingWrongCodec_ForTransportStream()
            {
                FFmpegProfile ffmpegProfile = TestProfile() with
                {
                    NormalizeAudioCodec = true,
                    AudioCodec = "aac"
                };

                var version = new MediaVersion { AudioCodec = "ac3" };

                FFmpegPlaybackSettings actual = _calculator.CalculateSettings(
                    StreamingMode.TransportStream,
                    ffmpegProfile,
                    version,
                    DateTimeOffset.Now,
                    DateTimeOffset.Now);

                actual.AudioCodec.Should().Be("aac");
            }

            [Test]
            public void Should_SetCopyAudioCodec_When_NormalizingWrongCodec_ForHttpLiveStreaming()
            {
                FFmpegProfile ffmpegProfile = TestProfile() with
                {
                    NormalizeAudioCodec = true,
                    AudioCodec = "aac"
                };

                var version = new MediaVersion { AudioCodec = "ac3" };

                FFmpegPlaybackSettings actual = _calculator.CalculateSettings(
                    StreamingMode.HttpLiveStreaming,
                    ffmpegProfile,
                    version,
                    DateTimeOffset.Now,
                    DateTimeOffset.Now);

                actual.AudioCodec.Should().Be("copy");
            }

            [Test]
            public void Should_SetAudioBitrate_When_NormalizingWrongCodec_ForTransportStream()
            {
                FFmpegProfile ffmpegProfile = TestProfile() with
                {
                    NormalizeAudioCodec = true,
                    AudioBitrate = 2424
                };

                var version = new MediaVersion { AudioCodec = "ac3" };

                FFmpegPlaybackSettings actual = _calculator.CalculateSettings(
                    StreamingMode.TransportStream,
                    ffmpegProfile,
                    version,
                    DateTimeOffset.Now,
                    DateTimeOffset.Now);

                actual.AudioBitrate.IfNone(0).Should().Be(2424);
            }

            [Test]
            public void Should_SetAudioBufferSize_When_NormalizingWrongCodec_ForTransportStream()
            {
                FFmpegProfile ffmpegProfile = TestProfile() with
                {
                    NormalizeAudioCodec = true,
                    AudioBufferSize = 2424
                };

                var version = new MediaVersion { AudioCodec = "ac3" };

                FFmpegPlaybackSettings actual = _calculator.CalculateSettings(
                    StreamingMode.TransportStream,
                    ffmpegProfile,
                    version,
                    DateTimeOffset.Now,
                    DateTimeOffset.Now);

                actual.AudioBufferSize.IfNone(0).Should().Be(2424);
            }

            [Test]
            public void ShouldNot_SetAudioChannels_When_CorrectCodec_ForTransportStream()
            {
                FFmpegProfile ffmpegProfile = TestProfile() with
                {
                    NormalizeAudioCodec = true,
                    NormalizeAudio = true,
                    AudioCodec = "ac3",
                    AudioChannels = 6
                };

                var version = new MediaVersion { AudioCodec = "ac3" };

                FFmpegPlaybackSettings actual = _calculator.CalculateSettings(
                    StreamingMode.TransportStream,
                    ffmpegProfile,
                    version,
                    DateTimeOffset.Now,
                    DateTimeOffset.Now);

                actual.AudioChannels.IsNone.Should().BeTrue();
            }

            [Test]
            public void ShouldNot_SetAudioSampleRate_When_CorrectCodec_ForTransportStream()
            {
                FFmpegProfile ffmpegProfile = TestProfile() with
                {
                    NormalizeAudioCodec = true,
                    NormalizeAudio = true,
                    AudioCodec = "ac3",
                    AudioSampleRate = 48
                };

                var version = new MediaVersion { AudioCodec = "ac3" };

                FFmpegPlaybackSettings actual = _calculator.CalculateSettings(
                    StreamingMode.TransportStream,
                    ffmpegProfile,
                    version,
                    DateTimeOffset.Now,
                    DateTimeOffset.Now);

                actual.AudioSampleRate.IsNone.Should().BeTrue();
            }

            [Test]
            public void Should_SetAudioChannels_When_NormalizingWrongCodecAndAudio_ForTransportStream()
            {
                FFmpegProfile ffmpegProfile = TestProfile() with
                {
                    NormalizeAudioCodec = true,
                    NormalizeAudio = true,
                    AudioChannels = 6
                };

                var version = new MediaVersion { AudioCodec = "ac3" };

                FFmpegPlaybackSettings actual = _calculator.CalculateSettings(
                    StreamingMode.TransportStream,
                    ffmpegProfile,
                    version,
                    DateTimeOffset.Now,
                    DateTimeOffset.Now);

                actual.AudioChannels.IfNone(0).Should().Be(6);
            }

            [Test]
            public void Should_SetAudioSampleRate_When_NormalizingWrongCodecAndAudio_ForTransportStream()
            {
                FFmpegProfile ffmpegProfile = TestProfile() with
                {
                    NormalizeAudioCodec = true,
                    NormalizeAudio = true,
                    AudioSampleRate = 48
                };

                var version = new MediaVersion { AudioCodec = "ac3" };

                FFmpegPlaybackSettings actual = _calculator.CalculateSettings(
                    StreamingMode.TransportStream,
                    ffmpegProfile,
                    version,
                    DateTimeOffset.Now,
                    DateTimeOffset.Now);

                actual.AudioSampleRate.IfNone(0).Should().Be(48);
            }
        }

        [TestFixture]
        public class CalculateSettingsQsv
        {
            private readonly FFmpegPlaybackSettingsCalculator _calculator;

            public CalculateSettingsQsv() => _calculator = new FFmpegPlaybackSettingsCalculator();

            [Test]
            public void Should_UseHardwareAcceleration()
            {
                FFmpegProfile ffmpegProfile =
                    TestProfile() with { HardwareAcceleration = HardwareAccelerationKind.Qsv };

                FFmpegPlaybackSettings actual = _calculator.CalculateSettings(
                    StreamingMode.TransportStream,
                    ffmpegProfile,
                    new MediaVersion(),
                    DateTimeOffset.Now,
                    DateTimeOffset.Now);

                actual.HardwareAcceleration.Should().Be(HardwareAccelerationKind.Qsv);
            }
        }

        private static FFmpegProfile TestProfile() =>
            new() { Resolution = new Resolution { Width = 1920, Height = 1080 } };
    }
}
