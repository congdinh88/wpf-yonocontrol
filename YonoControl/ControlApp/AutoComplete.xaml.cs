using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace YonoControl.ControlApp
{
    public class AutoComplete : TextBox
    {
        private Popup _popup;
        private DataGrid _dataGrid;
        private Button _toggleButton;
        private TextBlock _warningTextBlock;
        static AutoComplete()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(AutoComplete),
                new FrameworkPropertyMetadata(typeof(AutoComplete))
            );
        }

        // Dependency Properties
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(AutoComplete));

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly DependencyProperty IsPopupOpenProperty =
            DependencyProperty.Register("IsPopupOpen", typeof(bool), typeof(AutoComplete));

        public bool IsPopupOpen
        {
            get => (bool)GetValue(IsPopupOpenProperty);
            set => SetValue(IsPopupOpenProperty, value);
        }

        public static readonly DependencyProperty DisplayMemberPathProperty =
            DependencyProperty.Register(
                "DisplayMemberPath",
                typeof(string),
                typeof(AutoComplete),
                new PropertyMetadata("Col1")
            );

        public string DisplayMemberPath
        {
            get => (string)GetValue(DisplayMemberPathProperty);
            set => SetValue(DisplayMemberPathProperty, value);
        }

        public static readonly DependencyProperty SelectedItemProperty =
        DependencyProperty.Register("SelectedItem", typeof(object), typeof(AutoComplete));

        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _popup = GetTemplateChild("PART_Popup") as Popup;
            _dataGrid = GetTemplateChild("PART_Datagrid") as DataGrid;
            _toggleButton = GetTemplateChild("PART_ButtonToggle") as Button;
            _warningTextBlock = GetTemplateChild("PART_WarningTextblock") as TextBlock;
            // Event handlers
            _toggleButton.Click += (s, e) => IsPopupOpen = !IsPopupOpen;
            _dataGrid.SelectionChanged += DataGrid_SelectionChanged;
            this.TextChanged += AutoCompleteTextBox_TextChanged;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (!IsPopupOpen) return;

            switch (e.Key)
            {
                case Key.Enter:
                    SelectCurrentItem();
                    e.Handled = true;
                    break;

                case Key.Down:
                    MoveSelection(1); // Di chuyển xuống
                    e.Handled = true;
                    break;

                case Key.Up:
                    MoveSelection(-1); // Di chuyển lên
                    e.Handled = true;
                    break;

                case Key.Escape:
                    IsPopupOpen = false;
                    e.Handled = true;
                    break;
            }
        }

        private void SelectCurrentItem()
        {
            if (_dataGrid.SelectedItem == null) return;

            // Cập nhật giá trị TextBox từ item được chọn
            var selectedItem = _dataGrid.SelectedItem;
            var displayValue = selectedItem.GetType()
                .GetProperty(DisplayMemberPath)?
                .GetValue(selectedItem)?
                .ToString();
            Text = displayValue ?? "";
            IsPopupOpen = false;
        }

        private void MoveSelection(int direction)
        {
            if (_dataGrid.Items.Count == 0) return;
            int newIndex = _dataGrid.SelectedIndex + direction;
            // Xử lý vòng lặp (khi đến cuối/quay lại đầu)
            if (newIndex < 0) newIndex = _dataGrid.Items.Count - 1;
            if (newIndex >= _dataGrid.Items.Count) newIndex = 0;
            _dataGrid.SelectedIndex = newIndex;
            _dataGrid.ScrollIntoView(_dataGrid.SelectedItem);
        }
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataGrid.SelectedItem != null)
            {
                var selectedItem = _dataGrid.SelectedItem;
                var propertyInfo = selectedItem.GetType().GetProperty(DisplayMemberPath);
                SelectedItem = _dataGrid.SelectedItem;
                if (propertyInfo != null)
                {
                    this.Text = propertyInfo.GetValue(selectedItem)?.ToString();
                }
                IsPopupOpen = false;
            }
        }

        private void AutoCompleteTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(Text))
            {
                _warningTextBlock.Visibility = Visibility.Visible;
                _dataGrid.ItemsSource = null;
                return;
            }
            var filteredItems = ItemsSource?
                .Cast<object>()
                .Where(item =>
                {
                    var prop = item.GetType().GetProperty(DisplayMemberPath);
                    if (prop == null) return false;

                    var value = prop.GetValue(item)?.ToString()?.ToLower() ?? "";
                    return value.Contains(Text.ToLower());
                });

            _warningTextBlock.Visibility = filteredItems?.Any() == true
                ? Visibility.Collapsed
                : Visibility.Visible;
            _dataGrid.ItemsSource = filteredItems;
            IsPopupOpen = true;
        }
    }
}
