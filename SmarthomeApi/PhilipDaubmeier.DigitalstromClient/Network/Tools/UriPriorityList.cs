using PhilipDaubmeier.DigitalstromClient.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.DigitalstromClient.Network
{
    public class UriPriorityList : IDeepCloneable<UriPriorityList>
    {
        private List<Uri> _list;
        private List<bool> _authIncluded;
        private int _currentIndex = 0;

        public UriPriorityList(List<Uri> list)
        {
            _list = list;
            _authIncluded = Enumerable.Range(0, _list.Count).Select(i => false).ToList();
        }

        public UriPriorityList(IEnumerable<Uri> list, IEnumerable<bool> authIncluded)
        {
            _list = list.ToList();

            var authInclList = authIncluded.ToList();
            _authIncluded = Enumerable.Range(0, _list.Count).Select(i => i < authInclList.Count ? authInclList[i] : false).ToList();
        }

        public Uri GetCurrent()
        {
            if (_list.Count <= 0)
                return null;
            return _list[Math.Max(Math.Min(_currentIndex, _list.Count - 1), 0)];
        }

        public bool CurrentHasAuthIncluded()
        {
            if (_authIncluded.Count <= 0)
                return false;
            return _authIncluded[Math.Max(Math.Min(_currentIndex, _authIncluded.Count - 1), 0)];
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

        public UriPriorityList DeepClone()
        {
            return new UriPriorityList(_list, _authIncluded);
        }

        public static implicit operator UriPriorityList(List<Uri> list)
        {
            return new UriPriorityList(list);
        }

        public static implicit operator UriPriorityList(List<Tuple<Uri, bool>> list)
        {
            return new UriPriorityList(list.Select(x => x.Item1), list.Select(x => x.Item2));
        }
    }
}