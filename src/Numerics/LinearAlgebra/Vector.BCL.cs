﻿// <copyright file="Vector.BCL.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://numerics.mathdotnet.com
// http://github.com/mathnet/mathnet-numerics
// http://mathnetnumerics.codeplex.com
//
// Copyright (c) 2009-2013 Math.NET
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Storage;
using MathNet.Numerics.Properties;

namespace MathNet.Numerics.LinearAlgebra
{
  [DebuggerDisplay("[{ToString(\"F6\")}]")]
  public abstract partial class Vector<T>
  {
    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>
    ///    <c>true</c> if the current object is equal to the <paramref name="other"/> parameter; otherwise, <c>false</c>.
    /// </returns>
    public bool Equals(Vector<T> other)
    {
      return other != null && Storage.Equals(other.Storage);
    }

    /// <summary>
    /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
    /// </summary>
    /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
    /// <returns>
    ///     <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
    /// </returns>
    public override sealed bool Equals(object obj)
    {
      var other = obj as Vector<T>;
      return other != null && Storage.Equals(other.Storage);
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>
    /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
    /// </returns>
    public override sealed int GetHashCode()
    {
      return Storage.GetHashCode();
    }

#if !PORTABLE

    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>
    /// A new object that is a copy of this instance.
    /// </returns>
    object ICloneable.Clone()
    {
      return Clone();
    }

#endif

    int IList<T>.IndexOf(T item)
    {
      for (int i = 0; i < Count; ++i)
      {
        if (At(i).Equals(item))
          return i;
      }
      return -1;
    }

    void IList<T>.Insert(int index, T item)
    {
      throw new NotSupportedException();
    }

    void IList<T>.RemoveAt(int index)
    {
      throw new NotSupportedException();
    }

    bool ICollection<T>.IsReadOnly
    {
      get
      {
        return false;
      }
    }

    void ICollection<T>.Add(T item)
    {
      throw new NotSupportedException();
    }

    bool ICollection<T>.Remove(T item)
    {
      throw new NotSupportedException();
    }

    bool ICollection<T>.Contains(T item)
    {
      // ReSharper disable once LoopCanBeConvertedToQuery
      foreach (var x in this)
      {
        if (x.Equals(item))
          return true;
      }
      return false;
    }

    void ICollection<T>.CopyTo(T[] array, int arrayIndex)
    {
      if (array == null)
      {
        throw new ArgumentNullException("array");
      }

      Storage.CopySubVectorTo(new DenseVectorStorage<T>(array.Length, array), 0, arrayIndex, Count);
    }

    bool IList.IsReadOnly
    {
      get
      {
        return false;
      }
    }

    bool IList.IsFixedSize
    {
      get
      {
        return true;
      }
    }

    object IList.this[int index]
    {
      get
      {
        return Storage[index];
      }
      set
      {
        Storage[index] = (T)value;
      }
    }

    int IList.IndexOf(object value)
    {
      if (!(value is T))
      {
        return -1;
      }

      return ((IList<T>)this).IndexOf((T)value);
    }

    bool IList.Contains(object value)
    {
      if (!(value is T))
      {
        return false;
      }

      return ((ICollection<T>)this).Contains((T)value);
    }

    void IList.Insert(int index, object value)
    {
      throw new NotSupportedException();
    }

    int IList.Add(object value)
    {
      throw new NotSupportedException();
    }

    void IList.Remove(object value)
    {
      throw new NotSupportedException();
    }

    void IList.RemoveAt(int index)
    {
      throw new NotSupportedException();
    }

    bool ICollection.IsSynchronized
    {
      get
      {
        return false;
      }
    }

    object ICollection.SyncRoot
    {
      get
      {
        return Storage;
      }
    }

    void ICollection.CopyTo(Array array, int index)
    {
      if (array == null)
      {
        throw new ArgumentNullException("array");
      }
      if (array.Rank != 1)
      {
        throw new ArgumentException(Resources.ArgumentSingleDimensionArray, "array");
      }

      Storage.CopySubVectorTo(new DenseVectorStorage<T>(array.Length, (T[])array), 0, index, Count);
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
    /// </returns>
    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
      return Enumerate().GetEnumerator();
    }

    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
    /// </returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
      return Enumerate().GetEnumerator();
    }

    /// <summary>
    /// Returns a string that describes the type, dimensions and shape of this vector.
    /// </summary>
    public virtual string ToTypeString()
    {
      return string.Format("{0} {1}-{2}", GetType().Name, Count, typeof(T).Name);
    }

    public string[,] ToVectorStringArray(int maxPerColumn, int maxWidth, int padding, string ellipsis, Func<T, string> formatValue)
    {
      maxPerColumn = Math.Max(maxPerColumn, 1);
      maxWidth = Math.Max(maxWidth, 12);

      var columns = new List<Tuple<int, string[]>>();
      int chars = 0;
      int offset = 0;
      while (offset < Count)
      {
        // full column
        int height = Math.Min(maxPerColumn, Count - offset);
        var candidate = FormatCompleteColumn(offset, height, formatValue);
        chars += candidate.Item1 + padding;
        if (chars > maxWidth)
        {
          break;
        }
        columns.Add(candidate);
        offset += height;
      }
      if (offset < Count)
      {
        // we're not done yet, but adding the last column has failed
        // --> make the last column partial
        var last = columns[columns.Count - 1];
        var c = last.Item2;
        c[c.Length - 4] = ellipsis;
        c[c.Length - 3] = ellipsis;
        c[c.Length - 2] = formatValue(At(Count - 2));
        c[c.Length - 1] = formatValue(At(Count - 1));
      }

      int rows = columns[0].Item2.Length;
      int cols = columns.Count;
      var array = new string[rows, cols];
      int colIndex = 0;
      foreach (var column in columns)
      {
        for (int k = 0; k < column.Item2.Length; k++)
        {
          array[k, colIndex] = column.Item2[k];
        }
        for (int k = column.Item2.Length; k < rows; k++)
        {
          array[k, colIndex] = "";
        }
        colIndex++;
      }
      return array;
    }

    static String FormatStringArrayToString(String[,] array, String columnSeparator, String rowSeparator)
    {
      var rowCount = array.GetLength(0);
      var columnCount = array.GetLength(1);

      var widths = new int[columnCount];

      for (var rowIndex = 0; rowIndex < rowCount; rowIndex++)
      {
        for (var j = 0; j < columnCount; j++)
        {
          widths[j] = Math.Max(widths[j], array[rowIndex, j].Length);
        }
      }

      var stringBuilder = new StringBuilder();
      
      for (var rowIndex = 0; rowIndex < rowCount; rowIndex++)
      {
        stringBuilder.Append(array[rowIndex, 0].PadLeft(widths[0]));

        for (var columnIndex = 1; columnIndex < columnCount; columnIndex++)
        {
          stringBuilder.Append(columnSeparator);
          stringBuilder.Append(array[rowIndex, columnIndex].PadLeft(widths[columnIndex]));
        }

        stringBuilder.Append(rowSeparator);
      }

      return stringBuilder.ToString();
    }

    private Tuple<Int32, String[]> FormatCompleteColumn(Int32 offset, Int32 height, Func<T, String> formatValue)
    {
      var values = Enumerable.Range(offset, height).Select(At).Select(formatValue).ToArray();

      return Tuple.Create(values.Max(x => x.Length), values);
    }

    /// <summary>
    /// Returns a string that represents the content of this vector, column by column.
    /// </summary>
    public string ToVectorString(int maxPerColumn, int maxWidth, string ellipsis, string columnSeparator, string rowSeparator, Func<T, string> formatValue)
    {
      return FormatStringArrayToString(ToVectorStringArray(maxPerColumn, maxWidth, columnSeparator.Length, ellipsis, formatValue), columnSeparator, rowSeparator);
    }

    /// <summary>
    /// Returns a string that represents the content of this vector, column by column.
    /// </summary>
    public string ToVectorString(int maxPerColumn, int maxWidth, string format = null, IFormatProvider provider = null)
    {
      if (format == null)
      {
        format = "G6";
      }

      return ToVectorString(maxPerColumn, maxWidth, "..", "  ", Environment.NewLine, x => x.ToString(format, provider));
    }

    /// <summary>
    /// Returns a string that represents the content of this vector, column by column.
    /// </summary>
    public String ToVectorString(String format = null, IFormatProvider provider = null, String columnSeparator = null, String rowSeparator = null)
    {
      if (format == null)
      {
        format = "G6";
      }

      return ToVectorString(12, 80, "..", columnSeparator ?? "  ", rowSeparator ?? Environment.NewLine, x => x.ToString(format, provider));
    }

    /// <summary>
    /// Returns a string that summarizes this vector.
    /// </summary>
    public string ToString(int maxPerColumn, int maxColumns, string format = null, IFormatProvider provider = null)
    {
      return string.Concat(ToTypeString(), Environment.NewLine, ToVectorString(maxPerColumn, maxColumns, format, provider));
    }

    /// <summary>
    /// Returns a string that summarizes this vector.
    /// The maximum number of cells can be configured in the <see cref="Control"/> class.
    /// </summary>
    public override sealed String ToString()
    {
      return ToString(null);
    }

    /// <summary>
    /// Returns a string that summarizes this vector.
    /// The maximum number of cells can be configured in the <see cref="Control"/> class.
    /// The format string is ignored.
    /// </summary>
    public String ToString(String format, IFormatProvider formatProvider = null)
    {
#if DEBUG
      return String.Join(", ", Enumerable.Range(0, Count).Select(At).Select(value => value.ToString(format, formatProvider)));
#else
      return string.Concat(ToTypeString(), Environment.NewLine, ToVectorString(format, formatProvider));
#endif
    }
  }
}
