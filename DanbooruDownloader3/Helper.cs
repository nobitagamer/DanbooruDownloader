﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using DanbooruDownloader3.Entity;

namespace DanbooruDownloader3
{
    public static class Helper
    {
        /// <summary>
        /// Generate hashed password+salt using SHA1
        /// </summary>
        /// <param name="password"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public static string GeneratePasswordHash(string password, string salt)
        {
            string hash = "";
            string temp = salt.Replace("%PASSWORD%", password);

            byte[] result = SHA1.Create().ComputeHash(Encoding.ASCII.GetBytes(temp));

            foreach (byte b in result)
            {
                string hexValue = b.ToString("X").ToLower(); // Lowercase for compatibility on case-sensitive systems
                hash += (hexValue.Length == 1 ? "0" : "") + hexValue;
            }

            return hash;
        }

        /// <summary>
        /// Pad user agent with current date/time.
        /// </summary>
        /// <param name="originalUserAgent"></param>
        /// <returns></returns>
        public static string PadUserAgent(string originalUserAgent)
        {
            if (originalUserAgent != null && originalUserAgent.Length > 0)
            {
                return originalUserAgent + DateTime.UtcNow;
            }
            else return null;
        }

        /// <summary>
        /// Sanitize the filename.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string SanitizeFilename(string input)
        {
            if (input == null) return "";
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                input = input.Replace(c, '_');
            }
            input = input.Replace(':', '_');
            input = input.Replace('\\', '_');
            return input;
        }

        /// <summary>
        /// For constructing filename.
        /// </summary>
        /// <param name="baseFolder"></param>
        /// <param name="format"></param>
        /// <param name="post"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public static string MakeFilename(string baseFolder, string format, DanbooruPost post, int limit)
        {
            string filename = format;
            string provider = post.Provider;
            string query = post.Query;
            string searchTags = post.SearchTags;

            filename = filename.Replace("%provider%", Helper.SanitizeFilename(provider));
            filename = filename.Replace("%id%", post.Id);
            filename = filename.Replace("%tags%", Helper.SanitizeFilename(post.Tags));
            filename = filename.Replace("%rating%", post.Rating);
            filename = filename.Replace("%md5%", post.MD5);
            filename = filename.Replace("%query%", Helper.SanitizeFilename(query));
            filename = filename.Replace("%searchtag%", Helper.SanitizeFilename(searchTags));

            var artist = string.Join(" ", post.TagsEntity.Where<DanbooruTag>(x => x.Type == DanbooruTagType.Artist).Select(x => x.Name));
            filename = filename.Replace("%artist%", Helper.SanitizeFilename(artist));
            var copyright = string.Join(" ", post.TagsEntity.Where<DanbooruTag>(x => x.Type == DanbooruTagType.Copyright).Select(x => x.Name));
            filename = filename.Replace("%copyright%", Helper.SanitizeFilename(copyright));
            var character = string.Join(" ", post.TagsEntity.Where<DanbooruTag>(x => x.Type == DanbooruTagType.Character).Select(x => x.Name));
            filename = filename.Replace("%character%", Helper.SanitizeFilename(character));
            var circle = string.Join(" ", post.TagsEntity.Where<DanbooruTag>(x => x.Type == DanbooruTagType.Circle).Select(x => x.Name));
            filename = filename.Replace("%circle%", Helper.SanitizeFilename(circle));
            var faults = string.Join(" ", post.TagsEntity.Where<DanbooruTag>(x => x.Type == DanbooruTagType.Faults).Select(x => x.Name));
            filename = filename.Replace("%faults%", Helper.SanitizeFilename(faults));
            

            if (baseFolder.EndsWith(@"\")) filename = baseFolder + filename;
            else filename = baseFolder + @"\" + filename;

            filename = filename.Substring(0, filename.Length < limit ? filename.Length : limit);

            string dir = filename.Substring(0, filename.LastIndexOf(@"\"));
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            return filename;
        }

    }
}