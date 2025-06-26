using Unity.Collections;
using Unity.Mathematics;

namespace ECS
{
    public struct NativeMinHeap<T> where T : unmanaged
    {
        private NativeList<T> _items;
        private NativeList<float> _priorities;

        public NativeMinHeap(int capacity, Allocator allocator)
        {
            _items = new NativeList<T>(capacity, allocator);
            _priorities = new NativeList<float>(capacity, allocator);
        }

        public int Count => _items.Length;
        public bool IsEmpty => _items.Length == 0;

        public void Enqueue(T item, float priority)
        {
            _items.Add(item);
            _priorities.Add(priority);
            BubbleUp(_items.Length - 1);
        }

        public T Dequeue()
        {
            var item = _items[0];
            var last = _items.Length - 1;

            _items[0] = _items[last];
            _priorities[0] = _priorities[last];

            _items.RemoveAt(last);
            _priorities.RemoveAt(last);

            BubbleDown(0);
            return item;
        }

        private void BubbleUp(int index)
        {
            while (index > 0)
            {
                int parent = (index - 1) / 2;
                if (_priorities[index] >= _priorities[parent]) break;

                Swap(index, parent);
                index = parent;
            }
        }

        private void BubbleDown(int index)
        {
            int count = _items.Length;
            while (true)
            {
                int smallest = index;
                int left = 2 * index + 1;
                int right = 2 * index + 2;

                if (left < count && _priorities[left] < _priorities[smallest])
                    smallest = left;

                if (right < count && _priorities[right] < _priorities[smallest])
                    smallest = right;

                if (smallest == index) break;

                Swap(index, smallest);
                index = smallest;
            }
        }

        private void Swap(int a, int b)
        {
            (_items[a], _items[b]) = (_items[b], _items[a]);
            (_priorities[a], _priorities[b]) = (_priorities[b], _priorities[a]);
        }

        public void Dispose()
        {
            if (_items.IsCreated) _items.Dispose();
            if (_priorities.IsCreated) _priorities.Dispose();
        }
    }
}