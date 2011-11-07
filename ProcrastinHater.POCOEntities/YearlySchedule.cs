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
    public partial class YearlySchedule : SchedulingInformation
    {
        #region Navigation Properties
    
        public virtual ICollection<DayOfYearSpecifier> DayOfYearSpecifiers
        {
            get
            {
                if (_dayOfYearSpecifiers == null)
                {
                    var newCollection = new FixupCollection<DayOfYearSpecifier>();
                    newCollection.CollectionChanged += FixupDayOfYearSpecifiers;
                    _dayOfYearSpecifiers = newCollection;
                }
                return _dayOfYearSpecifiers;
            }
            set
            {
                if (!ReferenceEquals(_dayOfYearSpecifiers, value))
                {
                    var previousValue = _dayOfYearSpecifiers as FixupCollection<DayOfYearSpecifier>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupDayOfYearSpecifiers;
                    }
                    _dayOfYearSpecifiers = value;
                    var newValue = value as FixupCollection<DayOfYearSpecifier>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupDayOfYearSpecifiers;
                    }
                }
            }
        }
        private ICollection<DayOfYearSpecifier> _dayOfYearSpecifiers;

        #endregion
        #region Association Fixup
    
        private void FixupDayOfYearSpecifiers(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (DayOfYearSpecifier item in e.NewItems)
                {
                    item.YearlySchedule = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (DayOfYearSpecifier item in e.OldItems)
                {
                    if (ReferenceEquals(item.YearlySchedule, this))
                    {
                        item.YearlySchedule = null;
                    }
                }
            }
        }

        #endregion
    }
}
