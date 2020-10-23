using NUnit.Framework;
using System;
using System.Collections;

namespace List.Tests
{
    public class MyListTest
    {
        [Test]
        public void TestAdd()
        {
            var list = new MyList<int>();

            for (var i = 0; i < 100; i++)
            {
                list.Add(i);
            }

            Assert.AreEqual(100, list.Count);
        }

        [Test]
        public void TestSetGetElement()
        {
            var list = GetSortedList(12);

            list[10] = 'f';

            Assert.Throws<ArgumentOutOfRangeException>(() => list[-1] = 5);
            Assert.Throws<ArgumentOutOfRangeException>(() => list[5] = list[-1]);
            Assert.AreEqual(12, list.Count);
            Assert.AreEqual('f', list[10]);
            Assert.AreEqual(2, list[2]);
        }

        [Test]
        public void TestGetEnumerator()
        {
            var list = GetSortedList(17);

            var counter = 0;
            foreach (var i in list)
            {
                Assert.AreEqual(counter, i);
                counter++;
            }

            IEnumerator enumerator = list.GetEnumerator();

            Assert.AreEqual(0, enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());
        }

        [Test]
        public void TestClear()
        {
            var list = GetSortedList(30);

            Assert.AreEqual(30, list.Count);

            list.Clear();

            Assert.AreEqual(0, list.Count);
        }

        [Test]
        public void TestContains()
        {
            var list = GetSortedList(75);

            Assert.IsTrue(list.Contains(49));
            Assert.IsFalse(list.Contains(80));
        }


        [Test]
        public void TestCopyTo()
        {
            var list = GetSortedList(50);
            var testArray = new int[100];

            Assert.Throws<ArgumentOutOfRangeException>(() => list.CopyTo(testArray, -1));
            Assert.Throws<ArgumentException>(() => list.CopyTo(new int[0], 0));
            Assert.Throws<ArgumentNullException>(() => list.CopyTo(null, 0));
            Assert.Throws<ArgumentException>(() => list.CopyTo(testArray, 55));

            list.CopyTo(testArray, 0);

            Assert.AreEqual(15, testArray[15]);
            Assert.IsTrue(list.Contains(49));
        }

        [Test]
        public void TestIndexOf()
        {
            var list = GetSortedList(80);

            Assert.AreEqual(69, list.IndexOf(69));
            Assert.AreEqual(-1, list.IndexOf(500));
        }

        [Test]
        public void TestInsert()
        {
            var list = GetSortedList(90);

            list.Insert(30, 500);

            Assert.AreEqual(500, list[30]);
            Assert.Throws<ArgumentOutOfRangeException>(() => list.Insert(92, 5));
        }

        [Test]
        public void TestRemove()
        {
            var list = GetSortedList(100);

            Assert.IsTrue(list.Remove(25));
            Assert.AreEqual(26, list[25]);
            Assert.AreEqual(99, list.Count);
        }

        [Test]
        public void TestRemoveAt()
        {
            var list = GetSortedList(100);

            list.Remove(25);
            
            Assert.AreEqual(26, list[25]);
            Assert.AreEqual(99, list.Count);
            Assert.Throws<ArgumentOutOfRangeException>(() => list.RemoveAt(101));
        }

        private static MyList<int> GetSortedList(int num)
        {
            var list = new MyList<int>();

            for (var i = 0; i < num; i++)
            {
                list.Add(i);
            }

            return list;
        }
    }
}