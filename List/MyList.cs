using System;
using System.Collections;
using System.Collections.Generic;

namespace List
{
    public class MyList<T> : IList<T>
    {
        private T[] _array;
        internal int Version;

        public MyList() : this(16) { }

        public MyList(int size)
        {
            _array = new T[size];
        }
        
        public int Count { get; private set; }
        
        public bool IsReadOnly { get; } = false;
        
        public T this[int index]
        {
            get => GetItem(index);
            set => SetItem(index, value);
        }
        
        public void Add(T item)
        {
            CheckingReadOnlyProperty();
            
            Version++;
            
            if (Count == _array.Length)
            {
                Resize();
            }

            _array[Count] = item;
            Count++;
        }
        
        public void Clear()
        {
            CheckingReadOnlyProperty();
            
            Version = 0;
            
            Array.Clear(_array, 0, Count);
            Count = 0;
        }
        
        public bool Contains(T item)
        {
            return IndexOf(item) != -1;
        }
        
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
                array[index] = _array[i];
            }
        }
        
        public IEnumerator<T> GetEnumerator()
        {
            return new MyEnum<T>(this);
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        public int IndexOf(T item)
        {
            for (var i = 0; i < Count; i++)
            {
                if (Equals(_array[i], item))
                {
                    return i;
                }
            }

            return -1;
        }
        
        public void Insert(int index, T item)
        {
            CheckingListBounds(index);
            CheckingReadOnlyProperty();

            Add(_array[Count - 1]);
            
            for (var i = Count - 1; i > index; i--)
            {
                _array[i] = _array[i - 1];
            }

            _array[index] = item;
        }
        
        public bool Remove(T item)
        {
            CheckingReadOnlyProperty();

            var elementIndex = IndexOf(item);

            if (elementIndex == -1)
            {
                return false;
            }

            RemoveAt(elementIndex);

            return true;
        }
        
        public void RemoveAt(int index)
        {
            CheckingReadOnlyProperty();
            CheckingListBounds(index);

            Version++;
            
            ShiftItems(index);

            Count--;
        }
        
        private T GetItem(int index)
        {
            CheckingListBounds(index);

            return _array[index];
        }

        private void SetItem(int index, T item)
        {
            CheckingListBounds(index);
            CheckingReadOnlyProperty();

            _array[index] = item;
        }

        private void Resize()
        {
            var intermediateArray = new T[_array.Length * 2];

            CopyTo(intermediateArray, 0);
            _array = intermediateArray;
        }
        
        private void ShiftItems(int index)
        {
            for (var i = index; i < Count - 1; i++)
            {
                _array[i] = _array[i + 1];
            }
        }
        
        private void CheckingListBounds(int index)
        {
            if (index >= Count || index < 0)
            {
                throw new ArgumentOutOfRangeException();
            }
        }
        
        private void CheckingReadOnlyProperty()
        {
            if (IsReadOnly)
            {
                throw new NotSupportedException();
            }
        }
    }

    public class MyEnum<T> : IEnumerator<T>
    {
        private readonly MyList<T> _listForEnum;
        private readonly int _currentVersion;
        private int _index = -1;

        public MyEnum(MyList<T> listForEnum)
        {
            _listForEnum = listForEnum;
            _currentVersion = listForEnum.Version;
        }

        public bool MoveNext()
        {
            CheckVersion();

            _index++;
            return (_index < _listForEnum.Count);
        }

        public void Reset()
        {
            _index = -1;
        }

        public object Current { get; }

        T IEnumerator<T>.Current => _listForEnum[_index];

        private void CheckVersion()
        {
            if (_currentVersion != _listForEnum.Version)
            {
                throw new InvalidOperationException();
            }
        }

        public void Dispose()
        {
        }
    }
}