﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertSystem.WPF.Helpers.IfThen
{
    public class IfThenConditionGroup
    {
        public ObservableCollection<IfThenCondition> Conditions { get; set; } = new ObservableCollection<IfThenCondition>();    
    }
}
