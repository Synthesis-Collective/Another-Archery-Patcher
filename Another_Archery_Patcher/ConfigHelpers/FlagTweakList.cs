using Mutagen.Bethesda.Skyrim;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Another_Archery_Patcher.ConfigHelpers
{
    public class FlagTweakList : ICollection<FlagTweak>, IEnumerable<FlagTweak>, IEnumerable, IList<FlagTweak>, IReadOnlyCollection<FlagTweak>, IReadOnlyList<FlagTweak>, ICollection, IList
    {
        public FlagTweakList() => List = new();
        public FlagTweakList(List<FlagTweak> tweaks) => List = tweaks;
        public FlagTweakList(params FlagTweak[] tweaks) => List = tweaks.ToList();

        public List<FlagTweak> List;

        public Projectile ApplyTo(Projectile proj)
        {
            List.ForEach(tweak => proj = tweak.ApplyTo(proj));
            return proj;
        }

        public Projectile ApplyTo(Projectile proj, out uint changes)
        {
            changes = 0;
            foreach (var tweak in List)
            {
                tweak.ApplyTo(proj, out bool changed);
                if (changed) ++changes;
            }
            return proj;
        }

        public FlagTweak this[int index] { get => ((IList<FlagTweak>)List)[index]; set => ((IList<FlagTweak>)List)[index] = value; }

        public int Count => ((ICollection<FlagTweak>)List).Count;

        public bool IsReadOnly => ((ICollection<FlagTweak>)List).IsReadOnly;

        public bool IsFixedSize => ((IList)List).IsFixedSize;

        public bool IsSynchronized => ((ICollection)List).IsSynchronized;

        public object SyncRoot => ((ICollection)List).SyncRoot;

        object? IList.this[int index] { get => ((IList)List)[index]; set => ((IList)List)[index] = value; }

        public void Add(FlagTweak item) => ((ICollection<FlagTweak>)List).Add(item);
        public void Clear() => ((ICollection<FlagTweak>)List).Clear();
        public bool Contains(FlagTweak item) => ((ICollection<FlagTweak>)List).Contains(item);
        public void CopyTo(FlagTweak[] array, int arrayIndex) => ((ICollection<FlagTweak>)List).CopyTo(array, arrayIndex);
        public IEnumerator<FlagTweak> GetEnumerator() => ((IEnumerable<FlagTweak>)List).GetEnumerator();
        public int IndexOf(FlagTweak item) => ((IList<FlagTweak>)List).IndexOf(item);
        public void Insert(int index, FlagTweak item) => ((IList<FlagTweak>)List).Insert(index, item);
        public bool Remove(FlagTweak item) => ((ICollection<FlagTweak>)List).Remove(item);
        public void RemoveAt(int index) => ((IList<FlagTweak>)List).RemoveAt(index);
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)List).GetEnumerator();
        public int Add(object? value) => ((IList)List).Add(value);
        public bool Contains(object? value) => ((IList)List).Contains(value);
        public int IndexOf(object? value) => ((IList)List).IndexOf(value);
        public void Insert(int index, object? value) => ((IList)List).Insert(index, value);
        public void Remove(object? value) => ((IList)List).Remove(value);
        public void CopyTo(Array array, int index) => ((ICollection)List).CopyTo(array, index);
    }
}
