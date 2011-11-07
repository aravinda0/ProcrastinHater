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
    public abstract partial class SchedulingInformation
    {
        #region Primitive Properties
    
        public virtual int ScheduleID
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> NextRun
        {
            get;
            set;
        }
    
        public virtual int TimeoutActionID
        {
            get { return _timeoutActionID; }
            set
            {
                if (_timeoutActionID != value)
                {
                    if (TimeoutAction != null && TimeoutAction.TimeoutActionID != value)
                    {
                        TimeoutAction = null;
                    }
                    _timeoutActionID = value;
                }
            }
        }
        private int _timeoutActionID;
    
        public virtual bool IsEnabled
        {
            get;
            set;
        }
    
        public virtual string Name
        {
            get;
            set;
        }
    
        public virtual Nullable<long> DurationTicks
        {
            get;
            set;
        }

        #endregion
        #region Navigation Properties
    
        public virtual TimeoutAction TimeoutAction
        {
            get { return _timeoutAction; }
            set
            {
                if (!ReferenceEquals(_timeoutAction, value))
                {
                    var previousValue = _timeoutAction;
                    _timeoutAction = value;
                    FixupTimeoutAction(previousValue);
                }
            }
        }
        private TimeoutAction _timeoutAction;
    
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
    
        private void FixupTimeoutAction(TimeoutAction previousValue)
        {
            if (previousValue != null && previousValue.SchedulingInformationSet.Contains(this))
            {
                previousValue.SchedulingInformationSet.Remove(this);
            }
    
            if (TimeoutAction != null)
            {
                if (!TimeoutAction.SchedulingInformationSet.Contains(this))
                {
                    TimeoutAction.SchedulingInformationSet.Add(this);
                }
                if (TimeoutActionID != TimeoutAction.TimeoutActionID)
                {
                    TimeoutActionID = TimeoutAction.TimeoutActionID;
                }
            }
        }
    
        private void FixupTasks(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Task item in e.NewItems)
                {
                    item.SchedulingInformation = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Task item in e.OldItems)
                {
                    if (ReferenceEquals(item.SchedulingInformation, this))
                    {
                        item.SchedulingInformation = null;
                    }
                }
            }
        }

        #endregion
    }
}
