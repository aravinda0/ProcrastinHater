//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace ProcrastinHater.POCOEntities
{
    public partial class Status
    {
        #region Primitive Properties
    
        public virtual int StatusID
        {
            get;
            set;
        }
    
        public virtual string Name
        {
            get;
            set;
        }

        #endregion
        #region Navigation Properties
    
        public virtual ICollection<Task> Tasks
        {
            get
            {
                if (_tasks == null)
                {
                    var newCollection = new FixupCollection<Task>();
                    newCollection.CollectionChanged += FixupTasks;
                    _tasks = newCollection;
                }
                return _tasks;
            }
            set
            {
                if (!ReferenceEquals(_tasks, value))
                {
                    var previousValue = _tasks as FixupCollection<Task>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupTasks;
                    }
                    _tasks = value;
                    var newValue = value as FixupCollection<Task>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupTasks;
                    }
                }
            }
        }
        private ICollection<Task> _tasks;

        #endregion
        #region Association Fixup
    
        private void FixupTasks(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Task item in e.NewItems)
                {
                    item.Status = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Task item in e.OldItems)
                {
                    if (ReferenceEquals(item.Status, this))
                    {
                        item.Status = null;
                    }
                }
            }
        }

        #endregion
    }
}
