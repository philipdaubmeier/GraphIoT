using System;
using System.Collections.Generic;

namespace DigitalstromClient.Network
{
    public class UriPriorityList
    {
        private List<Uri> _list;
        private int _currentIndex = 0;

        public UriPriorityList(List<Uri> list)
        {
            _list = list;
        }

        public Uri GetCurrent()
        {
            if (_list.Count <= 0)
                return null;
            return _list[Math.Max(Math.Min(_currentIndex, _list.Count - 1), 0)];
        }

        public void First()
        {
            _currentIndex = 0;
        }

        public void MoveNext()
        {
            _currentIndex++;
        }

        public bool IsLast()
        {
            return _currentIndex + 1 >= _list.Count;
        }

        public static implicit operator UriPriorityList(List<Uri> list)
        {
            return new UriPriorityList(list);
        }
    }
}
