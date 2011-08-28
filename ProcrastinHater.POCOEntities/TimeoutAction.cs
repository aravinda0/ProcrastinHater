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
    public partial class TimeoutAction
    {
        #region Primitive Properties
    
        public virtual int TimeoutActionID
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
    
        public virtual ICollection<TimedTaskSettings> TimedTaskSettingsSet
        {
            get
            {
                if (_timedTaskSettingsSet == null)
                {
                    var newCollection = new FixupCollection<TimedTaskSettings>();
                    newCollection.CollectionChanged += FixupTimedTaskSettingsSet;
                    _timedTaskSettingsSet = newCollection;
                }
                return _timedTaskSettingsSet;
            }
            set
            {
                if (!ReferenceEquals(_timedTaskSettingsSet, value))
                {
                    var previousValue = _timedTaskSettingsSet as FixupCollection<TimedTaskSettings>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupTimedTaskSettingsSet;
                    }
                    _timedTaskSettingsSet = value;
                    var newValue = value as FixupCollection<TimedTaskSettings>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupTimedTaskSettingsSet;
                    }
                }
            }
        }
        private ICollection<TimedTaskSettings> _timedTaskSettingsSet;

        #endregion
        #region Association Fixup
    
        private void FixupTimedTaskSettingsSet(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (TimedTaskSettings item in e.NewItems)
                {
                    item.TimeoutAction = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (TimedTaskSettings item in e.OldItems)
                {
                    if (ReferenceEquals(item.TimeoutAction, this))
                    {
                        item.TimeoutAction = null;
                    }
                }
            }
        }

        #endregion
    }
}
