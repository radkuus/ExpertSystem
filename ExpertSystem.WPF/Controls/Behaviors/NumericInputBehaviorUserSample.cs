using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;
using System.Globalization;

namespace ExpertSystem.WPF.Controls.Behaviors
{
    public class NumericInputBehaviorUserSample :Behavior<TextBox>
    {
        private static readonly Regex _regex = new Regex(@"^[-\d][\d,]{0,6}$");

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
                string pastedText = (string)e.DataObject.GetData(DataFormats.Text);
                var textBox = sender as TextBox;
                string newText = textBox.Text.Insert(textBox.SelectionStart, pastedText);
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
            if (string.IsNullOrWhiteSpace(text) || text.Length > 6)
            {
                return false;
            }
                
            if (!_regex.IsMatch(text))
            {
                return false;
            }

            if (text == "-" || text == ",")
            {
                return true;
            }

            if (text.StartsWith("-,"))
            {
                return false;
            }

            if (!double.TryParse(text, out _))
            {
                return false;
            }

            var cleanText = text.StartsWith("-") ? text.Substring(1) : text;
            var parts = cleanText.Split(',');

            if (parts[0].Length > 1 && parts[0].StartsWith("0"))
            {
                return false;
            }

            return true;
        }

    }
}
