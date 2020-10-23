using System;
using System.Collections;
using System.Collections.Generic;

namespace List
{
    public class MyList<T> : IList<T>
    {
        private T[] _array;
        private int _size;

        public MyList() : this(16) { }

        public MyList(int size)
        {
            _size = size;
            _array = new T[_size];
        }

        
        /// <summary>
        /// Returns the number of items contained in the list.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Reports the state of the object is read only.
        /// </summary>
        public bool IsReadOnly { get; } = false;

        /// <summary>
        /// Gets or sets the item at the specified index.
        /// </summary>
        /// <param name="index">The ordinal number of the item.</param>
        /// <exception cref="ArgumentOutOfRangeException">Throw when index is out of bounds of array.</exception>
        /// <exception cref="NotSupportedException">Throw when state IsReadOnly equals true.</exception>
        public T this[int index]
        {
            get => GetItem(index);
            set => Insert(index, value);
        }

        /// <summary>
        /// Adds an item to the end of the list.
        /// </summary>
        /// <param name="item">The item to add to the list.</param>
        /// <exception cref="NotSupportedException">Throw when state IsReadOnly equals true.</exception>
        public void Add(T item)
        {
            CheckingReadOnlyProperty();

            if (Count == _array.Length)
            {
                Resize();
            }

            _array[Count] = item;
            Count++;
        }

        /// <summary>
        /// Removes all items from the list.
        /// </summary>
        /// <exception cref="NotSupportedException">Throw when state IsReadOnly equals true.</exception>
        public void Clear()
        {
            CheckingReadOnlyProperty();

            _array = new T[_size];
            Count = 0;
        }

        /// <summary>
        /// Determines whether the list contains the specified item.
        /// </summary>
        /// <param name="item">Item to search in list.</param>
        /// <returns>True if the item is found, otherwise false.</returns>
        public bool Contains(T item)
        {
            return IndexOf(item) != -1;
        }

        /// <summary>
        /// Copies the items of the list to an Array, starting at the specified index of the Array.
        /// </summary>
        /// <param name="array">The one-dimensional Array into which the items from the list are copied. </param>
        /// <param name="arrayIndex">The zero-based index in array that indicates the start of copying.</param>
        /// <exception cref="ArgumentNullException">Throw when Array equals null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Throw when arrayIndex is less than 0.</exception>
        /// <exception cref="ArgumentException">The number of items in the list is greater than the available space
        /// from the position specified by the value of arrayIndex to the end of the destination array.</exception>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException();
            }

            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            var needLength = array.Length - arrayIndex;
            if (Count > needLength)
            {
                throw new ArgumentException();
            }

            var index = arrayIndex;
            for (var i = 0; i < Count; i++, index++)
            {
                array.SetValue(_array[i], index);
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates over the list.
        /// </summary>
        /// <returns>The IEnumerator used to traverse the list.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            var intermediateArray = new T[Count];
            CopyTo(intermediateArray, 0);

            foreach (var i in intermediateArray)
            {
                yield return i;
            }
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Determines the index of the specified list item.
        /// </summary>
        /// <param name="item">Object to search in list.</param>
        /// <returns>The index of item, if found in the list, otherwise, -1.</returns>
        public int IndexOf(T item)
        {
            var index = 0;

            foreach (var element in _array)
            {
                if (element.Equals(item))
                {
                    return index;
                }

                index++;
            }

            return -1;
        }

        /// <summary>
        /// Inserts an item into the list at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which to insert the item.</param>
        /// <param name="item">The object to insert into the list.</param>
        /// <exception cref="ArgumentOutOfRangeException">Throw when index is out of bounds of array.</exception>
        /// <exception cref="NotSupportedException">Throw when state IsReadOnly equals true.</exception>
        public void Insert(int index, T item)
        {
            CheckingListBounds(index);
            CheckingReadOnlyProperty();

            _array[index] = item;
        }

        /// <summary>
        /// Removes the first occurrence of the specified item from the list.
        /// </summary>
        /// <param name="item">The item to be removed from the list.</param>
        /// <returns>True if item was successfully removed from list, otherwise - false.</returns>
        public bool Remove(T item)
        {
            CheckingReadOnlyProperty();

            var elementIndex = IndexOf(item);

            if (elementIndex == -1)
            {
                return false;
            }

            ShiftItems(elementIndex);

            Count--;
            return true;
        }

        /// <summary>
        /// Removes the list item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException">Throw when index is out of bounds of array.</exception>
        /// <exception cref="NotSupportedException">Throw when state IsReadOnly equals true.</exception>
        public void RemoveAt(int index)
        {
            CheckingReadOnlyProperty();
            CheckingListBounds(index);

            ShiftItems(index);

            Count--;
        }

        /// <summary>
        /// Gets the item at the specified index.
        /// </summary>
        /// <param name="index">The ordinal number of the item.</param>
        /// <returns>The item at the given index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Throw when index is out of bounds of array.</exception>
        private T GetItem(int index)
        {
            CheckingListBounds(index);

            return _array[index];
        }

        /// <summary>
        /// Doubles the size of the original _array.
        /// </summary>
        private void Resize()
        {
            _size *= 2;

            var intermediateArray = new T[_size];

            CopyTo(intermediateArray, 0);
            _array = intermediateArray;
        }

        /// <summary>
        /// Shifts list items by one position.
        /// </summary>
        /// <param name="index">The index at which the shift starts.</param>
        private void ShiftItems(int index)
        {
            for (var i = index; i < _array.Length - 1; i++)
            {
                _array[i] = _array[i + 1];
            }
        }

        /// <summary>
        /// Checks if the index is within the bounds of the list.
        /// </summary>
        /// <param name="index">The index being accessed.</param>
        /// <exception cref="ArgumentOutOfRangeException">Throw when index is out of bounds of array.</exception>
        private void CheckingListBounds(int index)
        {
            if (index > Count || index < 0)
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Checks the state of the IsReadOnly object.
        /// </summary>
        /// <exception cref="NotSupportedException">Throw when state IsReadOnly equals true.</exception>
        private void CheckingReadOnlyProperty()
        {
            if (IsReadOnly)
            {
                throw new NotSupportedException();
            }
        }
    }
}
