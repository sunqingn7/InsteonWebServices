﻿using System;
using System.Collections.Generic;
using System.Text;
using Insteon.Network.Enum;

namespace Insteon.Network.Helpers
{
    internal static class Utilities
    {
        public static byte[] ArraySubset(byte[] data, int offset)
        {
            return ArraySubset(data, offset, data.Length - offset);
        }

        public static byte[] ArraySubset(byte[] data, int offset, int count)
        {
            if (count > data.Length - offset)
            {
                count = data.Length - offset;
            }
            var result = new byte[count];
            Array.Copy(data, offset, result, 0, count);
            return result;
        }

        public static bool ArraySequenceEquals(byte[] a, byte[] b)
        {
            if (a != null && b != null && a.Length == b.Length)
            {
                for (int i = 0; i < a.Length; ++i)
                {
                    if (a[i] != b[i])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static string ByteArrayToString(byte[] data)
        {
            return ByteArrayToString(data, 0, data.Length);
        }

        public static string ByteArrayToString(byte[] data, int offset)
        {
            return ByteArrayToString(data, offset, data.Length - offset);
        }

        public static string ByteArrayToString(byte[] data, int offset, int count)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = offset; i < offset + count; ++i)
            {
                if (i < data.Length)
                {
                    sb.AppendFormat("{1}{0:X2}", data[i], i == offset ? "" : " ");
                }
            }
            return sb.ToString();
        }

        public static string FormatHex(int value)
        {
            if (value <= 0xFF)
            {
                return string.Format("{0:X2}", value);
            }
            if (value <= 0xFFFF)
            {
                return string.Format("{0:X4}", value);
            }
            if (value <= 0xFFFFFF)
            {
                return string.Format("{0:X6}", value);
            }
            return string.Format("{0:X8}", value);
        }

        public static string FormatProperties(Dictionary<PropertyKey, int> properties, bool multiline, bool filterMessageFlags)
        {
            bool first = true;
            StringBuilder sb = new StringBuilder();
            foreach (var item in properties)
            {
                if (!filterMessageFlags || (item.Key != PropertyKey.MessageFlagsRemainingHops && item.Key != PropertyKey.MessageFlagsMaxHops))
                {
                    if (multiline)
                    {
                        sb.AppendLine();
                        sb.Append("  ");
                    }
                    else
                    {
                        if (!first)
                        {
                            sb.Append(" ");
                        }
                    }
                    sb.AppendFormat("{0}={1}", item.Key, FormatHex(item.Value));
                    first = false;
                }
            }
            return sb.ToString();
        }
    }
}