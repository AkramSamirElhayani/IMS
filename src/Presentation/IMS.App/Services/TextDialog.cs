using System.Windows;

namespace IMS.Services
{
    public class TextDialog : Window
    {
        private System.Windows.Controls.TextBox textBox;

        public string ResponseText
        {
            get { return textBox.Text; }
            set { textBox.Text = value; }
        }

        public TextDialog(string title, string message, string defaultValue = "")
        {
            Title = title;
            Width = 300;
            Height = 150;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            
            var grid = new System.Windows.Controls.Grid();
            grid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            grid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            grid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });

            var messageLabel = new System.Windows.Controls.Label { Content = message, Margin = new Thickness(5) };
            System.Windows.Controls.Grid.SetRow(messageLabel, 0);
            grid.Children.Add(messageLabel);

            textBox = new System.Windows.Controls.TextBox { Text = defaultValue, Margin = new Thickness(5) };
            System.Windows.Controls.Grid.SetRow(textBox, 1);
            grid.Children.Add(textBox);

            var buttonPanel = new System.Windows.Controls.StackPanel { Orientation = System.Windows.Controls.Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(5) };
            var okButton = new System.Windows.Controls.Button { Content = "OK", Width = 60, Margin = new Thickness(5) };
            var cancelButton = new System.Windows.Controls.Button { Content = "Cancel", Width = 60, Margin = new Thickness(5) };

            okButton.Click += (s, e) => { DialogResult = true; Close(); };
            cancelButton.Click += (s, e) => { DialogResult = false; Close(); };

            buttonPanel.Children.Add(okButton);
            buttonPanel.Children.Add(cancelButton);
            System.Windows.Controls.Grid.SetRow(buttonPanel, 2);
            grid.Children.Add(buttonPanel);

            Content = grid;
        }
    }
}
