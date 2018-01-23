using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace RapidServer.Http.Type2
{
    // '' <summary>
    // '' A pool of reusable SocketAsyncEventArgs objects.
    // '' </summary>
    // '' <remarks></remarks>
    internal class SocketAsyncEventArgsPool
    {
        private Stack<SocketAsyncEventArgs> m_pool;

        public SocketAsyncEventArgsPool(int capacity)
        {
            m_pool = new Stack<SocketAsyncEventArgs>(capacity);
        }

        //  Add a SocketAsyncEventArg instance to the pool
        //
        // The "item" parameter is the SocketAsyncEventArgs instance
        //  to add to the pool
        public void Push(SocketAsyncEventArgs item)
        {
            if (item == null)
                throw new ArgumentNullException("Items added to a SocketAsyncEventArgsPool cannot be null");

            lock (m_pool)
                m_pool.Push(item);
        }

        //  The number of SocketAsyncEventArgs instances in the pool
        public int Count
        {
            get => m_pool.Count;
        }
    }
}