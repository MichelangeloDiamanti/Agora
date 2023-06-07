using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanTerritoriality.Utilities
{
    /** A fixed size circular buffer data structure
     */
    public class RingBuffer<T>
    {
        /** Get the size of the buffer. */
        public int Capacity { get { return capacity; } }

        /** The size of the buffer. */
        protected int capacity;

        /** The data in the buffer */
        protected T[] data;

        /** The next index where a new item will be inserted */
        protected int nextIndex;

        /** Number of positions in the buffer that have a value in them. */
        public int FilledPositions { get { return filledPositions; } }
        protected int filledPositions = 0;

        /** Constructor
         * @param size The size of the buffer. It
         * is the maximum capacity of it.
         * */
        public RingBuffer(int size)
        {
            this.capacity = size;
            data = new T[size];
            nextIndex = 0;
        }

        /** Insert a new item into the buffer.
         * @param item The item to insert.
         */
        public void Insert(T item)
        {
            data[nextIndex] = item;
            nextIndex++;
            if (nextIndex >= capacity) nextIndex = 0;
            filledPositions++;
            if (filledPositions > capacity) filledPositions = capacity;
        }

        /** Convert the buffer into an array.
         * The first item in the array (at index 0)
         * is the item that was last inserted into
         * the buffer.
         * @return Returns an array of the items
         * in the buffer.
         */
        public T[] ToArray()
        {
            T[] arr = new T[filledPositions];
            int index = nextIndex;
            for (int i = 0; i < filledPositions; i++)
            {
                index--;
                if (index < 0) index = capacity - 1;
                arr[i] = data[index];
            }
            return arr;
        }
    }
}

