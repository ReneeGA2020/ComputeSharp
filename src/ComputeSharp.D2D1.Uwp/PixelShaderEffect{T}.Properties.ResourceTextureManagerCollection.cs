using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using ComputeSharp.D2D1.Helpers;
using ComputeSharp.D2D1.Interop;
using ComputeSharp.D2D1.Shaders.Loaders;
using ComputeSharp.D2D1.Uwp.Buffers;

#pragma warning disable CS0618, CS8767

namespace ComputeSharp.D2D1.Uwp;

/// <inheritdoc/>
partial class PixelShaderEffect<T>
{
    /// <summary>
    /// Represents the collection of <see cref="D2D1ResourceTextureManager"/> objects in a <see cref="PixelShaderEffect{T}"/> instance.
    /// </summary>
    public sealed class ResourceTextureManagerCollection : IList<D2D1ResourceTextureManager?>, IReadOnlyList<D2D1ResourceTextureManager?>, IList, IFixedCountList<D2D1ResourceTextureManager?>
    {
        /// <summary>
        /// The bitmask of valid indices for resource texture managers in the current shader type.
        /// </summary>
        private static readonly int IndexBitmask = GetIndexBitmask();

        /// <summary>
        /// The fixed buffer of <see cref="D2D1ResourceTextureManager"/> instances.
        /// </summary>
        private ResourceTextureManagerBuffer fixedBuffer;

        /// <summary>
        /// Creates a new <see cref="ResourceTextureManagerCollection"/> instance with the specified parameters.
        /// </summary>
        /// <param name="owner">The owning <see cref="PixelShaderEffect{T}"/> instance.</param>
        internal ResourceTextureManagerCollection(PixelShaderEffect<T> owner)
        {
            Owner = owner;
        }

        /// <inheritdoc/>
        public int Count => Indices.Length;

        /// <summary>
        /// Gets or sets the <see cref="D2D1ResourceTextureManager"/> object at a specified index.
        /// </summary>
        /// <param name="index">The index of the <see cref="D2D1ResourceTextureManager"/> source to get or set.</param>
        /// <returns>The <see cref="D2D1ResourceTextureManager"/> object at the specified index.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the value is set to <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="index"/> is not a valid index for the current effect.</exception>
        [DisallowNull]
        public D2D1ResourceTextureManager? this[int index]
        {
            get
            {
                ValidateIndex(index);

                return Owner.GetResourceTextureManager(index);
            }
            set
            {
                if (value is null)
                {
                    ThrowHelper.ThrowArgumentNullException(nameof(value), "Input resource texture managers cannot be null.");
                }

                ValidateIndex(index);

                Owner.SetResourceTextureManager(value, index);
            }
        }

        /// <inheritdoc/>
        object? IList.this[int index]
        {
            get => this[index];
            set => this[index] = (D2D1ResourceTextureManager?)value!;
        }

        /// <summary>
        /// Gets the collection of valid resource texture indices for the current effect.
        /// </summary>
        internal static ImmutableArray<int> Indices { get; } = GetIndices();

        /// <summary>
        /// The owning <see cref="PixelShaderEffect{T}"/> instance.
        /// </summary>
        internal PixelShaderEffect<T> Owner { get; }

        /// <summary>
        /// Gets a reference to the <see cref="ResourceTextureManagerBuffer"/> value containing the available <see cref="D2D1ResourceTextureManager"/>-s.
        /// </summary>
        internal ref ResourceTextureManagerBuffer Storage => ref this.fixedBuffer;

        /// <inheritdoc/>
        ImmutableArray<int> IFixedCountList<D2D1ResourceTextureManager?>.Indices => Indices;

        /// <inheritdoc/>
        bool ICollection<D2D1ResourceTextureManager?>.IsReadOnly => false;

        /// <inheritdoc/>
        bool IList.IsReadOnly => false;

        /// <inheritdoc/>
        bool IList.IsFixedSize => true;

        /// <inheritdoc/>
        bool ICollection.IsSynchronized => true;

        /// <inheritdoc/>
        object ICollection.SyncRoot => throw new NotSupportedException("ICollection.SyncRoot is not supported for ResourceTextureManagerCollection.");

        /// <inheritdoc/>
        void ICollection<D2D1ResourceTextureManager?>.Add(D2D1ResourceTextureManager? item)
        {
            throw new NotSupportedException("ICollection<T>.Add is not supported for ResourceTextureManagerCollection.");
        }

        /// <inheritdoc/>
        int IList.Add(object value)
        {
            throw new NotSupportedException("IList.Add is not supported for ResourceTextureManagerCollection.");
        }

        /// <inheritdoc/>
        void ICollection<D2D1ResourceTextureManager?>.Clear()
        {
            throw new NotSupportedException("ICollection<T>.Clear is not supported for ResourceTextureManagerCollection.");
        }

        /// <inheritdoc/>
        void IList.Clear()
        {
            throw new NotSupportedException("IList.Clear is not supported for ResourceTextureManagerCollection.");
        }

        /// <inheritdoc/>
        bool ICollection<D2D1ResourceTextureManager?>.Contains(D2D1ResourceTextureManager? item)
        {
            return FixedCountList<D2D1ResourceTextureManager?>.IndexOf(this, item) != -1;
        }

        /// <inheritdoc/>
        bool IList.Contains(object value)
        {
            return FixedCountList<D2D1ResourceTextureManager?>.IndexOf(this, (D2D1ResourceTextureManager?)value) != -1;
        }

        /// <inheritdoc/>
        void ICollection<D2D1ResourceTextureManager?>.CopyTo(D2D1ResourceTextureManager?[] array, int arrayIndex)
        {
            FixedCountList<D2D1ResourceTextureManager?>.CopyTo(this, array, arrayIndex);
        }

        /// <inheritdoc/>
        void ICollection.CopyTo(Array array, int index)
        {
            FixedCountList<D2D1ResourceTextureManager?>.CopyTo(this, array, index);
        }

        /// <inheritdoc/>
        IEnumerator<D2D1ResourceTextureManager?> IEnumerable<D2D1ResourceTextureManager?>.GetEnumerator()
        {
            return FixedCountList<D2D1ResourceTextureManager?>.GetEnumerator(this);
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return FixedCountList<D2D1ResourceTextureManager?>.GetEnumerator(this);
        }

        /// <inheritdoc/>
        int IList<D2D1ResourceTextureManager?>.IndexOf(D2D1ResourceTextureManager? item)
        {
            return FixedCountList<D2D1ResourceTextureManager?>.IndexOf(this, item);
        }

        /// <inheritdoc/>
        int IList.IndexOf(object value)
        {
            return FixedCountList<D2D1ResourceTextureManager?>.IndexOf(this, (D2D1ResourceTextureManager?)value);
        }

        /// <inheritdoc/>
        void IList<D2D1ResourceTextureManager?>.Insert(int index, D2D1ResourceTextureManager? item)
        {
            throw new NotSupportedException("IList<T>.Insert is not supported for ResourceTextureManagerCollection.");
        }

        /// <inheritdoc/>
        void IList.Insert(int index, object value)
        {
            throw new NotSupportedException("IList.Insert is not supported for ResourceTextureManagerCollection.");
        }

        /// <inheritdoc/>
        bool ICollection<D2D1ResourceTextureManager?>.Remove(D2D1ResourceTextureManager? item)
        {
            throw new NotSupportedException("ICollection<T>.Remove is not supported for ResourceTextureManagerCollection.");
        }

        /// <inheritdoc/>
        void IList.Remove(object value)
        {
            throw new NotSupportedException("IList.Remove is not supported for ResourceTextureManagerCollection.");
        }

        /// <inheritdoc/>
        void IList<D2D1ResourceTextureManager?>.RemoveAt(int index)
        {
            throw new NotSupportedException("IList<T>.RemoveAt is not supported for ResourceTextureManagerCollection.");
        }

        /// <inheritdoc/>
        void IList.RemoveAt(int index)
        {
            throw new NotSupportedException("IList.RemoveAt is not supported for ResourceTextureManagerCollection.");
        }

        /// <summary>
        /// Validates the input index for <see cref="this[int]"/>.
        /// </summary>
        /// <param name="index">The index to validate.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="index"/> is out of range.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ValidateIndex(int index)
        {
            if ((uint)index >= 16 || (IndexBitmask & (1 << index)) == 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(nameof(index), "The input index is not a valid resource texture manager index for the current effect.");
            }
        }

        /// <summary>
        /// Gets the bitmask of indices for resource textures in the target shader.
        /// </summary>
        /// <returns>The bitmask of indices for resource textures in the target shader.</returns>
        private static int GetIndexBitmask()
        {
            D2D1IndexBitmaskResourceTextureDescriptionsLoader bitmaskLoader = default;

            Unsafe.SkipInit(out T shader);

            shader.LoadResourceTextureDescriptions(ref bitmaskLoader);

            return bitmaskLoader.GetResultingIndexBitmask();
        }

        /// <summary>
        /// Gets the collection of valid indices for the current effect.
        /// </summary>
        /// <returns>The collection of valid indices for the current effect.</returns>
        private static ImmutableArray<int> GetIndices()
        {
            D2D1ImmutableArrayResourceTextureDescriptionsLoader indicesLoader = default;

            Unsafe.SkipInit(out T shader);

            shader.LoadResourceTextureDescriptions(ref indicesLoader);

            return indicesLoader.GetResultingIndices();
        }
    }
}