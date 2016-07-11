﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FlacLibSharp
{
    /// <summary>
    /// Wrapper class for fast access to some of the FLAC functions
    /// </summary>
    public class FastFlac
    {

        #region Metadata Fetchers

        /// <summary>
        /// Gives you all available metadata blocks in the flac file.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static List<MetadataBlock> GetMetaData(string path)
        {
            using (FlacFile flac = new FlacFile(path))
            {
                return flac.Metadata;
            }
        }

        /// <summary>
        /// Gives you the StreamInfo metadata
        /// </summary>
        /// <param name="path"></param>
        /// <returns>The StreamInfo metadata or null if no StreamInfo metadata is found.</returns>
        public static StreamInfo GetStreamInfo(string path)
        {
            using (FlacFile flac = new FlacFile(path))
            {
                return flac.StreamInfo;
            }
        }

        /// <summary>
        /// Gives you the vorbis comment metadata (ID3V2 tags).
        /// </summary>
        /// <param name="path"></param>
        /// <returns>The vorbis comment metadata or null if none is available.</returns>
        public static VorbisComment GetVorbisComment(string path)
        {
            using (FlacFile flac = new FlacFile(path))
            {
                return flac.VorbisComment;
            }
        }

        #endregion

        #region Detail Fetchers

        /// <summary>
        /// Gets the specific vorbis field name (example = ARTIST) if it is available.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fieldName"></param>
        /// <returns>The value of the field or an empty string if the field is not available.</returns>
        public static VorbisCommentValues GetVorbisField(string path, string fieldName)
        {
            using (FlacFile flac = new FlacFile(path))
            {
                if (flac.VorbisComment != null && flac.VorbisComment.ContainsField(fieldName))
                {
                    return flac.VorbisComment[fieldName];
                }
                return new VorbisCommentValues();
            }
        }

        /// <summary>
        /// Gets the first artist of the track.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Empty string if the track number isn't specified in the metadata</returns>
        public static string GetArtist(string path)
        {
            return GetVorbisField(path, "ARTIST").First;
        }
        
        /// <summary>
        /// Gets the first title of the track.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Empty string if the track number isn't specified in the metadata</returns>
        public static string GetTitle(string path)
        {
            return GetVorbisField(path, "TITLE").First;
        }

        /// <summary>
        /// Gets the first album name.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Empty string if the track number isn't specified in the metadata</returns>
        public static string GetAlbum(string path)
        {
            return GetVorbisField(path, "ALBUM").First;
        }

        /// <summary>
        /// Gets the first track number.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Empty string if the track number isn't specified in the metadata</returns>
        public static string GetTrackNumber(string path)
        {
            return GetVorbisField(path, "TRACKNUMBER").First;
        }

        /// <summary>
        /// Gets the first genre of the track.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Empty string if no genre is specified in the metadata.</returns>
        public static string GetGenre(string path)
        {
            return GetVorbisField(path, "GENRE").First;
        }

        /// <summary>
        /// Gets the duration of the track in seconds.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>The duration of the track in seconds or 0 if the duration is not known (sample count is missing from streaminfo metadata)</returns>
        public static int GetDuration(string path)
        {
            using (FlacFile file = new FlacFile(path))
            {
                return file.StreamInfo.Duration;
            }
        }

        #endregion


    }
}
