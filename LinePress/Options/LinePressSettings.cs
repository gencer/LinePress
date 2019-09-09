using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System;

namespace LinePress.Options
{
   public class LinePressSettings : ISettings, INotifyPropertyChanged
   {
      #region Fields

      private bool compressEmptyLines = false;
      private bool compressCustomTokens = false;

      private int emptyLineScale = 50;
      private int customTokensScale = 25;

      private int lineSpacingPercent = 45;
      private bool applySpacingToComments = false;

      private ObservableCollection<string> customTokens
         = new ObservableCollection<string> {};

      #endregion

      #region Events

      public event Action TokenAdded;

      #endregion

      #region Constructors

      public LinePressSettings()
      {
         InsertTokenCommand = new RelayCommand<string>(CanInsertToken, t =>
         {
            CustomTokens.Add(t);
            TokenAdded?.Invoke();
         });

         DeleteTokenCommand = new RelayCommand<string>(CanDeleteToken, t => CustomTokens.Remove(t));
      }

      #endregion

      #region Settings Properties

      [Setting]
      public bool CompressEmptyLines
      {
         get { return compressEmptyLines; }
         set { SetField(ref compressEmptyLines, value); }
      }

      [Setting]
      public int EmptyLineScale
      {
         get { return emptyLineScale; }
         set { SetField(ref emptyLineScale, value); }
      }

      [Setting]
      public bool CompressCustomTokens
      {
         get { return compressCustomTokens; }
         set { SetField(ref compressCustomTokens, value); }
      }

      [Setting]
      public int CustomTokensScale
      {
         get { return customTokensScale; }
         set { SetField(ref customTokensScale, value); }
      }

      [Setting]
      public string CustomTokensString
      {
         get { return ConvertTokensListToString(); }
         set { BuildTokensListFromString(value); }
      }

      [Setting]
      public int LineSpacingPercent
      {
         get { return lineSpacingPercent; }
         set { SetField(ref lineSpacingPercent, value); }
      }

      [Setting]
      public bool ApplySpacingToComments
      {
         get { return applySpacingToComments; }
         set { SetField(ref applySpacingToComments, value); }
      }

      #endregion

      #region Non-Settings Properties

      public ObservableCollection<string> CustomTokens => customTokens;

      #endregion

      #region Commands

      public RelayCommand<string> InsertTokenCommand { get; private set; }
      public RelayCommand<string> DeleteTokenCommand { get; private set; }

      private bool CanInsertToken(string token) =>
         !token.IsNullOrWhiteSpace() && !CustomTokens.Contains(token);

      private bool CanDeleteToken(string token) =>
         !token.IsNullOrWhiteSpace() && CustomTokens.Contains(token);

      #endregion

      #region Helpers

      private void BuildTokensListFromString(string str)
      {
         customTokens.Clear();
         var tokens = str.Split(null);
         foreach (var token in tokens)
            customTokens.Add(token);
      }

      private string ConvertTokensListToString()
      {
         var stringBuilder = new StringBuilder(customTokens[0]);

         for (var i = 1; i < customTokens.Count; i++)
         {
            stringBuilder.Append(' ');
            stringBuilder.Append(customTokens[i]);
         }

         return stringBuilder.ToString();
      }

      #endregion

      #region ISettings Members

      public string Key => "LinePress";

      #endregion

      #region INotifyPropertyChanged Members

      public event PropertyChangedEventHandler PropertyChanged;

      protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
      {
         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
      }

      private void SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
      {
         if (EqualityComparer<T>.Default.Equals(field, value))
            return;

         field = value;

         OnPropertyChanged(propertyName);
      }
      #endregion
   }
}
