﻿using ExpertSystem.WPF.State.Navigators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ExpertSystem.WPF.Commands
{
    public class RenavigateCommand : ICommand
    {
        public readonly IRenavigator _renavigator;

        public RenavigateCommand(IRenavigator renavigator)
        {
            _renavigator = renavigator;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            _renavigator.Renavigate();
        }
    }
}
