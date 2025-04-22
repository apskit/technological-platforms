using System;
using System.ComponentModel;

namespace lab10_2
{
    public class SortableBindingList<T> : BindingList<T>
    {
        private bool isSorted;
        private ListSortDirection sortDirection;
        private PropertyDescriptor sortProperty;

        protected override bool SupportsSortingCore => true;

        protected override bool SupportsSearchingCore => true;

        protected override bool IsSortedCore => isSorted;

        protected override ListSortDirection SortDirectionCore => sortDirection;

        protected override PropertyDescriptor SortPropertyCore => sortProperty;

        public SortableBindingList() : base() { }

        public SortableBindingList(List<T> list) : base(list) { }

        protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
        {
            List<T> itemsList = (List<T>)Items;

            if (prop.PropertyType.GetInterface("IComparable") != null)
            {
                PropertyComparer<T> comparer = new PropertyComparer<T>(prop, direction);
                itemsList.Sort(comparer);
                isSorted = true;
                sortDirection = direction;
                sortProperty = prop;
                OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
            }
            else
            {
                throw new NotSupportedException("Sorting is not supported for this type of property");
            }
        }

        protected override void RemoveSortCore()
        {
            isSorted = false;
            sortDirection = ListSortDirection.Ascending;
            sortProperty = null;
        }

        protected override int FindCore(PropertyDescriptor prop, object key)
        {
            if (prop != null && key != null)
            {
                for (int i = 0; i < Count; ++i)
                {
                    T item = this[i];
                    if (prop.GetValue(item).Equals(key))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }
    }

    public class PropertyComparer<T> : IComparer<T>
    {
        private PropertyDescriptor property;
        private ListSortDirection direction;

        public PropertyComparer(PropertyDescriptor property, ListSortDirection direction)
        {
            this.property = property;
            this.direction = direction;
        }

        public int Compare(T x, T y)
        {
            object xValue = property.GetValue(x);
            object yValue = property.GetValue(y);

            if (direction == ListSortDirection.Ascending)
            {
                return Comparer<object>.Default.Compare(xValue, yValue);
            }
            else
            {
                return Comparer<object>.Default.Compare(yValue, xValue);
            }
        }
    }
}