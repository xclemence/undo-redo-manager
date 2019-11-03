using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace UndoRedo
{
    public static class EnterValidationBehavior
    {
        public static readonly DependencyProperty AttachProperty = DependencyProperty.RegisterAttached(
                            "Attach",
                            typeof(bool),
                            typeof(EnterValidationBehavior),
                            new UIPropertyMetadata(false, OnAttachChanged));

      
        internal static void OnAttachChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            if (!(target is TextBox textBox)) return;

            var b = new KeyBinding
            {
                Command = GetTextBindingCommand(),
                CommandParameter = textBox,
                Key = Key.Enter
            };

            textBox.InputBindings.Add(b);
        }

        public static ICommand GetTextBindingCommand() => new RelayCommand<TextBox>((x) =>
        {
            var binding = BindingOperations.GetBindingExpression(x, TextBox.TextProperty);
            
            if(binding.IsDirty)
                binding.UpdateSource();
        });

        public static void SetAttach(FrameworkElement element, bool value) => element.SetValue(AttachProperty, value);
    }
}
