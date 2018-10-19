/*==========================================================
**
** Логика взаимодействия для DialogInputDirName.xaml
**
** Copyright(c) Alexey Bur'yanov, 2017
** <OWNER>Alexey Bur'yanov</OWNER>
** 
===========================================================*/

namespace Client.Windows
{
    using System.Windows;

    public partial class DialogInputDirName : Window
    {
        public string subdir;

        public DialogInputDirName()
        {
            InitializeComponent();

            TextBoxNameDir.CaretIndex = TextBoxNameDir.Text.Length;
            TextBoxNameDir.Focus();
        } // DialogInputDirName

        public DialogInputDirName(Window owner) : this()
        {
            Owner = owner;
        } // DialogInputDirName

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        } // ButtonCancel_Click

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            subdir = TextBoxNameDir.Text;
            Close();
        } // ButtonOk_Click
    } // DialogInputDirName
} // Client