using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Laboratorna22
{
    public partial class MainWindow : Window
    {
        private bool ukrainian = true;
        private bool isUpdatingControls = false;

        public MainWindow()
        {
            InitializeComponent();

            cmbFontFamily.ItemsSource = Fonts.SystemFontFamilies.OrderBy(f => f.Source);

            cmbFontSize.ItemsSource = new List<double>
            {
                8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72
            };

            cmbFontFamily.SelectedItem = Fonts.SystemFontFamilies.FirstOrDefault(f => f.Source == "Consolas");
            cmbFontSize.Text = "16";

            ApplySelectedFontSize();
            UpdateStatus();
        }

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "RTF файли (*.rtf)|*.rtf|Текстові файли (*.txt)|*.txt|Усі файли (*.*)|*.*";

            if (dialog.ShowDialog() != true)
                return;

            try
            {
                TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);

                using (FileStream stream = new FileStream(dialog.FileName, FileMode.Open))
                {
                    if (Path.GetExtension(dialog.FileName).ToLower() == ".rtf")
                        range.Load(stream, DataFormats.Rtf);
                    else
                        range.Load(stream, DataFormats.Text);
                }

                Title = "Laboratorna22 - " + Path.GetFileName(dialog.FileName);
                UpdateStatus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка відкриття");
            }
        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "RTF файл (*.rtf)|*.rtf|Текстовий файл (*.txt)|*.txt";
            dialog.FileName = "Document.rtf";

            if (dialog.ShowDialog() != true)
                return;

            try
            {
                TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);

                using (FileStream stream = new FileStream(dialog.FileName, FileMode.Create))
                {
                    if (Path.GetExtension(dialog.FileName).ToLower() == ".rtf")
                        range.Save(stream, DataFormats.Rtf);
                    else
                        range.Save(stream, DataFormats.Text);
                }

                Title = "Laboratorna22 - " + Path.GetFileName(dialog.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка збереження");
            }
        }

        private void cmbFontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isUpdatingControls || rtbEditor == null)
                return;

            if (cmbFontFamily.SelectedItem is FontFamily family)
            {
                rtbEditor.Focus();

                if (rtbEditor.Selection.IsEmpty)
                {
                    TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
                    range.ApplyPropertyValue(TextElement.FontFamilyProperty, family);
                    rtbEditor.FontFamily = family;
                }
                else
                {
                    rtbEditor.Selection.ApplyPropertyValue(TextElement.FontFamilyProperty, family);
                }
            }
        }

        private void cmbFontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplySelectedFontSize();
        }

        private void cmbFontSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplySelectedFontSize();
        }

        private void ApplySelectedFontSize()
        {
            if (isUpdatingControls || rtbEditor == null || cmbFontSize == null)
                return;

            if (!double.TryParse(cmbFontSize.Text.Replace('.', ','), out double size))
                return;

            if (size < 6 || size > 100)
                return;

            rtbEditor.Focus();

            if (rtbEditor.Selection.IsEmpty)
            {
                TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
                range.ApplyPropertyValue(TextElement.FontSizeProperty, size);
                rtbEditor.FontSize = size;
            }
            else
            {
                rtbEditor.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, size);
            }
        }

        private void AlignLeft_Click(object sender, RoutedEventArgs e)
        {
            rtbEditor.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Left);
            rtbEditor.Focus();
        }

        private void AlignCenter_Click(object sender, RoutedEventArgs e)
        {
            rtbEditor.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Center);
            rtbEditor.Focus();
        }

        private void AlignRight_Click(object sender, RoutedEventArgs e)
        {
            rtbEditor.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Right);
            rtbEditor.Focus();
        }

        private void cmbLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
                return;

            ukrainian = cmbLanguage.SelectedIndex == 0;
            ApplyLanguage();
        }

        private void ApplyLanguage()
        {
            if (ukrainian)
            {
                btnOpen.Content = "Відкрити";
                btnSave.Content = "Зберегти";
                btnLeft.Content = "Ліворуч";
                btnCenter.Content = "Центр";
                btnRight.Content = "Праворуч";
            }
            else
            {
                btnOpen.Content = "Open";
                btnSave.Content = "Save";
                btnLeft.Content = "Left";
                btnCenter.Content = "Center";
                btnRight.Content = "Right";
            }

            UpdateStatus();
        }

        private void rtbEditor_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (rtbEditor == null)
                return;

            isUpdatingControls = true;

            object temp = rtbEditor.Selection.GetPropertyValue(Inline.FontWeightProperty);
            btnBold.IsChecked = temp != DependencyProperty.UnsetValue && temp.Equals(FontWeights.Bold);

            temp = rtbEditor.Selection.GetPropertyValue(Inline.FontStyleProperty);
            btnItalic.IsChecked = temp != DependencyProperty.UnsetValue && temp.Equals(FontStyles.Italic);

            temp = rtbEditor.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            btnUnderline.IsChecked = temp != DependencyProperty.UnsetValue && temp.Equals(TextDecorations.Underline);

            temp = rtbEditor.Selection.GetPropertyValue(Inline.FontFamilyProperty);
            if (temp != DependencyProperty.UnsetValue)
                cmbFontFamily.SelectedItem = temp;

            temp = rtbEditor.Selection.GetPropertyValue(Inline.FontSizeProperty);
            if (temp != DependencyProperty.UnsetValue)
                cmbFontSize.Text = temp.ToString();

            isUpdatingControls = false;

            UpdateStatus();
        }

        private void rtbEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateStatus();
        }

        private void UpdateStatus()
        {
            if (txtStatus == null || rtbEditor == null)
                return;

            TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
            string text = range.Text.Replace("\r", "").Replace("\n", "");

            if (ukrainian)
                txtStatus.Text = "Символів: " + text.Length;
            else
                txtStatus.Text = "Characters: " + text.Length;
        }
    }
}