namespace BYteWare.Utils
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    /// <summary>
    /// BindingList with support for Remove
    /// </summary>
    /// <typeparam name="T">Type for the elements</typeparam>
    public class BindingListRemove<T> : BindingList<T>
    {
        /// <summary>
        /// Event which is called on removing an item from the list
        /// </summary>
        public event ListChangedEventHandler Removing;

        /// <summary>
        /// Event Handler for the Removing event
        /// </summary>
        /// <param name="e">The ListChangedEventArgs</param>
        protected void OnRemoving(ListChangedEventArgs e)
        {
            Removing?.Invoke(this, e);
        }

        /// <inheritdoc/>
        protected override void RemoveItem(int index)
        {
            if (RaiseListChangedEvents)
            {
                OnRemoving(new ListChangedEventArgs(ListChangedType.ItemDeleted, index));
            }
            base.RemoveItem(index);
        }
    }
}
