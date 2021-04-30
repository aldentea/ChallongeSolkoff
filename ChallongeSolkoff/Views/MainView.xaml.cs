using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using MvvmCross.Base;
using MvvmCross.ViewModels;
using MvvmCross.Binding.BindingContext;


namespace Aldentea.ChallongeSolkoff.Views
{
	/// <summary>
	/// MainView.xaml の相互作用ロジック
	/// </summary>
	public partial class MainView : MvvmCross.Platforms.Wpf.Views.MvxWpfView	{
		public MainView()
		{
			InitializeComponent();

      // おまじない
      var set = this.CreateBindingSet<MainView, Core.ViewModels.MainViewModel>();
      set.Bind(this).For(view => view.SelectSaveFileInteraction).To(vm => vm.SelectSaveFileInteraction).OneWay();
      set.Bind(this).For(view => view.CopyToClipboardInteraction).To(vm => vm.CopyToClipboardInteraction).OneWay();
      set.Apply();
    }

    public IMvxInteraction<Core.SelectSaveFileQuestion> SelectSaveFileInteraction
		{
      get => _selectSaveFileInteraction;
      set
      {
        if (_selectSaveFileInteraction != null)
        {
          _selectSaveFileInteraction.Requested -= OnSelectSaveFileInteractionRequested;
        }
        _selectSaveFileInteraction = value;
        if (value != null)
          _selectSaveFileInteraction.Requested += OnSelectSaveFileInteractionRequested;
      }
    }
    IMvxInteraction<Core.SelectSaveFileQuestion> _selectSaveFileInteraction;

    void OnSelectSaveFileInteractionRequested(object sender, MvxValueEventArgs<Core.SelectSaveFileQuestion> eventArgs)
    {
      var question = eventArgs.Value;

      Microsoft.Win32.SaveFileDialog dialog
         = new Microsoft.Win32.SaveFileDialog
         {
           Filter = "csvファイル(*.csv)|*.csv|すべてのファイル|*",
           DefaultExt = ".csv",
           AddExtension = true
         };

      if (dialog.ShowDialog() == true)
			{
        question.Callback(dialog.FileName);
      }
      else
			{
        question.Callback(string.Empty);
			}

    }

    public IMvxInteraction<string> CopyToClipboardInteraction
    {
      get => _copyToClipboardInteraction;
      set
      {
        if (_copyToClipboardInteraction != null)
        {
          _copyToClipboardInteraction.Requested -= OnCopyToClipboardInteractionRequested;
        }
        _copyToClipboardInteraction = value;
        if (value != null)
          _copyToClipboardInteraction.Requested += OnCopyToClipboardInteractionRequested;
      }
    }
    IMvxInteraction<string> _copyToClipboardInteraction;


    void OnCopyToClipboardInteractionRequested(object sender, MvxValueEventArgs<string> eventArgs)
    {
      var value = eventArgs.Value;
      System.Windows.Clipboard.SetText(value);
    }

    }
  }
