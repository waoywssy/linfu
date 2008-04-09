using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.DynamicProxy;

namespace LinFu.Lazy
{
    public class LazyList<TItem> : LazyInterceptor<IList<TItem>>
        where TItem : class
    {
        public LazyList(Func<IList<TItem>> getList)
            : base(getList)
        {

        }
        public TItem CreateListItem(Func<IList<TItem>, TItem> getListItem)
        {
            ProxyFactory factory = new ProxyFactory();

            IList<TItem> lazyList = factory.CreateProxy<IList<TItem>>(this);

            Func<TItem> initializeList = () =>
            {
                return getListItem(lazyList);
            };

            TItem lazyListItem = LazyBuilder.CreateLazyItem<TItem>(initializeList);
            return lazyListItem;
        }
    }
}
