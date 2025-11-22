using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows;

namespace ExpertSystem.WPF.Controls.Behaviors
{
    public class NumericInputBehavior : Behavior<TextBox>
    {
        private static readonly Regex _regex = new Regex(@"^[01](\,\d{0,2})?$");

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewTextInput += OnPreviewTextInput;
            AssociatedObject.PreviewKeyDown += OnPreviewKeyDown;
            DataObject.AddPastingHandler(AssociatedObject, OnPaste);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewTextInput -= OnPreviewTextInput;
            AssociatedObject.PreviewKeyDown -= OnPreviewKeyDown;
            DataObject.RemovePastingHandler(AssociatedObject, OnPaste);
        }
        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            string newText = textBox.Text.Insert(textBox.SelectionStart, e.Text);
            e.Handled = !IsTextValid(newText);
        }

        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(DataFormats.Text))
            {
                string text = (string)e.DataObject.GetData(DataFormats.Text);
                var textBox = sender as TextBox;
                string newText = textBox.Text.Insert(textBox.SelectionStart, text);
                if (!IsTextValid(newText))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private bool IsTextValid(string text)
        {
            if (!_regex.IsMatch(text) || text.Length > 4)
            {
                return false;
            }

            if (double.TryParse(text, out double value))
            {
                return value >= 0 && value <= 1;
            }
            return false;
        }

    }
}
