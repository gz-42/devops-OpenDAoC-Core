using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;

namespace DOL.GS
{
    public static class EntityManager
    {
        public const int UNSET_ID = -1;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public enum EntityType
        {
            Player,
            Npc,
            Effect,
            AttackComponent,
            CastingComponent,
            EffectListComponent,
            CraftComponent
        }

        private static Dictionary<EntityType, dynamic> Entities = new()
        {
            { EntityType.Player, new EntityArrayWrapper<GamePlayer>(ServerProperties.Properties.MAX_PLAYERS) },
            { EntityType.Npc, new EntityArrayWrapper<GameNPC>(ServerProperties.Properties.MAX_ENTITIES) },
            { EntityType.Effect, new EntityArrayWrapper<ECSGameEffect>(250) },
            { EntityType.AttackComponent, new EntityArrayWrapper<AttackComponent>(1250) },
            { EntityType.CastingComponent, new EntityArrayWrapper<CastingComponent>(1250) },
            { EntityType.EffectListComponent, new EntityArrayWrapper<EffectListComponent>(3000) },
            { EntityType.CraftComponent, new EntityArrayWrapper<CraftComponent>(250) }
        };

        public static int Add<T>(EntityType type, T entity)
        {
            return Entities[type].Add(entity);
        }

        public static int Remove(EntityType type, int id)
        {
            Entities[type].Remove(id);
            return UNSET_ID;
        }

        public static List<T> GetAll<T>(EntityType type)
        {
            return Entities[type].Elements;
        }

        public static int GetLastNonNullIndex(EntityType type)
        {
            return Entities[type].LastNonNullIndex;
        }

        private class EntityArrayWrapper<T> where T : class
        {
            private static Comparer<int> _descendingOrder = Comparer<int>.Create((x, y) => x < y ? 1 : x > y ? -1 : 0);
            public List<T> Elements { get; private set; }
            public int LastNonNullIndex { get; private set; } = -1;
            private SortedSet<int> _deletedIndexes = new(_descendingOrder);
            private object _lock = new();

            public EntityArrayWrapper(int capacity)
            {
                Elements = new List<T>(capacity);
            }

            public int Add(T element)
            {
                lock (_lock)
                {
                    if (_deletedIndexes.Any())
                    {
                        int index = _deletedIndexes.Max;
                        _deletedIndexes.Remove(index);
                        Elements[index] = element;

                        if (index > LastNonNullIndex)
                            LastNonNullIndex = index;

                        return index;
                    }
                    else
                    {
                        LastNonNullIndex++;

                        // Increase the capacity of the list in the event that it's too small. This is a costly operation.
                        // 'Add' already does it, but we want to know when it happens and control by how much it grows (instead of doubling it).
                        if (LastNonNullIndex >= Elements.Capacity)
                        {
                            int newCapacity = Elements.Capacity + 100;
                            log.Warn($"{typeof(T)} {nameof(Elements)} is too short. Resizing it to {newCapacity}.");
                            ListExtras.Resize(Elements, newCapacity);
                        }

                        Elements.Add(element);
                        return LastNonNullIndex;
                    }
                }
            }

            public void Remove(int id)
            {
                if (id == -1)
                    return;

                lock (_lock)
                {
                    Elements[id] = null;
                    _deletedIndexes.Add(id);

                    if (id == LastNonNullIndex)
                    {
                        if (_deletedIndexes.Any())
                        {
                            int lastIndex = _deletedIndexes.Min;

                            // Find the first non-contiguous number. For example if the collection contains 7 6 3 1, we should return 5.
                            foreach (int index in _deletedIndexes)
                            {
                                if (lastIndex - index > 0)
                                    break;

                                lastIndex--;
                            }

                            LastNonNullIndex = lastIndex;
                        }
                        else
                            LastNonNullIndex--;
                    }
                }
            }
        }
    }

    // Extension methods for 'List<T>' that could be moved elsewhere.
    public static class ListExtras
    {
        public static void Resize<T>(this List<T> list, int size, bool fill = false, T element = default)
        {
            int count = list.Count;

            if (size < count)
            {
                list.RemoveRange(size, count - size);
                list.TrimExcess();
            }
            else if (size > count)
            {
                if (size > list.Capacity)
                    list.Capacity = size; // Creates a new internal array.

                if (fill)
                    list.AddRange(Enumerable.Repeat(element, size - count));
            }
        }
    }
}
